using Newtonsoft.Json;
using Smadot.Models.Entities.Autenticacion.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Smadot.Autenticacion.Api.Helper
{
    /// <summary>
    /// Helper encargado de la creacion de los tokens
    /// </summary>
    public static class AutenticacionHelper
    {
        /// <summary>
        /// Método encargado procesar la información de las respuesta del SP
        /// </summary>
        /// <param name="inicioSesion"></param>
        /// <returns></returns>
        public static ResponseGeneric<TokenResponse> ProcesarRespuestaSesion(AutenticacionResponse autenticacion, IConfiguration configuration)
        {

            if (autenticacion != null)
            {
                return new ResponseGeneric<TokenResponse>(new TokenResponse
                {
                    Token = GeneracionToken.GenerarToken(ObtenerDatosJwt(configuration, autenticacion))
                });
            }
            else
            {
                return new ResponseGeneric<TokenResponse>(new TokenResponse
                {
                    Mensaje = autenticacion?.CodigoError == (int)EnumSesion.CorreoNoExiste ?
                              "El correo ingresado es incorrecto, favor de validar su información" :
                             autenticacion?.CodigoError == (int)EnumSesion.Contrasenia ?
                                "La contraseña ingresada es incorrecta, favor de validar su información" : "Aún no se encuentra registrado"
                });
            }
        }
        /// <summary>
        /// Método encargado de crear el jwt para la API
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="inicioSesion"></param>
        /// <returns></returns>
        public static GenericTokenModel ObtenerDatosJwt(IConfiguration configuration, AutenticacionResponse inicioSesion)
        {
            // var userData = new
            // {
            //     inicioSesion.IdUser,
            //     inicioSesion.UserName,
            //     inicioSesion.Email,
            //     inicioSesion.Nombre,
            //     inicioSesion.IdVerificentro,
            //     inicioSesion.IdPuesto,
            //     inicioSesion.IdNivelConsulta,
            //     inicioSesion.NombreVerificentro 
            // };

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserData", JsonConvert.SerializeObject(inicioSesion))
            };

            if (inicioSesion.Roles != null)
            {
                foreach (var role in inicioSesion.RoleNames)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            return new GenericTokenModel
            {
                Issuer = configuration["JWT:Issuer"],
                Audience = configuration["JWT:Audience"],
                KeySecret = configuration["JWT:ClaveSecreta"],
                TimeExpire = int.Parse(configuration["TiempoExpiracionToken"]),
                Claims = claims
            };
        }
    }
}
