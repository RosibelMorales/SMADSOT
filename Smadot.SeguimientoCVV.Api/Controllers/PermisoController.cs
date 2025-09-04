using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Permiso.Request;
using Smadot.SeguimientoCVV.Model.Negocio;
using Smadot.Utilities.Modelos;

namespace Smadot.SeguimientoCVV.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PermisoController : Controller
    {
        private IPermisoNegocio _negocio;

        public PermisoController(IPermisoNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet("Rol")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var resultado = await _negocio.GetRoles();
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
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var resultado = await _negocio.GetPermisosTreeByRol(id);
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
        public async Task<IActionResult> Post(PermisoTreeRequest request)
        {
            try
            {
                var resultado = await _negocio.SavePermisos(request);
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
