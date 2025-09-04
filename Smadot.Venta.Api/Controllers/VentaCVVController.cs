using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.VentaCVV.Request;
using Smadot.Models.Entities.VentaCVV.Response;
using Smadot.Utilities.Modelos;
using Smadot.Venta.Model.Negocio;

namespace Smadot.Venta.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class VentaCVVController : ControllerBase
    {
        private IVentaCVVNegocio _negocio;

        public VentaCVVController(IVentaCVVNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SolicitudFormaValoradaListRequest request)
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VentaFVRequest request)
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

        [HttpGet, Route("InventarioVentaCVV")]
        public async Task<IActionResult> GetInventarioVentaCVV([FromQuery] long idAlmacen)
        {
            try
            {
                var resultado = await _negocio.DetalleInventarioVentaCVV(idAlmacen);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("VerificentrosAutocomplete")]
        public async Task<IActionResult> Get([FromQuery] string request)
        {
            try
            {
                var resultado = await _negocio.ConsultaAutocomplete(request);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("VentaCertificados")]
        public async Task<IActionResult> GetVentaCertificados([FromQuery] long idAlmacen, [FromQuery] long idVenta)
        {
            try
            {
                var resultado = await _negocio.DetalleVentaCertificado(idAlmacen, idVenta);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet("AutocompleteSolicitud")]
        public async Task<IActionResult> AutocompleteSolicitud([FromQuery] VentaCVVAutocompletRequest request)
        {
            try
            {
                var resultado = await _negocio.AutocompleteSolicitud(request);
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
