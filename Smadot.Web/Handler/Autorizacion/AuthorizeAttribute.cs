using Microsoft.AspNetCore.Mvc;

namespace Smadot.Web.Handler.Autorizacion
{
    /// <summary>
    /// Clase encargada de filtrado en el data notation [Authorize]
    /// </summary>
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        #region Constructor
        /// <summary>
        /// Constructor encargado de inicializar el Authorize
        /// </summary>
        public AuthorizeAttribute(params string[] Arg) 
            : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { Arg };
        }
        #endregion
    }
}
