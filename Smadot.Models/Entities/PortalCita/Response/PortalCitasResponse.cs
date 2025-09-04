using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.PortalCitas.Response
{

    public class PortalCitasResponse
    {
        public List<int> DisableDays { get; set; }

        public List<string> DisableDates { get; set; }
        public List<PortalCitasDiasResponse> Dias { get; set; }
        public List<PortalCitasHorasResponse> HorasResponses { get; set; }

        public string ViewPartial { get; set; }
    }

    public class PortalCitasDiasResponse
    {
        public string Dia { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    }

    public class PortalCitasHorasResponse
    {
        public string Fecha { get; set; }
        public List<string> HorasDisponible { get; set; }

    }

    public class PortalCitasRegistroResponse
    {
        public string ErrorMessage { get; set; }
        public string FolioResult { get; set; }
    }

    public class PortalCitasComprobanteResponse
    {
        public long Id { get; set; }

        public string? NombrePersona { get; set; }

        public DateTime? Fecha { get; set; }

        public string? NombreCentroVerificacion { get; set; }

        public string? DireccionCentroVerificacion { get; set; }

        public string? Folio { get; set; }

        public bool Cancelada { get; set; }

        public string UrlComprobante { get; set; }

        public string Placa { get; set; } = null!;

        public string? Marca { get; set; }

        public string? Modelo { get; set; }

        public string Serie { get; set; } = null!;

        public short Anio { get; set; }

        public bool Poblano { get; set; }

        public string? Estado { get; set; }
    }
}
