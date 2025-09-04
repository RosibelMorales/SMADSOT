using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Smadot.Models.Entities.PortalCitas.Request
{
    public class PortalCitaRequest
    {
        public long Id { get; set; }
        public int IdTipoCombustible { get; set; }

        public string Nombre { get; set; }
        public string NombreGeneraCita { get; set; }

        public string? RazonSocial { get; set; }

        public string? Correo { get; set; }

        public DateTime Fecha { get; set; }

        public long IdCVV { get; set; }

        public string Placa { get; set; }


        public long IdSubMarca { get; set; }
        public string? Marca { get; set; }
        public string? SubMarcaNueva { get; set; }

        public short Anio { get; set; }

        public bool Acepto { get; set; }
        public bool Poblano { get; set; }
        public bool EsPErsonaMoral { get; set; }

        public string Serie { get; set; }
        public string? ClaveEstado { get; set; }

        public string? ColorVehiculo { get; set; }

        public string FechaString { get; set; }

        public string HoraString { get; set; }

        public string? Logo { get; set; }
        public string Token { get; set; }
    }

    public class PortalCitaCancelarRequest
    {
        public long Id { get; set; }

        public string Folio { get; set; }
        public bool? ErrorTablaMaestra { get; set; }
    }
    public class PortalConsultaCitaRequest
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("folio")]

        public string? Folio { get; set; }
    }

    public class ConfirmarCita
    {
        public string Folio { get; set; }
    }
}
