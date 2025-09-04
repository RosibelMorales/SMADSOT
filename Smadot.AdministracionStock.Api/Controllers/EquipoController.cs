using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Equipo.Request;
using Smadot.Models.Entities.Equipo.Response;
using Smadot.Models.Entities.Linea.Request;
using Smadot.Models.Entities.Linea.Response;
using Smadot.AdministracionStock.Negocio;
using Smadot.Utilities.Modelos;
using Smadot.AdministracionStock.Negocio.Negocio;

namespace Smadot.AdministracionStock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class EquipoController : Controller
    {
        private IEquipoNegocio _negocio;

        public EquipoController(IEquipoNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] EquipoRequest request)
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
        public async Task<IActionResult> Post(EquipoResponse request)
        {
            try
            {
                var resultado = await _negocio.Registro(request);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Validar([FromBody] AuxValidarResponse request)
        {
            try
            {
                var resultado = await _negocio.Validar(request);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<bool>(ex));
            }
        }

        [HttpGet("Linea")]
        public async Task<IActionResult> Consulta([FromQuery] LineaRequest request)
        {
            try
            {
                var resultado = await _negocio.ConsultaLinea(request);
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

        [HttpPost("Validacion")]
        public async Task<IActionResult> ActualizaEstatus(AuxEstatusResponse request)
        {
            try
            {
                var resultado = await _negocio.ActualizaEstatus(request);
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
