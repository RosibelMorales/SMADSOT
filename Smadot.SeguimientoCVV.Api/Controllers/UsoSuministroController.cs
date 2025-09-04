using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.UsoSuministro.Request;
using Smadot.Models.Entities.UsoSuministro.Response;
using Smadot.Utilities.Modelos;
using Smadot.SeguimientoCVV.Model.Negocio;


namespace Smadot.SeguimientoCVV.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsoSuministroController : Controller
    {

        private IUsoSuministroNegocio _negocio;

        public UsoSuministroController(IUsoSuministroNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] UsoSuministroRequest request)
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
        public async Task<IActionResult> Post(UsoSuministroResponse request)
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
    }
}
