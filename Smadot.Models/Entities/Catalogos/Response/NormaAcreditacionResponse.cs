using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Catalogos.Response
{
    public class NormaAcreditacionResponse
    {
        public int Id { get; set; }

        public string? Clave { get; set; }

        public string? Descripcion { get; set; }

        public int Total { get; set; }
    }
}
