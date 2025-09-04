using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Models.Entities.Autenticacion.Response;

namespace Smadot.Web.Helper.Identida
{
    public static class IdentidaHelper
    {
        /// <summary>
        /// Creacion del token del sitio web
        /// </summary>
        /// <param name="autenticacion">Token que se creo el api</param>
        /// <param name="configuration">Configuraciones del appsettings json</param>
        /// <returns></returns>
        public static string CreacionTokenWebSite(IConfiguration configuration,
                                                  TokenResponse autenticacion)
        {
            var jsonTokenData = new JwtSecurityTokenHandler().ReadJwtToken(autenticacion.Token) as JwtSecurityToken;

            //var RolActual = jsonTokenData.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;

            return GeneracionToken.GenerarToken(new GenericTokenModel
            {
                Issuer = configuration["JWT:Issuer"],
                Audience = configuration["JWT:Audience"],
                KeySecret = configuration["JWT:ClaveSecreta"],
                TimeExpire = int.Parse(configuration["TiempoExpiracionSesion"]),
                Claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("TokenSMADSOT", autenticacion.Token),
                    //new Claim(RolActual, RolActual)
                }
            });
        }

    }
}
