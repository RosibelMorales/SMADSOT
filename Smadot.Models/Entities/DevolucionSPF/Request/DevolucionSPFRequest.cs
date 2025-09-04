using Smadot.Models.Entities.Generic.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.DevolucionSPF.Request
{
    public class DevolucionSPFRequest
    {
        public int IdDevolucionSPF { get; set; }
        public int NumeroDevolucion { get; set; }
        public long IdAlmacen { get; set; }
        public long? NumeroSolicitud { get; set; }
        public string ResponsableEntrega { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string RecibioEnSPF { get; set; }
        public List<TablaFijaViewModel.TablaFijaRow> TablaFijaViewModel { get; set; }

        public string? UrlDoc1 { get; set; }

        public string? UrlDoc2 { get; set; }

        public string? UrlDoc3 { get; set; }
        public List<DevolucionSPFFiles>? Files { get; set; }
    }

    public class DevolucionSPFFiles
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }
}
