using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.ProgramacionCalibracion.Request;
using Smadot.Models.Entities.ProgramacionCalibracion.Response;
using Smadot.Models.Entities.EquipoTipoCalibracion.Request;
using Smadot.Models.Entities.EquipoTipoCalibracion.Response;
using Smadot.AdministracionStock.Negocio;
using Smadot.Utilities.Modelos;
using Smadot.AdministracionStock.Negocio.Negocio;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Models.Entities.Equipo.Response;

namespace Smadot.AdministracionStock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProgramacionCalibracionController : Controller
    {
        private IProgramacionCalibracionNegocio _negocio;

        public ProgramacionCalibracionController(IProgramacionCalibracionNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProgramacionCalibracionRequest request)
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
                var resultado = await _negocio.Detalle(id);
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
        public async Task<IActionResult> Post(EquipoTipoCalibracionResponse request)
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

        [HttpPost("Validar")]
        public async Task<IActionResult> CambioEstatus([FromBody] AuxiliarEstatusResponse request)
        {
            try
            {
                var resultado = await _negocio.CambioEstatus(request);
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

        [HttpPost("Rechazar")]
        public async Task<IActionResult> EstatusRechazar([FromBody] AuxiliarEstatusResponse request)
        {
            try
            {
                var resultado = await _negocio.EstatusRechazar(request);
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
