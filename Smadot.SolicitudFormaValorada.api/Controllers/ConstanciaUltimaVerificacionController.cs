using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Models.Entities.ConstanciaUltimaVerificacion.Request;
using Smadot.Models.Entities.Refrendo.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.SolicitudFormaValorada.Model.Negocio;
using Smadot.Utilities.Modelos;

namespace Smadot.SolicitudFormaValorada.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Produces("application/json")]
	public class ConstanciaUltimaVerificacionController : Controller
	{
		private IConstanciaUltimaVerificacionNegocio _negocio;

		public ConstanciaUltimaVerificacionController(IConstanciaUltimaVerificacionNegocio negocio)
		{
			_negocio = negocio;
		}

		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] SolicitudFormaValoradaListRequest request)
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

		[HttpGet("{id}/{verificacion}")]
		public async Task<IActionResult> Get(long id, bool verificacion)
		{
			try
			{
				var resultado = await _negocio.GetById(id, verificacion);
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

		[HttpGet("Autocomplete")]
		public async Task<IActionResult> Autocomplete([FromQuery] RefrendoAutocompletRequest request)
		{
			try
			{
				var resultado = await _negocio.Autocomplete(request);
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

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> Detalle([FromRoute] long id)
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

        [HttpPost]
		public async Task<IActionResult> Post([FromBody] ConstanciaUltimaVerificacionRequest request)
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
	}
}
