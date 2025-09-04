using Microsoft.AspNetCore.Mvc;
using Smadot.IngresoFormaValorada.Model.Negocio;
using Smadot.Models.Entities.Caja.Request;
using Smadot.Models.Entities.Caja.Response;
using Smadot.Utilities.Modelos;

namespace Smadot.IngresoFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CajaController : Controller
    {
        private ICajaNegocio _negocio;

        public CajaController(ICajaNegocio negocio)
        {
            _negocio = negocio;
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

        [HttpPost]
        public async Task<IActionResult> Post(List<CajaResponse> request)
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
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("GetCajaById")]
        public async Task<IActionResult> GetCajaById(long id)
        {
            try
            {
                var resultado = await _negocio.GetCajaById(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

    }
}
