using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Capacitacion.Request;
using Smadot.Models.Entities.Capacitacion.Response;
using Smadot.Utilities.Modelos;

namespace Smadot.DirectorioCentrosVerificacion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CapacitacionController : Controller
    {
        private ICapacitacionNegocio _negocio;

        public CapacitacionController(ICapacitacionNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CapacitacionListRequest request)
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
		public async Task<IActionResult> Post(List<CapacitacionResponse> request)
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

        [HttpGet, Route("GetByIdEmpleado")]
        public async Task<IActionResult> GetByIdEmpleado(long id)
        {
            try
            {
                var resultado = await _negocio.GetByIdEmpleado(id);
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
