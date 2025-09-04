using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Calibracion.Request;
using Smadot.Models.Entities.Calibracion.Response;
using Smadot.AdministracionStock.Negocio;
using Smadot.Utilities.Modelos;
using Smadot.AdministracionStock.Negocio.Negocio;
using Smadot.Models.Entities.Equipo.Request;
using Smadot.Models.Entities.SeguimientoCVV.Response;

namespace Smadot.AdministracionStock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CalibracionController : Controller
    {
        private ICalibracionNegocio _negocio;

        public CalibracionController(ICalibracionNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CalibracionListRequest request)
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
        public async Task<IActionResult> Post([FromBody] CalibracionRequest request)
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
                    return BadRequest(resultado);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] CalibracionRequest request)
        {
            try
            {
                var resultado = await _negocio.Editar(request);
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPut("SolicitudModificacion/{id}")]
        public async Task<IActionResult> SolicitarModificacion([FromRoute] long id)
        {
            try
            {
                var resultado = await _negocio.SolicitarModificacion(id);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPut("Validacion/{id}")]
        public async Task<IActionResult> Validar([FromRoute] long id)
        {
            try
            {
                var resultado = await _negocio.Validar(id);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPut("RechazarDocumentacion/{id}")]
        public async Task<IActionResult> RechazarDocumentacion([FromRoute] long id)
        {
            try
            {
                var resultado = await _negocio.RechazarDocumentacion(id);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPut("Autorizacion/{id}")]
        public async Task<IActionResult> Autorizar([FromRoute] long id)
        {
            try
            {
                var resultado = await _negocio.Autorizar(id);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPut("RechazarSolicitar/{id}")]
        public async Task<IActionResult> RechazarSolicitar([FromRoute] long id)
        {
            try
            {
                var resultado = await _negocio.RechazarSolicitar(id);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
    }
}
