using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Catalogos.Response
{
    public class MotivoTramiteResponse
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }

        public int Total { get; set; }
    }
}
