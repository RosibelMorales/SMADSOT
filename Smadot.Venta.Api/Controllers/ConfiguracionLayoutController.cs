using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.ConfiguracionLayout.Response;
using Smadot.Utilities.Modelos;
using Smadot.Venta.Model.Negocio;

namespace Smadot.Venta.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ConfiguracionLayoutController : Controller
    {
        private IConfiguracionLayoutNegocio _negocio;
        public ConfiguracionLayoutController(IConfiguracionLayoutNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var resultado = await _negocio.Consulta();
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

        [HttpPost]
        public async Task<IActionResult> Post(List<ConfiguracionLayoutResponse> request)
        {
            try
            {
                var resultado = await _negocio.Registro(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(resultado);
                }
                else
                {
                    return BadRequest(new ResponseGeneric<string>(resultado.CurrentException));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
    }
}
