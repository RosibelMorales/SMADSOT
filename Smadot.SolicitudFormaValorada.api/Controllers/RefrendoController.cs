using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Refrendo.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.SolicitudFormaValorada.model.Negocio;
using Smadot.SolicitudFormaValorada.Model.Negocio;
using Smadot.Utilities.Modelos;

namespace Smadot.SolicitudFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RefrendoController : Controller
    {
        private IRefrendoNegocio _negocio;

        public RefrendoController(IRefrendoNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var resultado = await _negocio.Consulta(id);
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

        [HttpGet("Detalle/{id}")]
        public async Task<IActionResult> GetId(long id)
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
        public async Task<IActionResult> Post(RefrendoRequest request)
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

        // [HttpGet("PDFFormaValorada/{id}")]
        // public async Task<IActionResult> GetPDFFormaValorada(long id)
        // {
        //     try
        //     {
        //         var resultado = await _negocio.GetExentoFormaValorada(id);
        //         if (resultado.Status == ResponseStatus.Success)
        //         {
        //             return Ok(resultado);
        //         }
        //         else
        //         {
        //             return NoContent();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new ResponseGeneric<string>(ex.Message));
        //     }
        // }
    }
}
