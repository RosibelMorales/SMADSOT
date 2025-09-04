using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Catalogo.Model.Negocio;
using Smadot.Utilities.Modelos;

namespace Smadot.Catalogo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TipoCertificadoController : Controller
    {
        private ITipoCertificado _negocio;

        public TipoCertificadoController(ITipoCertificado negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TipoCertificadoRequest request)
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
    }
}
