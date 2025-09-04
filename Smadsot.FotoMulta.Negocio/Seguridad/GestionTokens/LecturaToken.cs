using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Smadsot.FotoMulta.Negocio.Seguridad.GestionTokens
{
    /// <summary>
    /// Helper encargado de obener datos del usuario en sesion.
    /// </summary>
    public static class LecturaToken
    {
        /// <summary>
        /// Método encargado de obtener el token de la  web api
        /// </summary>
        /// <param name="currentUser">Instancia actual del HTTP</param>
        /// <returns></returns>
        public static string ObtenerTokenWebAPISmadsot(HttpContext currentUser)
        {
            string token = "";
            try
            {
                if (currentUser.User.HasClaim(claim => claim.Type == "TokenSMADSOT"))
                {
                    var tokenAPI = currentUser.User.Claims.FirstOrDefault(claim => claim.Type == "TokenSMADSOT").Value;
                    if (!string.IsNullOrEmpty(tokenAPI))
                        token = tokenAPI;
                }
            }
            catch (Exception)
            {
                return token;
            }

            return token;
        }

        public static ApplicationUser ObtenerInformacionUsuario(string token)
        {
            ApplicationUser usuario = null;
            try
            {
                var jsonTokenData = new JwtSecurityTokenHandler().ReadJwtToken(token) as JwtSecurityToken;

                var jsonUsuario = jsonTokenData.Claims.FirstOrDefault(claim => claim.Type == "UserData").Value;

                if (!string.IsNullOrEmpty(jsonUsuario))
                {
                    usuario = JsonConvert.DeserializeObject<ApplicationUser>(jsonUsuario);
                }
            }
            catch (Exception)
            {
                return usuario;
            }

            return usuario;
        }

        /// <summary>
        /// Método encargado de obtener el usuario actual en sesion
        /// </summary>
        /// <param name="currentUser">Instancia actual del HTTP</param>
        /// <returns></returns>
        public static ApplicationUser UsuarioActualEnSesion(HttpContext currentUser)
        {
            ApplicationUser usuario = null;
            try
            {
                var jsonTokenData = new JwtSecurityTokenHandler().ReadJwtToken(ObtenerTokenWebAPISmadsot(currentUser)) as JwtSecurityToken;

                var jsonUsuario = jsonTokenData.Claims.FirstOrDefault(claim => claim.Type == "UserData").Value;

                if (!string.IsNullOrEmpty(jsonUsuario))
                {
                    usuario = JsonConvert.DeserializeObject<ApplicationUser>(jsonUsuario);
                }
            }
            catch (Exception)
            {
                return usuario;
            }

            return usuario;
        }
    }
}
