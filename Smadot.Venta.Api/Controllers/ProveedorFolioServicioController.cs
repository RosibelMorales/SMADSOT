using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smadot.Utilities.Modelos;
using Smadot.Venta.Model.Negocio;
using static Smadot.Models.Entities.ProveedorFolioServicio.Request.ProveedorFolioServicioRequestData;

namespace Smadot.Venta.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProveedorFolioServicioController : ControllerBase
    {
        private IProveedorFolioServicioNegocio _negocio;

        public ProveedorFolioServicioController(IProveedorFolioServicioNegocio negocio)
        {
            _negocio= negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] RequestList request)
        {
            try
            {
                var resultado = await _negocio.Consulta(request);
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
        public async Task<IActionResult> Post(ProveedorFolioServicioRequest request)
        {
            try
            {
                var resultado = await _negocio.GuardarProveedorFolio(request);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var resultado = await _negocio.GetById(id);
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
