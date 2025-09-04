using Azure;
using MathNet.Numerics.Distributions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Namespace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.Entities.DashboardLineas.Response;
using Smadot.Models.Entities.FoliosFormaValorada.Request;
using Smadot.Models.Entities.FoliosFormaValorada.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.SeguimientoCVV.Request;
using Smadot.Models.Entities.Verificacion;
using Smadot.Models.Entities.Verificacion.Request;
using Smadot.Models.Entities.Verificacion.Response;
using Smadot.Models.GenericProcess;
using Smadot.Utilities;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.CognitiveServices.Interfaces;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Utilities.Seguridad;
using Smadot.Utilities.Seguridad.Modelo;
using Smadot.Utilities.ServicioMultas;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.Arm;
using System.Transactions;
using System.Web.Mvc;

namespace Smadot.Venta.Model.Negocio;

public class VerificacionNegocio : IVerificacionNegocio
{
    private SmadotDbContext _context;
    private readonly IUserResolver _userResolver;
    private readonly IComputerVisionService _computerVisionService;
    private readonly ICustomVision _customVision;
    private readonly BlobStorage _blobStorage;
    private readonly ILogger<VerificacionNegocio> logger;
    private readonly IConsultaVehiculoServicio _consultaVehiculoServicio;
    private readonly SmadsotGenericInserts _smadsotGenericInserts;


