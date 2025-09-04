using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Smadsot.FotoMulta.Model.Entities.Request;
using Smadsot.FotoMulta.Negocio.Operaciones;
using Smadsot.FotoMulta.Negocio.Seguridad.GestionTokens;
using Smadsot.FotoMulta.Negocio.Seguridad.Permisos;
using Smadsot.FotoMultaService.Api.HttpAttributes;
using Smadsot.FotoMultaService.Api.HttpAttributes.Configuration;

namespace Smadsot.FotoMultaService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VerificacionVehicularController : ControllerBase
    {

        private IVerificacionVehicularNegocio _negocio;

        private readonly IUserResolver _userResolver;

        public VerificacionVehicularController(IVerificacionVehicularNegocio negocio, IUserResolver userResolver, IOptions<RateLimitOptions> options)
        {
            _negocio = negocio;
            _userResolver = userResolver;
        }

        [HttpGet]
        [Authorize]
        [RateLimit]
        public async Task<IActionResult> Get([FromQuery] VerificacionRequest request)
        {
            try
            {
                var hasPermision = _userResolver.HasPermission(DictPermisos.MenuSolicitudesFormasValoradas.Consultar);
                if (!hasPermision)
                    return Unauthorized(new { menssage = "El usuario no tiene permiso para consultar la vigencia." });

                var procesarPlaca = await _negocio.ValidarVerificacion(request);
                if (procesarPlaca.Status == FotoMulta.Model.Modelos.ResponseStatus.Failed)
                {
                    return Problem(procesarPlaca.mensaje);
                }
                else if (procesarPlaca.Response == null)
                    return NotFound(new { message = procesarPlaca.mensaje });

                return Ok(procesarPlaca.Response);

            }
            catch (Exception)
            {

                return Problem(detail: "Ocurrió un error al procesar la placa. Vuelva a intentar más tarde.");
            }
        }
    }
}