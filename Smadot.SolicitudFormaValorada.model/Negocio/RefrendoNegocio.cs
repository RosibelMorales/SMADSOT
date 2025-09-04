using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Refrendo.Response;
using Smadot.Models.Entities.Refrendo.Request;
using Smadot.Utilities.BlobStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Transactions;
using Smadot.Models.GenericProcess;

namespace Smadot.SolicitudFormaValorada.Model.Negocio
{
    public class RefrendoNegocio : IRefrendoNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;
        public RefrendoNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration, SmadsotGenericInserts smadsotGenericInserts)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _smadsotGenericInserts = smadsotGenericInserts;
        }

        public async Task<ResponseGeneric<ResponseGrid<RefrendoGridResponse>>> Consulta(long id)
        {
            try
            {
                var refrendos = _context.vRefrendos.Where(x => x.IdExento == id);

                var tot = await refrendos.CountAsync();
                var result = new ResponseGrid<RefrendoGridResponse>
                {
                    RecordsTotal = tot,
                    RecordsFiltered = tot,
                    Data = JsonConvert.DeserializeObject<List<RefrendoGridResponse>>(JsonConvert.SerializeObject(await refrendos.ToListAsync()))
                };
                return new ResponseGeneric<ResponseGrid<RefrendoGridResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<RefrendoGridResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<RefrendoGridResponse>> GetById(long Id)
        {
            try
            {
                var result = new RefrendoGridResponse();
                if (Id > 0)
                {
                    //Se modifica los refrendos para pertenecer a un Exento (reunion 13/01/2023)
                    var refrendo = _context.vRefrendos.FirstOrDefault(x => x.Id == Id);
                    if (refrendo != null)
                    {
                        string r = JsonConvert.SerializeObject(refrendo);
                        result = JsonConvert.DeserializeObject<RefrendoGridResponse>(r);
                        result.Exentos = await _context.vExentos.Where(x => x.Placa == refrendo.Placa || x.Serie == refrendo.Serie).ToListAsync();
                    }
                    //if (verificacion)
                    //{
                    //    var verif = _context.vVerificacions.FirstOrDefault(x => x.Id == Id);
                    //    result.Exentos = await _context.vVerificacions.Where(x => x.Placa == verif.Placa || x.Serie == verif.Serie).ToListAsync();
                    //}
                    //else
                    //{
                    //    var refrendo = _context.vRefrendos.FirstOrDefault(x => x.Id == Id);
                    //    if (refrendo != null)
                    //    {
                    //        string r = JsonConvert.SerializeObject(refrendo);
                    //        result = JsonConvert.DeserializeObject<RefrendoGridResponse>(r);
                    //        result.Exentos = await _context.vVerificacions.Where(x => x.Id == refrendo.IdExento).ToListAsync();
                    //    }
                    //}
                }
                else
                {
                    throw new Exception("No sé encontró un refrendo.");
                }
                return new ResponseGeneric<RefrendoGridResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<RefrendoGridResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> Registro(RefrendoRequest request)
        {
            try
            {
                var refrendo = new Refrendo();
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (request.Id > 0)
                    {
                        refrendo = _context.Refrendos.FirstOrDefault(x => x.Id == request.Id);
                        if (refrendo == null)
                            return new ResponseGeneric<long>(0) { mensaje = "No sé encontró un refrendo." };

                    }
                    else
                    {
                        var user = _userResolver.GetUser();
                        var exento = _context.vExentos.FirstOrDefault(x => x.Id == request.IdExento);
                        if (exento == null)
                        {
                            return new ResponseGeneric<long>(0) { mensaje = "El exento que se intenta refrendar no existe o ha sido eliminado." };

                        }
                        if ((exento.Vigencia ?? DateTime.Now.AddDays(-1)).AddMonths(-1) > DateTime.Now)
                        {
                            return new ResponseGeneric<long>(0) { mensaje = "El último holograma sigue vigente." };

                        }
                        else if (exento.Combustible.Equals(TipoCombustible.DictTipoCombustible[TipoCombustible.Electricos]))
                        {
                            return new ResponseGeneric<long>(0) { mensaje = "El vehículo tiene un certificado permanente." };
                        }
                        var verificentro = await _context.vVerificentros.FirstOrDefaultAsync(x => x.Clave.Equals("SMADSOT-00"));
                        var validado = await _smadsotGenericInserts.ValidateFolio(TipoCertificado.Exentos, verificentro.Id, TipoTramite.CE, user.IdUser, exento.EntidadProcedencia, null, null, request.IdExento);
                        if (!validado.IsSucces)
                        {
                            return new ResponseGeneric<long>(0) { mensaje = validado.Description ?? "" };
                        }

                        refrendo = new Refrendo
                        {
                            IdExento = request.IdExento,
                            NumeroReferencia = request.NumeroReferencia,
                            UrlDoc1 = request.UrlDoc1,
                            UrlDoc2 = request.UrlDoc2,
                            UrlDoc3 = request.UrlDoc3,
                            FechaCartaFactura = request.FechaCartaFactura,
                            VigenciaHoloAnterior = exento.Vigencia.Value,
                            Placa = request.Placa.ToUpper(),
                            Propietario = request.Propietario,
                            FechaRegistro = DateTime.Now,
                            IdUserRegistro = _userResolver.GetUser().IdUser,
                            IdFolioFormaValoradaVerificentro = validado.recordId,
                            Vigencia = exento.Vigencia.Value.AddYears(8)
                        };
                        _context.Refrendos.Add(refrendo);
                    }
                    var result = await _context.SaveChangesAsync() > 0;

                    var i = 0;
                    if (request.Files != null && request.Files.Count > 0)
                    {
                        foreach (var f in request.Files)
                        {
                            var url = await _blobStorage.UploadFileAsync(new byte[0], "Refrendo/" + refrendo.Id + "/" + f.Nombre, f.Base64);
                            if (!string.IsNullOrEmpty(url))
                            {
                                switch (i)
                                {
                                    case 0:
                                        refrendo.UrlDoc1 = url; break;
                                    case 1:
                                        refrendo.UrlDoc2 = url; break;
                                    case 2:
                                        refrendo.UrlDoc3 = url; break;
                                }
                            }
                            i++;
                        }
                        result = await _context.SaveChangesAsync() > 0;
                    }

                    scope.Complete();
                    return result ? new ResponseGeneric<long>(refrendo.Id) : new ResponseGeneric<long>();
                }
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex) { mensaje = "Ocurrió un error al guardar la información del Refrendo." };
            }
        }

        // public async Task<ResponseGeneric<List<RefrendoAutocompleteResponse>>> Autocomplete(RefrendoAutocompletRequest request)
        // {
        //     try
        //     {
        //         var verificaciones = _context.vVerificacions.Where(x => x.Placa.ToLower().Contains(request.Term.ToLower()) || x.Serie.ToLower().Contains(request.Term.ToLower())).GroupBy(x => x.Placa).AsQueryable();
        //         var tot = verificaciones.Count();
        //         verificaciones = verificaciones.Skip(request.Start).Take(request.End);
        //         var result = await verificaciones.Select(x => new RefrendoAutocompleteResponse
        //         {
        //             Id = x.FirstOrDefault().Id,
        //             Text = x.FirstOrDefault().Placa + " / " + x.FirstOrDefault().Serie
        //         }).ToListAsync();
        //         return new ResponseGeneric<List<RefrendoAutocompleteResponse>>(result);
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ResponseGeneric<List<RefrendoAutocompleteResponse>>(ex);
        //     }
        // }

        // public async Task<ResponseGeneric<vFoliosFormaValoradaExentosImpresion>> GetExentoFormaValorada(long Id)
        // {
        //     try
        //     {
        //         var result = new vFoliosFormaValoradaExentosImpresion();
        //         if (Id > 0)
        //         {
        //             result = _context.vFoliosFormaValoradaExentosImpresions.FirstOrDefault(x => x.Id == Id);
        //         }
        //         else
        //         {
        //             throw new Exception("No sé encontró una forma valorada.");
        //         }
        //         return new ResponseGeneric<vFoliosFormaValoradaExentosImpresion>(result);
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ResponseGeneric<vFoliosFormaValoradaExentosImpresion>(ex);
        //     }
        // }
    }

    public interface IRefrendoNegocio
    {
        Task<ResponseGeneric<ResponseGrid<RefrendoGridResponse>>> Consulta(long id);
        Task<ResponseGeneric<RefrendoGridResponse>> GetById(long Id);
        Task<ResponseGeneric<long>> Registro(RefrendoRequest request);

    }
}
