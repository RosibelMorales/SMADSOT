using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Alertas.Request;
using Smadot.SeguimientoCVV.Model.Negocio;
using Smadot.Utilities.Modelos;


namespace Smadot.SeguimientoCVV.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AlertaController : Controller
    {

        private IAlertaNegocio _negocio;

        public AlertaController(IAlertaNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AlertaGridRequest request)
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

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> Detalle(long id)
        {
            try
            {
                var resultado = await _negocio.Detalle(id);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("Leer")]
        public async Task<IActionResult> LeerAlerta(long id)
        {
            try
            {
                var resultado = await _negocio.Leer(id);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPost, Route("AutorizacionIngresoManual")]
        public async Task<IActionResult> AutorizacionIngresoManual([FromBody] AutorizacionIngresoManualRequest request)
        {
            try
            {
                var resultado = await _negocio.AutorizacionIngresoManual(request);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
    }
}
