using Smadot.Models.Entities.Generic.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.AsignacionStock.Request
{

    public class RegistroStockventanillaRequest
    {
        public long Id { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public long IdAlmacen { get; set; }
        public long IdUserSolicita { get; set; }

        public string? UserSolicita { get; set; }

        public DateTime? FechaEntrega { get; set; }

        public long? IdUserRecibe { get; set; }

        public string UserRecibe { get; set; } = null!;

        public List<TablaFijaViewModel.TablaFijaRow> Asignaciones { get; set; }
    }
}
