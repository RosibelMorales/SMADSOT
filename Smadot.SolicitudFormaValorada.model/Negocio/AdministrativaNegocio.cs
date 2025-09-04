using Smadot.Models.DataBase;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using static Smadot.Models.Entities.Administrativa.Response.AdministrativaResponseData;
using static Smadot.Models.Entities.Administrativa.Request.AdministrativaRequestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using System.Transactions;
using Smadot.Models.Dicts;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.GenericProcess;

namespace Smadot.SolicitudFormaValorada.Model.Negocio
{
    public class AdministrativaNegocio : IAdministrativaNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;

        public AdministrativaNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration, SmadsotGenericInserts smadsotGenericInserts)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _smadsotGenericInserts = smadsotGenericInserts;
        }
        public async Task<ResponseGeneric<List<AdministrativaResponse>>> Consulta(RequestList request)
        {
            try
            {
                ///Se agrego ordenamiento debido a que el grid mostraba los registros desordenados
                ///REVISAR OPCIONES
                var catalogo = _context.vAdministrativas.Where(x => x.IdAdministrativa != null);

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }
                //if (request.Activo.HasValue)
                //{
                //    catalogo = catalogo.Where(x => x.Activo == request.Activo);
                //}

                //SE OBTIENE EL TOTAL DE REGISTROS PARA INFORMACION EN EL GRID
                var tot = catalogo.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x =>
                                                    x.Placa.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Serie.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.NumeroReferencia.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.FolioCertificado.ToString().ToLower().Contains(request.Busqueda.ToLower()));
                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new AdministrativaResponse
                {
                    IdFolio = x.Id,
                    FolioAsignado = x.FolioAnterior,
                    IdAdministrativa = x.IdAdministrativa,
                    IdCatMotivoTramite = x.IdCatMotivoTramite,
                    MotivoTramite = x.MotivoTramite,
                    NumeroReferencia = x.NumeroReferencia,
                    IdUserRegistro = x.IdUserRegistro,
                    Placa = x.Placa,
                    Serie = x.Serie,
                    UsuarioRegistro = x.UsuarioRegistro,
                    FechaRegistro = x.FechaRegistro.ToString("dd/MM/yyyy"),
                    ClaveTramite = x.ClaveTramite,
                    Total = tot,
                }).ToListAsync();

                return new ResponseGeneric<List<AdministrativaResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<AdministrativaResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> GuardarAdministrativa(AdministrativaApiRequest request)
        {
            try
            {
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = new ResponseGeneric<bool>();

                    var user = _userResolver.GetUser();
                    var idUser = user.IdUser;
                    var verificentro = await _context.vVerificentros.FirstOrDefaultAsync(x => x.Clave.Equals("SMADSOT-00"));

                    var fechaActual = DateTime.Now;
                    var adm = new Administrativa
                    {
                        IdCatMotivoTramite = request.IdCatMotivoTramite,
                        Placa = request.Placa.Trim(),
                        Serie = request.Serie.Trim(),
                        NumeroReferencia = request.NumeroReferencia,
                        FolioAsignado = request.FolioAsignado??string.Empty,// Es el folio anterior
                        FechaRegistro = fechaActual,
                        Combustible = Combustible.DictCombustible[request.IdTipoCombustible],
                        IdTipoCombustible = request.IdTipoCombustible,
                        // adm.Vigencia = fechaActual.AddYears(2);
                        NombrePropietario = request.NombrePropietario,
                        Marca = request.Marca,
                        Submarca = request.Submarca,
                        Modelo = request.Modelo ?? 0,
                        TarjetaCirculacion = request.TarjetaCirculacion,
                        IdUserRegistro = idUser
                    };
                    
                    adm.Vigencia = fechaActual.AddMonths(6);
                    _context.Administrativas.Add(adm);
                    await _context.SaveChangesAsync();
                    var validado = await _smadsotGenericInserts.ValidateFolio(request.IdCatTipoCertificado, verificentro.Id, TipoTramite.CA, idUser, request.EntidadProcedencia, null, adm.Id, null);
                    if (!validado.IsSucces)
                    {
                        result.Status = ResponseStatus.Failed;
                        result.mensaje = validado.Description ?? "";
                        result.Response = true;
                        return result;
                    }


                    var i = 0;
                    foreach (var file in request.Files)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Admnistrativa/" + adm.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            switch (i)
                            {
                                case 0:
                                    adm.UrlDoc1 = url; break;
                                case 1:
                                    adm.UrlDoc2 = url; break;
                                case 2:
                                    adm.UrlDoc3 = url; break;
                                case 3:
                                    adm.UrlDoc4 = url; break;
                            }
                        }
                        i++;
                    }

                    await _context.SaveChangesAsync();
                    ts.Complete();
                    ts.Dispose();
                    result.mensaje = "La información se guardo correctamente";
                    result.Response = true;

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<List<PruebaAutocompletePlacaApi>>> ConsultaAutocomplete(string prefix)
        {
            try
            {
                var list = GetListAutocomplete();

                var result = list.Where(x => x.Placa.ToLower().Contains(prefix.ToLower()) || x.Serie.ToLower().Contains(prefix.ToLower())).Take(10).ToList();
                return new ResponseGeneric<List<PruebaAutocompletePlacaApi>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<PruebaAutocompletePlacaApi>>(ex);
            }
        }

        public async Task<ResponseGeneric<AdministrativaResponse>> GetById(long Id)
        {
            try
            {
                var FolioAnterior = _context.vVerificacionReposicions.FirstOrDefault(x => x.Id == Id);
                var adm = _context.vAdministrativas.FirstOrDefault(x => x.IdAdministrativa == FolioAnterior.IdAdministrativa);
                var result = new AdministrativaResponse
                {
                    IdFolio = FolioAnterior.Id,
                    FolioAsignado = adm.FolioAsignado,
                    IdAdministrativa = adm.IdAdministrativa,
                    IdCatMotivoTramite = adm.IdCatMotivoTramite,
                    MotivoTramite = adm.MotivoTramite,
                    NumeroReferencia = adm.NumeroReferencia,
                    IdUserRegistro = adm.IdUserRegistro,
                    Placa = adm.Placa,
                    Serie = adm.Serie,
                    UsuarioRegistro = adm.UsuarioRegistro,
                    UrlDoc1 = adm.UrlDoc1,
                    UrlDoc2 = adm.UrlDoc2,
                    UrlDoc3 = adm.UrlDoc3,
                    UrlDoc4 = adm.UrlDoc4,
                    FechaRegistro = adm.FechaRegistro.ToString("dd/MM/yyyy"),
                    Vigencia = adm.Vigencia.ToString("dd/MM/yyyy"),
                    NombrePropietario = adm.NombrePropietario,
                    Marca = adm.Marca,
                    SubMarca = adm.Submarca,
                    Modelo = adm.Modelo,
                    TarjetaCirculacion = adm.TarjetaCirculacion,
                    IdTipoCombustible = adm.IdTipoCombustible,
                    Combustible = adm.Combustible,
                    IdCatTipoCertificado = adm.IdCatTipoCertificado,
                    EntidadProcedencia = adm.EntidadProcedencia,
                    FechaEmisionRef = adm.FechaEmisionRef.Value.ToString("dd/MM/yyyy"),
                    FechaPago = adm.FechaPago.Value.ToString("dd/MM/yyyy")
                };

                return new ResponseGeneric<AdministrativaResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<AdministrativaResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Eliminar(long id)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var administrativaRegistro = await _context.Administrativas.FirstOrDefaultAsync(x => x.Id == id);

                    if (administrativaRegistro == null)
                    {
                        return new ResponseGeneric<bool>(false) { mensaje = "Ocurrio un error al encontrar el registro." };
                    }

                    var folioFormaValorada = _context.FoliosFormaValoradaVerificentros.Where(x => x.IdAdministrativa == id).ToList();
                    var aux = folioFormaValorada.Select(x => x.Id).ToList();
                    var folioActual = _context.FoliosFormaValoradaActuales.Where(x => aux.Contains((long)x.IdFolioFormaValoradaVerificentro));


                    _context.FoliosFormaValoradaActuales.RemoveRange(folioActual);
                    _context.FoliosFormaValoradaVerificentros.RemoveRange(folioFormaValorada);
                    _context.Administrativas.Remove(administrativaRegistro);
                    _context.SaveChanges();
                    transaction.Complete();

                    return new ResponseGeneric<bool>(true);

                }

            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        private IQueryable<PruebaAutocompletePlacaApi> GetListAutocomplete()
        {
            var list = new List<PruebaAutocompletePlacaApi>();


            list.Add(new PruebaAutocompletePlacaApi { Id = 1, Placa = "THL-2154", Serie = "3A1SF24241S188918" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 2, Placa = "MEK-9456", Serie = "3A1SF34142S215964" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 3, Placa = "NWN-8756", Serie = "3A1SF43243S214589" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 4, Placa = "QSS-5946", Serie = "3A1SF53144S015698" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 5, Placa = "BRE-7458", Serie = "3A1SF62245S356985" });

            list.Add(new PruebaAutocompletePlacaApi { Id = 6, Placa = "HFW-6494", Serie = "3H1SF24236F158963" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 7, Placa = "KEL-4185", Serie = "3H1SF34137F145896" });

            list.Add(new PruebaAutocompletePlacaApi { Id = 8, Placa = "VJW-8944", Serie = "3C1SF24228T045985" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 9, Placa = "CNM-4512", Serie = "3C1SF33229T148963" });

            list.Add(new PruebaAutocompletePlacaApi { Id = 10, Placa = "PLF-1231", Serie = "3N1SF24211H025896" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 11, Placa = "ABC-0001", Serie = "3N1SF34112H157896" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 12, Placa = "BWB-4111", Serie = "3N1SF43213H254796" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 13, Placa = "CEK-7410", Serie = "3N1SF53114H369852" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 14, Placa = "QPW-1234", Serie = "3N1SF62215H485963" });

            list.Add(new PruebaAutocompletePlacaApi { Id = 15, Placa = "PEL-9458", Serie = "3V1SF24256P452684" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 16, Placa = "LEK-9462", Serie = "3V1SF34157P264896" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 17, Placa = "KEM-5487", Serie = "3V1SF43258P015632" });

            list.Add(new PruebaAutocompletePlacaApi { Id = 18, Placa = "BQC-6451", Serie = "3F1SF24269C485695" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 19, Placa = "QCL-8613", Serie = "3F1SF34161C255896" });
            list.Add(new PruebaAutocompletePlacaApi { Id = 20, Placa = "BQL-4863", Serie = "3F1SF43262C044589" });

            return list.AsQueryable();
        }
    }

    public interface IAdministrativaNegocio
    {
        public Task<ResponseGeneric<List<AdministrativaResponse>>> Consulta(RequestList request);
        public Task<ResponseGeneric<bool>> GuardarAdministrativa(AdministrativaApiRequest request);
        public Task<ResponseGeneric<List<PruebaAutocompletePlacaApi>>> ConsultaAutocomplete(string request);
        public Task<ResponseGeneric<AdministrativaResponse>> GetById(long Id);

        public Task<ResponseGeneric<bool>> Eliminar(long id);
    }
}
