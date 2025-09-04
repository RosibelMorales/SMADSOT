using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Catalogos.Response
{
    public class TipoEquipoResponse
    {
        public int Id { get; set; }

        public string? Nombre { get; set; }

        public bool Estatus { get; set; }

        public int Total { get; set; }
    }
}
