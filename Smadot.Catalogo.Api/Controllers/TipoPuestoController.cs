using Microsoft.AspNetCore.Mvc;
using Smadot.Catalogo.Model.Negocio;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Utilities.Modelos;

namespace Smadot.Catalogo.Api.Controllers
{
    [Route("api/[controller]"), ApiController, Produces("application/json")]
    public class TipoPuestoController : ControllerBase
    {
        private ITipoPuestoNegocio _negocio;

        public TipoPuestoController(ITipoPuestoNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TipoPuestoRequest request)
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
    }
}
