using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smadot.SolicitudFormaValorada.Model.Negocio;
using Smadot.Utilities.Modelos;
using static Smadot.Models.Entities.ConsultaCircular.Request.ConsultaCircularRequestData;

namespace Smadot.SolicitudFormaValorada.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Produces("application/json")]
	public class ConsultaCircularController : ControllerBase
	{
		private ICircularNegocio _negocio;

		public ConsultaCircularController(ICircularNegocio negocio){
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
		public async Task<IActionResult> EnvioCircular([FromBody] ConsultaCircularRequest request)
		{
			try
			{
				var resultado = await _negocio.EnvioCircular(request);
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
				return BadRequest(new ResponseGeneric<bool>(ex));
			}
		}

        [HttpPost, Route("ConfirmarCircular")]
        public async Task<IActionResult> ConfirmarCircular([FromBody] ConfirmarCircularRequest request)
        {
            try
            {
                var resultado = await _negocio.ConfirmarCircular(request);
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
