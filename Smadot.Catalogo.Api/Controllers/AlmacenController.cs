using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Catalogo.Model.Negocio;
using Smadot.Utilities.Modelos;
using Microsoft.AspNetCore.Authorization;

namespace Smadot.Catalogo.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	[Produces("application/json")]
	public class AlmacenController : Controller
	{
		private IAlmacenNegocio _negocio;

		public AlmacenController(IAlmacenNegocio negocio)
		{
			_negocio = negocio;
		}

		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] AlmacenRequest request)
		{
			try
			{
				var resultado = await _negocio.Consulta(request);
				if(resultado.Status == ResponseStatus.Success)
				{
					return Ok(resultado);
				}
				else
				{
					return NoContent();
				}
			}
			catch(Exception ex)
			{
				return BadRequest(new ResponseGeneric<string>(ex.Message));
			}
		}
		[HttpGet("Id")]
		public async Task<IActionResult> Get([FromQuery] int Id)
		{
			try
			{
				var resultado = await _negocio.Consulta(Id);
				if(resultado.Status == ResponseStatus.Success)
				{
					return Ok(resultado);
				}
				else
				{
					return NoContent();
				}
			}
			catch(Exception ex)
			{
				return BadRequest(new ResponseGeneric<string>(ex.Message));
			}
		}
	}
}
