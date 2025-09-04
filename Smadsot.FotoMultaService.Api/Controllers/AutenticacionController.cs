using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Smadsot.FotoMultaService.Api.Helper;
using Smadsot.FotoMulta.Negocio.Operaciones;
using Smadsot.FotoMulta.Model.Entities.Request;
using Smadsot.FotoMulta.Model.Entities.Response;
using Smadsot.FotoMulta.Negocio.Seguridad;
using Smadsot.FotoMulta.Negocio.Seguridad.Modelo;
using Smadsot.FotoMulta.Model.Modelos;

namespace Smadsot.FotoMultaService.Api.Controllers
{
    [Route("VerificacionVehicular/[controller]")]
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

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AutenticacionRequest request)
        {
            try
            {
                request.Contrasenia = GestioEncriptacion.Cifrar(new SeguridadModelo
                {
                    Valor = request.Contrasenia,
                    LlaveCifrado = _config["JWT:ClaveSecreta"]
                });
                var resultado = await _negocio.Autenticar(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    var _ = AutenticacionHelper.CrearToken(resultado.Response, _config);
                    if (_.Status != ResponseStatus.Failed)
                    {

                        return Ok(_.Response);
                    }
                    else
                    {
                        return Unauthorized(_.mensaje);

                    }
                }
                else
                {
                    return Unauthorized(resultado.mensaje);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }

        }
        [HttpPut]
        public async Task<IActionResult> CambiarContrasenia([FromBody] CambioContraseniaRequest request)
        {
            try
            {
                request.ContraseniaActual = GestioEncriptacion.Cifrar(new SeguridadModelo
                {
                    Valor = request.ContraseniaActual,
                    LlaveCifrado = _config["JWT:ClaveSecreta"]
                });
                var resultado = await _negocio.CambiarContrasenia(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    if (resultado.Response)
                        return Ok();
                    else
                        return NotFound("Contraseña nueva diferente de la confirmación o contraseña actual incorrecta.");

                }
                else
                {
                    return BadRequest("Datos de cambio de contraseña inválidos.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<bool>(ex));
            }

        }

    }
}
