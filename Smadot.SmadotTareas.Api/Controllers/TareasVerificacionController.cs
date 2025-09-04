using Microsoft.AspNetCore.Mvc;
using Smadot.SmadotTareas.Negocio.Negocio;
using Smadot.Utilities.CustomAttributes;
using Smadot.Utilities.Modelos;

namespace Smadot.SmadotTareas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]

    public class TareasVerificacionController : Controller
    {
        private readonly ITareasVerificacionNegocio _negocio;

        public TareasVerificacionController(ITareasVerificacionNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpPost("CertificadosSinUso")]
#if !DEBUG
        [ApiKey]
#endif
        public async Task<IActionResult> Post()
        {
            try
            {
                await _negocio.BorrarCertificados();
                return Accepted();
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
        [HttpPost("EliminarNoProcesados")]
#if !DEBUG
        [ApiKey]
#endif
        public async Task<IActionResult> EliminarNoProcesados()
        {
            try
            {
                await _negocio.LimpiarVerificaciones();
                return Accepted();
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
    }
}
