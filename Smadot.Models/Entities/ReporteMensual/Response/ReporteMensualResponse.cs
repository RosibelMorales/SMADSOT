using Smadot.Models.Entities.IngresoFormaValorada.Response;
using Smadot.Models.Entities.SeguimientoCVV.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.ReporteMensual.Response
{
    public class ReporteMensualResponse
    {
        public long? IdReporte { get; set; }

        public long? NumeroReporte { get; set; }

        public DateTime? FechaModificacionReporte { get; set; }

        public DateTime? FechaRegistroReporte { get; set; }

        public long? IdCatEstatusReporte { get; set; }

        public string? NombreCatEstatusReporte { get; set; }

        public bool? ActivoCatEstatusReporte { get; set; }

        public long IdUsuario { get; set; }

        public string? NombreUsuario { get; set; }


        public string? NombreCatTipoCertificado { get; set; }

        public int? IdCatTipoCertificado { get; set; }

        public int? Cantidad { get; set; }

        public long? FolioInicial { get; set; }

        public long? FolioFinal { get; set; }


        public long? PruebasRealizadas { get; set; }

        public long? PruebasAprobadas { get; set; }

        public long? PruebasReprobadas { get; set; }

        public int? Entregados { get; set; }

        public int? Cancelados { get; set; }

        public int? SinUsar { get; set; }

        public int? ServicioTransportePublico { get; set; }

        public int? ServicioTransporteMercantil { get; set; }

        public int? Total { get; set; }

        public string? UrlExcel { get; set; }

        public string? UrlEntrega { get; set; }

        public List<ReporteMensualFiles>? Files { get; set; }
    }

    public class ReporteMensualResponseGrid : ReporteMensualResponse
    {
        public string Acciones { get; set; }
    }

    public class ReporteMensualFiles
    {
        public string? Nombre { get; set; }
        public string? Tipo { get; set; }
        public string? Base64 { get; set; }
    }
}
