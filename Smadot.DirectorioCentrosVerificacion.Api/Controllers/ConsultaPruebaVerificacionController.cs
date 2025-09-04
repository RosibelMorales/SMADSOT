using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.ConsultaPruebaVerificacion.Request;
using Smadot.Utilities.Modelos;

namespace Smadot.DirectorioCentrosVerificacion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ConsultaPruebaVerificacionController : ControllerBase
    {
        private IConsultaPruebaVerificacionNegocio _negocio;

        public ConsultaPruebaVerificacionController(IConsultaPruebaVerificacionNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ConsultaPruebaVerificacionRequest request)
        {
            try
            {
                var resultado = await _negocio.GetPruebaVerificacion(request);
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

        [HttpGet, Route("BuscarPlacaSerie")]
        public async Task<IActionResult> BuscarPlacaSerie([FromQuery] string prefix, string flag)
        {
            try
            {
                var resultado = await _negocio.ConsultaAutocomplete(prefix, flag);
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
    }
}