    public VerificacionNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration, IComputerVisionService computerVisionService, ICustomVision customVision, IConsultaVehiculoServicio consultaVehiculoServicio, ILogger<VerificacionNegocio> logger, SmadsotGenericInserts smadsotGenericInserts)
    {
        _context = context;
        _userResolver = userResolver;
        _computerVisionService = computerVisionService;
        _customVision = customVision;
        _consultaVehiculoServicio = consultaVehiculoServicio;
        _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        this.logger = logger;
        _smadsotGenericInserts = smadsotGenericInserts;
    }
    #region Methods
    public async Task<ResponseGeneric<ValidacionMatriculaResponse>> SavePlacas(VerificacionRequest request)
    {
        try
        {
            DateTime fecha = DateTime.Now;
            var finDia = new DateTime(fecha.Year, fecha.Month, fecha.Day, 23, 59, 59);
            var inicioDia = new DateTime(fecha.Year, fecha.Month, fecha.Day, 0, 0, 0);
            ValidacionMatriculaResponse response = new ValidacionMatriculaResponse();
            var verificacion = _context.vVerificacionCita.FirstOrDefault(x => (x.EstatusPrueba <= EstatusVerificacion.VerificacionFinalizada || x.EstatusPrueba == null) && x.IdVerificentro == request.IdVerificentro && x.Fecha >= inicioDia && x.Fecha <= finDia && (x.Serie.Trim().Equals(request.VinUpper.Trim()) || x.Id == request.Id));
            if (verificacion == null)
            {
                return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception("No se encontró una cita para este vehículo con el número de serie."))
                { mensaje = "No se encontró una cita para este vehículo con el número de serie." };
            }
            var infoVerificentro = _context.vVerificentros.FirstOrDefault(x => x.Id == request.IdVerificentro);
            if (infoVerificentro == null)
            {
                return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception("Hubo un error al intentar obtener la información del Centro de verificación."))
                { mensaje = "Hubo un error al intentar obtener la información del Centro de verificación." };

            }
            var capturaPlacas = await CapturarPlacas(infoVerificentro.ApiEndPoint, infoVerificentro.ApiKey, verificacion.ClaveLinea);
            // Si recibimos las placas quiere decir que se solicita ayuda dvrf y se manda alerta
            if (!string.IsNullOrEmpty(request.Placas) && request.IdVerificacion == null)
            {

                //Validamos que se encuentren los datos en la bd
                if (!verificacion.Placa.Equals(request.Placas.Trim()))
                {
                    return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception("No se han recibido los documentos del vehículo."))
                    { mensaje = "No se han recibido los documentos del vehículo." };
                }
                if (verificacion.IngresoManual)
                {
                    request.IdVerificacion = verificacion.Id;
                }
                else
                {
                    var responseAlerta = await PermisoIngresoManual(verificacion, request.IdUserTecnico);
                    if (responseAlerta.Status == ResponseStatus.Failed)
                    {
                        return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception("No se ha podido crear la alerta para notificar al DVRF."))
                        { mensaje = "No se ha podido crear la alerta para notificar al DVRF." };
                    }
                    if (responseAlerta.Response?.IdEstatusFinal == VerificacionIngresoManualEstatus.Autorizado)
                    {
                        request.IdVerificacion = responseAlerta.Response.TableId;

                    }
                    else if (responseAlerta.Response?.IdEstatusFinal == VerificacionIngresoManualEstatus.Rechazado)
                    {
                        return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception("DVRF no autorizo el ingreso manual de las placas del vehículo."))
                        { mensaje = "No se ha podido crear la alerta para notificar al DVRF." };
                    }
                    else
                    {
                        return new ResponseGeneric<ValidacionMatriculaResponse>(new ValidacionMatriculaResponse { IdVerificacion = verificacion.Id })
                        { mensaje = "La solicitud fue enviada al DVRF." };
                    }
                }
            }
            // Verificamos que venga IdVerificacion
            if (request.IdVerificacion != null)
            {
                var placas = "";
                // Sí IdVerificacion viene significa que se hace ingreso manual
                // verificacion = _context.vVerificacionCita.FirstOrDefault(x => x.Fecha >= inicioDia && x.Fecha <= finDia && x.Id == request.IdVerificacion);
                //Validamos que se encuentren los datos en la bd
                // if (verificacion == null)
                // {
                //     return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception("No se han recibido los documentos del vehículo."))
                //     { mensaje = "No se han recibido los documentos del vehículo." };

                // }
                // Verificamos que tenga permiso de ingreso manual
                if (!verificacion.IngresoManual)
                {
                    return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception("Tu solicitud aún no ha sido aprobada, vuelve a intentar en unos momentos o contacta al personal del Departamento de Verificación y Regulación de Fuentes ."))
                    { mensaje = "Tu solicitud aún no ha sido aprobada, vuelve a intentar en unos momentos o contacta al personal del Departamento de Verificación y Regulación de Fuentes ." };

                }
                placas = verificacion.Placa;
                response = new ValidacionMatriculaResponse { IdVerificacion = verificacion.Id };
            }
            // Si IdVerificacion es null significa que no es ingreso manual
            else
            {

                //if (capturaPlacas.Status == ResponseStatus.Failed)
                //{
                //    return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception(capturaPlacas.mensaje))
                //    { mensaje = capturaPlacas.mensaje };
                //}
                //if (capturaPlacas?.Response?.Trasera == null && capturaPlacas?.Response?.Trasera == null)
                //{
                //    var msj = "No sé detecto niguna imagen.";
                //    return new ResponseGeneric<ValidacionMatriculaResponse>(msj)
                //    { mensaje = msj };
                //}
                //// Validamos la imagen trasera de las camaras
                //bool validoCamaraDelantera = capturaPlacas?.Response?.Trasera == null ? true : false;
                //var readImageOcr = await ProcessImageOCR(validoCamaraDelantera ? capturaPlacas?.Response?.Delantera : capturaPlacas?.Response?.Trasera);
                //if (readImageOcr.Status == ResponseStatus.Failed && !validoCamaraDelantera)
                //{
                //    // Validamos la imagen delantera de las camaras en caso de que haya fallado la trasera
                //    readImageOcr = await ProcessImageOCR(capturaPlacas?.Response?.Delantera ?? new byte[0]);
                //    if (readImageOcr.Status == ResponseStatus.Failed)
                //    {
                //        return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception(readImageOcr.mensaje))
                //        { mensaje = readImageOcr.mensaje };
                //    }
                //    validoCamaraDelantera = true;
                //}
                //else if (readImageOcr.Status == ResponseStatus.Failed && !validoCamaraDelantera)
                //{
                //    return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception(readImageOcr.mensaje))
                //    { mensaje = readImageOcr.mensaje };
                //}

                ////Validamos que se encuentren los datos en la bd con los datos de la camara y que no haya sido la delantera
                //if (!verificacion.Placa.Equals(readImageOcr.Response) && !validoCamaraDelantera)
                //{
                //    // Validamos la información de la camara trasera
                //    readImageOcr = await ProcessImageOCR(capturaPlacas?.Response?.Delantera ?? new byte[0]);
                //    if (readImageOcr.Status == ResponseStatus.Failed)
                //    {
                //        return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception(readImageOcr.mensaje))
                //        { mensaje = readImageOcr.mensaje };
                //    }

                //    // Validamos que las placas reconocidas coincidan en la bd
                //    if (!verificacion.Placa.Equals(readImageOcr.Response))
                //    {
                //        return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception($"No se encontro información del vehículo con placas {readImageOcr.Response} y vin {request.Vin}."))
                //        { mensaje = $"No se encontro información del vehículo con placas {readImageOcr.Response} y vin {request.Vin}." };
                //    }
                //}
                response = new ValidacionMatriculaResponse { IdVerificacion = verificacion.Id };

            }
            // Consultamos adeudos de SPF
            // var consultaMultas = await _consultaVehiculoServicio.Consulta(placas, request.VinUpper);
            // Validamos que exista en la bd
            // if (!consultaMultas.BResultado)
            // {
            //     return new ResponseGeneric<ValidacionMatriculaResponse>(new Exception($"No se encontró registro de adeudos vehiculares para las placas {placas} y vin {request.Vin}."))
            //     { mensaje = $"No se encontró registro de adeudos vehiculares para las placas {placas} y vin {request.Vin}." };
            // }
            // Obtnemos la verificación para actualizarla
            var verificacionVehiculo = _context.Verificacions.Include(x => x.ResultadosVerificacion).FirstOrDefault(x => x.Id == response.IdVerificacion);
            verificacionVehiculo.IdUserTecnico = request.IdUserTecnico;
            // Sí aún no tiene un registro en resultados prueba lo registramos antes de continuar
            if (verificacionVehiculo.ResultadosVerificacion == null)
            {
                verificacionVehiculo.ResultadosVerificacion = new ResultadosVerificacion
                {
                    RESULTADO = 0,
                    RESULTADO_PROVEEDOR = 0,
                    InicioPruebas = DateTime.Now,
                    FinalizacionPruebas = DateTime.Now,
                    EstatusPrueba = 0,// Asignamos el estatus en 0 porque aún no inicia.
                };
                // await _context.ResultadosVerificacions.AddAsync(resultados);
                await _context.SaveChangesAsync();
            }
            var urlplacadelantera = await _blobStorage.UploadFileAsync(request.ImgPlacaDelantera, $"VerificacionExpediente/{verificacionVehiculo.Id}/{verificacionVehiculo.Placa}_delantera.jpg");
            var urlplacatrasera = await _blobStorage.UploadFileAsync(request.ImgPlacaDelantera, $"VerificacionExpediente/{verificacionVehiculo.Id}/{verificacionVehiculo.Placa}_trasera.jpg");
            string? urlcamara = null;
            string? urlcamaraTrasera = null;
            if (capturaPlacas.Status != ResponseStatus.Failed)
            {
                if (capturaPlacas.Response.Delantera != null)
                {
                    urlcamara = await _blobStorage.UploadFileAsync(capturaPlacas.Response.Delantera, $"VerificacionExpediente/{verificacionVehiculo.Id}/{verificacionVehiculo.Placa}_camara_delantera.jpg");
                }
                if (capturaPlacas.Response.Trasera != null)
                {
                    urlcamaraTrasera = await _blobStorage.UploadFileAsync(capturaPlacas.Response.Trasera, $"VerificacionExpediente/{verificacionVehiculo.Id}/{verificacionVehiculo.Placa}_camara_trasera.jpg");
                }
                verificacionVehiculo.URLPlacaCamara = urlcamara;
                verificacionVehiculo.URLPlacaCamaraTrasera = urlcamaraTrasera;
            }
            verificacionVehiculo.URLPlacaDelantera = urlplacadelantera;
            verificacionVehiculo.URLPlacaTrasera = urlplacatrasera;
            verificacionVehiculo.URLFotoTecnico = request.UrlFotoEvidencia;
            //await _context.SaveChangesAsync();
            // var alerta = new Alertum
            // {
            //     TableName = DictAlertas.ActualizacionPrueba,
            //     TableId = verificacionVehiculo.Id,
            //     IdVerificentro = request.IdVerificentro,
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
            // _context.Alerta.Add(alerta);
            await _context.SaveChangesAsync();
            response.Vehiculo = new ConsultaVehiculoResponse
            {
                BResultado = true,
                SmAnioModelo = verificacionVehiculo.Anio,
                IEstatusProceso = 0,
                TiEstatusAdeudo = 0,
                TiEstatusFotomulta = 0,
                VchPlaca = verificacionVehiculo.Placa,
                VchSerie = verificacionVehiculo.Serie,
                VchMarca = verificacionVehiculo.Marca,
                VchLinea = verificacionVehiculo.Modelo
            };
            response.IdTipoCombustible = verificacionVehiculo.IdTipoCombustible;
            response.IdTipoPrueba = verificacionVehiculo.ResultadosVerificacion.EstatusPrueba;
            return new ResponseGeneric<ValidacionMatriculaResponse>(response);
        }
        catch (Exception e)
        {
            await _smadsotGenericInserts.SaveLog(e, DictTipoLog.ExcepcionSavePlacas);
            return new ResponseGeneric<ValidacionMatriculaResponse>(e);

        }

    }

    public async Task<ResponseGeneric<ConsultaVerificacionResponse<object>>> PruebaVisualDiesel(PruebaVisualRequest<ResultadosDiesel> request)
    {
        try
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var verificacion = _context.vVerificacionDatosProveedorUserData.FirstOrDefault(x => x.IdVerificacion == request.IdVerificacion);
                if (verificacion == null)
                {
                    return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception("No se encontró registro de la verificacion."))
                    { mensaje = "No sé encontró registro de la verificacion.", Response = new() };
                }
                if (verificacion.IdResultadosVerificacion == null)
                {
                    return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception("Aún no se ha realizado la validación de placas del vehículo."))
                    { mensaje = "Aún no se ha realizado la validación de placas del vehículo.", Response = new() };
                }
                var verificacionResultado = _context.ResultadosVerificacions.FirstOrDefault(x => x.Id == verificacion.IdResultadosVerificacion);
                if (verificacionResultado.EstatusPrueba == 0)
                {
                    verificacionResultado.EstatusPrueba = 2;
                    verificacionResultado.FugasSistemaEscape = request.Resultados.Resultados.SistemaEscape;
                    verificacionResultado.GobernadorBuenEstado = request.Resultados.Resultados.GobernadorBuenEstado;
                    verificacionResultado.NumeroEscapes = request.NumeroEscapes;
                    var listChecks = new List<bool>()
                    {
                        (bool)verificacionResultado.FugasSistemaEscape,
                        (bool)verificacionResultado.GobernadorBuenEstado,
                    };
                    if (listChecks.Any(x => !x))
                    {
                        if (request.FotoEvidencia == null)
                        {
                            return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception("Se debe tomar la evidencia de la falla en la inspección visual."))
                            { mensaje = "Se debe tomar la evidencia de la falla en la inspección visual.", Response = new() };
                        }
                        var url = await _blobStorage.UploadFileAsync(request.FotoEvidencia, $"VerificacionExpediente/FallaPruebaVisual/{verificacion.Id}/{verificacion.Placa}_trasera.jpg");
                        verificacionResultado.EstatusPrueba = EstatusVerificacion.VerificacionFinalizada;
                        verificacionResultado.C_RECHAZO = CausaRechazo.Visual;
                        verificacionResultado.RESULTADO = Resultados.Rechazo;
                        verificacionResultado.URLEvidenciaFalla = url;

                        var validado = await _smadsotGenericInserts.ValidateFolio(TipoCertificado.ConstanciasNoAprobado, verificacion.IdVerificentro, TipoTramite.CV, verificacion.IdUserT.Value, verificacion.Estado, verificacion.IdVerificacion, null, null);
                        if (!validado.IsSucces)
                        {
                            return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception(validado.Description))
                            { mensaje = validado.Description ?? "", Response = new() };
                        }

                    }
                    else
                    {
                        var infoVerificentro = _context.vVerificentros.FirstOrDefault(x => x.Id == verificacion.IdVerificentro);
                        if (infoVerificentro == null)
                        {
                            return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception("No sé encontró información del verificentro."))
                            { mensaje = "No sé encontró información del verificentro.", Response = new() };
                        }
                        var pruebaProveedor = new PruebaInicio
                        {
                            Id = verificacion.IdVerificacion,
                            ClaveHolograma = verificacion.DOBLECERO == 1 ? "DOBLECERO" : "",
                            MotivoVerificacion = (verificacion.IdMotivoVerificacion ?? 2).ToString("00"),
                            ClaveLinea = verificacion.Clave,
                            NumeroSerie = verificacion.Serie,
                            TecnicoVerificador = verificacion.NumeroTrabajador,
                            Marca = verificacion.MarcaProveedor,
                            Submarca = verificacion.IdCatSubmarcaVehiculo,
                            Anio = verificacion.Anio,
                            Placa = verificacion.Placa,
                            RazonSocial = verificacion.RazonSocial,
                            NombrePropietario = verificacion.Propietario,
                            RevolucionesMaxGobernador = verificacion.RPM_GOB ?? verificacion.POTMAX_RPM,
                            ClaveCombustible = verificacion.ClaveCombustible,
                            Combustible = verificacion.IdTipoCombustible,
                            Combustible_POTMAX_RPM = verificacion.POTMAX_RPM,
                            RAL_FAB = verificacion.RAL_FAB,
                            GOB_FAB = verificacion.POTMAX_RPM,
                            Pot_5024 = verificacion.POT_5024,
                            Pot_2540 = verificacion.POT_2540,
                            PotMax_RPM = verificacion.POTMAX_RPM,
                            Protocolo = verificacion.PROTOCOLO,
                            Cilindros = verificacion.CILINDROS,
                            Cilindrada = verificacion.CILINDRADA,
                            Motor_DSL = verificacion.Motor_DSL,
                            PBV = (ulong)verificacion.PBV,
                            PBV_Equivalente = verificacion.PBV_EQUIV,
                            PBV_ASM = verificacion.PBV_ASM,
                            C_ABS = verificacion.C_ABS,
                            ConvertidorCatalitico = verificacion.CONV_CATAL,
                            OBD = (ulong)verificacion.OBD,
                            DobleCero = (ulong)verificacion.DOBLECERO,
                            CERO_GASOL = (ulong)verificacion.CERO_GASOL,
                            CERO_GASLP = (ulong)verificacion.CERO_GASLP,
                            CERO_GASNC = (ulong)verificacion.CERO_GASNC,
                            CERO_DSL = (ulong)verificacion.CERO_DSL,
                            TarjetaCirculacion = verificacion.TarjetaCirculacion

                        };
                        var envioEvento = await EnviarEventoEntrada(infoVerificentro.ApiEndPoint, infoVerificentro.ApiKey, DictTipoEvento.IniciarVerificacion, $"Se inicio la verificación con id {verificacion.IdVerificentro}.", verificacion.NumeroTrabajador, infoVerificentro.Nombre, verificacion.IdVerificacion.ToString(), pruebaProveedor, $"No se pudo enviar la solicitud al servicio del {infoVerificentro.Clave}.");
                        if (envioEvento.Status != ResponseStatus.Success)
                        {
                            var tipoEvento = CausaRechazo.DictClaveRechazo[CausaRechazo.AbortadaTecnico];
                            var mensaje = $"Ocurrió un error en el envío del evento {tipoEvento}.";
                            return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception(mensaje))
                            { mensaje = string.IsNullOrEmpty(envioEvento.mensaje) ? mensaje : envioEvento.mensaje, Response = new() };
                        }
                        //var servicioEventos = new ServicioEventosVerificentro();
                    }
                    // _context.ResultadosVerificacions.Update(verificacionResultado);
                    await _context.SaveChangesAsync();
                }
                var response = new ConsultaVerificacionResponse<object>
                {
                    IdEstatusPrueba = verificacionResultado.EstatusPrueba,
                    Resultados = new()

                };
                // var alerta = new Alertum
                // {
                //     TableName = DictAlertas.ActualizacionPrueba,
                //     TableId = 0,
                //     IdVerificentro = request.IdVerificentro,
                //     Data = string.Empty,
                //     IdUser = null,
                //     IdEstatusInicial = 0,
                //     MovimientoInicial = "Actualización Prueba",
                //     Fecha = DateTime.Now,
                //     IdEstatusFinal = null,
                //     MovimientoFinal = null,
                //     Leido = false,
                //     Procesada = false
                // };
                // _context.Alerta.Add(alerta);
                await _context.SaveChangesAsync();
                transaction.Complete();
                return new ResponseGeneric<ConsultaVerificacionResponse<object>>(response);

            }
        }
        catch (Exception e)
        {
            await _smadsotGenericInserts.SaveLog(e, DictTipoLog.ExcepcionPruebaDiesel);

            return new ResponseGeneric<ConsultaVerificacionResponse<object>>(e) { Response = new() };
        }
    }
    public async Task<ResponseGeneric<ConsultaVerificacionResponse<object>>> PruebaVisualGasolina(PruebaVisualRequest<ResultadosGasolina> request)
    {
        try
        {

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var verificacion = _context.vVerificacionDatosProveedorUserData.FirstOrDefault(x => x.IdVerificacion == request.IdVerificacion);
                if (verificacion == null)
                {
                    return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception("No se encontró registro de la verificacion."))
                    { mensaje = "No sé encontró registro de la verificacion.", Response = new() };
                }
                if (verificacion.IdResultadosVerificacion == null)
                {
                    return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception("Aún no se ha realizado la validación de placas del vehículo."))
                    { mensaje = "Aún no se ha realizado la validación de placas del vehículo.", Response = new() };
                }
                var verificacionResultado = _context.ResultadosVerificacions.FirstOrDefault(x => x.Id == verificacion.IdResultadosVerificacion);
                if (verificacionResultado == null)
                {
                    return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception("No hey registros de resultados en la bd."))
                    { mensaje = "Aún no se han registrado resultados para este vehículo.", Response = new() };
                }
                if (verificacionResultado.EstatusPrueba == 0)
                {
                    verificacionResultado.EstatusPrueba = 2;
                    verificacionResultado.FugasSistemaEscape = request.Resultados.Resultados.SistemaEscape;
                    verificacionResultado.PortafiltroAire = request.Resultados.Resultados.PortafiltroAire;
                    verificacionResultado.TaponDispositivoAceite = request.Resultados.Resultados.TaponDispositivoAceite;
                    verificacionResultado.TaponCombustible = request.Resultados.Resultados.TaponCombustible;
                    verificacionResultado.Bayoneta = request.Resultados.Resultados.Bayoneta;
                    verificacionResultado.DesperfectosNeumaticos = request.Resultados.Resultados.NeumaticosBuenEstato;
                    verificacionResultado.ControlEmisionAlterados = request.Resultados.Resultados.ComponenteControlEmisiones;
                    verificacionResultado.FugaLiquidoRefrigerante = request.Resultados.Resultados.FugaFluidos;
                    verificacionResultado.NumeroEscapes = request.NumeroEscapes;
                    var listChecks = new List<bool>()
                        {(bool)verificacionResultado.FugasSistemaEscape,
                        (bool)verificacionResultado.PortafiltroAire,
                        (bool)verificacionResultado.TaponDispositivoAceite,
                        (bool)verificacionResultado.TaponCombustible,
                        (bool)verificacionResultado.Bayoneta,
                        (bool)verificacionResultado.DesperfectosNeumaticos,
                        (bool)verificacionResultado.ControlEmisionAlterados,
                        (bool)verificacionResultado.FugaLiquidoRefrigerante,
                        };
                    if (listChecks.Any(x => !x))
                    {
                        if (request.FotoEvidencia == null)
                        {
                            return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception("Se debe tomar la evidencia de la falla en la inspección visual."))
                            { mensaje = "Se debe tomar la evidencia de la falla en la inspección visual.", Response = new() };
                        }
                        var url = await _blobStorage.UploadFileAsync(request.FotoEvidencia, $"VerificacionExpediente/FallaPruebaVisual/{verificacion.Id}/{verificacion.Placa}_trasera.jpg");
                        verificacionResultado.URLEvidenciaFalla = url;
                        verificacionResultado.EstatusPrueba = EstatusVerificacion.VerificacionFinalizada;
                        verificacionResultado.C_RECHAZO = CausaRechazo.Visual;
                        verificacionResultado.RESULTADO = Resultados.Rechazo;
                        var validado = await _smadsotGenericInserts.ValidateFolio(TipoCertificado.ConstanciasNoAprobado, verificacion.IdVerificentro, TipoTramite.CV, verificacion.IdUserT.Value, verificacion.Estado, verificacion.IdVerificacion, null, null);
                        if (!validado.IsSucces)
                        {
                            return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception(validado.Description))
                            { mensaje = validado.Description ?? "", Response = new() };
                        }
                    }
                    else
                    {
                        var infoVerificentro = _context.vVerificentros.FirstOrDefault(x => x.Id == verificacion.IdVerificentro);
                        if (infoVerificentro == null)
                        {
                            return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception("No sé encontró información del verificentro."))
                            { mensaje = "No sé encontró información del verificentro.", Response = new() };
                        }

                        var pruebaProveedor = new PruebaInicio
                        {
                            Id = verificacion.IdVerificacion,
                            ClaveHolograma = verificacion.DOBLECERO == 1 ? "DOBLECERO" : "",
                            MotivoVerificacion = (verificacion.IdMotivoVerificacion ?? 2).ToString("00"),
                            ClaveLinea = verificacion.Clave,
                            NumeroSerie = verificacion.Serie,
                            TecnicoVerificador = verificacion.NumeroTrabajador ?? "",
                            Marca = verificacion.MarcaProveedor,
                            Submarca = verificacion.IdCatSubmarcaVehiculo,
                            Anio = verificacion.Anio,
                            Placa = verificacion.Placa,
                            RazonSocial = verificacion.RazonSocial,
                            NombrePropietario = verificacion.Propietario,
                            RevolucionesMaxGobernador = verificacion.RPM_GOB ?? verificacion.POTMAX_RPM,
                            ClaveCombustible = verificacion.ClaveCombustible,
                            Combustible = verificacion.IdTipoCombustible,
                            Combustible_POTMAX_RPM = verificacion.POTMAX_RPM,
                            RAL_FAB = verificacion.RAL_FAB,
                            GOB_FAB = verificacion.POTMAX_RPM,
                            Pot_5024 = verificacion.POT_5024,
                            Pot_2540 = verificacion.POT_2540,
                            PotMax_RPM = verificacion.POTMAX_RPM,
                            Protocolo = verificacion.PROTOCOLO,
                            Cilindros = verificacion.CILINDROS,
                            Cilindrada = verificacion.CILINDRADA,
                            Motor_DSL = verificacion.Motor_DSL,
                            PBV = (ulong)verificacion.PBV,
                            PBV_Equivalente = verificacion.PBV_EQUIV,
                            PBV_ASM = verificacion.PBV_ASM,
                            C_ABS = verificacion.C_ABS,
                            ConvertidorCatalitico = verificacion.CONV_CATAL,
                            OBD = (ulong)verificacion.OBD,
                            DobleCero = (ulong)verificacion.DOBLECERO,
                            CERO_GASOL = (ulong)verificacion.CERO_GASOL,
                            CERO_GASLP = (ulong)verificacion.CERO_GASLP,
                            CERO_GASNC = (ulong)verificacion.CERO_GASNC,
                            CERO_DSL = (ulong)verificacion.CERO_DSL,
                            TarjetaCirculacion = verificacion.TarjetaCirculacion

                        };
                        var envioEvento = await EnviarEventoEntrada(infoVerificentro.ApiEndPoint, infoVerificentro.ApiKey, DictTipoEvento.IniciarVerificacion, $"Se inicio la verificación con id {verificacion.IdVerificentro}.", pruebaProveedor.TecnicoVerificador, infoVerificentro.Nombre, verificacion.IdVerificacion.ToString(), pruebaProveedor, $"No se pudo enviar la solicitud al servicio del {infoVerificentro.Clave}. ");
                        if (envioEvento.Status != ResponseStatus.Success)
                        {
                            var tipoEvento = CausaRechazo.DictClaveRechazo[CausaRechazo.AbortadaTecnico];
                            var mensaje = $"Ocurrió un error en el envío del evento {tipoEvento}.";
                            return new ResponseGeneric<ConsultaVerificacionResponse<object>>(new Exception(mensaje))
                            { mensaje = string.IsNullOrEmpty(envioEvento.mensaje) ? mensaje : envioEvento.mensaje, Response = new() };
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                var response = new ConsultaVerificacionResponse<object>
                {
                    IdEstatusPrueba = verificacionResultado.EstatusPrueba,
                    Resultados = null
                };
                // var alerta = new Alertum
                // {
                //     TableName = DictAlertas.ActualizacionPrueba,
                //     TableId = 0,
                //     IdVerificentro = request.IdVerificentro,
                //     Data = string.Empty,
                //     IdUser = null,
                //     IdEstatusInicial = 0,
                //     MovimientoInicial = "Actualización Prueba",
                //     Fecha = DateTime.Now,
                //     IdEstatusFinal = null,
                //     MovimientoFinal = null,
                //     Leido = false,
                //     Procesada = false
                // };
                // _context.Alerta.Add(alerta);
                await _context.SaveChangesAsync();
                transaction.Complete();
                return new ResponseGeneric<ConsultaVerificacionResponse<object>>(response);

            }
        }
        catch (Exception e)
        {
            await _smadsotGenericInserts.SaveLog(e, DictTipoLog.ExcepcionPruebaGasolina);

            return new ResponseGeneric<ConsultaVerificacionResponse<object>>(e) { Response = new() };
        }
    }

    public ResponseGeneric<ConsultaVerificacionResponse<PruebaVisualResponse>> ConsultaPruebaVisual(ConsultaVerificacionRequest request)
    {
        var verificacion = _context.vVerificacionDatosProveedors.FirstOrDefault(x => x.IdVerificacion == request.IdVerificacion);
        if (verificacion == null)
        {
            return new ResponseGeneric<ConsultaVerificacionResponse<PruebaVisualResponse>>(new Exception("No se encontró registro de la verificacion."))
            { mensaje = "No sé encontró registro de la verificacion." };
        }
        if (verificacion.IdResultadosVerificacion == null)
        {
            return new ResponseGeneric<ConsultaVerificacionResponse<PruebaVisualResponse>>(new Exception("Aún no se ha realizado la validación de placas del vehículo."))
            { mensaje = "Aún no se ha realizado la validación de placas del vehículo." };
        }
        var prueba = new PruebaVisualResponse();
        if (request.IdTipoPrueba >= EstatusVerificacion.TerminaPruebaVisual)
        {
            prueba = new PruebaVisualResponse
            {
                TipoCombustible = verificacion.IdTipoCombustible ?? 0,
                NumeroEscapes = verificacion.NumeroEscapes ?? 0,
                FugasSistemaEscape = verificacion.FugasSistemaEscape ?? false,
                GobernadorBuenEstado = verificacion.GobernadorBuenEstado ?? false,
                PortafiltroAire = verificacion.PortafiltroAire ?? false,
                FiltroAire = verificacion.FiltroAire ?? false,
                TaponDispositivoAceite = verificacion.TaponDispositivoAceite ?? false,
                TaponCombustible = verificacion.TaponCombustible ?? false,
                Bayoneta = verificacion.Bayoneta ?? false,
                FugaAceiteMotor = verificacion.FugaAceiteMotor ?? false,
                FugaAceiteTransmision = verificacion.FugaAceiteTransmision ?? false,
                FugaLiquidoRefrigerante = verificacion.FugaLiquidoRefrigerante ?? false,
                DibujoNeumaticos = verificacion.DibujoNeumaticos ?? false,
                DesperfectosNeumaticos = verificacion.DesperfectosNeumaticos ?? false,
                DimensionesNeumaticoIncorrectas = verificacion.DimensionesNeumaticoIncorrectas ?? false,
                ControlEmisionDesconectados = verificacion.ControlEmisionDesconectados ?? false,
                ControlEmisionAlterados = verificacion.ControlEmisionAlterados ?? false,
                PlacasCorrespondientes = verificacion.PlacasCorrespondientes ?? false

            };
        }

        var response = new ConsultaVerificacionResponse<PruebaVisualResponse>
        {
            IdEstatusPrueba = (int)verificacion.EstatusPrueba,
            Resultados = prueba,
        };
        return new ResponseGeneric<ConsultaVerificacionResponse<PruebaVisualResponse>>(response);
    }

    public async Task<ResponseGeneric<long>> AbortarPrueba(ConsultaVerificacionRequest request)
    {
        try
        {
            var response = new ResponseGeneric<long>() { Response = new() };
            var verificacion = _context.vVerificacionDatosProveedors.FirstOrDefault(x => x.IdVerificacion == request.IdVerificacion && x.IdVerificentro == request.IdVerificentro);
            if (verificacion == null)
            {
                return new ResponseGeneric<long>(new Exception("No se encontró registro de la verificacion."))
                { mensaje = "No sé encontró registro de la verificacion.", Response = new() };
            }
            if (verificacion.IdResultadosVerificacion == null)
            {
                return new ResponseGeneric<long>(new Exception("Aún no se ha realizado la validación de placas del vehículo."))
                { mensaje = "Aún no se ha realizado la validación de placas del vehículo.", Response = new() };
            }
            var verificaciondb = _context.Verificacions.Include(v => v.ResultadosVerificacion).FirstOrDefault(v => v.Id == request.IdVerificacion);
            verificaciondb.ResultadosVerificacion.FinalizacionPruebas = DateTime.Now;
            verificaciondb.ResultadosVerificacion.C_RECHAZO = request.ClaveRechazo;
            verificaciondb.ResultadosVerificacion.RESULTADO = Resultados.Rechazo;
            verificaciondb.ResultadosVerificacion.EstatusPrueba = EstatusVerificacion.VerificacionAbortada;
            verificaciondb.IdUserTecnico = request.IdUserTecnico;
            // _context.Update(verificaciondb);
            if (request.ClaveRechazo == CausaRechazo.CertificadoAnteriorRobado)
            {
                var alertaAbort = new Alertum
                {
                    TableName = DictAlertas.Cita,
                    TableId = 0,
                    IdVerificentro = request.IdVerificentro,
                    Data = null,
                    IdUser = null,
                    IdEstatusInicial = 0,
                    MovimientoInicial = string.Format("Se detecto el uso de un certificado robado en el auto como placas {0}, número de serie {1} y propietario {2}", verificaciondb.Placa, verificaciondb.Serie, verificacion.Propietario),
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
                //     IdVerificentro = request.IdVerificentro,
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

                response.Response = await _context.SaveChangesAsync();
                response.Status = response.Response > 0 ? ResponseStatus.Success : ResponseStatus.Failed;
                return response;
            }
            var validado = await _smadsotGenericInserts.ValidateFolio(TipoCertificado.ConstanciasNoAprobado, verificacion.IdVerificentro, TipoTramite.CV, request.IdUserTecnico, verificacion.Estado, verificacion.IdVerificacion, null, null);
            if (!validado.IsSucces)
            {
                return new ResponseGeneric<long>(new Exception(validado.Description))
                { mensaje = validado.Description ?? "", Response = new() };
            }

            if (verificacion.EstatusPrueba != null)
            {
                var infoVerificentro = _context.vVerificentros.FirstOrDefault(x => x.Id == verificacion.IdVerificentro);
                if (infoVerificentro == null)
                {
                    return new ResponseGeneric<long>(new Exception("No sé encontró información del verificentro."))
                    { mensaje = "No sé encontró información del verificentro.", Response = new() };
                }
                var pruebaUpdate = new PruebaInicio
                {
                    Id = verificaciondb.Id,
                    CRechazo = CausaRechazo.AbortadaTecnico,
                    Resultado = Resultados.Escapado,
                    IdEstatus = EstatusVerificacion.VerificacionAbortada,
                };
                if (verificacion.EstatusPrueba >= EstatusVerificacion.TerminaPruebaVisual)
                {
                    var envioEvento = await EnviarEventoEntrada(infoVerificentro.ApiEndPoint, infoVerificentro.ApiKey, DictTipoEvento.ActualizarPrueba, "Abortada mediante la aplicación móvil por el Técnico.", verificacion.NombreTecnico, infoVerificentro.Nombre, verificaciondb.Id.ToString(), pruebaUpdate, $"No se pudo enviar la solicitud al servicio del {infoVerificentro.Clave}. ");
                    if (envioEvento.Status != ResponseStatus.Success)
                    {
                        var tipoEvento = CausaRechazo.DictClaveRechazo[CausaRechazo.AbortadaTecnico];
                        var mensaje = $"Ocurrió un error en el envío del evento {tipoEvento}.";
                        return new ResponseGeneric<long>(new Exception(mensaje))
                        { mensaje = string.IsNullOrEmpty(envioEvento.mensaje) ? mensaje : envioEvento.mensaje, Response = new() };
                    }
                }
            }
            // var alerta = new Alertum
            // {
            //     TableName = DictAlertas.ActualizacionPrueba,
            //     TableId = 0,
            //     IdVerificentro = request.IdVerificentro,
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
            // _context.Alerta.Add(alerta);
            response.Response = await _context.SaveChangesAsync();
            response.Status = response.Response > 0 ? ResponseStatus.Success : ResponseStatus.Failed;
            return response;
        }
        catch (Exception e)
        {
            await _smadsotGenericInserts.SaveLog(e, DictTipoLog.ExcepcionAbortarPrueba);

            return new ResponseGeneric<long>(new Exception("Ocurrió un error a intentar abortar la prueba."))
            { mensaje = "Ocurrió un error a intentar abortar la prueba.", Response = new() };
        }

    }
    public async Task<ResponseGeneric<vVerificacionExpediente>> GetExpediente(long id)
    {
        try
        {

            var expediente = await _context.vVerificacionExpedientes.FirstOrDefaultAsync(x => x.IdVerificacion == id);
            return new ResponseGeneric<vVerificacionExpediente>(expediente);

        }
        catch (Exception ex)
        {

            return new ResponseGeneric<vVerificacionExpediente>(ex);
        }
    }

    public async Task<ResponseGeneric<bool>> VolverImprimir(long id)
    {
        try
        {
            if (id > 0)
            {
                var aux = await _context.FoliosFormaValoradaVerificentros.FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("No se encontró registro");


                aux.Impreso = false;
                var resultadosVerificacion = await _context.ResultadosVerificacions.FirstOrDefaultAsync(x => x.Id == aux.IdVerificacion) ?? throw new Exception("No se encontró registro");
                resultadosVerificacion.EstatusPrueba = EstatusVerificacion.VerificacionFinalizada;
                _context.SaveChanges();

                return new ResponseGeneric<bool>(true);

            }
            else
            {
                return new ResponseGeneric<bool>(false);
            }
        }
        catch (Exception ex)
        {
            return new ResponseGeneric<bool>("Ocurrio un error");
        }
    }

    public async Task<ResponseGeneric<List<DashboardLineaResponseData>>> ObtnerInformacionLineas()
    {
        try
        {
            var responseGeneric = new ResponseGeneric<List<DashboardLineaResponseData>>(new List<DashboardLineaResponseData>());
            var currentUser = _userResolver.GetUser();
            var today = DateTime.Now.Date;
            var otherDay = today.AddDays(1);
            var verificacionesEnCola = await _context.vVerificacionCita.Where(x => x.IdVerificentro == currentUser.IdVerificentro
            && (x.EstatusPrueba < EstatusVerificacion.FolioImpreso || x.EstatusPrueba == null) && x.Fecha >= today && x.Fecha < otherDay).ToListAsync();

            var LineasVerificentro = _context.Lineas.Where(l => l.IdVerificentro == currentUser.IdVerificentro).OrderBy(x => x.Clave).Select(l => new { l.Id, l.Nombre, Activa = l.IdCatEstatusLinea == CierreAperturaLineaDic.IdActivo, l.Clave });
            foreach (var item in LineasVerificentro)
            {
                var dataLinea = new DashboardLineaResponseData();
                var camara = await TestCamaraLinea(item.Clave);
                if (camara.Status != ResponseStatus.Success)
                    dataLinea.Camara = false;
                dataLinea.IdLinea = item.Id;
                dataLinea.Linea = item.Nombre;
                dataLinea.Line = item.Activa;
                var verificacionesEnColaLinea = verificacionesEnCola.Where(vl => vl.IdLinea == item.Id && vl.Orden != null).OrderBy(x => x.Orden).ToList();
                var autoActual = verificacionesEnColaLinea.FirstOrDefault(x => x.EstatusPrueba != null);
                if (autoActual != null)
                {
                    if (autoActual.EstatusPrueba == EstatusVerificacion.VerificacionAbortada || autoActual.EstatusPrueba == EstatusVerificacion.VerificacionFinalizada)
                    {
                        var autosEstatus = verificacionesEnColaLinea.Where(x => x.EstatusPrueba != null);
                        autoActual = autosEstatus.FirstOrDefault(x => x.EstatusPrueba != EstatusVerificacion.VerificacionAbortada || x.EstatusPrueba != EstatusVerificacion.VerificacionFinalizada) ?? autoActual;
                    }
                    verificacionesEnColaLinea.Remove(autoActual);
                    dataLinea.Modelo = autoActual.Modelo;
                    dataLinea.EstatusPrueba = (int)autoActual.EstatusPrueba;
                    dataLinea.Turno = ((int)autoActual.Orden).ToString("0000");
                    dataLinea.Duenio = autoActual.NombrePropietario;
                    dataLinea.Placas = autoActual.Placa;
                    dataLinea.Serie = autoActual.Serie;
                    dataLinea.PruebaOBD = autoActual.PruebaObd ?? false;
                    dataLinea.PruebaEmisiones = autoActual.PruebaEmisiones ?? false;
                    dataLinea.PruebaOpacidad = autoActual.PruebaOpacidad ?? false;
                    dataLinea.CRechazo = autoActual.C_RECHAZO ?? 0;
                    dataLinea.Resultado = autoActual.RESULTADO;
                }
                dataLinea.AutosEspera = verificacionesEnColaLinea.Count();

                responseGeneric.Response.Add(dataLinea);
            }
            return responseGeneric;
        }
        catch (Exception e)
        {
            return new ResponseGeneric<List<DashboardLineaResponseData>>(e);
        }


    }
    public async Task<ResponseGeneric<object>> TestCamaraLinea(string clave)
    {
        try
        {
            var responseGeneric = new ResponseGeneric<object>();
            var infoVerificentro = _context.vVerificentros.FirstOrDefault(x => x.Id == _userResolver.GetUser().IdVerificentro);
            var capturaPlacas = await CapturarPlacas(infoVerificentro.ApiEndPoint, infoVerificentro.ApiKey, clave);
            responseGeneric.Status = capturaPlacas.Status;
            if (capturaPlacas.Status == ResponseStatus.Success)
            {
                if (capturaPlacas.Response.Delantera == null && capturaPlacas.Response.Trasera == null)
                {
                    responseGeneric.Status = ResponseStatus.Failed;
                    await _smadsotGenericInserts.SaveLog(JsonConvert.SerializeObject(capturaPlacas), DictTipoLog.FallaCamaras);

                }
            }
            return responseGeneric;
        }
        catch (Exception e)
        {
            await _smadsotGenericInserts.SaveLog(e, DictTipoLog.ExcepcionCamaraLineas);
            return new ResponseGeneric<object>(e) { mensaje = "Ocurrió un error al conectar con las camaras" };
        }


    }
    public async Task<ResponseGeneric<ConsultaVerificacionResponse<string>>> PruebaFinalizada(long id)
    {
        try
        {

            var expediente = await _context.vVerificacionDatosProveedors.FirstOrDefaultAsync(x => x.IdVerificacion == id);

            return new ResponseGeneric<ConsultaVerificacionResponse<string>>(new ConsultaVerificacionResponse<string>
            {
                IdEstatusPrueba = expediente?.EstatusPrueba ?? 0,
                Resultados = ((expediente?.EstatusPrueba ?? 0) != 9 && (expediente?.EstatusPrueba ?? 0) != 10) ? "La prueba no ha finalizado" : "La prueba finalizó. " + Resultados.DictResultados[(int)(expediente?.RESULTADO ?? 0)]
            });

        }
        catch (Exception ex)
        {

            return new ResponseGeneric<ConsultaVerificacionResponse<string>>(ex);
        }
    }
    public async void InsertarPruebaTest(long idprueba)
    {
        var verificacion = _context.vVerificacionDatosProveedorUserData.FirstOrDefault(x => x.IdVerificacion == idprueba);
        var infoVerificentro = _context.vVerificentros.FirstOrDefault(x => x.Id == 17);

        var pruebaProveedor = new PruebaInicio
        {
            Id = verificacion.IdVerificacion,
            ClaveHolograma = verificacion.DOBLECERO == 1 ? "DOBLECERO" : "",
            ClaveLinea = verificacion.Clave,
            NumeroSerie = verificacion.Serie,
            TecnicoVerificador = verificacion.NumeroTrabajador ?? "",
            Marca = verificacion.MarcaProveedor,
            Submarca = verificacion.IdCatSubmarcaVehiculo,
            Anio = verificacion.Anio,
            Placa = verificacion.Placa,
            RazonSocial = verificacion.RazonSocial,
            NombrePropietario = verificacion.Propietario,
            RevolucionesMaxGobernador = verificacion.RPM_GOB ?? verificacion.POTMAX_RPM,
            ClaveCombustible = verificacion.ClaveCombustible,
            Combustible = verificacion.IdTipoCombustible,
            Combustible_POTMAX_RPM = verificacion.POTMAX_RPM,
            RAL_FAB = verificacion.RAL_FAB,
            GOB_FAB = verificacion.POTMAX_RPM,
            Pot_5024 = verificacion.POT_5024,
            Pot_2540 = verificacion.POT_2540,
            PotMax_RPM = verificacion.POTMAX_RPM,
            Protocolo = verificacion.PROTOCOLO,
            Cilindros = verificacion.CILINDROS,
            Cilindrada = verificacion.CILINDRADA,
            Motor_DSL = verificacion.Motor_DSL,
            PBV = (ulong)verificacion.PBV,
            PBV_Equivalente = verificacion.PBV_EQUIV,
            PBV_ASM = verificacion.PBV_ASM,
            C_ABS = verificacion.C_ABS,
            ConvertidorCatalitico = verificacion.CONV_CATAL,
            OBD = (ulong)verificacion.OBD,
            DobleCero = (ulong)verificacion.DOBLECERO,
            CERO_GASOL = (ulong)verificacion.CERO_GASOL,
            CERO_GASLP = (ulong)verificacion.CERO_GASLP,
            CERO_GASNC = (ulong)verificacion.CERO_GASNC,
            CERO_DSL = (ulong)verificacion.CERO_DSL,

        };
        var envioEvento = await EnviarEventoEntrada(infoVerificentro.ApiEndPoint, infoVerificentro.ApiKey, DictTipoEvento.IniciarVerificacion, $"Se inicio la verificación con id {verificacion.IdVerificentro}.", pruebaProveedor.TecnicoVerificador, infoVerificentro.Nombre, verificacion.IdVerificacion.ToString(), pruebaProveedor, $"No se pudo enviar la solicitud al servicio del {infoVerificentro.Clave}. ");
        if (envioEvento.Status != ResponseStatus.Success)
        {
            var tipoEvento = CausaRechazo.DictClaveRechazo[CausaRechazo.AbortadaTecnico];
            var mensaje = $"Ocurrió un error en el envío del evento {tipoEvento}.";
        }
    }
    #endregion
    #region Private methods
    private async Task<ResponseGeneric<vAlertum>> PermisoIngresoManual(vVerificacionCitum verificacion, long IdUser)
    {
        try
        {
            DateTime fecha = DateTime.Now;
            var finDia = new DateTime(fecha.Year, fecha.Month, fecha.Day, 23, 59, 59);
            var inicioDia = new DateTime(fecha.Year, fecha.Month, fecha.Day, 0, 0, 0);
            var alertaDb = _context.vAlerta.FirstOrDefault(a => a.TableId == verificacion.Id && a.TableName.Equals(DictAlertas.Verificacion) && a.Fecha >= inicioDia && a.Fecha <= finDia);
            if (alertaDb?.MovimientoFinal != null)
            {
                return new ResponseGeneric<vAlertum>() { Response = alertaDb, Status = ResponseStatus.Success };
            }
            var alerta = new Alertum
            {
                TableName = DictAlertas.Verificacion,
                TableId = verificacion.Id,
                IdVerificentro = verificacion.IdVerificentro,
                Data = JsonConvert.SerializeObject(verificacion, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    NullValueHandling = NullValueHandling.Ignore
                }),
                IdUser = IdUser,
                IdEstatusInicial = 0,
                MovimientoInicial = string.Format(MovimientosDicts.DictMovimientoVerificacion[VerificacionIngresoManualEstatus.Pendiente], verificacion.Linea, verificacion.Placa, verificacion.Serie),
                Fecha = DateTime.Now,
                IdEstatusFinal = null,
                MovimientoFinal = null,
                Leido = false,
                Procesada = false
            };
            _context.Alerta.Add(alerta);
            await _context.SaveChangesAsync();

            return new ResponseGeneric<vAlertum>() { Response = new vAlertum() { TableId = alerta.TableId }, Status = ResponseStatus.Success };


        }
        catch (Exception e)
        {
            await _smadsotGenericInserts.SaveLog(e, DictTipoLog.ExcepcionPermisoManual);

            return new ResponseGeneric<vAlertum>(e);
        }
    }
    private async Task<ResponseGeneric<string>> EnviarEventoEntrada(string ApiEndPoint, string ApiKey, int TipoEvento, string nota, string usuarioTecnico, string verificentro, string Id, PruebaInicio? prueba, string error)
    {
        var serviceVerificentro = new ServicioEventosVerificentro(ApiEndPoint, ApiKey);
        var requestService = new EventoEntradaRequest
        {
            Evento = new EventoEntrada
            {
                Fecha = DateTime.Now,
                IdIdentificador = Id,
                IdTipo = TipoEvento,
                Nota = nota,
                TecnicoVerificador = usuarioTecnico
            },
            Prueba = prueba
        };
        var responseService = await serviceVerificentro.PostAsync<EventoEntradaRequest, object>("RecepcionEventos/EventoEntrada", requestService, verificentro, error);
        if (responseService.Status != ResponseStatus.Success)
        {
            await _smadsotGenericInserts.SaveLog(JsonConvert.SerializeObject(responseService), DictTipoLog.ExcepcionEventoEntrada);
            return new ResponseGeneric<string>() { mensaje = responseService.mensaje, CurrentException = responseService.CurrentException, Status = ResponseStatus.Failed };
        }
        return new ResponseGeneric<string>("", true);
    }
    private async Task<ResponseGeneric<PlacasResponse>> CapturarPlacas(string ApiEndPoint, string ApiKey, string clave)
    {
        var serviceVerificentro = new ServicioEventosVerificentro(ApiEndPoint, ApiKey);

        var responseService = await serviceVerificentro.GetAsync<PlacasResponse>($"RecepcionEventos/CapturaPlacas/{clave}", clave, $"No se pudo obtener imagen de las cámaras del {clave}.");
        if (responseService.Status == ResponseStatus.Failed)
        {
            await _smadsotGenericInserts.SaveLog(JsonConvert.SerializeObject(responseService), DictTipoLog.ExcepcionEventoEntrada);
        }
        return responseService;
    }
    private async Task<ResponseGeneric<string>> ProcessImageOCR(byte[] image)
    {
        // hacemos el recorte de la imagen
        var croppedImage = _customVision.ProcessLicensePlate(image);
        // Validamos que no hay ningún error
        if (croppedImage.Status == ResponseStatus.Failed)
        {
            return new ResponseGeneric<string>(new Exception("No se pudo procesar la imagen o no se detectaron placas de vehículo en ella."))
            { mensaje = "No se pudo procesar la imagen o no se detectaron placas de vehículo en ella." }; ;
        }
        // Envíamos el recorte de la imagen para validar las placas por OCR
        var analisisPlacas = await _computerVisionService.AnalyzeImage(croppedImage.Response);
        // Validamos que no hubo ningún error
        if (analisisPlacas.Status == ResponseStatus.Failed)
        {
            var msj = string.IsNullOrEmpty(analisisPlacas?.mensaje) ? "Las placas son ilegibles, no se pudo obtener la información." : analisisPlacas.mensaje;
            return new ResponseGeneric<string>(new Exception(msj))
            { mensaje = msj };

        }
        if (string.IsNullOrEmpty(analisisPlacas?.Response?.Text))
        {
            return new ResponseGeneric<string>(new Exception($"El servicio de OCR no logro reconocer las placas."))
            { mensaje = $"El servicio de OCR no logro reconocer las placas." };

        }
        // obtenemos las placas
        return new ResponseGeneric<string>(analisisPlacas.Response.Text.Replace("-", "").Trim(), true);

    }

    public async Task<ResponseGeneric<ResponseGrid<string>>> GetvVerificacionCitas(VerificacionCitaRequest request)
    {
        try
        {
            DateTime fecha = DateTime.Now;
            var finDia = new DateTime(fecha.Year, fecha.Month, fecha.Day, 23, 59, 59);
            var inicioDia = new DateTime(fecha.Year, fecha.Month, fecha.Day, 0, 0, 0);

            var tot = await _context.vVerificacionCita.Where(x => x.Fecha >= inicioDia && x.Fecha <= finDia && x.IdVerificentro == request.IdVerificentro
            && x.EstatusPrueba > EstatusVerificacion.TerminaPruebaVisual).CountAsync();
            var citas = _context.vVerificacionCita.Where(x => x.Fecha >= inicioDia && x.Fecha <= finDia && x.IdVerificentro == request.IdVerificentro
            && (string.IsNullOrEmpty(request.Busqueda)
                                || EF.Functions.Like(x.Marca.ToLower(), $"%{request.Busqueda}%")) && (x.EstatusPrueba == null || x.EstatusPrueba <= EstatusVerificacion.TerminaPruebaVisual)).AsQueryable();

            //if (!string.IsNullOrEmpty(request.Busqueda))
            //{
            //    citas = citas.Where(x => x.Serie.ToLower().Contains(request.Busqueda.ToLower()));
            //}

            var filtered = string.IsNullOrEmpty(request.Busqueda) ? tot : citas.Count();

            if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
            {
                citas = citas.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
            }

            if (request.Pagina > 0 && request.Registros > 0 && (request.Pagination ?? false))
            {
                citas = citas.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
            }
            DateTime now = DateTime.Now;
            var result = new ResponseGrid<string>
            {
                RecordsTotal = tot,
                RecordsFiltered = filtered,
                Data = await citas.Select(x => x.Serie).ToListAsync(),
            };
            return new ResponseGeneric<ResponseGrid<string>>(result);
        }
        catch (Exception ex)
        {
            return new ResponseGeneric<ResponseGrid<string>>(ex);
        }
    }
    #endregion
}

