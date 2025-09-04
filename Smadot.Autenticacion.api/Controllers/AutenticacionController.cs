using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Smadot.Autenticacion.Api.Helper;
using Smadot.Autenticacion.Model.Negocio;
using Smadot.Models.Entities.Autenticacion.Request;
using Smadot.Models.Entities.Autenticacion.Response;
using Smadot.Utilities.Modelos;

namespace Smadot.Autenticacion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private IConfiguration _config;
        private IAutenticacionNegocio _negocio;

        public AutenticacionController(IConfiguration config, IAutenticacionNegocio negocio)
        {
            _config = config;
            _negocio = negocio;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] AutenticacionRequest request)
        {
            try
            {
                var resultado = await _negocio.Consultar(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(AutenticacionHelper.ProcesarRespuestaSesion(resultado.Response, _config));
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

        [HttpPost("[action]")]
        public async Task<IActionResult> Registro([FromBody] RegistroRequest request)
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
                return BadRequest(new ResponseGeneric<bool>(ex));
            }

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CambiarContrasenia([FromBody] CambioContraseniaRequest request)
        {
            try
            {
                var resultado = await _negocio.CambiarContrasenia(request);
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
                return BadRequest(new ResponseGeneric<bool>(ex));
            }

        }
        [HttpPost("[action]")]
        public async Task<IActionResult> CambiarVerificentro([FromBody] AutenticacionResponse request)
        {
            try
            {
                var resultado = _negocio.ValidarUsuarioVerificentro(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    if (resultado.Response != null)
                    {
                        request.NombreVerificentro = resultado.Response.Nombre;
                        request.ClaveVerificentro = resultado.Response.Clave ?? string.Empty;
						return Ok(AutenticacionHelper.ProcesarRespuestaSesion(request, _config));
                    }

                    return Ok(new TokenResponse() { Mensaje = "La información es inválida, vuelva a iniciar sesión." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<bool>(ex));
            }

        }

        [HttpGet("[action]/{username}")]
        public async Task<IActionResult> GetVerificentrosByUsername(string username)
        {
            try
            {
                var resultado = await _negocio.GetVerificentrosByUsername(username);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok((resultado));
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
