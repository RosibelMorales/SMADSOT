using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.ConsultaTablaMaestra.Request;
using Smadot.Models.Entities.ConsultaTablaMaestra.Response;
using Smadot.Models.Entities.Equipo.Response;
using Smadot.SolicitudFormaValorada.Model.Negocio;
using Smadot.Utilities.Modelos;
using static Smadot.Models.Entities.ConsultaTablaMaestra.Request.ConsultaTablaMaestraRequestData;

namespace Smadot.SolicitudFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ConsultaTablaMaestraController : ControllerBase
    {
        private IConsultaTablaMaestraNegocio _negocio;

        public ConsultaTablaMaestraController(IConsultaTablaMaestraNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ConsultaTablaMaestraRequestList request)
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

        [HttpGet("ConsultaSubMarca")]
        public async Task<IActionResult> Get([FromQuery] SubMarcaVehiculoRequest request)
        {
            try
            {
                var resultado = await _negocio.ConsultaSubMarca(request);
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
        public async Task<IActionResult> Post(TablaMaestraRequest request)
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

        [HttpGet, Route("ConsultaCicloVerificacion")]
        public async Task<IActionResult> GetCicloVerificacion([FromQuery] RequestList request)
        {
            try
            {
                var resultado = await _negocio.ConsultaCicloVerificacion(request);
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

        [HttpGet, Route("AutocompleteMarca")]
        public async Task<IActionResult> AutocompleteMarca([FromQuery] string prefix)
        {
            try
            {
                var resultado = await _negocio.AutocompleteMarca(prefix);
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

        [HttpGet, Route("AutocompleteSubmarca")]
        public async Task<IActionResult> AutocompleteSubmarca([FromQuery] string prefix)
        {
            try
            {
                var resultado = await _negocio.AutocompleteSubmarca(prefix);
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
        [HttpPost, Route("ActualizarTablaMaestra")]
        public async Task<IActionResult> actualizar(TablaMaestraDbfRequest request)
        {
            try
            {
                var resultado = await _negocio.ActualizarTablaMaestra(request);
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

        [HttpGet, Route("AutocompleteProtocolo")]
        public async Task<IActionResult> AutocompleteLinea([FromQuery] string prefix)
        {
            try
            {
                var resultado = await _negocio.AutocompleteProtocolo(prefix);
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

        [HttpGet, Route("AutocompleteModelo")]
        public async Task<IActionResult> AutocompleteModelo([FromQuery] string prefix)
        {
            try
            {
                var resultado = await _negocio.AutocompleteModelo(prefix);
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
