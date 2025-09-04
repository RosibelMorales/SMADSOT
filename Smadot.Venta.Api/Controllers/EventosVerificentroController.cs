using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Vml;
using Smadot.Models.Entities.FoliosFormaValorada.Request;
using Smadot.Models.Entities.FoliosFormaValorada.Response;
using Smadot.Models.Entities.SeguimientoCVV.Request;
using Smadot.Models.Entities.Verificacion;
using Smadot.Utilities.CustomAttributes;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Venta.Model.Negocio;
using static Smadot.Models.Entities.FoliosCancelados.Request.FoliosCanceladosRequestData;

namespace Smadot.Venta.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [Produces("application/json")]
    public class EventosVerificentroController : Controller
    {
        private IEventosVerificentroNegocio _negocio;
        private readonly IUserResolver _userResolver;
        private ILogger<EventosVerificentroController> _logger;
        private readonly Smadsot.Api.VentaApi ventaApi;
        private readonly List<long> exludedCvvs = new() { 27, 34, 41 };
        public EventosVerificentroController(IEventosVerificentroNegocio negocio, ILogger<EventosVerificentroController> logger, IUserResolver userResolver, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _negocio = negocio;
            _logger = logger;
            _userResolver = userResolver;
            HttpClient httpClient = new();
            var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Add("Smadot-API-Key", configuration["SmadotAPISettings:APIKey"]);
            ventaApi = new(configuration["VentaApiUrl"], httpClient);
        }
        [ApiKey]
        [IgnoreAntiforgeryToken]
        [HttpPost("{clave}/RegistrarEvento")]
        public async Task<IActionResult> RegistrarEvento(string clave, [FromBody] EventoSalida Evento)
        {
            try
            {
                var resultado = await _negocio.RegistrarEvento(clave, Evento);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(resultado);
                }

                return UnprocessableEntity(resultado);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }
        [ApiKey]
        [IgnoreAntiforgeryToken]
        [HttpPost("{clave}/ActualizarPrueba")]
        public async Task<IActionResult> ActualizarPrueba(string clave, [FromBody] Prueba Prueba)
        {
            try
            {
                var resultado = await _negocio.ActualizarPrueba(clave, Prueba);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(resultado);
                }

                return UnprocessableEntity(resultado);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }
        [HttpPost("TestResultados/{id}")]
        public async Task<IActionResult> ActualizarPrueba()
        {
            try
            {
                await _negocio.TestResultadosVerificacion();

                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }
        [HttpPost("ReimprimirFolio")]
        public async Task<IActionResult> ReimprimirFolio(FolioCanceladosRequest folio)
        {
            try
            {
                var usuarioActual = _userResolver.GetUser().IdUser;
                folio.IdUser = usuarioActual;
                var resultado = await _negocio.ReimpresionFV(folio);

                return Ok(resultado);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }
        [HttpPost("GenerarFolio/{id}")]
        public async Task<IActionResult> GenerarFolioFV(long id)
        {
            try
            {
                var resultado = _negocio.GenerarFolioFV(id);

                return Ok(resultado);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }
        [HttpGet("DataImpresion")]
        [ProducesResponseType(typeof(ResponseGeneric<FolioFormaValoradaImpresionResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ResponseGeneric<FolioFormaValoradaImpresionResponse>), (int)HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> DataImpresion([FromQuery] GenerateImpresionRequest request, [FromHeader] bool sinRedireccion)
        {
            try
            {
                var user = _userResolver.GetUser();

                if (exludedCvvs.Contains(user.IdVerificentro ?? 0) && !sinRedireccion)
                {
                    var callApi = await ventaApi.DataImpresionAsync(request.Id, request.Idverificacion, request.Imprimir, true);
                    var resApi = new ResponseGeneric<FolioFormaValoradaImpresionResponse>()
                    {
                        Status = callApi.Status == Smadsot.Api.ResponseStatus._0 ? ResponseStatus.Failed : ResponseStatus.Success,
                        CurrentException = callApi.CurrentException,
                        mensaje = callApi.Mensaje,
                        respuesta = callApi.Respuesta,
                        AccesoTotalVerificentros = callApi.AccesoTotalVerificentros,
                        Response = new()
                        {
                            PROTOCOLO = callApi.Response.Protocolo,
                            OPACIDADK = (decimal?)callApi.Response.Opacidadk,
                            Anio = callApi.Response.Anio,
                            ApiEndPoint = callApi.Response.ApiEndPoint,
                            ApiKey = callApi.Response.ApiKey,
                            Clave = callApi.Response.Clave,
                            CO2_2540 = (decimal?)callApi.Response.CO2_2540,
                            CO2_5024 = (decimal?)callApi.Response.CO2_5024,
                            COCO2_2540 = (decimal?)callApi.Response.CocO2_2540,
                            COCO2_5024 = (decimal?)callApi.Response.CocO2_5024,
                            CO_2540 = (decimal?)callApi.Response.CO_2540,
                            CO_5024 = (decimal?)callApi.Response.CO_5024,
                            Combustible = callApi.Response.Combustible,
                            C_RECHAZO = callApi.Response.C_RECHAZO,
                            Etapa = callApi.Response.Etapa,
                            FechaRegistro = callApi.Response.FechaRegistro == null ? new DateTime() : new DateTime(callApi.Response.FechaRegistro.Value.Year, callApi.Response.FechaRegistro.Value.Month, callApi.Response.FechaRegistro.Value.Day, callApi.Response.FechaRegistro.Value.Hour, callApi.Response.FechaRegistro.Value.Minute, callApi.Response.FechaRegistro.Value.Second),
                            FinalizacionPruebas = callApi.Response.FinalizacionPruebas == null ? new DateTime() : new DateTime(callApi.Response.FinalizacionPruebas.Value.Year, callApi.Response.FinalizacionPruebas.Value.Month, callApi.Response.FinalizacionPruebas.Value.Day, callApi.Response.FinalizacionPruebas.Value.Hour, callApi.Response.FinalizacionPruebas.Value.Minute, callApi.Response.FinalizacionPruebas.Value.Second),
                            FolioFoliosFormaValoradaVerificentro = callApi.Response.FolioFoliosFormaValoradaVerificentro,
                            HC_2540 = callApi.Response.HC_2540,
                            HC_5024 = callApi.Response.HC_5024,
                            Id = callApi.Response.Id,
                            IdUserTecnico = callApi.Response.IdUserTecnico,
                            IdUserCapturista = callApi.Response.IdUserCapturista,
                            IdVerificacion = callApi.Response.IdVerificacion,
                            IdRefrendoExento = callApi.Response.IdRefrendoExento,
                            IdExento = callApi.Response.IdExento,
                            InicioPruebas = callApi.Response.InicioPruebas == null ? new DateTime() : new DateTime(callApi.Response.InicioPruebas.Value.Year, callApi.Response.InicioPruebas.Value.Month, callApi.Response.InicioPruebas.Value.Day, callApi.Response.InicioPruebas.Value.Hour, callApi.Response.InicioPruebas.Value.Minute, callApi.Response.InicioPruebas.Value.Second),
                            KPH_2540 = (decimal?)callApi.Response.KpH_2540,
                            KPH_5024 = (decimal?)callApi.Response.KpH_5024,
                            LAMDA_2540 = (decimal?)callApi.Response.LamdA_2540,
                            C_RECHAZO_OBD = callApi.Response.C_RECHAZO_OBD,
                            Marca = callApi.Response.Marca,
                            IdVerificentro = callApi.Response.IdVerificentro,
                            LAMDA_5024 = (decimal?)callApi.Response.LamdA_5024,
                            IdCatTipoCertificado = callApi.Response.IdCatTipoCertificado,
                            Modelo = callApi.Response.Modelo,
                            NombreVerificentro = callApi.Response.NombreVerificentro,
                            NO_2540 = callApi.Response.NO_2540,
                            NO_5024 = callApi.Response.NO_5024,
                            O2_2540 = (decimal?)callApi.Response.O2_2540,
                            O2_5024 = (decimal?)callApi.Response.O2_5024,
                            OBD_CATAL = callApi.Response.ObD_CATAL,
                            OBD_CILIN = callApi.Response.ObD_CILIN,
                            OBD_COMBU = callApi.Response.ObD_COMBU,
                            OBD_INTEG = callApi.Response.ObD_INTEG,
                            OBD_MIL = callApi.Response.ObD_MIL,
                            OBD_OXIGE = callApi.Response.ObD_OXIGE,
                            OBD_TIPO_SDB = callApi.Response.ObD_TIPO_SDB,
                            OPACIDADP = (decimal?)callApi.Response.Opacidadp,
                            Placa = callApi.Response.Placa,
                            POTMIN_RPM = callApi.Response.PotmiN_RPM,
                            Propietario = callApi.Response.Propietario,
                            PruebaEmisiones = callApi.Response.PruebaEmisiones,
                            PruebaObd = callApi.Response.PruebaObd,
                            PruebaOpacidad = callApi.Response.PruebaOpacidad,
                            RESULTADO = callApi.Response.Resultado,
                            RPM_2540 = callApi.Response.RpM_2540,
                            RPM_5024 = callApi.Response.RpM_5024,
                            RPOTMAX_RPM = callApi.Response.RpotmaX_RPM,
                            Serie = callApi.Response.Serie,
                            SPS_2540 = callApi.Response.SpS_2540,
                            SPS_5024 = callApi.Response.SpS_5024,
                            SPS_Humo = callApi.Response.SpS_Humo,
                            TEMP_2540 = (decimal?)callApi.Response.TemP_2540,
                            TEMP_5024 = (decimal?)callApi.Response.TemP_5024,
                            TEMP_CAM = callApi.Response.TemP_CAM,
                            TEMP_GAS = callApi.Response.TemP_GAS,
                            TEMP_MOT = callApi.Response.TemP_MOT,
                            THP_2540 = (decimal?)callApi.Response.ThP_2540,
                            THP_5024 = (decimal?)callApi.Response.ThP_5024,
                            TipoCertificado = callApi.Response.TipoCertificado,
                            VEL_GOB = callApi.Response.VeL_GOB,
                            Vigencia = callApi.Response.Vigencia == null ? new DateTime() : new DateTime(callApi.Response.Vigencia.Value.Year, callApi.Response.Vigencia.Value.Month, callApi.Response.Vigencia.Value.Day, callApi.Response.Vigencia.Value.Hour, callApi.Response.Vigencia.Value.Minute, callApi.Response.Vigencia.Value.Second),
                            VOLTS_2540 = (decimal?)callApi.Response.VoltS_2540,
                            VOLTS_5024 = (decimal?)callApi.Response.VoltS_5024,
                            FolioCertificadoAnterior = callApi.Response.FolioCertificadoAnterior,
                            NumeroSerieEquipo = callApi.Response.NumeroSerieEquipo,
                            IdCatTipoTramite = callApi.Response.IdCatTipoTramite,
                            NombreEquipo = callApi.Response.NombreEquipo,
                            IdEquipo = callApi.Response.IdEquipo,
                            NombreCapturista = callApi.Response.NombreCapturista,
                            NumeroCapturista = callApi.Response.NumeroCapturista,
                            NombreTecnico = callApi.Response.NombreTecnico,
                            NumeroTecnico = callApi.Response.NumeroTecnico,
                            Linea = callApi.Response.Linea,
                            FolioCertificado = callApi.Response.FolioCertificado,
                            ClaveTramite = callApi.Response.ClaveTramite,
                            EntidadProcedencia = callApi.Response.EntidadProcedencia,
                            IdMotivoVerificacion = callApi.Response.IdMotivoVerificacion,
                            PBV = callApi.Response.Pbv,
                            LeyendaCNA = callApi.Response.LeyendaCNA,
                            Semestre = callApi.Response.Semestre,
                            VigenciaVistaPrevia = callApi.Response.VigenciaVistaPrevia == null ? new DateTime() : new DateTime(callApi.Response.VigenciaVistaPrevia.Value.Year, callApi.Response.VigenciaVistaPrevia.Value.Month, callApi.Response.VigenciaVistaPrevia.Value.Day, callApi.Response.VigenciaVistaPrevia.Value.Hour, callApi.Response.VigenciaVistaPrevia.Value.Minute, callApi.Response.VigenciaVistaPrevia.Value.Second),
                        }
                    };
                    if (resApi.Status == ResponseStatus.Success)
                    {
                        return Ok(resApi);
                    }

                    return UnprocessableEntity(resApi);
                }
                if (request.Idverificacion != null && request.Id == null)
                {

                    var response = await _negocio.GenerarFolioFV(request.Idverificacion.Value);
                    if (response.Status != ResponseStatus.Success || response.Response == 0)
                    {
                        return UnprocessableEntity(new ResponseGeneric<FolioFormaValoradaImpresionResponse>() { mensaje = response.mensaje, Status = ResponseStatus.Failed });
                    }
                    request.Id = response.Response;
                }
                var reponse = await _negocio.GetDataImpresion(request.Id.Value, request.Imprimir);
                if (reponse.Status == ResponseStatus.Success)
                {
                    return Ok(reponse);
                }
                reponse.Response ??= new();

                return UnprocessableEntity(reponse);
            }
            catch (Smadsot.Api.ApiException<Smadsot.Api.FolioFormaValoradaImpresionResponseResponseGeneric> e)
            {
                var resApi = new ResponseGeneric<long>()
                {
                    Status = e.Result.Status == Smadsot.Api.ResponseStatus._0 ? ResponseStatus.Failed : ResponseStatus.Success,
                    CurrentException = e.Result.CurrentException,
                    mensaje = e.Result.Mensaje,
                    respuesta = e.Result.Respuesta,
                    AccesoTotalVerificentros = e.Result.AccesoTotalVerificentros,
                    Response = new()
                };
                if (resApi.Status == ResponseStatus.Success)
                {
                    return Ok(resApi);
                }

                return UnprocessableEntity(resApi);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<Smadsot.Api.FolioFormaValoradaImpresionResponseResponseGeneric>(e.Message) { Response = new() });
            }

        }
    }
}
