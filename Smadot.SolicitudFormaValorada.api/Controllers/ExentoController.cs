using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Exento.Request;
using Smadot.Models.Entities.Exento.Response;
using Smadot.SolicitudFormaValorada.Model.Negocio;
using Smadot.Utilities.Modelos;


namespace Smadot.SolicitudFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ExentoController : Controller
    {
        private IExentoNegocio _negocio;

        public ExentoController(IExentoNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ExentoRequest request)
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
        public async Task<IActionResult> Post(ExentoResponse request)
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
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var resultado = await _negocio.Detalle(id);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var resultado = await _negocio.Eliminar(id);
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
