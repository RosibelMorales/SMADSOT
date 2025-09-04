using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Smadot.Utilities.GestionTokens;

namespace Smadot.Web.Handler.Autorizacion
{
    /// <summary>
    /// Clase encargada Manipular el data notation del  [Authorize]
    /// </summary>
    public class AuthorizeFilter : IAuthorizationFilter
    {
        readonly string[] _argSesion;

        #region Constructor Inicial
        /// <summary>
        /// Constructor Inicial
        /// </summary>
        public AuthorizeFilter(params string[] arg)
        {
            _argSesion = arg;
        }
        #endregion

        /// <summary>
        /// Método encargado de validar la autorizacion de los peticiones [Authorize()]
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var esAutenticado = context.HttpContext.User.Identity.IsAuthenticated;

            if (esAutenticado)
            {
                bool flagClaim = false;
                //long permiso = 0;
                var svc = context.HttpContext.RequestServices;
                var memCache = svc.GetService<IUserResolver>();
                //Obtenemos el usuario
                var user = memCache.GetUser();
                var semestre = DateTime.Now.Month <= 6 ? 1 : 2;
                // var semestre1FechaInicio = new DateTime(fechaActual.Year, 1, 1);
                // var semestre1FechaFinal = new DateTime(fechaActual.Year, 6, 30, 23, 59, 59);
                // var semestre = 2;
                // // Validamos que la sesión tenga el semestre correcti
                // if (fechaActual >= semestre1FechaInicio && fechaActual <= semestre1FechaFinal)
                // {
                //     semestre = 1;
                // }
                //Sí no es el semestre correcto lo redirigimos
                if (user.Semestre != semestre)
                {
                    context.HttpContext.Session.Clear();

                    context.Result = new UnauthorizedResult();

                }
                foreach (var item in _argSesion)
                {
                    if (context.HttpContext.User.HasClaim(item, item))
                        flagClaim = true;
                    //long.TryParse(item, out permiso);
                }

                if (user.RoleNames.Contains("Usuario Verificador"))
                {
                    // Obtener la ruta solicitada
                    string requestedPath = context.HttpContext.Request.Path;

                    // Opcional: Puedes obtener el método HTTP utilizado (GET, POST, etc.)
                    string httpMethod = context.HttpContext.Request.Method;

                    // Ahora puedes trabajar con la ruta y el método HTTP según sea necesario
                    // Por ejemplo, puedes realizar lógica de autorización personalizada basada en la ruta.

                    // Ejemplo de lógica de autorización
                    if (!IsAuthorized(requestedPath))
                    {
                        context.Result = new RedirectResult("/");
                    }

                }

                //if (!flagClaim)
                //{
                //    context.Result = new RedirectResult("~/Home");
                //}
                //if (permiso > 0)
                //{
                //    if (!memCache.HasPermission(permiso))
                //    {
                //        context.Result = new UnauthorizedResult();
                //    }
                //}
            }
            else
            {
                context.Result = new RedirectResult("~/");
            }
        }
        private bool IsAuthorized(string requestedPath)
        {
            // Implementa tu lógica de autorización aquí, por ejemplo, verifica si el usuario tiene permiso para acceder a la ruta.
            // Devuelve true si el usuario está autorizado, de lo contrario, devuelve false.
            // Puedes consultar bases de datos, roles de usuario u otros criterios de autorización aquí.
            // Este es solo un ejemplo simple.

            // Supongamos que solo se permite el acceso a ciertas rutas:
            List<string> authorizedPaths = new List<string>
                {
                    "/Autenticacion/CerrarSesion",
                    "/Autenticacion/CambiarContrasenia",
                    "/PortalCitas",
                    // Agrega otras rutas autorizadas aquí
                };

            // Verifica si la ruta solicitada está en la lista de rutas autorizadas.
            return authorizedPaths.Contains(requestedPath);
        }

    }
}
