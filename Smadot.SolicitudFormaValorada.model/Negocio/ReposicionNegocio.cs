using Smadot.Models.DataBase;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Smadot.Models.Entities.Reposicion.Response.ReposicionResponseData;
using System.Linq.Dynamic.Core;
using static Smadot.Models.Entities.FoliosCancelados.Response.FoliosCanceladosResponseData;
using static Smadot.Models.Entities.FoliosCancelados.Request.FoliosCanceladosRequestData;
using Smadot.Models.Entities.Reposicion.Request;
using static Smadot.Models.Entities.Reposicion.Request.ReposicionRequestData;
using Microsoft.EntityFrameworkCore;
using Smadot.Utilities.BlobStorage;
using Microsoft.Extensions.Configuration;
using Smadot.Utilities.GestionTokens;
using System.Transactions;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Reposicion.Response;
using Newtonsoft.Json;
using Smadot.Models.GenericProcess;

namespace Smadot.SolicitudFormaValorada.Model.Negocio
{
    public class ReposicionNegocio : IReposicionNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;
        public ReposicionNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration, SmadsotGenericInserts smadsotGenericInserts)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _smadsotGenericInserts = smadsotGenericInserts;
        }

        public async Task<ResponseGeneric<List<ReposicionResponse>>> Consulta(RequestList request)
        {
            try
            {
                // Se agrego ordenamiento debido a que el grid mostraba los registros desordenados
                // REVISAR OPCIONES
                var catalogo = _context.vReposicions.AsQueryable();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                //SE OBTIENE EL TOTAL DE REGISTROS PARA INFORMACION EN EL GRID
                var tot = catalogo.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x =>
                                                    x.NumeroReferencia.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.UsuarioRegistro.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Placa.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Serie.ToLower().Contains(request.Busqueda.ToLower()));
                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new ReposicionResponse
                {
                    IdReposicion = x.IdReposicion,
                    IdVerificacion = x.IdVerificacion,
                    FechaRegistro = x.FechaRegistro.ToString("dd/MM/yyyy"),
                    NumeroReferencia = x.NumeroReferencia,
                    UsuarioRegistro = x.UsuarioRegistro,
                    Placa = x.Placa.Trim(),
                    Serie = x.Serie.Trim(),
                    IdFoliosFormaValoradaVerificentro = x.IdFoliosFormaValoradaVerificentro,
                    ClaveTramite = x.ClaveTramite,
                    Folio = x.Folio,
                    Total = tot,
                }).ToListAsync();


                //var result = new List<ReposicionResponse>();
                //result.Add(new ReposicionResponse
                //{
                //    IdVerificacion =1,
                //    FechaRegistro = DateTime.Now.ToString("dd-MM-yyyy"),
                //    NumeroReferencia = "REF-000001",
                //    UsuarioRegistro = "ADMIN",
                //    Placa = "ABC-1234",
                //    Serie = Guid.NewGuid().ToString(),
                //    Total = 2
                //});
                //result.Add(new ReposicionResponse
                //{
                //    IdVerificacion = 2,
                //    FechaRegistro = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy"),
                //    NumeroReferencia = "REF-000002",
                //    UsuarioRegistro = "ADMIN",
                //    Placa = "ABC-1234",
                //    Serie = Guid.NewGuid().ToString(),
                //    Total = 2
                //});

                return new ResponseGeneric<List<ReposicionResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<ReposicionResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<vBusquedaPlacaSerie>>> ConsultaAutocomplete(string prefix)
        {
            try
            {
                var result = _context.vBusquedaPlacaSeries.Where(x => x.Placa.ToLower().Contains(prefix.ToLower()) || x.Serie.ToLower().Contains(prefix.ToLower())).Take(10).ToList();
                return new ResponseGeneric<List<vBusquedaPlacaSerie>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<vBusquedaPlacaSerie>>(ex);
            }
        }

        public async Task<ResponseGeneric<vReposicionVerificacion>> GetById(long Id)
        {
            try
            {
                var result = _context.vReposicionVerificacions.FirstOrDefault(x => x.IdReposicion == Id);

                return new ResponseGeneric<vReposicionVerificacion>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<vReposicionVerificacion>(ex);
            }
        }

        //public async Task<ResponseGeneric<List<vVerificacion>>> GetListDataVerificacion(string PlacaSerie)
        //{
        //    try
        //    {
        //        var catalogo = _context.vVerificacions.Where(x => x.Placa == PlacaSerie).AsQueryable();
        //        var result = await catalogo.Select(x => new vVerificacion
        //        {
        //            Id = x.Id,
        //            Fecha = x.Fecha,
        //            Placa = x.Placa,
        //            Serie = x.Serie,
        //            FolioCertificado = x.FolioCertificado,
        //            Vigencia = x.Vigencia,
        //            IdResultadosVerificacion = x.IdResultadosVerificacion,
        //            TipoCertificado = x.TipoCertificado,
        //            Semestre = x.Semestre,
        //            Marca = x.Marca,
        //            Modelo = x.Modelo,
        //            Combustible = x.Combustible,
        //            TarjetaCirculacion = x.TarjetaCirculacion
        //        }).ToListAsync();

        //        return new ResponseGeneric<List<vVerificacion>>(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseGeneric<List<vVerificacion>>(ex);
        //    }
        //}

        public async Task<ResponseGeneric<vDataTramiteVentanillaResponse>> GetDataVerificacion(long Id)
        {
            try
            {
                var verif = await _context.vDataTramitesVentanillas.FirstOrDefaultAsync(x => x.Id == Id);
                if (verif is null)
                    return new ResponseGeneric<vDataTramiteVentanillaResponse>();

                var result = new vDataTramiteVentanillaResponse
                {
                    Id = verif.Id,
                    IdVerificacion = verif.Id,
                    IdAdministrativa = verif.IdAdministrativa,
                    IdExento = verif.IdExento,
                    IdTestificacion = verif.IdTestificacion,
                    IdConstanciaUltimaVerificacion = verif.IdConstanciaUltimaVerificacion,
                    TipoCertificadoFV = verif.TipoCertificadoFV,
                    FolioCertificado = verif.FolioCertificado,
                    ClaveTramite = verif.ClaveTramite,
                    IdCatTipoCertificado = verif.IdCatTipoCertificado,
                    IdCatTipoTramite = verif.IdCatTipoTramite,
                    NombreVerificentro = verif.NombreVerificentro,
                    ApiEndPoint = verif.ApiEndPoint,
                    ApiKey = verif.ApiKey,
                    IdVerificentro = verif.IdVerificentro,
                    ImporteActual = verif.ImporteActual,
                    EntidadProcedencia = verif.EntidadProcedencia,
                    NumeroReferencia = verif.NumeroReferencia,
                    Fecha = verif.Fecha,
                    Placa = verif.Placa,
                    Serie = verif.Serie,
                    Vigencia = verif.Vigencia,
                    Marca = verif.Marca,
                    Submarca = verif.Submarca,
                    Combustible = verif.Combustible,
                    TarjetaCirculacion = verif.TarjetaCirculacion,
                    Modelo = verif.Modelo,
                    FolioAnterior = verif.FolioAnterior,
                    IdUserCapturista = verif.IdUserCapturista,
                    NumeroTrabajadorCapturista = verif.NumeroTrabajadorCapturista,
                    IdUserTecnico = verif.IdUserTecnico,
                    NumeroTrabajadorTecnico = verif.NumeroTrabajadorTecnico,
                    Propietario = verif.Propietario,
                    CausaRechazo = verif.CausaRechazo,
                    MotivoVerificacion = verif.MotivoVerificacion
                };

                return new ResponseGeneric<vDataTramiteVentanillaResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<vDataTramiteVentanillaResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> GuardarReposicion(ReposicionApiRequest request)
        {
            try
            {
                var result = new ResponseGeneric<bool>();
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                   
                        
                        var data = new Reposicion();
                        var folioForma = await _context.vDataTramitesVentanillas.FirstOrDefaultAsync(x => x.Id == request.Id);

                        
                        if (folioForma == null)
                        {
                            result.mensaje = "No existe un folio impreso para la reposición.";
                            return result;
                        }
                        var user = _userResolver.GetUser();
                        var idVeri = user.IdVerificentro ?? 0;
                        var verificentro = await _context.vVerificentros.FirstOrDefaultAsync(x => x.Clave.Equals("SMADSOT-00"));

                        var validado = await _smadsotGenericInserts.ValidateFolio(folioForma.IdCatTipoCertificado ?? 0, verificentro.Id, TipoTramite.CR, user.IdUser, request.EntidadProcedencia,
                        folioForma.IdVerificacion, folioForma.IdAdministrativa, folioForma.IdExento,folioForma.IdTestificacion,true);
                        if (!validado.IsSucces)
                        {
                            result.Status = ResponseStatus.Failed;
                            result.mensaje = validado.Description;
                            return result;
                        }



                        data.IdFoliosFormaValoradaVerificentro = validado.recordId;
                        data.FechaRegistro = DateTime.Now;
                        data.NumeroReferencia = request.NumeroReferencia;
                        data.IdUserRegistro = request.IdUserRegistro;

                        _context.Reposicions.Add(data);
                        _context.SaveChanges();

                        var i = 0;
                        if (request.Files != null && request.Files.Count() > 0)
                        {
                            foreach (var file in request.Files)
                            {
                                var url = await _blobStorage.UploadFileAsync(new byte[0], "Reposicion/" + data.Id + "/" + file.Nombre, file.Base64);
                                if (!string.IsNullOrEmpty(url))
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            data.UrlDoc1 = url; break;
                                        case 1:
                                            data.UrlDoc2 = url; break;
                                        case 2:
                                            data.UrlDoc3 = url; break;
                                    }
                                }
                                i++;
                            }
                        }
                        _context.SaveChanges();


                        result.Response = true;

                    
                    //else
                    //{
                    //    reposicion.NumeroReferencia = request.NumeroReferencia;
                    //    var i = 0;
                    //    if (request.Files != null && request.Files.Count() > 0)
                    //    {
                    //        foreach (var file in request.Files)
                    //        {
                    //            var url = await _blobStorage.UploadFileAsync(new byte[0], "Reposicion/" + reposicion.Id + "/" + file.Nombre, file.Base64);
                    //            if (!string.IsNullOrEmpty(url))
                    //            {
                    //                switch (i)
                    //                {
                    //                    case 0:
                    //                        reposicion.UrlDoc1 = url; break;
                    //                    case 1:
                    //                        reposicion.UrlDoc2 = url; break;
                    //                    case 2:
                    //                        reposicion.UrlDoc3 = url; break;
                    //                }
                    //            }
                    //            i++;
                    //        }
                    //    }

                    //    _context.SaveChanges();
                    //    result.Response = true;
                    //}
                    scope.Complete();
                }

                return result;
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Eliminar(long id)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var reposicionRegistro = await _context.Reposicions.FirstOrDefaultAsync(x => x.IdFoliosFormaValoradaVerificentro == id);

                    if (reposicionRegistro == null)
                    {
                        return new ResponseGeneric<bool>(false) { mensaje = "Ocurrio un error al encontrar el registro." };
                    }

                    var folioFormaValorada = _context.FoliosFormaValoradaVerificentros.Where(x => x.Id == id).ToList();
                    var aux = folioFormaValorada.Select(x => x.Id).ToList();
                    var folioActual = _context.FoliosFormaValoradaActuales.Where(x => aux.Contains((long)x.IdFolioFormaValoradaVerificentro));


                    _context.FoliosFormaValoradaActuales.RemoveRange(folioActual);
                    _context.Reposicions.Remove(reposicionRegistro);
                    _context.FoliosFormaValoradaVerificentros.RemoveRange(folioFormaValorada);
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
    }

    public interface IReposicionNegocio
    {
        public Task<ResponseGeneric<List<ReposicionResponse>>> Consulta(RequestList request);
        public Task<ResponseGeneric<List<vBusquedaPlacaSerie>>> ConsultaAutocomplete(string prefix);
        public Task<ResponseGeneric<vReposicionVerificacion>> GetById(long Id);
        public Task<ResponseGeneric<bool>> GuardarReposicion(ReposicionApiRequest request);
        //public Task<ResponseGeneric<List<vVerificacion>>> GetListDataVerificacion(string PlacaSerie);
        public Task<ResponseGeneric<vDataTramiteVentanillaResponse>> GetDataVerificacion(long Id);

        public Task<ResponseGeneric<bool>> Eliminar(long id);
    }
}
