using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smadsot.FotoMulta.Negocio.Seguridad.Permisos
{
    public class DictPermisos
    {
        public class PermisosMenu
        {
            public const int VerificacionVehicularService = 49;
        }
        public class MenuSolicitudesFormasValoradas
        {
            public const long Consultar = 49001;
            public const long PermisoMenuPadre = PermisosMenu.VerificacionVehicularService;
        }
    }
}