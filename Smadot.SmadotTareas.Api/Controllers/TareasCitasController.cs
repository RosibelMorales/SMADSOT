using Microsoft.AspNetCore.Mvc;
using Smadot.SmadotTareas.Negocio.Negocio;
using Smadot.Utilities.CustomAttributes;
using Smadot.Utilities.Modelos;

namespace Smadot.SmadotTareas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]

    public class TareasCitasController : Controller
    {
        private readonly ITareasCitasNegocio _negocio;

        public TareasCitasController(ITareasCitasNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpPost("ProcesarHistoricoCitas")]
#if !DEBUG
        [ApiKey]
#endif
        public async Task<IActionResult> Post()
        {
            try
            {
                await _negocio.ProcesarHistoricoCitas();
                return Accepted();
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

    }
}
