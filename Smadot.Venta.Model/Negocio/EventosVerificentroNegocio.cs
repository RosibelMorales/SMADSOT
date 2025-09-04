using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Namespace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.Entities.Calibracion.Extensions;
using Smadot.Models.Entities.Calibracion.Request;
using Smadot.Models.Entities.FoliosFormaValorada.Request;
using Smadot.Models.Entities.FoliosFormaValorada.Response;
using Smadot.Models.Entities.Verificacion;
using Smadot.Models.GenericProcess;
using Smadot.Utilities;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Utilities.Seguridad;
using Smadot.Venta.Model.Queues;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Globalization;
using System.Text.Json.Nodes;
using System.Transactions;
using System.Web;
using static Smadot.Models.Entities.FoliosCancelados.Request.FoliosCanceladosRequestData;

namespace Smadot.Venta.Model.Negocio
{
    public class EventosVerificentroNegocio : IEventosVerificentroNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;

        private readonly IPdfBuider _pdfBuilder;
        private readonly string siteUrl;
        private readonly string _llaveCifrado;
        private readonly string _version;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;
        private IListGeneric _utilityList;
        public EventosVerificentroNegocio(SmadotDbContext context, IConfiguration configuration, IUserResolver userResolver, IPdfBuider pdfBuilder, SmadsotGenericInserts smadsotGenericInserts, IListGeneric utilityList)
        {
            _context = context;
            _userResolver = userResolver;
            _llaveCifrado = configuration["Encrypter:ClaveSecreta"] ?? "";
            siteUrl = configuration["SiteUrl"] ?? "";
            _pdfBuilder = pdfBuilder;
            _smadsotGenericInserts = smadsotGenericInserts;
            _version = "1.8.1";
            _utilityList = utilityList;
        }
        public async Task<ResponseGeneric<bool>> RegistrarEvento(string clave, EventoSalida req)
        {
            try
            {
                var verificentro = _context.vVerificentros.FirstOrDefault(x => x.Clave == clave) ?? throw new ValidationException("No se encontró un verificentro activo con esa clave.");
                req.TecnicoVerificador = req.TecnicoVerificador.Trim();
                var esEspacio = string.IsNullOrEmpty(req.TecnicoVerificador);
                if (esEspacio)
                {
                    throw new ValidationException("El formato del número de empleado no es correcto.");
                }
                var UserRegistro = _context.vPersonalAutorizacions.OrderByDescending(x => x.Id).FirstOrDefault(x => x.IdVerificentro == verificentro.Id && x.NumeroTrabajador.Trim().Equals(req.TecnicoVerificador.Trim()))
                ?? throw new ValidationException("No se encontró un técnico verificador activo con ese número.");
                var idUserRegistro = UserRegistro.Id;
                var alerta = new Alertum();
                if (req.Fecha.Date != DateTime.Now.Date)
                    throw new ValidationException("El evento que se intenta registrar no tiene la fecha actual.");
                var cambios = false;
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    switch (req.IdTipo)
                    {
                        case DictTipoEvento.PuertaAbierta:
                        case DictTipoEvento.PuertaCerrada:
                            var lineaPuerta = _context.Lineas.FirstOrDefault(x => x.Clave == req.IdIdentificador && x.IdVerificentro == verificentro.Id) ?? throw new ValidationException("No se encontró un equipo con la información proporcionada.");
                            var IdLinea = lineaPuerta?.Id;
                            var res = _context.vEquipos.FirstOrDefault(x => (x.NumeroSerie == req.IdIdentificador && x.IdVerificentro == verificentro.Id) || (x.IdCatTipoEquipo == EquiposDict.MICROBANCA && x.IdLinea == IdLinea && x.IdVerificentro == verificentro.Id))
                            ?? throw new ValidationException("No se encontró un equipo con la información proporcionada.");
                            var nota = string.Format(req.IdTipo == DictTipoEvento.PuertaAbierta ? "{1} Manipulación de equipo Detectada, Motivo: {0}. {3}, {2}" : "{1} Manipulación de equipo Detectada, Motivo: {0}. {3}, {2}. Se solicita apertura de línea, sí la línea esta desactivada", req.Nota, res.NombreEquipo, lineaPuerta?.Nombre, verificentro.Nombre);
                            // if (req.IdTipo == DictTipoEvento.PuertaAbierta)
                            // {
                            var fechaHoy = req.Fecha;
                            var fechaInicio = new DateTime(fechaHoy.Year, fechaHoy.Month, fechaHoy.Day, 0, 0, 0);
                            var fechaFin = new DateTime(fechaHoy.Year, fechaHoy.Month, fechaHoy.Day, 11, 59, 59);
                            var orden = _context.vOrdenServicios.FirstOrDefault(x => x.IdEquipo == res.Id && x.FechaRegistro >= fechaInicio && x.FechaRegistro <= fechaFin);

                            if (orden == null && (lineaPuerta != null || res.IdLinea != null))
                            {
                                if (lineaPuerta == null)
                                {
                                    lineaPuerta = await _context.Lineas.FirstOrDefaultAsync(x => x.Id == res.IdLinea);
                                    if (lineaPuerta == null)
                                    {
                                        throw new ValidationException("No existe la línea del equipo con la información proporcionada.");
                                    }
                                }
                                if (lineaPuerta.IdCatEstatusLinea != LineaDic.Cerrado)
                                {
                                    //lineaPuerta.IdCatEstatusLinea = LineaDic.Cerrado;

                                    //_context.LineaMotivos.Add(new LineaMotivo
                                    //{
                                    //    IdLinea = lineaPuerta.Id,
                                    //    Estatus = false,
                                    //    FechaRegistro = req.Fecha,
                                    //    IdUserRegistro = idUserRegistro,
                                    //    IdCatMotivoLinea = MotivoLinea.Irregularidad,
                                    //    Notas = nota
                                    //});
                                    //await _context.SaveChangesAsync();
                                    //var envioEvento = await EnviarEventoEntrada(verificentro.ApiEndPoint, verificentro.ApiKey, DictTipoEvento.DesbloquearLinea, "Linea cerrada por apertura de Equipo no autorizada.", req.TecnicoVerificador, verificentro.Nombre, lineaPuerta.Clave, null);
                                    //if (envioEvento.Status == ResponseStatus.Failed)
                                    //{
                                    //    alerta = new Alertum
                                    //    {
                                    //        TableName = DictAlertas.Linea,
                                    //        TableId = lineaPuerta.Id,
                                    //        IdVerificentro = verificentro.Id,
                                    //        Data = null,
                                    //        IdUser = null,
                                    //        MovimientoInicial = string.Format("Error al intentar enviar evento de bloqueo de la línea en el software del proveedor, Datos: {0}. {2}, {1}.", envioEvento.mensaje, lineaPuerta.Nombre, verificentro.Nombre),
                                    //        Fecha = req.Fecha,
                                    //        IdEstatusFinal = null,
                                    //        MovimientoFinal = null,
                                    //        Leido = false,
                                    //        Procesada = false
                                    //    };
                                    //    await _smadsotGenericInserts.SaveLog(JsonConvert.SerializeObject(envioEvento), DictTipoLog.FalloPuertaAbierta);
                                    //}
                                    alerta = new Alertum
                                    {
                                        TableName = DictAlertas.Equipo,
                                        TableId = res.Id,
                                        IdVerificentro = verificentro.Id,
                                        Data = null,
                                        IdUser = null,
                                        MovimientoInicial = nota,
                                        Fecha = req.Fecha,
                                        IdEstatusFinal = null,
                                        MovimientoFinal = null,
                                        Leido = false,
                                        Procesada = false
                                    };
                                }

                            }
                            // }

                            cambios = true;
                            break;
                            // case DictTipoEvento.DesbloquearLinea:
                            // case DictTipoEvento.BloquearLinea:
                            //     if (!req.IdIdentificador.Contains('L'))
                            //         req.IdIdentificador = $"L{req.IdIdentificador.Trim()}";
                            //     var lineaBloqueo = _context.Lineas.FirstOrDefault(x => x.Clave == req.IdIdentificador.Trim() && x.IdVerificentro == verificentro.Id) ?? throw new ValidationException("No se encontró una linea con la información proporcionada.");
                            //     var notaBloqueo = string.Format(req.IdTipo == DictTipoEvento.BloquearLinea ? "Linea Bloqueada, Motivo: {0}. {2}, {1}." : "Solicita desbloqueo de linea, Motivo: {0}. {2}, {1}.", req.Nota, lineaBloqueo.Nombre, verificentro.Nombre);
                            //     if (req.IdTipo == DictTipoEvento.BloquearLinea)
                            //     {
                            //         if (lineaBloqueo.IdCatEstatusLinea != LineaDic.Cerrado)
                            //         {
                            //             lineaBloqueo.IdCatEstatusLinea = LineaDic.Cerrado;

                            //             _context.LineaMotivos.Add(new LineaMotivo
                            //             {
                            //                 IdLinea = lineaBloqueo.Id,
                            //                 Estatus = false,
                            //                 FechaRegistro = req.Fecha,
                            //                 IdUserRegistro = idUserRegistro,
                            //                 IdCatMotivoLinea = MotivoLinea.Irregularidad,
                            //                 Notas = notaBloqueo
                            //             });
                            //             await _context.SaveChangesAsync();
                            //             // throw new ValidationException("La linea ya se encuentra bloqueada.");
                            //         }

                            //     }

                            //     alerta = new Alertum
                            //     {
                            //         TableName = DictAlertas.Linea,
                            //         TableId = lineaBloqueo.Id,
                            //         IdVerificentro = verificentro.Id,
                            //         Data = null,
                            //         IdUser = null,
                            //         MovimientoInicial = notaBloqueo,
                            //         Fecha = req.Fecha,
                            //         IdEstatusFinal = null,
                            //         MovimientoFinal = null,
                            //         Leido = false,
                            //         Procesada = false
                            //     };
                            //     cambios = true;
                            //     break;
                            //     case DictTipoEvento.CalibracionRequerida:
                            //     case DictTipoEvento.CalibracionFinalizada:
                            //         var Equipo = _context.vEquipos.FirstOrDefault(x => x.NumeroSerie.Trim().Equals(req.IdIdentificador.Trim()) && x.IdVerificentro == verificentro.Id) ?? throw new ValidationException("No se encontró el equipo con la información proporcionada."); ;

                            //         if (req.IdTipo == DictTipoEvento.CalibracionFinalizada)
                            //         {
                            //             var calibracion = JsonConvert.DeserializeObject<CalibracionProveedorRequest>(req.Data) ?? throw new ValidationException("No se pudo deserializar la información del evento.");
                            //             var tipoCalibracion = _context.EquipoTipoCalibracions.OrderByDescending(x => x.Id).FirstOrDefault(x => x.IdUserValido != null && x.IdEquipo == Equipo.Id);
                            //             // Creamos el tipo de calibración para el equipo si no existe.
                            //             if (tipoCalibracion == null)
                            //             {
                            //                 tipoCalibracion = new EquipoTipoCalibracion
                            //                 {
                            //                     FechaRegistro = DateTime.Now,
                            //                     IdEquipo = Equipo.Id,
                            //                     PrimeraFechaCalibracion = calibracion.FechaCalibracionParse,
                            //                     IdUserRegistro = idUserRegistro,
                            //                     IdUserValido = idUserRegistro,
                            //                     IdCatTipoCalibracion = GenerarPeriodoCalibracion(calibracion.FechaCalibracionParse, calibracion.FechaProximaCalibracioParse)
                            //                 };
                            //                 _context.EquipoTipoCalibracions.Add(tipoCalibracion);
                            //                 await _context.SaveChangesAsync();

                            //             }
                            //             var ultimaCalibracion = _context.vCalibracions.OrderByDescending(x => x.Id)
                            //                                                         .FirstOrDefault(x => x.IdEquipoTipoCalibracion == tipoCalibracion.Id);
                            //             // DateTime fechaProgramada = ultimaCalibracion?.FechaProximaCalibracion ?? tipoCalibracion.PrimeraFechaCalibracion;
                            //             // DateTime fechaProximaCalibracion = CalibracionExtension.ObtenerProximaFechaCalibracion(fechaProgramada, tipoCalibracion.IdCatTipoCalibracion);
                            //             var currentDate = DateTime.Now;
                            //             DateTime inicioDia = new(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);
                            //             DateTime finDia = new(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59);
                            //             int consecutivo = (ultimaCalibracion?.Consecutivo ?? 0) + 1;
                            //             var vcalibracionDb = _context.vCalibracions.OrderByDescending(x => x.Id)
                            //                                                                             .FirstOrDefault(x => x.IdEquipo == Equipo.Id && ((x.FechaRegistro >= inicioDia && x.FechaRegistro <= finDia) || x.IdCatEstatusCalibracion == CalibracionEstatus.PendienteCalibracion));
                            //             Calibracion calibracionDb = new()
                            //             {
                            //                 IdEquipoTipoCalibracion = tipoCalibracion.Id,
                            //                 Consecutivo = consecutivo,
                            //                 FechaCalibracion = calibracion.FechaCalibracionParse,
                            //                 FechaProgramada = calibracion.FechaCalibracionParse,
                            //                 FechaProximaCalibracion = calibracion.FechaProximaCalibracioParse,
                            //                 IdUserRegistro = idUserRegistro,
                            //                 FechaRegistro = DateTime.Now,
                            //                 IdUserValido = null,
                            //                 Nota = string.IsNullOrEmpty(calibracion.MotivoFalla) ? req.Nota : calibracion.MotivoFalla,
                            //                 IdCatEstatusCalibracion = !calibracion.CalibracionExitosa ? CalibracionEstatus.CalibracionFallida : CalibracionEstatus.PermiteModificar,
                            //                 NumeroCertificado = null,
                            //                 FechaEmisionCertificad = null
                            //             };
                            //             if (vcalibracionDb == null)
                            //                 await _context.Calibracions.AddAsync(calibracionDb);
                            //             else
                            //             {
                            //                 calibracionDb.Id = vcalibracionDb.Id;
                            //                 calibracionDb.Id = vcalibracionDb.Consecutivo;
                            //                 _context.Calibracions.Update(calibracionDb);
                            //             }
                            //             var EquipoDb = _context.Equipos.FirstOrDefault(x => x.Id == Equipo.Id);
                            //             if (EquipoDb != null && calibracion.CalibracionExitosa && EquipoDb.IdCatEstatusEquipo != EstatusEquipo.Activo)
                            //             {
                            //                 EquipoDb.IdCatEstatusEquipo = EstatusEquipo.Activo;
                            //             }
                            //             else if (EquipoDb != null && (EquipoDb.IdCatEstatusEquipo == EstatusEquipo.Activo || EquipoDb.IdCatEstatusEquipo == EstatusEquipo.DocumentacionInvalida))
                            //             {
                            //                 EquipoDb.IdCatEstatusEquipo = EstatusEquipo.SinCalibrar;

                            //             }
                            //             // if (calibracion.CalibracionExitosa)
                            //             // {
                            //             //     Equipo.IdCatEstatusEquipo = 3;
                            //             // }
                            //             alerta = new Alertum
                            //             {
                            //                 TableName = DictAlertas.Calibracion,
                            //                 TableId = Equipo.Id,
                            //                 IdVerificentro = verificentro.Id,
                            //                 Data = JsonConvert.SerializeObject(req, new JsonSerializerSettings
                            //                 {
                            //                     ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            //                     PreserveReferencesHandling = PreserveReferencesHandling.None,
                            //                     NullValueHandling = NullValueHandling.Ignore
                            //                 }),
                            //                 IdUser = null,
                            //                 MovimientoInicial = calibracion.CalibracionExitosa ? string.Format("Calibración Fallida, Nota:{0} {1}", req.Nota, calibracion.MotivoFalla) : string.Format("Calibración Finalizada, Nota:{0} {1}", req.Nota, calibracion.MotivoFalla),
                            //                 Fecha = req.Fecha,
                            //                 IdEstatusFinal = null,
                            //                 MovimientoFinal = null,
                            //                 Leido = false,
                            //                 Procesada = false
                            //             };
                            //         }
                            //         else
                            //         {
                            //             // var cal = _context.Calibracions.FirstOrDefault(x => x.IdEquipoTipoCalibracionNavigation.IdEquipoNavigation.NumeroSerie == req.IdIdentificador && x.IdEquipoTipoCalibracionNavigation.IdEquipoNavigation.IdVerificentro == verificentro.Id)?.Id;
                            //             // if (cal == null)
                            //             // {
                            //             //     return new ResponseGeneric<bool>
                            //             //     {
                            //             //         Response = false,
                            //             //         mensaje = "No se encontró un equipo con la información proporcionada.",
                            //             //         Status = ResponseStatus.Failed,
                            //             //     };
                            //             // }
                            //             var EquipoDb = _context.Equipos.FirstOrDefault(x => x.Id == Equipo.Id);
                            //             EquipoDb.IdCatEstatusEquipo = EstatusEquipo.SinCalibrar;
                            //             alerta = new Alertum
                            //             {
                            //                 TableName = DictAlertas.Equipo,
                            //                 TableId = Equipo.Id,
                            //                 IdVerificentro = verificentro.Id,
                            //                 Data = JsonConvert.SerializeObject(req, new JsonSerializerSettings
                            //                 {
                            //                     ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            //                     PreserveReferencesHandling = PreserveReferencesHandling.None,
                            //                     NullValueHandling = NullValueHandling.Ignore
                            //                 }),
                            //                 IdUser = null,
                            //                 IdEstatusInicial = CalibracionEstatus.PendienteCalibracion,
                            //                 MovimientoInicial = string.Format(MovimientosDicts.DictMovimientoCalibracion[CalibracionEstatus.PendienteCalibracion], Equipo.NombreEquipo, $"{Equipo.NombreLinea} " + verificentro.Nombre, DateTime.Now.ToString("d")),
                            //                 Fecha = req.Fecha,
                            //                 IdEstatusFinal = null,
                            //                 MovimientoFinal = null,
                            //                 Leido = false,
                            //                 Procesada = false
                            //             };
                            //         }
                            //         cambios = true;
                            //         break;
                    }
                    var result = true;
                    if (cambios)
                    {
                        _context.Alerta.Add(alerta);
                        result = await _context.SaveChangesAsync() > 0;
                        transaction.Complete();
                    }
                    else
                    {
                        transaction.Dispose();
                    }
                    return new ResponseGeneric<bool>(result);
                }
            }
            catch (ValidationException e)
            {
                await _smadsotGenericInserts.SaveLog(e, DictTipoLog.ExcepcionEventoVerificentro);

                return new ResponseGeneric<bool>
                {
                    Response = false,
                    mensaje = e.Message,
                    Status = ResponseStatus.Success,
                };
            }
            catch (Exception ex)
            {
                await _smadsotGenericInserts.SaveLog(ex, req, DictTipoLog.ExcepcionEventoVerificentro);
                return new ResponseGeneric<bool>
                {
                    Response = false,
                    mensaje = ex.Message + " " + ex.StackTrace,
                    Status = ResponseStatus.Success,
                };
            }
        }
        public async Task<ResponseGeneric<bool>> ActualizarPrueba(string clave, Prueba req)
        {
            try
            {
                await _smadsotGenericInserts.SaveLogPrueba(req, clave);
                await _context.SaveChangesAsync();

                Verificacion? verificacion = _context.Verificacions.Include(x => x.ResultadosVerificacion).Include(x => x.ParametrosTablaMaestraVerificacion).FirstOrDefault(x => x.Id == req.Id);
                if (verificacion == null)
                {
                    return new ResponseGeneric<bool>(false) { mensaje = "No se encontró el registro de la verificación en el sistema." };
                }
                ResultadosVerificacion? dbRes = verificacion.ResultadosVerificacion;
                if (dbRes == null)
                {
                    return new ResponseGeneric<bool>(false) { mensaje = "No se encontró el registro de los resultados de la verificación en el sistema." };
                }

                var resultadosFinales = _context.vVerificacionDatosProveedors
                    .FirstOrDefault(x => x.IdVerificacion == req.Id);
                if (resultadosFinales == null)
                {
                    return new ResponseGeneric<bool>(false) { mensaje = "No existen o no se han guardado lo resultados finales de la prueba." };
                }

                if (verificacion?.ParametrosTablaMaestraVerificacion == null)
                {
                    return new ResponseGeneric<bool>(false) { mensaje = "No existen párametros de evaluación para la prueba." };
                }

                //Información del verificentro para mandar las alertas
                var verificentro = _context.Verificentros.FirstOrDefault(x => x.Clave.Equals(clave));

                //dbRes.RESULTADO = Resultados.Escapado;

                if (req.C_RECHAZO == CausaRechazo.CertificadoAnteriorRobado)
                {
                    //dbRes.RESULTADO = Resultados.Escapado;
                    var alertaAbort = new Alertum
                    {
                        TableName = DictAlertas.Cita,
                        TableId = 0,
                        IdVerificentro = verificentro.Id,
                        Data = null,
                        IdUser = null,
                        IdEstatusInicial = 0,
                        MovimientoInicial = string.Format("Se detectó el uso de un certificado robado en el auto con placas {0}, número de serie {1} y propietario {2}", resultadosFinales.Placa, resultadosFinales.Serie, resultadosFinales.Propietario ?? resultadosFinales.RazonSocial),
                        Fecha = DateTime.Now,
                        IdEstatusFinal = null,
                        MovimientoFinal = null,
                        Leido = false,
                        Procesada = false
                    };
                    _context.Add(alertaAbort);

                    // var alerta2 = new Alertum
                    // {
                    //     TableName = DictAlertas.ActualizacionPrueba,
                    //     TableId = 0,
                    //     IdVerificentro = verificentro.Id,
                    //     Data = null,
                    //     IdUser = null,
                    //     IdEstatusInicial = 0,
                    //     MovimientoInicial = "Actualización Prueba",
                    //     Fecha = DateTime.Now,
                    //     IdEstatusFinal = null,
                    //     MovimientoFinal = null,
                    //     Leido = false,
                    //     Procesada = false
                    // };
                    // _context.Add(alerta2);

                    await _context.SaveChangesAsync();

                    return new ResponseGeneric<bool>(false) { mensaje = "El vehículo cuenta con un certificado robado." };
                }

                if (dbRes.EstatusPrueba != EstatusVerificacion.VerificacionFinalizada && dbRes.EstatusPrueba != EstatusVerificacion.VerificacionAbortada && dbRes.EstatusPrueba != EstatusVerificacion.FolioImpreso)
                {

                    dbRes.RESULTADO_PROVEEDOR = req.RESULTADO;
                    dbRes.FinalizacionPruebas = DateTime.Now;
                    var fechapruebaInicio = dbRes.InicioPruebas;
                    if ((req.C_RECHAZO_OBD ?? 0) != OBD.SinCodigoError)
                    {
                        dbRes.PruebaObd = true;
                    }
                    var parentProperties = req.GetType().GetProperties();
                    var childProperties = dbRes.GetType().GetProperties();
                    if (req.Protocolo != verificacion.ParametrosTablaMaestraVerificacion.PROTOCOLO)
                    {
                        verificacion.ParametrosTablaMaestraVerificacion.PROTOCOLO = req.Protocolo ?? resultadosFinales.PROTOCOLO;
                        vLineaEquipo? equipo = null;
                        switch (verificacion.ParametrosTablaMaestraVerificacion.PROTOCOLO)
                        {
                            case Protocolos.ACELERACIONDIESEL:
                            case Protocolos.DIESELSINDATOS:
                                equipo = await _context.vLineaEquipos.FirstOrDefaultAsync(x => x.IdLinea == verificacion.IdLinea && x.Nombre.Contains(EquiposDict.Equipos[EquiposDict.OPACIMETRO]));
                                break;
                            case Protocolos.DINAMICO:
                                equipo = await _context.vLineaEquipos.FirstOrDefaultAsync(x => x.IdLinea == verificacion.IdLinea && x.Nombre.Contains(EquiposDict.Equipos[EquiposDict.DINAMOMETRO]));
                                break;
                            case Protocolos.ESTATICO:
                                equipo = await _context.vLineaEquipos.FirstOrDefaultAsync(x => x.IdLinea == verificacion.IdLinea && x.IdCatTipoEquipo == EquiposDict.TACOMETROENCENDEDOR || x.IdCatTipoEquipo == EquiposDict.TACOMETRONOCONTACTO || x.IdCatTipoEquipo == EquiposDict.TACOMETROPINZAS || x.IdCatTipoEquipo == EquiposDict.TACOMETROTRANSDUCTOR);
                                break;
                        }
                        verificacion.IdEquipoVerificacion = equipo?.Id;
                    }
                    foreach (var parentProperty in parentProperties)
                    {
                        var prop = childProperties.FirstOrDefault(x => x.Name.ToLower().Equals(parentProperty.Name.ToLower()));
                        if (prop != null &&
                            !prop.Name.Equals("Id") &&
                            !prop.Name.Equals("FugasSistemaEscape") &&
                            !prop.Name.Equals("PortafiltroAire") &&
                            !prop.Name.Equals("FiltroAire") &&
                            !prop.Name.Equals("TaponDispositivoAceite") &&
                            !prop.Name.Equals("TaponCombustible") &&
                            !prop.Name.Equals("Bayoneta") &&
                            !prop.Name.Equals("FugaAceiteMotor") &&
                            !prop.Name.Equals("FugaAceiteTransmision") &&
                            !prop.Name.Equals("FugaLiquidoRefrigerante") &&
                            !prop.Name.Equals("DibujoNeumaticos") &&
                            !prop.Name.Equals("DesperfectosNeumaticos") &&
                            !prop.Name.Equals("DimensionesNeumaticoIncorrectas") &&
                            !prop.Name.Equals("ControlEmisionDesconectados") &&
                            !prop.Name.Equals("ControlEmisionAlterados") &&
                            !prop.Name.Equals("PlacasCorrespondientes") &&
                            !prop.Name.Equals("NumeroEscapes") &&
                            !prop.Name.Equals("InicioPruebas") &&
                            !prop.Name.Equals("FinalizacionPruebas")
                            )
                        {

                            if (
                            //prop.Name.Equals("RESULTADO") ||
                            prop.Name.Equals("C_RECHAZO") ||
                            prop.Name.Equals("C_RECHAZO_OBD"))
                            {

                                if (req.RESULTADO != null || req.C_RECHAZO != null || req.C_RECHAZO_OBD != null)
                                    prop.SetValue(dbRes, parentProperty.GetValue(req));
                            }
                            else
                            {
                                prop.SetValue(dbRes, parentProperty.GetValue(req));

                            }
                        }
                    }
                    dbRes.FUERZA = null;
                    dbRes.InicioPruebas = fechapruebaInicio;

                    dbRes.PruebaOpacidad = req.PruebaDiesel ?? false;
                    switch (dbRes.EstatusPrueba)
                    {
                        case EstatusVerificacion.EnPruebaEstaticaDinamica:
                        case EstatusVerificacion.TerminaPruebaEstaticaDinamica:
                            dbRes.PruebaEmisiones = true;
                            break;
                        case EstatusVerificacion.TerminaPruebaOBD:
                            dbRes.PruebaObd = true;
                            break;
                        case EstatusVerificacion.EnPruebaOpacidad:
                        case EstatusVerificacion.TerminaPruebaOpacidad:
                            dbRes.PruebaOpacidad = true;
                            break;
                    }
                    dbRes.EstatusPrueba = (int)req.IdEstatus;
                    _context.ResultadosVerificacions.Update(dbRes);
                    verificacion.FechaVerificacion = DateTime.Now;
                    _context.Verificacions.Update(verificacion);
                    //await _context.SaveChangesAsync();
                    //var generateFV = await GenerarFolioFV(verificacion, resultadosFinales, dbRes);

                    //if (!generateFV.Response)
                    //{
                    //    return generateFV;
                    //}
                }
                await _context.SaveChangesAsync();

                var result = true;
                return new ResponseGeneric<bool>(result);
            }
            // var verificacion = _context.Verificacions.Include(r => r.IdResultadosVerificacionNavigation).FirstOrDefault(x => x.Id == req.Id);
            // var resultadosFinales = _context.vVerificacionDatosProveedors.FirstOrDefault(x => x.IdVerificacion == req.Id);

            //Información del verificentro para mandar las alertas
            //     var verificentro = _context.Verificentros.FirstOrDefault(x => x.Clave.Equals(clave));
            //     _context.ResultadosVerificacions.Update(dbRes);
            //     await _context.SaveChangesAsync();
            //     if (req.C_RECHAZO == CausaRechazo.CertificadoAnteriorRobado)
            //     {
            //         var alertaAbort = new Alertum
            //         {
            //             TableName = DictAlertas.Cita,
            //             TableId = 0,
            //             IdVerificentro = verificentro.Id,
            //             Data = null,
            //             IdUser = null,
            //             IdEstatusInicial = 0,
            //             MovimientoInicial = string.Format("Se detecto el uso de un certificado robado en el auto con placas {0}, número de serie {1} y propietario {2}", resultadosFinales.Placa, resultadosFinales.Serie, resultadosFinales.Propietario ?? resultadosFinales.RazonSocial),
            //             Fecha = DateTime.Now,
            //             IdEstatusFinal = null,
            //             MovimientoFinal = null,
            //             Leido = false
            //         };
            //         _context.Add(alertaAbort);
            //         var alerta2 = new Alertum
            //         {
            //             TableName = DictAlertas.ActualizacionPrueba,
            //             TableId = 0,
            //             IdVerificentro = verificentro.Id,
            //             Data = null,
            //             IdUser = null,
            //             IdEstatusInicial = 0,
            //             MovimientoInicial = "Actualización Prueba",
            //             Fecha = DateTime.Now,
            //             IdEstatusFinal = null,
            //             MovimientoFinal = null,
            //             Leido = false
            //         };
            //         _context.Add(alerta2);

            //         await _context.SaveChangesAsync();
            //         return new ResponseGeneric<bool>(false) { mensaje = "El vehículo cuenta con un certificado robado." };
            //     }
            //     if (req.IdEstatus == EstatusVerificacion.VerificacionFinalizada || req.IdEstatus == EstatusVerificacion.VerificacionAbortada)
            //     {
            //         dbRes.FinalizacionPruebas = DateTime.Now;
            //         _context.ResultadosVerificacions.Update(dbRes);
            //         await _context.SaveChangesAsync();
            //         var generateFV = await GenerarFolioFV(verificacion, resultadosFinales, dbRes);
            //         if (!generateFV.Response)
            //         {
            //             return generateFV;
            //         }
            //     }
            //     var result = true;
            //     return new ResponseGeneric<bool>(result);
            // }
            catch (Exception ex)
            {
                await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionResultadosVerificacion);
                return new ResponseGeneric<bool>(false) { mensaje = ex.Message + ex.StackTrace }; ;
            }
        }
        public async Task<ResponseGeneric<bool>> ReimpresionFV(FolioCanceladosRequest request)
        {
            try
            {

                var result = new ResponseGeneric<bool>();
                DateTime parseDateTime;

                if (request.IdFolio != null)
                {

                    var fv = _context.FoliosFormaValoradaVerificentros.FirstOrDefault(x => x.Id == request.IdFolio);
                    if (fv == null)
                    {
                        result.mensaje = "No se encontró el Folio indicado";
                        result.Response = false;
                        return result;
                    }
                    if (string.IsNullOrEmpty(request.FechaCancelacion))
                    {
                        parseDateTime = DateTime.Now;
                    }
                    else
                    {
                        request.FechaCancelacion = request.FechaCancelacion;
                        var tryparseDate = DateTime.TryParseExact(request.FechaCancelacion, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parseDateTime);

                    }
                    if (request.MotivoCancelacion == 3)
                    {
                        request.OtroMotivo = string.IsNullOrEmpty(request.OtroMotivo) ? "Sin especificar" : request.OtroMotivo;
                    }
                    else
                    {
                        request.OtroMotivo = null;
                    }
                    var count = _context.FoliosFormaValoradaVerificentros.Where(x => x.Cancelado && x.IdCatTipoTramite != null).Count() + 1;
                    fv.FechaCancelacion = parseDateTime;
                    fv.IdCatMotivoCancelacion = request.MotivoCancelacion;
                    fv.OtroMotivo = request.OtroMotivo;
                    fv.Cancelado = true;
                    fv.ClaveTramiteCancelado = $"{TipoTramite.Dict[TipoTramite.CANCELADO]}";
                    fv.ConsecutivoTramiteCancelado = count;
                    fv.IdUserCancelo = _userResolver.GetUser().IdUser;
                    var generarFV = await GenerarFolioFV(fv.IdVerificacion ?? 0, false);
                    if (generarFV.Status != ResponseStatus.Success || generarFV.Response == 0)
                    {
                        result.mensaje = generarFV.mensaje;
                        result.Response = false;
                    }
                    await _context.SaveChangesAsync();
                    result.mensaje = "El folio se volvió a generar correctamente.";
                    result.Response = true;
                }
                else
                {
                    var fv = _context.FoliosFormaValoradaVerificentros.FirstOrDefault(x => x.IdVerificacion == request.IdVerificacion && !x.Cancelado && !x.Reposicion);
                    if (fv != null)
                    {
                        result.mensaje = "Se ha encontrado un folio asignado a la verificación. Vuelva a intentarlo para reimprimir y cancelar el folio anterior.";
                        result.Response = false;
                        return result;
                    }

                    var generarFV = await GenerarFolioFV(request.IdVerificacion ?? 0, true);
                    if (generarFV.Status != ResponseStatus.Success || generarFV.Response == 0)
                    {
                        result.mensaje = generarFV.mensaje;
                        result.Response = false;
                    }
                }
                result.mensaje = "El Folio se generó correctamente.";
                result.Response = true;

                return result;
            }
            catch (Exception ex)
            {
                await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionReimpresionFolio);

                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> GenerarFolioFV(long IdVerificacion, bool validar)
        {
            try
            {
                var fechaHoy = DateTime.Now;
                Verificacion? verificacion = await _context.Verificacions.Include(x => x.ResultadosVerificacion).FirstOrDefaultAsync(x => x.Id == IdVerificacion);
                FoliosFormaValoradaVerificentro? folio = await _context.FoliosFormaValoradaVerificentros.FirstOrDefaultAsync(x => x.IdVerificacion == IdVerificacion);
                long IdfolioCreado = 0;
                // #if !DEBUG
                if (folio != null && validar)
                {
                    return new ResponseGeneric<long>(folio.Id);
                }
                // #endif
                if (verificacion == null)
                {
                    return new ResponseGeneric<long>($"No sé encontró la verificación relacionada a al certificado que se quiere generar.", true) { Status = ResponseStatus.Failed, mensaje = $"No sé encontró la verificación relacionada a al certificado que se quiere generar." };
                }
                ResultadosVerificacion? dbRes = verificacion.ResultadosVerificacion;
                if (dbRes == null)
                {
                    return new ResponseGeneric<long>($"No se encontraron resultados de la verificación.", true) { Status = ResponseStatus.Failed, mensaje = $"No se encontraron resultados de la verificación." };
                }
                if (dbRes.EstatusPrueba == EstatusVerificacion.VerificacionAbortada && (dbRes.C_RECHAZO == 0 || dbRes.C_RECHAZO == null || dbRes.C_RECHAZO == CausaRechazo.AbortadaTecnico))
                {
                    return new ResponseGeneric<long>($"La prueba no se completo exitosamente. Favor de reiniciar la prueba.", true) { Status = ResponseStatus.Failed, mensaje = $"La prueba no se completo exitosamente. Favor de reiniciar la prueba." };
                }
                // #if !DEBUG
                else if (dbRes.EstatusPrueba != EstatusVerificacion.VerificacionAbortada && dbRes.EstatusPrueba != EstatusVerificacion.VerificacionFinalizada && dbRes.EstatusPrueba != EstatusVerificacion.FolioImpreso)
                {
                    return new ResponseGeneric<long>(0) { mensaje = "La prueba aún no finaliza." };
                }
                else if (dbRes.EstatusPrueba == EstatusVerificacion.FolioImpreso && folio != null && validar)
                {
                    return new ResponseGeneric<long>(0) { mensaje = "El folio ya fue generado." };
                }
                //#endif

                vVerificacionDatosLimite? resultadosFinales = await _context.vVerificacionDatosLimites.FirstOrDefaultAsync(x => x.IdVerificacion == IdVerificacion);
                if (resultadosFinales == null)
                {
                    return new ResponseGeneric<long>($"No sé encontraron los parámetros de evaluación de la verificación.", true) { Status = ResponseStatus.Failed, mensaje = $"No sé encontraron los parámetros de evalucación de la verificación." };
                    //return new ResponseGeneric<long>(0) { mensaje = "No sé encontraron los parámetros de evalucación de la verificación." };
                }
                // Se agrega a una lista fija para no generar doble folio o más
                if (_utilityList.GetColaVerifcacion().Any(x => x == IdVerificacion))
                    return new ResponseGeneric<long>(0) { mensaje = "Los resultados de la verificación están siendo procesados par asignar el certificado." };
                _utilityList.AddColaVerificacion(IdVerificacion);
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    _context.ChangeTracker.AutoDetectChangesEnabled = false;
                    var folios = _context.vVerificacionReposicions
                        .Where(x => x.Placa != null && x.Placa.Equals(verificacion.Placa.Trim())
                            && x.Serie != null && x.Serie.Equals(verificacion.Serie.Trim())
                            && x.IdReposicion == null && !(x.Cancelado ?? false) && x.IdVerificacion != verificacion.Id);

                    if (!folios.Any(x => x.IdVerificacion == verificacion.Id))
                    {
                        if (CausaRechazo.DictPruebaAbortada.Any(x => x.Key == verificacion.ResultadosVerificacion?.C_RECHAZO) || dbRes.EstatusPrueba == EstatusVerificacion.VerificacionAbortada)
                        {

                            if (verificacion.ResultadosVerificacion?.C_RECHAZO == CausaRechazo.Visual && verificacion.ResultadosVerificacion?.C_RECHAZO > OBD.SinCodigoError)
                            {
                                dbRes.C_RECHAZO = CausaRechazo.OBD;
                            }
                            if (dbRes.C_RECHAZO == 0 || dbRes.C_RECHAZO == null)
                            {
                                // Se elimina de la lista para poder volver a procesar los resultados de la verificación
                                _utilityList.RemoveColaVerificacion(IdVerificacion);
                                return new ResponseGeneric<long>($"La prueba no se completo exitosamente. Favor de reiniciar la prueba.", true) { Status = ResponseStatus.Failed, mensaje = $"La prueba no se completo exitosamente. Favor de reiniciar la prueba." };
                            }
                            dbRes.RESULTADO = Resultados.Rechazo;
                            var validado = await _smadsotGenericInserts.ValidateFolio(
                                TipoCertificado.ConstanciasNoAprobado,
                                resultadosFinales.IdVerificentro,
                                TipoTramite.CV,
                                verificacion.IdUserTecnico ?? 0,
                                resultadosFinales.Estado ?? "Puebla",
                                verificacion.Id,
                                null,
                                null
                            );
                            if (!validado.IsSucces)
                            {
                                _utilityList.RemoveColaVerificacion(IdVerificacion);
                                return new ResponseGeneric<long>(0) { mensaje = validado.Description };
                            }

                            verificacion.Vigencia = fechaHoy.AddDays(30);
                        }
                        else
                        {
                            bool candidatoDoblecero = false;
                            bool refrendoDobleCero = false;
                            if (resultadosFinales?.DOBLECERO > 0
                                && !new List<int> { TipoServicio.TRANSPPASAJEPUBINDIVIDUAL, TipoServicio.TRANSPPASAJEPUBCOLECTIVO, TipoServicio.TRANSPPASAJEMERCANTILYPRIV, TipoServicio.TRANSPCARGAPUBMERCPRIVPA }.Contains(resultadosFinales?.IdCatTipoServicio ?? -1))
                            {
                                var vecesRecibioDobleCero = folios.Count(x => x.IdCatTipoCertificado == TipoCertificado.DobleCero);
                                refrendoDobleCero = vecesRecibioDobleCero < 2 && (resultadosFinales.REF_00 > 0) && verificacion.IdMotivoVerificacion == MotivoVerificacionDict.HologramaDobleceroRefrendo;
                                candidatoDoblecero = refrendoDobleCero || (verificacion.Anio >= fechaHoy.Year - 2 && resultadosFinales.DOBLECERO >= 1 &&
                                (verificacion.IdMotivoVerificacion == MotivoVerificacionDict.HologramaDoblecero || verificacion.IdMotivoVerificacion == MotivoVerificacionDict.NuevoSinVerificacionAnterior || verificacion.IdMotivoVerificacion == MotivoVerificacionDict.RECHAZOHOLOGRAMADOBLECERO));
                            }

                            var c_rechazoobd = await ValidarOBD(resultadosFinales);
                            var c_rechazo = c_rechazoobd == 1 ? CausaRechazo.OBD : CausaRechazo.NoAplica;
                            var procesoResultados = await _smadsotGenericInserts.ValidarResultados(resultadosFinales.PROTOCOLO,
                                resultadosFinales.PBV_EQUIV,
                                resultadosFinales.Anio,
                                resultadosFinales.OPACIDADK ?? 0M,
                                resultadosFinales.PruebaObd ?? false,
                                resultadosFinales.PruebaEmisiones ?? false,
                                resultadosFinales.PruebaOpacidad ?? false,
                                c_rechazo,
                                c_rechazoobd,
                                //resultadosFinales.C_RECHAZO_OBD ?? 0,
                                resultadosFinales.LAMDA_2540 ?? 0,
                                resultadosFinales.LAMDA_5024 ?? 0,
                                resultadosFinales.HC_2540 ?? 0,
                                resultadosFinales.HC_5024 ?? 0,
                                resultadosFinales.CO_2540 ?? 0,
                                resultadosFinales.CO_5024 ?? 0,
                                resultadosFinales.NO_2540 ?? 0,
                                resultadosFinales.NO_5024 ?? 0,
                                resultadosFinales.O2_2540 ?? 0,
                                resultadosFinales.O2_5024 ?? 0,
                                resultadosFinales.COCO2_2540 ?? 0,
                                resultadosFinales.COCO2_5024 ?? 0,
                                resultadosFinales.IdTipoCombustible ?? 0,
                                candidatoDoblecero);
                            // Validamos que la evaluación de resultados sea correcta.
                            if (procesoResultados.Error)
                            {
                                await _smadsotGenericInserts.SaveLog("{" + $"Data Prueba: {procesoResultados.Mensaje}" + "}", DictTipoLog.LogLimitesNoEncontrados);
                                _utilityList.RemoveColaVerificacion(IdVerificacion);

                                return new ResponseGeneric<long>(0) { mensaje = procesoResultados.Mensaje };
                            }
                            // verificacion.IdLimiteVerificacion = procesoResultados.IdLimiteVerificacion;
                            if (resultadosFinales.IdLimiteVerificacionParametro == null)
                            {
                                var limiteVerificacion = new LimiteVerificacionParametro()
                                {
                                    IdVerificacion = verificacion.Id,
                                    IdCatTipoCertificado = procesoResultados.LimiteVerificacion.IdCatTipoCertificado,
                                    AnioInicio = procesoResultados.LimiteVerificacion.AnioInicio,
                                    AnioFin = procesoResultados.LimiteVerificacion.AnioFin,
                                    PBVMin = procesoResultados.LimiteVerificacion.PBVMin,
                                    PBVMax = procesoResultados.LimiteVerificacion.PBVMax,
                                    CombustibleGas = procesoResultados.LimiteVerificacion.CombustibleGas,
                                    Hc = procesoResultados.LimiteVerificacion.Hc,
                                    Co = procesoResultados.LimiteVerificacion.Co,
                                    No = procesoResultados.LimiteVerificacion.No,
                                    O2 = procesoResultados.LimiteVerificacion.O2,
                                    Coco2_Min = procesoResultados.LimiteVerificacion.Coco2_Min,
                                    Coco2_Max = procesoResultados.LimiteVerificacion.Coco2_Max,
                                    Factor_Lamda = procesoResultados.LimiteVerificacion.Factor_Lamda,
                                    Coeficiente_Absorcion_Luz = procesoResultados.LimiteVerificacion.Coeficiente_Absorcion_Luz,
                                    Opacidad = procesoResultados.LimiteVerificacion.Opacidad,
                                };
                                await _context.LimiteVerificacionParametros.AddAsync(limiteVerificacion);
                            }
                            dbRes.RESULTADO = procesoResultados.ResultadoPrueba;
                            dbRes.C_RECHAZO = procesoResultados.CausaRechazo;
                            //dbRes.C_RECHAZO_OBD = procesoResultados.CausaRechazoOBD;
                            dbRes.C_RECHAZO_OBD = c_rechazoobd;
                            dbRes.PruebaObd = c_rechazoobd == 2 ? false : dbRes.PruebaObd;


                            var validado = await _smadsotGenericInserts.ValidateFolio(
                                procesoResultados.ResultadoTipoCertificado,
                                resultadosFinales.IdVerificentro,
                                TipoTramite.CV,
                                verificacion.IdUserTecnico ?? 0,
                                resultadosFinales.Estado ?? "Puebla",
                                verificacion.Id,
                                null,
                                null
                            );

                            if (!validado.IsSucces)
                            {
                                _utilityList.RemoveColaVerificacion(IdVerificacion);

                                return new ResponseGeneric<long>(0) { mensaje = validado.Description };
                            }
                            IdfolioCreado = validado.recordId;
                            // var folionCNAAnterior = _context.vVerificacionReposicions.FirstOrDefault(r =>
                            //     ((r.Vigencia.HasValue && r.Vigencia.Value.Month > 6 && r.FechaRegistro.HasValue && r.FechaRegistro.Value.Month <= 6 && r.FechaRegistro.Value.Year == r.Vigencia.Value.Year) ||
                            //     (r.Vigencia.HasValue && r.Vigencia.Value.Month <= 6 && r.FechaRegistro.HasValue && r.FechaRegistro.Value.Month > 6 && r.FechaRegistro.Value.Year == r.Vigencia.Value.Year - 1))
                            //     && r.IdCatTipoCertificado == TipoCertificado.ConstanciasNoAprobado && r.Placa != null && r.Placa.Equals(verificacion.Placa) && r.Serie != null && r.Serie.Equals(verificacion.Serie) && r.IdReposicion == null);

                            // if (folionCNAAnterior != null)
                            // {
                            //     if (fechaHoy <= folionCNAAnterior.Vigencia)
                            //         fechaHoy = folionCNAAnterior.FechaRegistro.Value;
                            // }

                            var semestre = fechaHoy.Month <= 6 ? 1 : 2;

                            var fechaFacturacion = resultadosFinales.FechaFacturacion ?? DateTime.Now;
                            if (refrendoDobleCero)
                            {
                                fechaFacturacion = fechaFacturacion.AddYears(2);
                            }
                            verificacion.Vigencia = await GenerateExpireDate(procesoResultados.ResultadoTipoCertificado, verificacion.Placa,
                                verificacion.Semestre, fechaFacturacion, (verificacion.IdMotivoVerificacion == MotivoVerificacionDict.VERIFICACIONVOLUNTARIA || verificacion.IdMotivoVerificacion == MotivoVerificacionDict.RECHAZOVOLUNTARIO), verificacion.AnioVerificacion);
                            // verificacion.FolioCertificado = folios.FirstOrDefault()?.FolioCertificado.ToString() ?? "0";
                            // verificacion.FolioCertificado = folios.FirstOrDefault()?.FolioCertificado.ToString() ?? "";
                        }
                        _context.ResultadosVerificacions.Update(dbRes);
                        _context.Verificacions.Update(verificacion);
                    }

                    // var alerta = new Alertum
                    // {
                    //     TableName = DictAlertas.ActualizacionPrueba,
                    //     TableId = verificacion.Id,
                    //     IdVerificentro = resultadosFinales.IdVerificentro,
                    //     Data = null,
                    //     IdUser = null,
                    //     MovimientoInicial = "Actualización Prueba",
                    //     Fecha = DateTime.Now,
                    //     IdEstatusFinal = null,
                    //     MovimientoFinal = null,
                    //     Leido = false,
                    //     Procesada = false
                    // };
                    // dbRes.EstatusPrueba = EstatusVerificacion.FolioImpreso;
                    // _context.Alerta.Add(alerta);

                    await _context.SaveChangesAsync();

                    ts.Complete();
                    _context.ChangeTracker.AutoDetectChangesEnabled = true;

                }
                _utilityList.RemoveColaVerificacion(IdVerificacion);

                return new ResponseGeneric<long>(IdfolioCreado);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
                await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionGenerarFolioId);
                _utilityList.RemoveColaVerificacion(IdVerificacion);
                return new ResponseGeneric<long>(0) { mensaje = "Ocurrió un error inesperado al intentar crear el folio de forma valorada." };
            }
        }
        public async Task TestResultadosVerificacion()
        {
            var fechaInicio = DateTime.Now.AddDays(-5).Date;
            var fechaActual = DateTime.Now.AddDays(-2).Date;
            var registros = _context.vVerificacionDatosLimites.Where(x => x.EstatusPrueba >= 9 && x.IdLimiteVerificacionParametro == null && x.Fecha < fechaActual && x.Fecha >= fechaInicio && (x.PruebaEmisiones == true || x.PruebaObd == true || x.PruebaOpacidad == true));
            var cantidadLote = 200;
            var total = await registros.CountAsync();
            var indice = 0;

            while (indice < total)
            {
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var restante = total - indice;
                    // if (indice > 60000)
                    // {
                    //     break;
                    // }
                    var miniminoRestante = Math.Min(restante, cantidadLote);
                    var resultados = registros.AsNoTracking().OrderBy(x => x.IdVerificacion).Skip(indice).Take(miniminoRestante);
                    var LimitesAgregar = new List<LimiteVerificacionParametro>();

                    // using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
                    // {
                    foreach (var resultadosFinales in resultados)
                    {

                        bool candidatoDoblecero = false;
                        var fechaHoy = DateTime.Now;


                        var procesoResultados = await _smadsotGenericInserts.ValidarResultados(resultadosFinales.PROTOCOLO,
                                            resultadosFinales.PBV_EQUIV,
                                            resultadosFinales.Anio,
                                            resultadosFinales.OPACIDADK ?? 0M,
                                            resultadosFinales.PruebaObd ?? false,
                                            resultadosFinales.PruebaEmisiones ?? false,
                                            resultadosFinales.PruebaOpacidad ?? false,
                                            resultadosFinales.C_RECHAZO ?? 0,
                                            resultadosFinales.C_RECHAZO_OBD ?? 0,
                                            resultadosFinales.LAMDA_2540 ?? 0,
                                            resultadosFinales.LAMDA_5024 ?? 0,
                                            resultadosFinales.HC_2540 ?? 0,
                                            resultadosFinales.HC_5024 ?? 0,
                                            resultadosFinales.CO_2540 ?? 0,
                                            resultadosFinales.CO_5024 ?? 0,
                                            resultadosFinales.NO_2540 ?? 0,
                                            resultadosFinales.NO_5024 ?? 0,
                                            resultadosFinales.O2_2540 ?? 0,
                                            resultadosFinales.O2_5024 ?? 0,
                                            resultadosFinales.COCO2_2540 ?? 0,
                                            resultadosFinales.COCO2_5024 ?? 0,
                                            resultadosFinales.IdTipoCombustible ?? 0,
                                            candidatoDoblecero);
                        // Validamos que la evaluación de resultados sea correcta.
                        if (procesoResultados.Error || procesoResultados.LimiteVerificacion == null)
                        {
                            // await _smadsotGenericInserts.SaveLog("{" + $"Data Prueba: {procesoResultados.Mensaje}" + "}", DictTipoLog.LogLimitesNoEncontrados);
                            continue;
                        }
                        var limiteVerificacion = new LimiteVerificacionParametro()
                        {
                            IdVerificacion = resultadosFinales.IdVerificacion,
                            IdCatTipoProtolo = procesoResultados.LimiteVerificacion.IdCatTipoProtolo,
                            IdCatTipoCertificado = procesoResultados.LimiteVerificacion.IdCatTipoCertificado,
                            AnioInicio = procesoResultados.LimiteVerificacion.AnioInicio,
                            AnioFin = procesoResultados.LimiteVerificacion.AnioFin,
                            PBVMin = procesoResultados.LimiteVerificacion.PBVMin,
                            PBVMax = procesoResultados.LimiteVerificacion.PBVMax,
                            CombustibleGas = procesoResultados.LimiteVerificacion.CombustibleGas,
                            Hc = procesoResultados.LimiteVerificacion.Hc,
                            Co = procesoResultados.LimiteVerificacion.Co,
                            No = procesoResultados.LimiteVerificacion.No,
                            O2 = procesoResultados.LimiteVerificacion.O2,
                            Coco2_Min = procesoResultados.LimiteVerificacion.Coco2_Min,
                            Coco2_Max = procesoResultados.LimiteVerificacion.Coco2_Max,
                            Factor_Lamda = procesoResultados.LimiteVerificacion.Factor_Lamda,
                            Coeficiente_Absorcion_Luz = procesoResultados.LimiteVerificacion.Coeficiente_Absorcion_Luz,
                            Opacidad = procesoResultados.LimiteVerificacion.Opacidad,
                        };
                        LimitesAgregar.Add(limiteVerificacion);

                    }

                    indice += miniminoRestante;
                    await _context.LimiteVerificacionParametros.AddRangeAsync(LimitesAgregar);
                    await _context.SaveChangesAsync();
                    scope.Complete();
                    await Task.Delay(1000);
                }
            }
        }

        public async Task<ResponseGeneric<FolioFormaValoradaImpresionResponse>> GetDataImpresion(long id, bool imprimir)
        {
            try
            {

                var data = await _context.vFoliosFormaValoradaImpresions.Where(x => x.Id == id).Select(x => new FolioFormaValoradaImpresionResponse
                {
                    PROTOCOLO = x.PROTOCOLO,
                    OPACIDADK = x.OPACIDADK,
                    Anio = x.Anio,
                    ApiEndPoint = x.ApiEndPoint,
                    ApiKey = x.ApiKey,
                    Clave = x.Clave,
                    CO2_2540 = x.CO2_2540,
                    CO2_5024 = x.CO2_5024,
                    COCO2_2540 = x.COCO2_2540,
                    COCO2_5024 = x.COCO2_5024,
                    CO_2540 = x.CO_2540,
                    CO_5024 = x.CO_5024,
                    Combustible = x.Combustible,
                    C_RECHAZO = x.C_RECHAZO,
                    Etapa = x.Etapa,
                    FechaRegistro = x.FechaRegistro,
                    FinalizacionPruebas = x.FinalizacionPruebas,
                    FolioFoliosFormaValoradaVerificentro = x.FolioFoliosFormaValoradaVerificentro,
                    HC_2540 = x.HC_2540,
                    HC_5024 = x.HC_5024,
                    Id = x.Id,
                    IdUserTecnico = x.IdUserTecnico,
                    IdUserCapturista = x.IdUserCapturista,
                    IdVerificacion = x.IdVerificacion,
                    IdRefrendoExento = x.IdRefrendoExento,
                    IdExento = x.IdExento,
                    InicioPruebas = x.InicioPruebas,
                    KPH_2540 = x.KPH_2540,
                    KPH_5024 = x.KPH_5024,
                    LAMDA_2540 = x.LAMDA_2540,
                    C_RECHAZO_OBD = x.C_RECHAZO_OBD,
                    Marca = x.Marca,
                    IdVerificentro = x.IdVerificentro,
                    LAMDA_5024 = x.LAMDA_5024,
                    IdCatTipoCertificado = x.IdCatTipoCertificado,
                    Modelo = x.Modelo,
                    NombreVerificentro = x.NombreVerificentro,
                    NO_2540 = x.NO_2540,
                    NO_5024 = x.NO_5024,
                    O2_2540 = x.O2_2540,
                    O2_5024 = x.O2_5024,
                    OBD_CATAL = x.OBD_CATAL,
                    OBD_CILIN = x.OBD_CILIN,
                    OBD_COMBU = x.OBD_COMBU,
                    OBD_INTEG = x.OBD_INTEG,
                    OBD_MIL = x.OBD_MIL,
                    OBD_OXIGE = x.OBD_OXIGE,
                    OBD_TIPO_SDB = x.OBD_TIPO_SDB,
                    OPACIDADP = x.OPACIDADP,
                    Placa = x.Placa,
                    POTMIN_RPM = x.POTMIN_RPM,
                    Propietario = x.Propietario,
                    PruebaEmisiones = x.PruebaEmisiones,
                    PruebaObd = x.PruebaObd,
                    PruebaOpacidad = x.PruebaOpacidad,
                    RESULTADO = x.RESULTADO,
                    RPM_2540 = x.RPM_2540,
                    RPM_5024 = x.RPM_5024,
                    RPOTMAX_RPM = x.RPOTMAX_RPM,
                    Serie = x.Serie,
                    SPS_2540 = x.SPS_2540,
                    SPS_5024 = x.SPS_5024,
                    SPS_Humo = x.SPS_Humo,
                    TEMP_2540 = x.TEMP_2540,
                    TEMP_5024 = x.TEMP_5024,
                    TEMP_CAM = x.TEMP_CAM,
                    TEMP_GAS = x.TEMP_GAS,
                    TEMP_MOT = x.TEMP_MOT,
                    THP_2540 = x.THP_2540,
                    THP_5024 = x.THP_5024,
                    TipoCertificado = x.TipoCertificado,
                    VEL_GOB = x.VEL_GOB,
                    Vigencia = x.Vigencia,
                    VOLTS_2540 = x.VOLTS_2540,
                    VOLTS_5024 = x.VOLTS_5024,
                    FolioCertificadoAnterior = x.FolioCertificadoAnterior,
                    NumeroSerieEquipo = x.NumeroSerieEquipo,
                    IdCatTipoTramite = x.IdCatTipoTramite,
                    NombreEquipo = (x.IdCatTipoEquipo == EquiposDict.TACOMETROENCENDEDOR || x.IdCatTipoEquipo == EquiposDict.TACOMETRONOCONTACTO || x.IdCatTipoEquipo == EquiposDict.TACOMETROPINZAS || x.IdCatTipoEquipo == EquiposDict.TACOMETROTRANSDUCTOR) ? "TACÓMETRO" : x.NombreEquipo,
                    IdEquipo = x.IdEquipo,
                    NombreCapturista = x.NombreCapturista,
                    NumeroCapturista = x.NumeroCapturista,
                    NombreTecnico = x.NombreTecnico,
                    NumeroTecnico = x.NumeroTecnico,
                    Linea = x.Linea,
                    FolioCertificado = x.FolioCertificado,
                    ClaveTramite = x.ClaveTramite,
                    EntidadProcedencia = x.EntidadProcedencia,
                    IdMotivoVerificacion = x.IdMotivoVerificacion,
                    PBV = x.PBV,
                    LeyendaCNA = " "

                }).FirstOrDefaultAsync();
                // Calcular Semestre
                if (data?.IdVerificacion == null)
                {
                    var fechaActual = data.FechaRegistro.Value;
                    fechaActual = DateTime.Now;
                    int semestreInt = fechaActual.Month <= 6 ? 1 : 2;
                    // Validamos si es segundo semestre
                    if (semestreInt == 2)
                    {
                        data.Semestre = $"Segundo Semestre {fechaActual.Year}";
                    }
                    else
                    {
                        data.Semestre = $"Primer Semestre {fechaActual.Year}";
                    }
                }
                // Se agrega a una lista fija para no mandar a imprimir doble vez
                if (_utilityList.GetColaFolio().Any(x => x == id))
                    return new ResponseGeneric<FolioFormaValoradaImpresionResponse>("El certificado que se intenta imprimir ya se ha enviado al servidor para realizar la impresión del mismo.")
                    { mensaje = "El folio que se intenta imprimir ya se ha enviado al servidor para realizar la impresión del mismo. Vuelva a activar el certificado para imprimir desde el historial de citas, sí desea imprimirlo de nuevo", Response = new() };
                _utilityList.AddColaFolios(id);

                ResultadosVerificacion? resultados = null;
                if (data?.IdVerificacion != null)
                {
                    var ver = await _context.Verificacions.Include(x => x.ResultadosVerificacion).FirstOrDefaultAsync(x => x.Id == data.IdVerificacion);
                    if (ver == null)
                    {
                        _utilityList.RemoveColaFolios(id);
                        return new ResponseGeneric<FolioFormaValoradaImpresionResponse>() { mensaje = "No existe información de la prueba que se intenta imprimir. Compruebe si no fue recapturada.", Status = ResponseStatus.Failed, Response = new() };
                    }
                    resultados = ver?.ResultadosVerificacion;
                    // if (resultados?.EstatusPrueba == EstatusVerificacion.FolioImpreso)
                    // {
                    //     return new ResponseGeneric<FolioFormaValoradaImpresionResponse>("El certificado que se intenta imprimir ya se ha enviado al servidor para realizar la impresión del mismo.") { mensaje = "El folio que se intenta imprimir ya se ha enviado al servidor para realizar la impresión del mismo. Vuelva a activar el certificado para imprimir desde el historial de citas, sí desea imprimirlo de nuevo" };

                    // }
                    var placa = ver?.Placa ?? string.Empty;
                    var serie = ver?.Serie ?? string.Empty;
                    var anioVerifica = ver?.AnioVerificacion ?? 0;
                    var semestre = ver?.Semestre ?? 0;
                    var IntentosVerificaciones = await _context.Verificacions.Where(x => x.Placa.Equals(placa) && x.Serie.Equals(serie) && x.AnioVerificacion == anioVerifica && x.Semestre == semestre && x.Id != data.IdVerificacion).CountAsync();
                    if (ver != null)
                    {
                        ver.NoIntento = IntentosVerificaciones + 1;
                        _context.Verificacions.Update(ver);
                    }
                    if (ver.Semestre == 2)
                    {
                        data.Semestre = $"Segundo Semestre {ver.AnioVerificacion}";
                    }
                    else
                    {
                        data.Semestre = $"Primer Semestre {ver.AnioVerificacion}";
                    }
                }
                // Comprobamos que haya un CNA Previo en los ultimos 30 días marcado como extemporaneo
                var periodoGratuito = DateTime.Now.AddDays(-30);
                var registros = _context.vVerificacionReposicions.Where(r =>
                                    r.FechaRegistro <= periodoGratuito
                && r.Placa != null && r.Placa.Equals(data.Placa) && r.Serie != null && r.Serie.Equals(data.Serie) && r.IdReposicion == null && !(r.Cancelado ?? false)
                && r.Id != id);
                var certificadosNoAprobado = registros.Where(r => r.IdCatTipoCertificado == TipoCertificado.ConstanciasNoAprobado && r.IdMotivoVerificacion == MotivoVerificacionDict.Extemporaneo);
                var totalCertificados = await certificadosNoAprobado.CountAsync();

                // Definimos si el certificado lleva leyenda CNA, fecha de vigencia o ninguna.
                if ((totalCertificados % 2 == 0) && data.IdCatTipoCertificado == TipoCertificado.ConstanciasNoAprobado && (data.IdMotivoVerificacion != MotivoVerificacionDict.Rechazo && data.IdMotivoVerificacion != MotivoVerificacionDict.ExencionPeriodo && data.IdMotivoVerificacion != MotivoVerificacionDict.RECHAZOVOLUNTARIO && data.IdMotivoVerificacion != MotivoVerificacionDict.RECHAZOHOLOGRAMADOBLECERO && data.IdMotivoVerificacion != MotivoVerificacionDict.EXENCIONFUERADEPERIODOPRIMERINTENTO))
                {
                    // Se usa solo para la vista previa a imprimir el certificado
                    data.VigenciaVistaPrevia = data.Vigencia;
                    data.LeyendaCNA = string.Format("Está constancia tiene un período de vigencia de 30 días para evitar un costo extra hasta {0}.", data.FechaRegistro.Value.AddDays(30).ToString("d", new CultureInfo("es-MX")));
                }
                if (data.IdCatTipoCertificado != TipoCertificado.ConstanciasNoAprobado && data.IdCatTipoCertificado != TipoCertificado.Exentos)
                {
                    data.VigenciaVistaPrevia = data.Vigencia;
                    data.LeyendaCNA = string.Format("Recuerde verificar antes del día {0}.", data.Vigencia.Value.ToString("dd 'de' MMMM 'de' yyyy", new CultureInfo("es-MX")).ToUpper());
                }


                if (imprimir)
                {
                    var dataImpresion = await GetDataPDF(data); // Obtenemos los datos y registros para el certificado 

                    var pdfImpresion = await _pdfBuilder.GetImpresionCertificado(dataImpresion, data.IdCatTipoCertificado.Value); // Generamos el pdf

                    // Sí hay un error al generar el pdf regresamos un error
                    if (pdfImpresion.Status == ResponseStatus.Failed)
                    {
                        // Se elimina de la lista para poder volverlo a imprimir
                        _utilityList.RemoveColaFolios(id);
                        return new ResponseGeneric<FolioFormaValoradaImpresionResponse>("No se pudo generar correctamente el documento para imprimirlo.") { mensaje = "No se pudo generar correctamente el documento para imprimirlo.", Response = new() };
                    }
                    var auxpdf = Convert.ToBase64String(pdfImpresion.Response.DocumentoPDF);
                    // Generamos la instacia del Servicio para mandar la impresión al servidor
                    var serviceVerificentro = new ServicioEventosVerificentro(data.ApiEndPoint, data.ApiKey);

                    var requestService = new ImpresionRequest
                    {
                        TipoCertificado = data.IdCatTipoCertificado.Value,
                        PdfBytes = pdfImpresion.Response.DocumentoPDF
                    };
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        // Actualizamos el Impreso del Folio del Certificado
                        var foliodb = await _context.FoliosFormaValoradaVerificentros.FirstOrDefaultAsync(x => x.Id == data.Id);
                        foliodb.Impreso = true;

                        // Marcar los resultados como FolioImpreso
                        if (resultados != null)
                        {
                            resultados.EstatusPrueba = EstatusVerificacion.FolioImpreso;
                            _context.ResultadosVerificacions.Update(resultados);
                        }
                        await _context.SaveChangesAsync();                        
                        // mandamos el pdf al servidor para que se imprima.
                        var responseService = await serviceVerificentro.PostAsync<EventoEntradaRequest, object>("RecepcionEventos/ColaImpresion", requestService, data.NombreVerificentro, $"Hubo un error al mandar a imprimir al dispositivo del {data.NombreVerificentro}.");
                        if (responseService.Status != ResponseStatus.Success)
                        {
                            //transaction.Rollback();
                            _utilityList.RemoveColaFolios(id);
                            await _smadsotGenericInserts.SaveLog(JsonConvert.SerializeObject(responseService), DictTipoLog.ExcepcionImprimirCertificado);
                            return new ResponseGeneric<FolioFormaValoradaImpresionResponse>() { mensaje = responseService.mensaje, Status = ResponseStatus.Failed, Response = new() };
                        }
                        transaction.Commit();
                    }
                }
                _utilityList.RemoveColaFolios(id);
                return new ResponseGeneric<FolioFormaValoradaImpresionResponse>(data) { mensaje = "Se mando a imprimir correctamente." };

            }
            catch (Exception ex)
            {
                _utilityList.RemoveColaFolios(id);
                await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionImprimirCertificado);
                return new ResponseGeneric<FolioFormaValoradaImpresionResponse>(ex) { mensaje = "Hubo un error al mandar a imprimir.", Response = new() };
            }
        }




        #region Private methods
        private int GenerarPeriodoCalibracion(DateTime fecha1, DateTime fecha2)
        {
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan span = fecha2 - fecha1;
            int years = (zeroTime + span).Year - 1;
            int months = (zeroTime + span).Month - 1;
            int days = (zeroTime + span).Day;
            var tipo = 0;
            if (years >= 1)
            {
                tipo = CatTipoCalibracionDic.Anual;
            }
            else if (months >= 6)
            {
                tipo = CatTipoCalibracionDic.Semestral;
            }
            else if (months >= 3)
            {
                tipo = CatTipoCalibracionDic.Trimestral;

            }
            else if (months >= 1)
            {
                tipo = CatTipoCalibracionDic.Mensual;

            }
            else if (days >= 14)
            {
                tipo = CatTipoCalibracionDic.Quincenal;

            }
            else if (days >= 7)
            {
                tipo = CatTipoCalibracionDic.Semanal;

            }
            else
            {
                tipo = CatTipoCalibracionDic.Diario;
            }
            return tipo;
        }
        private async Task<ImpresionPDFResponse> GetDataPDF(FolioFormaValoradaImpresionResponse data)
        {

            var encrypter = new AesEncryption(_llaveCifrado);
            var idBase64 = encrypter.Encrypt(data.IdVerificacion.ToString());
            var dataImpresion = new ImpresionPDFResponse
            {
                Folio = data.FolioFoliosFormaValoradaVerificentro,
                Marca = data.Marca,
                Anio = data.Anio.ToString(),
                Placa = data.Placa,
                Holograma = data.TipoCertificado,
                Submarca = data.Modelo,
                Vigencia = data.VigenciaStr,
                VigenciaFecha = data.Vigencia.Value,
                Centro = data.NombreVerificentro,
                Linea = data.Clave,
                LeyendaCNA = data.LeyendaCNA,
                Equipo = data.NombreEquipo,
                FolioAnterior = data.FolioCertificadoAnterior,
                Fecha = data.FechaRegistro.Value.ToString("dd/MM/yyyy HH:mm"),
                HoraInicio = data.InicioPruebas.HasValue ? data.InicioPruebas.Value.ToString("HH:mm:ss") : "",
                HoraFin = data.FinalizacionPruebas.HasValue ? data.FinalizacionPruebas.Value.ToString("HH:mm:ss") : "",
                Nombre = data.Propietario,
                Semestre = data.Semestre,
                Combustible = data.Combustible,
                Version = "1.5",
                TecnicoCapturaNombre = data.NombreCapturista,
                TecnicoCapturaNumero = data?.NumeroCapturista ?? "-",
                TecnicoPruebaNombre = data.NombreTecnico,
                TecnicoPruebaNumero = data?.NumeroTecnico ?? "-",
                AprobadoPor = data?.AprobadoPor ?? "-",
                NumSerie = data.Serie,
                IdCatTipoTramite = TipoTramiteDict.CVV,
                UrlExpediente = (data.IdCatTipoCertificado != TipoCertificado.Exentos) ? $"{siteUrl}Citas/ExpedientePublico?clave={HttpUtility.UrlEncode(idBase64)}" : null,
                Emisiones = new List<ImpresionPDFEmisionResponse>(),
                Entidad = data.EntidadProcedencia
            };


            // Sí fue una CNA cambiamos el proceso de los mensajes y el formato de como se muestra en el pdf.
            if (data.IdCatTipoCertificado == TipoCertificado.ConstanciasNoAprobado)
            {
                var causaRechazo = CausaRechazo.DictClaveRechazo[data.C_RECHAZO ?? 0];
                dataImpresion.AprobadoPor = $"NO APROBADO {causaRechazo.ToUpper()}";
                if (data.C_RECHAZO_OBD != OBD.SinCodigoError && data.C_RECHAZO_OBD != OBD.FallaConexion && data.C_RECHAZO_OBD != null)
                {
                    var causaRechazoOBD = OBD.DictOBD[data.C_RECHAZO_OBD ?? 0];
                    dataImpresion.AprobadoPor += $": {causaRechazoOBD.ToUpper()}";
                }
                dataImpresion.Emisiones = await GenerarResultados(data.C_RECHAZO == CausaRechazo.Emisiones, data);
            }
            else // Sí es un certificado común mostramos los resultados de acuerdo al resultado que haya obtenido.
            {
                var mostrarResultados = false;
                // Sí fue aprobado por obd mostramos la leyenda y listo.
                if ((data.PruebaObd ?? false) && (data.C_RECHAZO_OBD == 0 || data.C_RECHAZO_OBD == null) && (data.C_RECHAZO_OBD != OBD.FallaConexion) && (data.C_RECHAZO == null || data.C_RECHAZO == CausaRechazo.NoAplica) && data.IdCatTipoCertificado != TipoCertificado.Uno && data.IdCatTipoCertificado != TipoCertificado.Dos)
                {
                    dataImpresion.AprobadoPor = "APROBADO POR INSPECCIÓN VISUAL, OBD Y PRUEBA DE HUMOS.";
                }
                else // en caso contrario mostramos los resultados de emisiones
                {
                    dataImpresion.AprobadoPor = "APROBADO POR INSPECCIÓN VISUAL Y PRUEBA DE HUMOS.";
                    mostrarResultados = true;
                }
                dataImpresion.Emisiones = await GenerarResultados(mostrarResultados, data);

            }
            return dataImpresion;

        }
        private async Task<List<ImpresionPDFEmisionResponse>> GenerarResultados(bool mostrar, FolioFormaValoradaImpresionResponse data)
        {
            var Emisiones = new List<ImpresionPDFEmisionResponse>();
            // Se obtiene el id del tipo de combustible. No es la mejor manera pero por tiempo fue la mejor solución.
            var idCombustible = Combustible.DictCombustible.FirstOrDefault(x => x.Value.Equals(data.Combustible)).Key;
            // Se obtienen los límites con los que fueron evaluados los resultados.
            var limites = await _smadsotGenericInserts.ObtenerLimites(data.PROTOCOLO ?? 0, data.PBV ?? 0, data.Anio ?? 0, idCombustible, data.OPACIDADK ?? 0);
            if (mostrar)
            {
                if (data.PROTOCOLO == Protocolos.DIESELSINDATOS || data.PROTOCOLO == Protocolos.ACELERACIONDIESEL)
                {
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "EMISIONES", PrimeraColumna = "", SegundaColumna = "", TerceraColumna = "LMP" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "Coeficiente de absorción de Luz (K)", PrimeraColumna = $"{data.OPACIDADK ?? 0:0.####}", SegundaColumna = "", TerceraColumna = $"{limites?.Coeficiente_Absorcion_Luz ?? 0:0.####}" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "Opacidad", PrimeraColumna = $"{data.OPACIDADP ?? 0:0.####}", SegundaColumna = "" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "RPM Gobernador", PrimeraColumna = $"{data.VEL_GOB ?? 0}", SegundaColumna = "" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "RPM Ralenti", PrimeraColumna = $"{data.POTMIN_RPM ?? 0}", SegundaColumna = "" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "PROTOCOLO", PrimeraColumna = $"{data.PROTOCOLO}", SegundaColumna = "" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = " ", PrimeraColumna = " ", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = " ", PrimeraColumna = " ", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = " ", PrimeraColumna = " ", SegundaColumna = " ", TerceraColumna = " " });
                }
                else
                {
                    var combustibleGasolina = idCombustible == Combustible.Gasolina;
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "EMISIONES", PrimeraColumna = "PAS5024", SegundaColumna = "PAS2540", TerceraColumna = "LMP" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "LAMBDA", PrimeraColumna = $"{data.LAMDA_2540 ?? 0:0.####}", SegundaColumna = $"{data.LAMDA_5024 ?? 0:0.####}", TerceraColumna = $"{limites?.Factor_Lamda ?? 0:0.####}" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "HC PPM", PrimeraColumna = $"{data.HC_5024 ?? 0}", SegundaColumna = $"{data.HC_2540 ?? 0}", TerceraColumna = $"{limites?.Hc ?? 0}" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "O2 VOL %", PrimeraColumna = $"{data.O2_5024 ?? 0:0.####}", SegundaColumna = $"{data.O2_2540 ?? 0:0.####}", TerceraColumna = $"{limites?.O2 ?? 0:0.####}" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "CO %", PrimeraColumna = $"{data.CO_5024 ?? 0:0.####}", SegundaColumna = $"{data.CO_2540 ?? 0:0.####}", TerceraColumna = $"{limites?.Co ?? 0:0.####}" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "CO2 %", PrimeraColumna = $"{data.CO2_5024 ?? 0:0.####}", SegundaColumna = $"{data.CO2_2540 ?? 0:0.####}", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "CO+CO2 %", PrimeraColumna = $"{data.COCO2_5024 ?? 0:0.####}", SegundaColumna = $"{data.COCO2_2540 ?? 0:0.####}", TerceraColumna = $"{limites?.Coco2_Min ?? 0:0.####} - {limites?.Coco2_Max ?? 0:0.####}" });

                    if (limites.No != null)
                        Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "NOX PPM", PrimeraColumna = $"{data.NO_5024 ?? 0}", SegundaColumna = $"{data.NO_2540 ?? 0}", TerceraColumna = $"{limites.No ?? 0}" });
                    else
                        Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = " ", PrimeraColumna = " ", SegundaColumna = " ", TerceraColumna = " " });

                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "RPM", PrimeraColumna = $"{data.RPM_5024 ?? 0}", SegundaColumna = $"{data.RPM_2540 ?? 0}" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "KM/H", PrimeraColumna = $"{data.KPH_5024 ?? 0:0.####}", SegundaColumna = $"{data.KPH_2540 ?? 0:0.####}" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "HP", PrimeraColumna = $"{data.THP_5024 ?? 0:0.####}", SegundaColumna = $"{data.THP_2540 ?? 0:0.####}" });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "PROTOCOLO", PrimeraColumna = $"{data.PROTOCOLO}", SegundaColumna = "" });
                }
            }
            else
            {
                if (data.IdVerificentro == 28)
                {
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "Monitor Detectado", PrimeraColumna = "Estado", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "Convertidor Catalítico", PrimeraColumna = $"{data.OBD_CATAL}", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "Ignición de cilindros", PrimeraColumna = $"{data.OBD_CILIN}", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "Sistema de combustible", PrimeraColumna = $"{data.OBD_COMBU}", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "Sistema de componentes integrales", PrimeraColumna = $"{data.OBD_INTEG}", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "Sistema de sensores de oxígeno", PrimeraColumna = $"{data.OBD_OXIGE}", SegundaColumna = " ", TerceraColumna = " " });
                }
                else
                {
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = "Nombre", PrimeraColumna = "Código", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = nameof(data.OBD_MIL), PrimeraColumna = $"{data.OBD_MIL}", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = nameof(data.OBD_CATAL), PrimeraColumna = $"{data.OBD_CATAL}", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = nameof(data.OBD_CILIN), PrimeraColumna = $"{data.OBD_CILIN}", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = nameof(data.OBD_COMBU), PrimeraColumna = $"{data.OBD_COMBU}", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = nameof(data.OBD_INTEG), PrimeraColumna = $"{data.OBD_INTEG}", SegundaColumna = " ", TerceraColumna = " " });
                    Emisiones.Add(new ImpresionPDFEmisionResponse { Nombre = nameof(data.OBD_OXIGE), PrimeraColumna = $"{data.OBD_OXIGE}", SegundaColumna = " ", TerceraColumna = " " });
                }

            }
            return Emisiones;
        }
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
            var responseService = await serviceVerificentro.PostAsync<EventoEntradaRequest, object>("RecepcionEventos/EventoEntrada", requestService, verificentro, $"La solicitud al servicio del {verificentro} excedió el tiempo de espera. ");
            if (responseService.Status != ResponseStatus.Success)
            {
                await _smadsotGenericInserts.SaveLog(JsonConvert.SerializeObject(responseService), DictTipoLog.ExcepcionEventoEntrada);
                return new ResponseGeneric<string>() { mensaje = responseService.mensaje, CurrentException = responseService.CurrentException, Status = ResponseStatus.Failed };
            }
            return new ResponseGeneric<string>("", true);
        }
        private int? GetLastNumberPlate(string placa)
        {
            char lastDigit = placa.LastOrDefault(char.IsDigit);

            if (char.IsDigit(lastDigit))
            {
                return int.Parse(lastDigit.ToString());
            }
            else
            {
                return null;
            }
        }
        private async Task<DateTime> GenerateExpireDate(int tipoCertificado, string placa, int semestre, DateTime? FechaFacturacion, bool voluntario, int anio)
        {
            var currentDate = DateTime.Now;
            if (tipoCertificado == TipoCertificado.DobleCero)
            {
                if (FechaFacturacion != null)
                    return FechaFacturacion.Value.AddYears(2);
                else
                    return currentDate.AddYears(2);

            }
            else if (tipoCertificado == TipoCertificado.ConstanciasNoAprobado)
                return currentDate.AddDays(30);
            else if (voluntario)
                return currentDate.AddDays(180);
            else
            {
                var digitoPlaca = GetLastNumberPlate(placa);
                if (digitoPlaca == null)
                    return currentDate.AddMonths(6);
                var calendario = await _context.CalendarioVerificacions.ToListAsync();
                return GenerateDigitExpireDate.GetExpireDate(digitoPlaca.Value, semestre, anio, calendario);
            }
        }

        private async Task<int> ValidarOBD(vVerificacionDatosLimite resultadosFinales)
        {          
            bool candidatoDoblecero = false;
            if ((resultadosFinales?.OBD_CATAL == null || resultadosFinales?.OBD_CILIN == null || resultadosFinales?.OBD_COMBU == null || resultadosFinales?.OBD_INTEG == null || resultadosFinales?.OBD_OXIGE == null) && resultadosFinales?.OBD_TIPO_SDB == null)
            {
                return 2; //Envia a hacer emisiones
            }
            if (resultadosFinales?.OBD_CATAL == 1 || resultadosFinales?.OBD_CILIN == 1 || resultadosFinales?.OBD_COMBU == 1 || resultadosFinales?.OBD_INTEG == 1 || resultadosFinales?.OBD_OXIGE == 1)
            {
                return 1; //Reprueba
            }
            if ((resultadosFinales?.OBD_CATAL == 9 || resultadosFinales?.OBD_CILIN == 9 || resultadosFinales?.OBD_COMBU == 9 || resultadosFinales?.OBD_INTEG == 9 || resultadosFinales?.OBD_OXIGE == 9) /*|| (resultadosFinales?.OBD_MIL == 1)*/ && !candidatoDoblecero)
            {
                return 2; //Emisiones
            }
            else
            {
                return 0; // Todo Ok
            }
        }   
        #endregion
    }

    public interface IEventosVerificentroNegocio
    {
        public Task<ResponseGeneric<bool>> RegistrarEvento(string clave, EventoSalida req);
        public Task<ResponseGeneric<bool>> ActualizarPrueba(string clave, Prueba req);
        public Task TestResultadosVerificacion();
        public Task<ResponseGeneric<bool>> ReimpresionFV(FolioCanceladosRequest request);
        public Task<ResponseGeneric<long>> GenerarFolioFV(long IdVerificacion, bool validar = true);
        public Task<ResponseGeneric<FolioFormaValoradaImpresionResponse>> GetDataImpresion(long id, bool imprimir);

    }


}
