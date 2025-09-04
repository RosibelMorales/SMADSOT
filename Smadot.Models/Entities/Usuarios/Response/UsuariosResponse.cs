using Smadot.Models.Entities.Roles.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Usuarios.Response
{
    public class UsuariosResponse
    {

        public int? Id { get; set; }

        public int? IdUsuario { get; set; }

        public int? IdRol { get; set; }

        public string? CorreoElectronico { get; set; }

        public string? NombreUsuario { get; set; }

        public string? NombreRol { get; set; }

        public string? Rol { get; set; }

        public int? Total { get; set; }

        public int? IdRolSelect { get; set; }

        public string? NombreRolSelect { get; set; }

        public string? Contrasenia { get; set; }
        public bool LockoutEnabled { get; set; }
    }
    public class UsuariosResponseGrid : UsuariosResponse
    {
        public string Acciones { get; set; }
    }
}
