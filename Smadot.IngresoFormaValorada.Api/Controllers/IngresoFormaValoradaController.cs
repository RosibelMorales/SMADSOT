using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.IngresoFormaValorada.Model.Negocio;
using Smadot.Utilities.Modelos;

using Smadot.Models.Entities.IngresoFormaValorada.Request;
using Smadot.Models.Entities.IngresoFormaValorada.Response;

namespace Smadot.IngresoFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class IngresoFormaValoradaController : Controller
    {
        private IIngresoFormaValoradaNegocio _negocio;

        public IngresoFormaValoradaController(IIngresoFormaValoradaNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SolicitudFormaValoradaListRequest request)
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
        public async Task<IActionResult> Post(IngresoFormaValoradaRequest request)
        {
            try
            {
                var resultado = await _negocio.Registro(request);
                return Ok(resultado);
                //if (resultado.Status == ResponseStatus.Success)
                //{
                //    return Ok(resultado);
                //}
                //else
                //{
                //    return NoContent();
                //}
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet,Route("GetIngresoCertificadoById")]
        public async Task<IActionResult> GetIngresoCertificadoById(long id)
        {
            try
            {
                var resultado = await _negocio.GetIngresoCertificadoById(id);
                return Ok(resultado);
                //if (resultado.Status == ResponseStatus.Success)
                //{
                //    return Ok(resultado);
                //}
                //else
                //{
                //    return NoContent();
                //}
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

    }
}
