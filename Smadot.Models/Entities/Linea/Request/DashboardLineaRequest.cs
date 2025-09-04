using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Linea.Request
{
    public class DashboardLineaRequest
    {
        public long Id { get; set; }
        public bool Bloquear { get; set; }
        public string Nota { get; set; }
        public int IdMotivo { get; set; }
    }
}
