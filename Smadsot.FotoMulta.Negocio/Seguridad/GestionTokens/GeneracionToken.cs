using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Smadsot.FotoMulta.Model.Modelos;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Smadsot.FotoMulta.Negocio.Seguridad.GestionTokens
{
    /// <summary>
    /// Clase generica para lageneacion del un token
    /// </summary>
    public static class GeneracionToken
    {
        /// <summary>
        /// Método generico de la creacion del token
        /// </summary>
        /// <param name="genericToken">Entidad de la sesion</param>
        /// <returns></returns>
        public static string GenerarToken(GenericTokenModel genericToken)
        {
            var _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(genericToken.KeySecret));

            var _signingCredentials = new SigningCredentials(
                    _symmetricSecurityKey, SecurityAlgorithms.HmacSha256
                );
            DateTime fechaActual = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)"));
            var _Header = new JwtHeader(_signingCredentials);

            var _Payload = new JwtPayload(
                    issuer: genericToken.Issuer,
                    audience: genericToken.Audience,
                    claims: genericToken.Claims,
                    notBefore: fechaActual,
                    expires: fechaActual.AddMinutes(genericToken.TimeExpire)
            );

            var _Token = new JwtSecurityToken(
                    _Header,
                    _Payload
                    );

            return new JwtSecurityTokenHandler().WriteToken(_Token);
        }
    }
}
