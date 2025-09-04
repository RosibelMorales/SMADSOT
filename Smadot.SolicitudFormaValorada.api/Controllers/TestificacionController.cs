using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Testificacion.Request;
using Smadot.SolicitudFormaValorada.Model.Negocio;
using Smadot.Utilities.Modelos;

namespace Smadot.SolicitudFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TestificacionController : ControllerBase
    {
        private ITestificacionNegocio _negocio;

        public TestificacionController(ITestificacionNegocio negocio)
        {
            _negocio = negocio;
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
        public async Task<IActionResult> Post(TestificacionApiRequest request)
        {
            try
            {
                var resultado = await _negocio.Guardar(request);
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

        // [HttpGet, Route("PlacaAutocomplete")]
        // public async Task<IActionResult> Get([FromQuery] string request)
        // {
        //     try
        //     {
        //         var resultado = await _negocio.ConsultaAutocomplete(request);
        //         if (resultado.Status == ResponseStatus.Success)
        //         {
        //             return Ok(resultado);
        //         }
        //         else
        //         {
        //             return NoContent();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new ResponseGeneric<string>(ex.Message));
        //     }
        // }
    }
}
