using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Catalogo.Model.Negocio;
using Smadot.Utilities.Modelos;

namespace Smadot.Catalogo.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Produces("application/json")]
	public class CatalogosVerificacionController : Controller
	{
		private ICatalogosVerificacion _negocio;

		public CatalogosVerificacionController(ICatalogosVerificacion negocio)
		{
			_negocio = negocio;
		}

		[HttpGet("TipoServicio")]
		public async Task<IActionResult> GetTipoServicio()
		{
			try
			{
				var resultado = await _negocio.GetCatalogoTipoServicio();
				return Ok(resultado);
			}
			catch (Exception ex)
			{
				return BadRequest(new ResponseGeneric<string>(ex.Message));
			}
		}
		[HttpGet("MotivoVerificacion")]
		public async Task<IActionResult> GetMotivoVerificacion()
		{
			try
			{
				var resultado = await _negocio.GetCatalogoMotivoVerificacion();
				return Ok(resultado);
			}
			catch (Exception ex)
			{
				return BadRequest(new ResponseGeneric<string>(ex.Message));
			}
		}
	}
}
