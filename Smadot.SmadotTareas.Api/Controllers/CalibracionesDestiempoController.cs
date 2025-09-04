using Microsoft.AspNetCore.Mvc;
using Smadot.SmadotTareas.Negocio.Negocio;
using Smadot.Utilities.Modelos;

namespace Smadot.SmadotTareas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
  
    public class CalibracionesDestiempoController : Controller
    {
        private ICalibracionesDestiempoNegocio _negocio;

        public CalibracionesDestiempoController(ICalibracionesDestiempoNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                var response = await _negocio.Generar();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }        
    }
}
