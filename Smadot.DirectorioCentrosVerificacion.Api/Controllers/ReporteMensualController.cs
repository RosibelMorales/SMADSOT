using Microsoft.AspNetCore.Mvc;
using Smadot.DirectorioCentrosVerificacion.Model.Negocio;
using Smadot.Models.Entities.IngresoFormaValorada.Response;
using Smadot.Models.Entities.ReporteMensual.Request;
using Smadot.Models.Entities.ReporteMensual.Response;
using Smadot.Utilities.Modelos;

namespace Smadot.DirectorioCentrosVerificacion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ReporteMensualController : Controller
    {
        private IReporteMensualNegocio _negocio;

        public ReporteMensualController(IReporteMensualNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ReporteMensualListRequest request)
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
        public async Task<IActionResult> Get(long? id)
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
        public async Task<IActionResult> Post(List<ReporteMensualResponse> request)
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
