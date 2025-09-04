using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.ReposicionCredencial.Request;
using Smadot.Utilities.Modelos;

namespace Smadot.SeguimientoCVV.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ReposicionCredencialController : Controller
    {
        private IReposicionCredencialNegocio _negocio;

        public ReposicionCredencialController(IReposicionCredencialNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ReposicionCredencialListRequest request)
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
        public async Task<IActionResult> Post([FromBody] ReposicionCredencialRequest request)
        {
            try
            {
                var resultado = await _negocio.Registro(request);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] long id)
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

        [HttpPut("Rechazo/{id}")]
        public async Task<IActionResult> Rechazar([FromRoute] long id)
        {
            try
            {
                var resultado = await _negocio.Rechazar(id);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPut("Aprobacion/{id}")]
        public async Task<IActionResult> Aprobar([FromRoute] long id)
        {
            try
            {
                var resultado = await _negocio.Aprobar(id);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet("UserPuestoAutocomplete")]
        public async Task<IActionResult> Autocomplete([FromQuery] UserPuestoAutoCompleteRequest request)
        {
            try
            {
                var resultado = await _negocio.Autocomplete(request);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
    }
}
