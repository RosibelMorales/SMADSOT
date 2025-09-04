using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Permiso.Response
{
    public class PermisoTreeResponse
    {
        public long Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public long? IdPermisoPadre { get; set; }
        public bool Selected { get; set; }
    }
}
