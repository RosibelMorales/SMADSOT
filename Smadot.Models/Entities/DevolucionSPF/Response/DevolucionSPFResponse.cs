using Smadot.Models.Entities.Generic.Response;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.DevolucionSPF.Response
{
    public class DevolucionSPFResponse
    {
        public int IdDevolucionSPF { get; set; }
        public long IdAlmacen { get; set; }
        public int NumeroDevolucion { get; set; }
        public int? NumeroSolicitud { get; set; }
        public string ResponsableEntrega { get; set; }
        public DateTime? FechaEntrega { get; set; }

        public int? Cantidad { get; set; }

        public string? UsuarioAprobo { get; set; }

        public string? RecibioSPF { get; set; }

        public string? NombreEncargado { get; set; }

        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public string? Folio { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public string RecibioEnSPF { get; set; }
        public List<TablaFijaViewModel.TablaFijaRow> TablaFijaViewModel { get; set; }
        public bool isReadOnly { get; set; }

        public string? FechaEntregaString { get; set; }

        public string? UrlDoc1 { get; set; }

        public string? UrlDoc2 { get; set; }

        public string? UrlDoc3 { get; set; }

        public string? UserAprobo { get; set; }

		public string FechaEntregaStr
		{
			get
			{

				return FechaEntrega?.ToString("d", CultureInfo.GetCultureInfo("es-ES"));
			}
		}
	}
}
