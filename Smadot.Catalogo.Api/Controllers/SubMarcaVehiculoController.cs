using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smadot.Catalogo.Model.Negocio;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Utilities.Modelos;

namespace Smadot.Catalogo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SubMarcaVehiculoController : ControllerBase
    {
        private ISubMarcaVehiculoNegocio _negocio;

        public SubMarcaVehiculoController(ISubMarcaVehiculoNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SubMarcaVehiculoRequest request)
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
        [HttpGet("ConsultaSecundaria")]
        public async Task<IActionResult> ConsultaSecundaria([FromQuery] SubMarcaVehiculoRequest request)
        {
            try
            {
                var resultado = await _negocio.ConsultaSecundaria(request);
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
    }
}
