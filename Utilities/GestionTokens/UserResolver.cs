using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.GestionTokens
{
    public class UserResolver : IUserResolver
    {
        private readonly string _token;
        private readonly ApplicationUser _applicationUser;

        public UserResolver(IHttpContextAccessor context)
        {
            _applicationUser = new ApplicationUser();
            if (context.HttpContext.User != null && context.HttpContext.User.Claims.Any())
            {
                _token = LecturaToken.ObtenerTokenWebAPISmadsot(context.HttpContext);

                if (string.IsNullOrWhiteSpace(_token))
                {
                    if (context.HttpContext.User.Claims.Any(x => x.Type == "UserData"))
                    {
                        _applicationUser = JsonConvert.DeserializeObject<ApplicationUser>(context.HttpContext.User.Claims.First(e => e.Type == "UserData")?.Value);
                    }
                }
                else
                {
                    _applicationUser = LecturaToken.ObtenerInformacionUsuario(_token);
                }
               

            }
        }

        public ApplicationUser GetUser()
        {
            return _applicationUser;
        }

        public string GetToken()
        {
            return _token;
        }

        public bool HasPermission(long id)
        {
            return _applicationUser.Permisos.Any(x => x == id);
        }

        public List<long> GetPermisos()
        {
            return _applicationUser.Permisos.ToList();
        }
    }
}
