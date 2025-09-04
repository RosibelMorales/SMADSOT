using Microsoft.AspNetCore.Mvc;
using Smadot.Autenticacion.Model.Negocio;
using Smadot.Models.Entities.SeguimientoCVV.Request;
using Smadot.Utilities.Modelos;
using System.Net;

namespace Smadot.Autenticacion.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorizacionTecnicoAppController : ControllerBase
    {
        private IAutorizacionTecnicoAppNegocio _negocio;
        public AutorizacionTecnicoAppController(IAutorizacionTecnicoAppNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpPost]
        public async Task<IActionResult> Post(AutorizacionTecnicoAppRequest request)
        {
            try
            {
                var resultado = await _negocio.Autorizar(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(resultado);
                }

                return UnprocessableEntity(resultado);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(){mensaje="Ocurrió un error inesperado en el servidor."});
            }

        }
    }
}
