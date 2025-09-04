using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smadot.IngresoFormaValorada.Model.Negocio;
using Smadot.Models.Entities.PortalCitas.Request;
using Smadot.Utilities.Modelos;
using static Smadot.Models.Entities.RecepcionDocumentos.Request.RecepcionDocumentosRequestData;

namespace Smadot.IngresoFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RecepcionDocumentosController : ControllerBase
    {
        private IRecepcionDocumentosNegocio _negocio;

        public RecepcionDocumentosController(IRecepcionDocumentosNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] RequestList request)
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
        public async Task<IActionResult> Post([FromBody] RecepcionDocumentosRequest request)
        {
            try
            {
                var resultado = await _negocio.Documentos(request);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPost, Route("Reagendar")]
        public async Task<IActionResult> Reagendar(ReagendarCitaRequest request)
        {
            try
            {
                var resultado = await _negocio.Reagendar(request);
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

        [HttpGet, Route("ConsultaTablaMaestra")]
        public async Task<IActionResult> ConsultaTablaMaestra([FromQuery] CatalogoTablaMaestraRequest request)
        {
            try
            {
                var resultado = await _negocio.ConsultaTablaMaestra(request);
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
        public async Task<IActionResult> Autocomplete([FromQuery] GenericSelect2AutocompleRequest request)
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

        [HttpGet, Route("ConsultaSubDiesel")]
        public async Task<IActionResult> Get([FromQuery] SubDieselRequest request)
        {
            try
            {
                var resultado = await _negocio.ConsultaSubDiesel(request);
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet("CambiarLinea/{idVerificacion}")]
        public async Task<IActionResult> CambiarLinea([FromRoute] long idVerificacion)
        {
            try
            {
                var resultado = await _negocio.CambiarLinea(idVerificacion);
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

        [HttpPost("CambiarLinea")]
        public async Task<IActionResult> CambiarLinea(RecepcionDocumentosCambiarLineaRequest request)
        {
            try
            {
                var resultado = await _negocio.CambiarLinea(request);
                return Ok(resultado);
                // if (resultado.Status == ResponseStatus.Success)
                // {
                // }
                // else
                // {
                //     return Enititu();
                // }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPost("ListaVerificaciones")]
        public async Task<IActionResult> ListaVerificaciones(string numeroSerie)
        {
            try
            {
                var resultado = await _negocio.ConsultaVeririfacion(numeroSerie);
                return Ok(resultado);
               
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
    }
}