public interface IVerificacionNegocio
{
    // ResponseGeneric<ConsultaVerificacionResponse<ResultadosPruebaOBD>> ConsultaVerificacionOBD(ConsultaVerificacionRequest request);
    // ResponseGeneric<ConsultaVerificacionResponse<ResultadosEmisiones>> ConsultaVerificacionEmisiones(ConsultaVerificacionRequest request);
    // ResponseGeneric<ConsultaVerificacionResponse<ResultadosOpacidad>> ConsultaVerificacionOpacidad(ConsultaVerificacionRequest request);
    Task<ResponseGeneric<object>> TestCamaraLinea(string clave);
    ResponseGeneric<ConsultaVerificacionResponse<PruebaVisualResponse>> ConsultaPruebaVisual(ConsultaVerificacionRequest request);
    Task<ResponseGeneric<ConsultaVerificacionResponse<object>>> PruebaVisualDiesel(PruebaVisualRequest<ResultadosDiesel> request);
    Task<ResponseGeneric<ConsultaVerificacionResponse<object>>> PruebaVisualGasolina(PruebaVisualRequest<ResultadosGasolina> request);
    public Task<ResponseGeneric<ValidacionMatriculaResponse>> SavePlacas(VerificacionRequest request);
    public Task<ResponseGeneric<long>> AbortarPrueba(ConsultaVerificacionRequest request);
    public Task<ResponseGeneric<vVerificacionExpediente>> GetExpediente(long id);

