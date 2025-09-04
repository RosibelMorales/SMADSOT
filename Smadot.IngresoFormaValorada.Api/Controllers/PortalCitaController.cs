using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smadot.IngresoFormaValorada.Model.Negocio;
using Smadot.Models.Entities.Cita.Request;
using Smadot.Models.Entities.PortalCitas.Request;
using Smadot.Models.Entities.PortalCitas.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;

namespace Smadot.IngresoFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PortalCitaController : ControllerBase
    {
        private IPortalCitaNegocio _negocio;
        private readonly IUserResolver userResolver;
        private readonly List<long> exludedCvvs = new() { 27, 34, 41 };
        private readonly Smadsot.Api.IngresoFVApi ingresoFVApi;

        public PortalCitaController(IPortalCitaNegocio negocio, IUserResolver userResolver, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            this.userResolver = userResolver;
            _negocio = negocio;
            var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Add("Smadot-API-Key", configuration["SmadotAPISettings:APIKey"]);
            ingresoFVApi = new(configuration["IngresoFVApiUrl"], httpClient);
        }

        [HttpGet, Route("GetPortalCitasCalendarioByIdCvv")]
        public async Task<IActionResult> GetPortalCitasCalendarioByIdCvv([FromQuery] long id)
        {
            try
            {
                var resultado = await _negocio.GetPortalCitasByIdCvv(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
        [HttpGet, Route("GetHistorialCitas")]
        public async Task<IActionResult> GetHistorialCitas([FromQuery] CitaGridRequest request)
        {
            try
            {
                var resultado = await _negocio.Consulta(request);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post(PortalCitaRequest request)
        {
            try
            {
                var resultado = await _negocio.RegistroCita(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(resultado);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("GetCitaData")]
        public async Task<IActionResult> GetCitaData([FromQuery] PortalConsultaCitaRequest request)
        {
            try
            {

                var resultado = new ResponseGeneric<PortalCitasComprobanteResponse>();
                if (request.Id != null)
                    resultado = await _negocio.GetCitaData(request.Id.Value);
                else
                    resultado = await _negocio.GetCitaData(request.Folio ?? "");

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPost, Route("CancelarCita")]
        public async Task<IActionResult> CancelarCita([FromBody] PortalCitaCancelarRequest request)
        {
            try
            {
                var resultado = await _negocio.CancelarCita(request);
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPost, Route("ReiniciarCita")]
        [ProducesResponseType(typeof(ResponseGeneric<long>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ReiniciarCita(ReiniciarCitaRequest request, [FromHeader] bool sinRedireccion)
        {
            try
            {
                var user = userResolver.GetUser();
                if (exludedCvvs.Contains(user.IdVerificentro ?? 0) && !sinRedireccion)
                {
                    var requestApi = new Smadsot.Api.ReiniciarCitaRequest
                    {
                        IdCita = request.IdCita,
                        IdVerificacion = request.IdVerificacion
                    };
                    var apiCall = await ingresoFVApi.ReiniciarCitaAsync(true, requestApi);
                    ResponseGeneric<long> responseOver = new()
                    {
                        Status = apiCall.Status == Smadsot.Api.ResponseStatus._0 ? ResponseStatus.Failed : ResponseStatus.Success,
                        CurrentException = apiCall.CurrentException,
                        mensaje = apiCall.Mensaje,
                        respuesta = apiCall.Respuesta,
                        AccesoTotalVerificentros = apiCall.AccesoTotalVerificentros,
                        Response = apiCall.Response,
                    };
                    return Ok(responseOver);

                }
                var resultado = await _negocio.ReiniciarCita(request);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPost, Route("ConfirmarCita")]
        public async Task<IActionResult> ConfirmarCita(ConfirmarCita request)
        {
            try
            {
                var resp = await _negocio.ConfirmarCita(request);

                return Ok(resp);
            }
            catch (Exception ex2)
            {
                Exception ex = ex2;
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
    }
}
