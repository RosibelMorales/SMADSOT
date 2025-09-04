using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.OrdenServicio.Request;
using Smadot.Utilities.Modelos;
using Smadot.Venta.Model.Negocio;

namespace Smadot.Venta.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OrdenServicioController : ControllerBase
    {
        private IOrdenServicioNegocio _negocio;
        public OrdenServicioController(IOrdenServicioNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Consulta([FromQuery] RequestList request)
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

        [HttpPost]
        public async Task<IActionResult> Post(OrdenServicioApiRequestData request)
        {
            try
            {
                var resultado = await _negocio.GuardarOrdenServicio(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(resultado);
                }
                else if (resultado.Status == ResponseStatus.Failed)
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

        [HttpGet, Route("EquipoAutocomplete")]
        public async Task<IActionResult> Get([FromQuery] string request)
        {
            try
            {
                var resultado = await _negocio.ConsultaAutocomplete(request);
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

        [HttpGet, Route("CatTipoOrdenServicio")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var resultado = await _negocio.CatTipoOrdenServicio();
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
