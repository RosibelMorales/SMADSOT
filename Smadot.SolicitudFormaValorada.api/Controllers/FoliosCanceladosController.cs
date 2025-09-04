using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.SolicitudFormaValorada.Model.Negocio;
using Smadot.Utilities.Modelos;
using static Smadot.Models.Entities.FoliosCancelados.Request.FoliosCanceladosRequestData;

namespace Smadot.SolicitudFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FoliosCanceladosController : ControllerBase
    {

        private IFoliosCanceladosNegocio _negocio;

        public FoliosCanceladosController(IFoliosCanceladosNegocio negocio)
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

        [HttpPost]
        public async Task<IActionResult> Post(FolioCanceladosRequest request)
        {
            try
            {
                var resultado = await _negocio.CancelarFolio(request);
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

        [HttpGet, Route("FoliosAutocomplete")]
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

    }
}
