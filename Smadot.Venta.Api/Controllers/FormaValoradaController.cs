using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Alertas.Request;
using Smadot.Models.Entities.FoliosFormaValorada.Request;
using Smadot.Venta.Model.Negocio;
using Smadot.Utilities.Modelos;
using Smadot.Models.Entities.FoliosFormaValorada.Response;

namespace Smadot.IngresoFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FormaValoradaController : ControllerBase
    {
        private IFormaValoradaNegocio _negocio;

        public FormaValoradaController(IFormaValoradaNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet("Consulta")]
        public async Task<IActionResult> Get([FromQuery] FolioFormaValoradaGridRequest request)
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
        [HttpGet("ProximosFolios")]
        public async Task<IActionResult> GetFolios()
        {
            try
            {
                var resultado = await _negocio.ProximosFolios();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPut("Recalcular")]
        public async Task<IActionResult> Recalcular([FromQuery] long idFolioFormaValorada, [FromQuery] long idVerificentro)
        {
            try
            {
                var resultado = await _negocio.RecalcularFolio(idFolioFormaValorada, idVerificentro);
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
