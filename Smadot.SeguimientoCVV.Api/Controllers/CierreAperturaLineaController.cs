using Microsoft.AspNetCore.Mvc;
using Smadot.Utilities.Modelos;
using Smadot.Models.Entities.SeguimientoCVV.Request;
using Smadot.Models.Entities.SeguimientoCVV.Response;
using Smadot.Utilities.CustomAttributes;
using Smadot.Models.Entities.Linea.Request;

namespace Smadot.SeguimientoCVV.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Produces("application/json")]
	public class CierreAperturaLineaController : Controller
	{
		private ICierreAperturaLineaNegocio _negocio;

		public CierreAperturaLineaController(ICierreAperturaLineaNegocio negocio)
		{
			_negocio = negocio;
		}

		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] SeguimientoCVVListRequest request)
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

		[HttpGet("GetDetalle")]
        public async Task<IActionResult> GetDetalle([FromQuery] RequestListLinea request)
        {
            try
            {
                var resultado = await _negocio.DetalleGrid(request);
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
                var resultado = await _negocio.GetByDetalleCierre(id);
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
		public async Task<IActionResult> Post(List<SeguimientoCVVResponse> request)
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

		[HttpGet, Route("GetByCA")]
		public async Task<IActionResult> GetByCA(long id)
		{
			try
			{
				var resultado = await _negocio.GetByIdCA(id);
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

        [HttpPost("DashboardLineaAperturaCierre")]
        public async Task<IActionResult> DashboardLineaAperturaCierre(DashboardLineaRequest request)
        {
            try
            {

                var reponse = await _negocio.DashboardLineaAperturaCierre(request);
                if (reponse.Status == ResponseStatus.Success)
                {
                    return Ok(reponse);
                }

                return UnprocessableEntity(reponse);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }
    }
}
