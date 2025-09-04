using Amazon.ECS;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Namespace;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Linea.Request;
using Smadot.Models.Entities.Linea.Response;
using Smadot.Models.Entities.Motivos.Response;
using Smadot.Models.Entities.SeguimientoCVV.Request;
using Smadot.Models.Entities.SeguimientoCVV.Response;
using Smadot.Models.Entities.Verificacion;
using Smadot.Models.Entities.Verificacion.Request;
using Smadot.Models.GenericProcess;
using Smadot.Utilities;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Transactions;
using System.Web.WebPages;
using static System.Formats.Asn1.AsnWriter;

namespace Smadot.SeguimientoCVV.Model.Negocio
{
    public class CierreAperturaLineaNegocio : ICierreAperturaLineaNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;

        public CierreAperturaLineaNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration, SmadsotGenericInserts smadsotGenericInserts)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _smadsotGenericInserts = smadsotGenericInserts;
        }

        public async Task<ResponseGeneric<List<SeguimientoCVVResponse>>> Consulta(SeguimientoCVVListRequest request)
        {
            try
            {
                //var seg = _context.vCALineas.Where(x => x.Id == 1).LastOrDefault();
                //var seguimiento = _context.vCALineas.Where(x => x.FechaRegistroLinea == x.FechaRegistroLineaMotivo).AsQueryable();
                var seguimiento = _context.vLineaGrids.Where(x => x.IdVerificentro == _userResolver.GetUser().IdVerificentro).AsQueryable();


                if (!string.IsNullOrEmpty(request.Busqueda))
                {

                    seguimiento = seguimiento.Where(x => x.NombreLinea.ToLower().Contains(request.Busqueda.ToLower()) || x.Id.ToString().Contains(request.Busqueda.ToLower()) || x.NombreLinea.ToLower().Contains(request.Busqueda.ToLower()) || x.NombreUsuario.ToLower().Contains(request.Busqueda.ToLower()) || x.Estatus.ToLower().Contains(request.Busqueda.ToLower()) || x.Motivo.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var tot = seguimiento.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    seguimiento = seguimiento.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    seguimiento = seguimiento.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }


                DateTime now = DateTime.Now;
                var result = await seguimiento.Select(x => new SeguimientoCVVResponse
                {
                    Id = x.Id,
                    IdLineaMotivo = x.IdLineaMotivo,
                    IdVerificentro = x.IdVerificentro,
                    NombreVerificentro = x.NombreVerificentro,
                    Clave = x.Clave,
                    IdUserRegistro = x.IdUserRegistro,
                    NombreUsuario = x.NombreUsuario,
                    NombreLinea = x.NombreLinea,
                    FechaRegistroLinea = (DateTime)x.FechaRegistroLinea,
                    FechaRegistroLineaMotivo = (DateTime)x.FechaRegistroLineaMotivo,
                    Estatus = x.Estatus,
                    Motivo = x.Motivo,
                    Total = tot

                }).ToListAsync();

                return new ResponseGeneric<List<SeguimientoCVVResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<SeguimientoCVVResponse>>(ex);
            }
        }

        //public async Task<ResponseGeneric<List<SeguimientoCVVResponse>>> GetById(long Id)
        //{
        //    try
        //    {
        //        var result2 = new List<SeguimientoCVVResponse>();
        //        var solicitud = _context.vCALineas.Where(x => x.Id == Id);

        //        if (Id > 0)
        //        {
        //            if (solicitud != null)
        //            {
        //                var motivo = _context.CatMotivoLineas.AsQueryable();
        //                //string r = JsonConvert.SerializeObject(solicitud);
        //                //result = JsonConvert.DeserializeObject<List<SeguimientoCVVResponse>>(r);

        //                var result = await motivo.Select(x => new SeguimientoCVVResponse
        //                {
        //                    IdMotivo = x.Id,
        //                    NombreMotivo = x.Nombre,
        //                    Activo = x.Activo
        //                }).ToListAsync();

        //                var tot = result.Count();

        //                result2 = await solicitud.Select(x => new SeguimientoCVVResponse
        //                {
        //                    Id = x.Id,
        //                    IdLineaMotivo = x.IdLineaMotivo,
        //                    IdVerificentro = x.IdVerificentro,
        //                    NombreVerificentro = x.NombreVerificentro,
        //                    IdUserRegistro = x.IdUserRegistro,
        //                    NombreUsuario = x.NombreUsuario,
        //                    NombreLinea = x.NombreLinea,
        //                    Clave = x.Clave,
        //                    FechaRegistroLinea = (DateTime)x.FechaRegistroLinea,
        //                    FechaRegistroLineaMotivo = (DateTime)x.FechaRegistroLineaMotivo,
        //                    Estatus = x.Estatus,
        //                    Motivo = x.Motivo,
        //                    UrlDocumento = x.UrlDocumento,
        //                    Notas = x.Notas,
        //                    UserRegistroMotivo = x.UserRegistroMotivo,
        //                    Total = tot

        //                }).ToListAsync();

        //                result.ForEach(item => result2.Add(item));

        //                result2.AddRange(result);

        //            }
        //        }
        //        else if (Id == 0)
        //        {
        //            var motivo = _context.CatMotivoLineas.Where(x => x.Id > 0);
        //            string t = JsonConvert.SerializeObject(motivo);
        //            result2 = JsonConvert.DeserializeObject<List<SeguimientoCVVResponse>>(t);
        //        }

        //        return new ResponseGeneric<List<SeguimientoCVVResponse>>(result2);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseGeneric<List<SeguimientoCVVResponse>>(ex);
        //    }
        //}

        public async Task<ResponseGeneric<SeguimientoCVVResponse>> GetByIdCA(long IdLinea)
        {
            try
            {
                var solicitud = await _context.vLineaGrids.FirstOrDefaultAsync(x => x.Id == IdLinea);

                var result = new SeguimientoCVVResponse
                {
                    Id = solicitud.Id,
                    IdLineaMotivo = solicitud.IdLineaMotivo,
                    IdVerificentro = solicitud.IdVerificentro,
                    NombreVerificentro = solicitud.NombreVerificentro,
                    IdUserRegistro = solicitud.IdUserRegistro,
                    NombreUsuario = solicitud.NombreUsuario,
                    NombreLinea = solicitud.NombreLinea,
                    FechaRegistroLinea = (DateTime)solicitud.FechaRegistroLinea,
                    FechaRegistroLineaMotivo = (DateTime)solicitud.FechaRegistroLineaMotivo,
                    Estatus = solicitud.Estatus,
                    Motivo = solicitud.Motivo,
                    Clave = solicitud.Clave,
                    UrlDocumento = solicitud.UrlDocumento,
                    Notas = solicitud.Notas
                };

                return new ResponseGeneric<SeguimientoCVVResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<SeguimientoCVVResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> Registro(List<SeguimientoCVVResponse> request)
        {
            try
            {
                var registroLineas = new Linea();
                var solicitudLineas = new Linea();
                var registroLineasMotivos = new LineaMotivo();
                var registroCatLineaMotivo = new CatMotivoLinea();
                var registroInstalacion = new Instalacion();
                var aux = await _context.vLineaGrids.Select(x => new SeguimientoCVVResponse { Clave = x.Clave }).ToListAsync();
                //Cierre
                if (request[0].Id > 0 && request[0].Estatus == CierreAperturaLineaDic.Activo)
                {
                    solicitudLineas = _context.Lineas.FirstOrDefault(x => x.Id == request[0].Id);
                    solicitudLineas.IdCatEstatusLinea = LineaDic.Cerrado;
                    _context.Update(solicitudLineas);
                    await _context.SaveChangesAsync();

                    //Registrar LineaMotivo
                    registroLineasMotivos = new LineaMotivo
                    {
                        IdLinea = request[0].Id,
                        FechaRegistro = (DateTime)request[0].FechaRegistroLineaMotivo,
                        IdUserRegistro = (long)request[0].IdUserRegistro,
                        IdCatMotivoLinea = request[0].IdMotivo,
                        Notas = request[0].NotasMotivo,
                        Estatus = true,
                    };
                    _context.LineaMotivos.Add(registroLineasMotivos);
                    await _context.SaveChangesAsync();

                    //Registrar UrlDocument
                    foreach (var file in request[0].Files)
                    {

                        var url = await _blobStorage.UploadFileAsync(new byte[0], "CierreLinea/" + registroLineasMotivos.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            registroLineasMotivos.UrlDocumento = url; break;
                        }
                    }

                }
                //Apertura de nuevo
                else if (request[0].Id > 0 && request[0].Estatus == CierreAperturaLineaDic.Inactivo)
                {
                    solicitudLineas = _context.Lineas.FirstOrDefault(x => x.Id == request[0].Id);
                    solicitudLineas.IdCatEstatusLinea = LineaDic.Abierto;
                    _context.Update(solicitudLineas);
                    await _context.SaveChangesAsync();

                    //Registrar LineaMotivo
                    registroLineasMotivos = new LineaMotivo
                    {
                        IdLinea = request[0].Id,
                        FechaRegistro = (DateTime)request[0].FechaRegistroLineaMotivo,
                        IdUserRegistro = (long)request[0].IdUserRegistro,
                        IdCatMotivoLinea = request[0].IdMotivo,
                        Notas = request[0].NotasMotivo,
                    };
                    _context.LineaMotivos.Add(registroLineasMotivos);
                    await _context.SaveChangesAsync();

                    //Registrar UrlDocument
                    foreach (var file in request[0].Files)
                    {

                        var url = await _blobStorage.UploadFileAsync(new byte[0], "AperturaLinea/" + registroLineasMotivos.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            registroLineasMotivos.UrlDocumento = url; break;
                        }
                    }

                }
                //Apertura
                else if (request[0].Id == 0)
                {
                    //Registrar Linea
                    registroLineas = new Linea
                    {
                        //IdVerificentro = (long)request[0].IdVerificentro,
                        IdVerificentro = (long)request[0].IdVerificentro,
                        Nombre = request[0].NombreLinea,
                        IdCatEstatusLinea = LineaDic.Abierto,
                        IdUserRegistro = (long)request[0].IdUserRegistro,
                        FechaRegistro = request[0].FechaRegistroLinea,
                        Clave = request[0].Clave
                    };
                    _context.Lineas.Add(registroLineas);
                    await _context.SaveChangesAsync();

                    //Registrar LineaMotivo
                    registroLineasMotivos = new LineaMotivo
                    {
                        IdLinea = registroLineas.Id,
                        FechaRegistro = (DateTime)request[0].FechaRegistroLineaMotivo,
                        IdUserRegistro = (long)request[0].IdUserRegistro,
                        IdCatMotivoLinea = request[0].IdMotivo,
                        Notas = request[0].NotasMotivo,
                        Estatus = false
                    };
                    _context.LineaMotivos.Add(registroLineasMotivos);
                    await _context.SaveChangesAsync();

                    //Registrar UrlDocument
                    foreach (var file in request[0].Files)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "AperturaLinea/" + registroLineasMotivos.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            registroLineasMotivos.UrlDocumento = url; break;
                        }
                    }
                }

                Linea lineaAlerta = (request[0].Id) > 0 ? solicitudLineas : registroLineas;

                GenerarAlertaLinea(lineaAlerta);

                var result = await _context.SaveChangesAsync() > 0;
                return result ? new ResponseGeneric<long>(registroLineasMotivos.Id) : new ResponseGeneric<long>();

            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex);
            }
        }


        private async void GenerarAlertaLinea(Linea linea)
        {

            Alertum alerta = new()
            {
                TableName = DictAlertas.Linea,
                TableId = linea.Id,
                IdVerificentro = linea.IdVerificentro,
                Data = JsonConvert.SerializeObject(linea, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    NullValueHandling = NullValueHandling.Ignore
                }),
                IdUser = _userResolver.GetUser().IdUser,
                MovimientoInicial = string.Format(MovimientosDicts.DictMovimientoLinea[linea.IdCatEstatusLinea], linea.Nombre, linea.FechaRegistro),
                IdEstatusInicial = linea.IdCatEstatusLinea,
                Fecha = DateTime.Now,
                Leido = false,
                Procesada = false
            };

            await _context.Alerta.AddAsync(alerta);
        }

        public async Task<ResponseGeneric<ResponseGrid<LineaDetalle>>> DetalleGrid(RequestListLinea req)
        {
            var seguimiento = _context.vCALineas.Where(x => x.IdLinea == req.IdLinea).AsQueryable();

            var tot = await seguimiento.CountAsync();

            if (!string.IsNullOrEmpty(req.ColumnaOrdenamiento) && !string.IsNullOrEmpty(req.Ordenamiento))
            {
                seguimiento = seguimiento.OrderBy(req.ColumnaOrdenamiento + " " + req.Ordenamiento);
            }

            if (req.Pagina > 0 && req.Registros > 0)
            {
                seguimiento = seguimiento.Skip((req.Pagina.Value - 1) * req.Registros.Value).Take(req.Registros.Value);
            }


            DateTime now = DateTime.Now;
            var list = await seguimiento.Select(x => new LineaDetalle
            {
                Id = x.Id,
                IdLineaMotivo = x.IdLineaMotivo,
                FechaRegistroLineaMotivo = (DateTime)x.FechaRegistroLineaMotivo,
                Estatus = x.Estatus,
                Motivo = x.Motivo,
                Notas = x.Notas,
                UrlDocumento = x.UrlDocumento,
                IdUserRegistroMotivo = x.IdUserRegistroMotivo,
                UserRegistroMotivo = x.UserRegistroMotivo
            }).ToListAsync();

            var result = new ResponseGrid<LineaDetalle>() { Data = list, RecordsTotal = tot, RecordsFiltered = tot };
            return new ResponseGeneric<ResponseGrid<LineaDetalle>>(result);
        }

        public async Task<ResponseGeneric<long>> DashboardLineaAperturaCierre(DashboardLineaRequest req)
        {
            try
            {
                var user = _userResolver.GetUser();
                var linea = _context.Lineas.FirstOrDefault(x => x.Id == req.Id && x.IdVerificentro == user.IdVerificentro);
                if (linea == null)
                {
                    return new ResponseGeneric<long>(new Exception("No se encontró la línea seleccionada."))
                    { mensaje = "No sé encontró registro de la verificacion." };
                }
                if (linea.IdCatEstatusLinea == (req.Bloquear ? LineaDic.Cerrado : LineaDic.Abierto))
                {
                    return new ResponseGeneric<long>(new Exception("La linea ya se encuentra " + (req.Bloquear ? "bloqueada" : "desbloqueada") + "."))
                    { mensaje = "La linea ya se encuentra " + (req.Bloquear ? "bloqueada" : "desbloqueada") + "." };
                }
                var response = new ResponseGeneric<long>();
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    linea.IdCatEstatusLinea = req.Bloquear ? LineaDic.Cerrado : LineaDic.Abierto;
                    await _context.SaveChangesAsync();

                    _context.LineaMotivos.Add(new LineaMotivo
                    {
                        IdLinea = linea.Id,
                        FechaRegistro = DateTime.Now,
                        IdUserRegistro = user.IdUser,
                        IdCatMotivoLinea = req.IdMotivo,
                        Estatus = linea.IdCatEstatusLinea == LineaDic.Abierto,
                        Notas = linea.IdCatEstatusLinea == LineaDic.Abierto ? "Línea abierta desde el dashboard de líneas" : "Línea cerrada desde el dashboard de líneas"
                    });
                    GenerarAlertaLinea(linea);
                    response.Response = await _context.SaveChangesAsync();

                    var infoVerificentro = _context.vVerificentros.FirstOrDefault(x => x.Id == user.IdVerificentro);
                    if (infoVerificentro == null)
                    {
                        return new ResponseGeneric<long>(new Exception("No sé encontró información del verificentro."))
                        { mensaje = "No sé encontró información del verificentro." };
                    }
                    var envioEvento = await EnviarEventoEntrada(infoVerificentro.ApiEndPoint, infoVerificentro.ApiKey, req.Bloquear ? DictTipoEvento.BloquearLinea : DictTipoEvento.DesbloquearLinea, "Línea " + (req.Bloquear ? "bloqueada " : "desbloqueada ") + "desde el dashboard de lineas.", user.Nombre, infoVerificentro.Nombre, linea.Clave, null);
                    if (envioEvento.Status != ResponseStatus.Success)
                    {
                        await _smadsotGenericInserts.SaveLog(JsonConvert.SerializeObject(envioEvento), DictTipoLog.ExcepcionEventoEntrada);
                        var tipoEvento = MovimientosDicts.DictMovimientoLinea[linea.IdCatEstatusLinea];
                        var mensaje = $"Ocurrió un error en el envío del evento {tipoEvento}.";
                        return new ResponseGeneric<long>(new Exception(mensaje))
                        { mensaje = string.IsNullOrEmpty(envioEvento.mensaje) ? mensaje : envioEvento.mensaje };
                    }
                    scope.Complete();
                }

                response.Status = response.Response > 0 ? ResponseStatus.Success : ResponseStatus.Failed;
                return response;
            }
            catch (Exception)
            {
                var estado = req.Bloquear ? "cerrar" : "abrir";
                var mensaje = $"Ocurrió un error a intentar {estado} la prueba.";
                return new ResponseGeneric<long>(new Exception(mensaje))
                { mensaje = mensaje };
            }

        }
        public async Task<ResponseGeneric<SeguimientoCVVResponse>> GetByDetalleCierre(long IdLinea)
        {
            try
            {
                // var solicitud = _context.vCALineas.Where(x => x.Id == IdLinea);
                var linea = new vLineaGrid();
                if (IdLinea != 0)
                {
                    linea = await _context.vLineaGrids.FirstOrDefaultAsync(x => x.Id == IdLinea) ?? throw new ArgumentException("No se encontró registro de la línea."); ;
                }
                var motivos = await _context.CatMotivoLineas.Where(x => x.Activo).Select(x => new MotivosReponse { Nombre = x.Nombre, Id = x.Id, Activo = x.Activo }).ToListAsync() ?? throw new ArgumentException("No se encontró registro de la línea."); ;

                var result = new SeguimientoCVVResponse
                {
                    Id = linea.Id,
                    IdLineaMotivo = linea.IdLineaMotivo,
                    IdVerificentro = linea.IdVerificentro,
                    NombreVerificentro = linea.NombreVerificentro,
                    IdUserRegistro = linea.IdUserRegistro,
                    NombreUsuario = linea.NombreUsuario,
                    NombreLinea = linea.NombreLinea,
                    FechaRegistroLinea = (DateTime)linea.FechaRegistroLinea,
                    FechaRegistroLineaMotivo = linea.FechaRegistroLineaMotivo ?? DateTime.Now,
                    Estatus = linea.Estatus,
                    Motivo = linea.Motivo,
                    Clave = linea.Clave,
                    UrlDocumento = linea.UrlDocumento,
                    Notas = linea.Notas,
                    Motivos = motivos
                    // UserRegistroMotivo = linea.UserRegistroMotivo,

                };

                return new ResponseGeneric<SeguimientoCVVResponse>(result);
            }
            catch (ArgumentException ex)
            {
                return new ResponseGeneric<SeguimientoCVVResponse>(ex);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<SeguimientoCVVResponse>(ex) { mensaje = "Ocurrió un error al intentar obtener la información." };
            }
        }
        #region Private methods
        private async Task<ResponseGeneric<string>> EnviarEventoEntrada(string ApiEndPoint, string ApiKey, int TipoEvento, string nota, string usuarioTecnico, string verificentro, string Id, PruebaInicio? prueba)
        {
            var serviceVerificentro = new ServicioEventosVerificentro(ApiEndPoint, ApiKey);
            var requestService = new EventoEntradaRequest();
            requestService.Evento = new EventoEntrada
            {
                Fecha = DateTime.Now,
                IdIdentificador = Id,
                IdTipo = TipoEvento,
                Nota = nota,
                TecnicoVerificador = usuarioTecnico
            };
            requestService.Prueba = prueba;
            var responseService = await serviceVerificentro.PostAsync<EventoEntradaRequest, object>("RecepcionEventos/EventoEntrada", requestService, verificentro, $"No se pudo enviar la solicitud al servicio del {verificentro}.");
            if (responseService.Status != ResponseStatus.Success)
            {

                await _smadsotGenericInserts.SaveLog(JsonConvert.SerializeObject(responseService), DictTipoLog.ExcepcionEventoEntrada);

                return new ResponseGeneric<string>() { mensaje = responseService.mensaje, CurrentException = responseService.CurrentException, Status = ResponseStatus.Failed };
            }
            return new ResponseGeneric<string>("", true);
        }
        #endregion
    }
}

public interface ICierreAperturaLineaNegocio
{
    public Task<ResponseGeneric<List<SeguimientoCVVResponse>>> Consulta(SeguimientoCVVListRequest request);

    //public Task<ResponseGeneric<List<SeguimientoCVVResponse>>> GetById(long Id);

    public Task<ResponseGeneric<SeguimientoCVVResponse>> GetByIdCA(long IdLinea);

    Task<ResponseGeneric<long>> Registro(List<SeguimientoCVVResponse> request);
    public Task<ResponseGeneric<long>> DashboardLineaAperturaCierre(DashboardLineaRequest req);

    public Task<ResponseGeneric<ResponseGrid<LineaDetalle>>> DetalleGrid(RequestListLinea req);
    public Task<ResponseGeneric<SeguimientoCVVResponse>> GetByDetalleCierre(long IdLinea);
}