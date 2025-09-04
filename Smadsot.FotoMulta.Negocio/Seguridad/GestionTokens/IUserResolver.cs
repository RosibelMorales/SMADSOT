using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadsot.FotoMulta.Negocio.Seguridad.GestionTokens
{
    public interface IUserResolver
    {
        ApplicationUser GetUser();

        public string GetToken();

        bool HasPermission(long id);
        public List<long> GetPermisos();
    }
}