    public Task<ResponseGeneric<List<DashboardLineaResponseData>>> ObtnerInformacionLineas();
    public Task<ResponseGeneric<ConsultaVerificacionResponse<string>>> PruebaFinalizada(long id);
    public Task<ResponseGeneric<ResponseGrid<string>>> GetvVerificacionCitas(VerificacionCitaRequest request);
    void InsertarPruebaTest(long idprueba);

    public Task<ResponseGeneric<bool>> VolverImprimir(long id);

}
//     public ResponseGeneric<ConsultaVerificacionResponse<ResultadosPruebaOBD>> ConsultaVerificacionOBD(ConsultaVerificacionRequest request)
//     {
//         var verificacion = _context.vVerificacionDatosProveedors.FirstOrDefault(x => x.IdVerificacion == request.IdVerificacion);
//         if (verificacion == null)
//         {
//             return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosPruebaOBD>>(new Exception("No se encontró registro de la verificacion."))
//             { mensaje = "No sé encontró registro de la verificacion." };
//         }
//         if (verificacion.IdResultadosVerificacion == null)
//         {
//             return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosPruebaOBD>>(new Exception("Aún no se ha realizado la validación de placas del vehículo."))
//             { mensaje = "Aún no se ha realizado la validación de placas del vehículo." };
//         }
//         if (verificacion.PruebaObd ?? false)
//         {
//             return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosPruebaOBD>>(new Exception("Aún no se registran los resultados para la prueba."))
//             { mensaje = "Aún no se registran los resultados para la prueba." };
//         }
//         var prueba = new ResultadosPruebaOBD();
//         if (verificacion.PruebaEmisiones ?? false)
//         {
//             if (verificacion.EstatusPrueba != EstatusVerificacion.EnPruebaOBD || verificacion.EstatusPrueba != EstatusVerificacion.TerminaPruebaOBD)
//             {
//                 return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosPruebaOBD>>(new ConsultaVerificacionResponse<ResultadosPruebaOBD>
//                 {
//                     IdEstatusPrueba = (int)verificacion.EstatusPrueba,
//                     Resultados = prueba
//                 });
//             }
//             return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosPruebaOBD>>(new Exception("Aún no se registran los resultados para la prueba de."))
//             { mensaje = "Aún no se ha realizado la validación de placas del vehículo." };
//         }
//         prueba = new ResultadosPruebaOBD
//         {
//             Etapa = verificacion.Etapa ?? "",
//             Protocolo = verificacion.PROTOCOLO,
//             OBD_TIPO_SDB = verificacion.OBD_TIPO_SDB,
//             OBD_MIL = verificacion.OBD_MIL,
//             OBD_CATAL = verificacion.OBD_CATAL,
//             OBD_CILIN = verificacion.OBD_CILIN,
//             OBD_COMBU = verificacion.OBD_COMBU,
//             OBD_INTEG = verificacion.OBD_INTEG,
//             OBD_OXIGE = verificacion.OBD_OXIGE,
//             SPS_Humo = verificacion.SPS_Humo,
//             SPS_2540 = verificacion.SPS_2540,
//             SPS_5024 = verificacion.SPS_5024,
//             HC = verificacion.HC,
//             CO = verificacion.CO,
//             CO2 = verificacion.CO2,
//             O2 = verificacion.O2,
//             NO = verificacion.NO,
//             LAMDA = verificacion.LAMDA,
//             FCNOX = verificacion.FCNOX,
//             FCDIL = verificacion.FCDIL,
//             RPM = verificacion.RPM,
//             KPH = verificacion.KPH,
//             VEL_LIN = verificacion.VEL_LIN,
//             VEL_ANG = verificacion.VEL_ANG,
//             BHP = verificacion.BHP,
//             PAR_TOR = verificacion.PAR_TOR,
//             FUERZA = verificacion.FUERZA,
//             POT_FRENO = verificacion.POT_FRENO,
//             TEMP = verificacion.TEMP,
//             PRESION = verificacion.PRESION,
//             HUMREL = verificacion.HUMREL,
//             LAMDA_5024 = verificacion.LAMDA_5024,
//             TEMP_5024 = verificacion.TEMP_5024,
//             HR_5024 = verificacion.HR_5024,
//             PSI_5024 = verificacion.PSI_5024,
//             FCNOX_5024 = verificacion.FCNOX_5024,
//             FCDIL_5024 = verificacion.FCDIL_5024,
//             RPM_5024 = verificacion.RPM_5024,
//             KPH_5024 = verificacion.KPH_5024,
//             THP_5024 = verificacion.THP_5024,
//             VOLTS_5024 = verificacion.VOLTS_5024,
//             HC_5024 = verificacion.HC_5024,
//             CO_5024 = verificacion.CO_5024,
//             CO2_5024 = verificacion.CO2_5024,
//             COCO2_5024 = verificacion.COCO2_5024,
//             O2_5024 = verificacion.O2_5024,
//             NO_5024 = verificacion.NO_5024,
//             LAMDA_2540 = verificacion.LAMDA_2540,
//             TEMP_2540 = verificacion.TEMP_2540,
//             HR_2540 = verificacion.HR_2540,
//             PSI_2540 = verificacion.PSI_2540,
//             FCNOX_2540 = verificacion.FCNOX_2540,
//             FCDIL_2540 = verificacion.FCDIL_2540,
//             RPM_2540 = verificacion.RPM_2540,
//             KPH_2540 = verificacion.KPH_2540,
//             THP_2540 = verificacion.THP_2540,
//             VOLTS_2540 = verificacion.VOLTS_2540,
//             HC_2540 = verificacion.HC_2540,
//             CO_2540 = verificacion.CO_2540,
//             CO2_2540 = verificacion.CO2_2540,
//             COCO2_2540 = verificacion.COCO2_2540,
//             O2_2540 = verificacion.O2_2540,
//             NO_2540 = verificacion.NO_2540,
//             C_Rechazo_OBD = verificacion.C_RECHAZO_OBD,
//             RESULTADO = verificacion.RESULTADO,
//             EstatusPrueba = verificacion.EstatusPrueba,
//         };
//         var response = new ConsultaVerificacionResponse<ResultadosPruebaOBD>
//         {
//             IdEstatusPrueba = (int)verificacion.EstatusPrueba,
//             Resultados = prueba,
//         };
//         return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosPruebaOBD>>(response);
//     }
//     public ResponseGeneric<ConsultaVerificacionResponse<ResultadosEmisiones>> ConsultaVerificacionEmisiones(ConsultaVerificacionRequest request)
//     {
//         var verificacion = _context.vVerificacionDatosProveedors.FirstOrDefault(x => x.IdVerificacion == request.IdVerificacion);
//         if (verificacion == null)
//         {
//             return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosEmisiones>>(new Exception("No se encontró registro de la verificacion."))
//             { mensaje = "No sé encontró registro de la verificacion." };
//         }
//         if (verificacion.IdResultadosVerificacion == null)
//         {
//             return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosEmisiones>>(new Exception("Aún no se ha realizado la validación de placas del vehículo."))
//             { mensaje = "Aún no se ha realizado la validación de placas del vehículo." };
//         }
//         var prueba = new ResultadosEmisiones();
//         if (verificacion.PruebaEmisiones ?? false)
//         {
//             if (verificacion.EstatusPrueba != EstatusVerificacion.EnPruebaEstaticaDinamica || verificacion.EstatusPrueba != EstatusVerificacion.TerminaPruebaEstaticaDinamica)
//             {
//                 return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosEmisiones>>(new ConsultaVerificacionResponse<ResultadosEmisiones>
//                 {
//                     IdEstatusPrueba = (int)verificacion.EstatusPrueba,
//                     Resultados = prueba
//                 });
//             }
//             return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosEmisiones>>(new Exception("Aún no se registran los resultados para la prueba de."))
//             { mensaje = "Aún no se ha realizado la validación de placas del vehículo." };
//         }
//         prueba = new ResultadosEmisiones
//         {
//             Etapa = verificacion.Etapa,
//             Protocolo = verificacion.PROTOCOLO,
//             SPS_Humo = verificacion.SPS_Humo,
//             SPS_2540 = verificacion.SPS_2540,
//             SPS_5024 = verificacion.SPS_5024,
//             HC = verificacion.HC,
//             CO = verificacion.CO,
//             CO2 = verificacion.CO2,
//             O2 = verificacion.O2,
//             NO = verificacion.NO,
//             LAMDA = verificacion.LAMDA,
//             FCNOX = verificacion.FCNOX,
//             FCDIL = verificacion.FCDIL,
//             RPM = verificacion.RPM,
//             KPH = verificacion.KPH,
//             VEL_LIN = verificacion.VEL_LIN,
//             VEL_ANG = verificacion.VEL_ANG,
//             BHP = verificacion.BHP,
//             PAR_TOR = verificacion.PAR_TOR,
//             FUERZA = verificacion.FUERZA,
//             POT_FRENO = verificacion.POT_FRENO,
//             TEMP = verificacion.TEMP,
//             PRESION = verificacion.PRESION,
//             HUMREL = verificacion.HUMREL,
//             LAMDA_5024 = verificacion.LAMDA_5024,
//             TEMP_5024 = verificacion.TEMP_5024,
//             HR_5024 = verificacion.HR_5024,
//             PSI_5024 = verificacion.PSI_5024,
//             FCNOX_5024 = verificacion.FCNOX_5024,
//             FCDIL_5024 = verificacion.FCDIL_5024,
//             RPM_5024 = verificacion.RPM_5024,
//             KPH_5024 = verificacion.KPH_5024,
//             THP_5024 = verificacion.THP_5024,
//             VOLTS_5024 = verificacion.VOLTS_5024,
//             HC_5024 = verificacion.HC_5024,
//             CO_5024 = verificacion.CO_5024,
//             CO2_5024 = verificacion.CO2_5024,
//             COCO2_5024 = verificacion.COCO2_5024,
//             O2_5024 = verificacion.O2_5024,
//             NO_5024 = verificacion.NO_5024,
//             LAMDA_2540 = verificacion.LAMDA_2540,
//             TEMP_2540 = verificacion.TEMP_2540,
//             HR_2540 = verificacion.HR_2540,
//             PSI_2540 = verificacion.PSI_2540,
//             FCNOX_2540 = verificacion.FCNOX_2540,
//             FCDIL_2540 = verificacion.FCDIL_2540,
//             RPM_2540 = verificacion.RPM_2540,
//             KPH_2540 = verificacion.KPH_2540,
//             THP_2540 = verificacion.THP_2540,
//             VOLTS_2540 = verificacion.VOLTS_2540,
//             HC_2540 = verificacion.HC_2540,
//             CO_2540 = verificacion.CO_2540,
//             CO2_2540 = verificacion.CO2_2540,
//             COCO2_2540 = verificacion.COCO2_2540,
//             O2_2540 = verificacion.O2_2540,
//             NO_2540 = verificacion.NO_2540,
//             C_RECHAZO = verificacion.C_RECHAZO,
//             RESULTADO = verificacion.RESULTADO,
//             EstatusPrueba = verificacion.EstatusPrueba,
//         };
//         var response = new ConsultaVerificacionResponse<ResultadosEmisiones>
//         {
//             IdEstatusPrueba = (int)verificacion.EstatusPrueba,
//             Resultados = prueba,
//         };
//         return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosEmisiones>>(response);
//     }
// public ResponseGeneric<ConsultaVerificacionResponse<ResultadosOpacidad>> ConsultaVerificacionOpacidad(ConsultaVerificacionRequest request)
// {
//     var verificacion = _context.vVerificacionDatosProveedors.FirstOrDefault(x => x.IdVerificacion == request.IdVerificacion);
//     if (verificacion == null)
//     {
//         return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosOpacidad>>(new Exception("No se encontró registro de la verificacion."))
//         { mensaje = "No sé encontró registro de la verificacion." };
//     }
//     if (verificacion.IdResultadosVerificacion == null)
//     {
//         return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosOpacidad>>(new Exception("Aún no se ha realizado la validación de placas del vehículo."))
//         { mensaje = "Aún no se ha realizado la validación de placas del vehículo." };
//     }
//     var prueba = new ResultadosOpacidad();
//     if (verificacion.PruebaOpacidad ?? false)
//     {
//         if (verificacion.EstatusPrueba != EstatusVerificacion.EnPruebaOpacidad || verificacion.EstatusPrueba != EstatusVerificacion.TerminaPruebaOpacidad)
//         {
//             return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosOpacidad>>(new ConsultaVerificacionResponse<ResultadosOpacidad>
//             {
//                 IdEstatusPrueba = (int)verificacion.EstatusPrueba,
//                 Resultados = prueba
//             });
//         }
//         return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosOpacidad>>(new Exception("Aún no se registran los resultados para la prueba de."))
//         { mensaje = "Aún no se ha realizado la validación de placas del vehículo." };
//     }
//     prueba = new ResultadosOpacidad
//     {
//         Etapa = verificacion.Etapa ?? "",
//         Protocolo = verificacion.PROTOCOLO,
//         OPACIDADP = verificacion.OPACIDADP,
//         OPACIDADK = verificacion.OPACIDADK,
//         TEMP_MOT = verificacion.TEMP_MOT,
//         VEL_GOB = verificacion.VEL_GOB,
//         POTMIN_RPM = verificacion.POTMIN_RPM,
//         POTMAX_RPM = verificacion.POTMAX_RPM,
//         TEMP_GAS = verificacion.TEMP_GAS,
//         TEMP_CAM = verificacion.TEMP_CAM,
//         C_RECHAZO = verificacion.C_RECHAZO,
//         RESULTADO = verificacion.RESULTADO,
//         EstatusPrueba = verificacion.EstatusPrueba,

//     };
//     var response = new ConsultaVerificacionResponse<ResultadosOpacidad>
//     {
//         IdEstatusPrueba = (int)verificacion.EstatusPrueba,
//         Resultados = prueba,
//     };
//     return new ResponseGeneric<ConsultaVerificacionResponse<ResultadosOpacidad>>(response);
// }