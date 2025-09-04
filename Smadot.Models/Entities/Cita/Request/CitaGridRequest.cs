using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.Cita.Request
{
    public class CitaGridRequest : RequestList
    {
        [JsonProperty("fecha1")]
        public DateTime? Fecha1 { get; set; }
        [JsonProperty("fecha2")]
        public DateTime? Fecha2 { get; set; }
        [JsonProperty("atendida")]
        public bool? Atendida { get; set; }
    }

    public class ReiniciarCitaRequest
    {
        public long IdCita { get; set; }
        public long IdVerificacion { get; set; }
    }

}
