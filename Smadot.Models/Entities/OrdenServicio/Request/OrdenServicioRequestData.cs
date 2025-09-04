using Newtonsoft.Json;

namespace Smadot.Models.Entities.OrdenServicio.Request
{
    public class OrdenServicioRequestData
    {
        [JsonProperty("IdEquipo")]
        public long IdEquipo { get; set; }

        [JsonProperty("TipoOrdenServicio")]
        public int TipoOrdenServicio { get; set; }

        [JsonProperty("Folio")]
        public long Folio { get; set; }

        [JsonProperty("Solucion")]
        public string? Solucion { get; set; }

        [JsonProperty("NoSerieActual")]
        public string? NoSerieActual { get; set; }

        [JsonProperty("NoSerieAnterior")]
        public string? NoSerieAnterior { get; set; }

        [JsonProperty("Motivo")]
        public string? Motivo { get; set; }

        [JsonProperty("idUserRegistro")]
        public long IdUserRegistro { get; set; }
        public string? FileString { get; set; }
    }

    public class OrdenServicioApiRequestData
    {
        [JsonProperty("IdEquipo")]
        public long IdEquipo { get; set; }

        [JsonProperty("TipoOrdenServicio")]
        public int TipoOrdenServicio { get; set; }

        [JsonProperty("Folio")]
        public long Folio { get; set; }

        [JsonProperty("Solucion")]
        public string? Solucion { get; set; }

        [JsonProperty("NoSerieActual")]
        public string? NoSerieActual { get; set; }

        [JsonProperty("NoSerieAnterior")]
        public string? NoSerieAnterior { get; set; }

        [JsonProperty("Motivo")]
        public string? Motivo { get; set; }

        [JsonProperty("idUserRegistro")]
        public long IdUserRegistro { get; set; }
        public List<OrdenServicioDoc> File { get; set; }
    }

    public class OrdenServicioDoc
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }
}
