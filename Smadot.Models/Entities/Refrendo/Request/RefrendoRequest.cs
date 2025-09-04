using Newtonsoft.Json;

namespace Smadot.Models.Entities.Refrendo.Request
{
    public class RefrendoRequest
    {
        public long Id { get; set; }

        public long IdExento { get; set; }

        public string NumeroReferencia { get; set; } = null!;

        public string? UrlDoc1 { get; set; }

        public string? UrlDoc2 { get; set; }

        public string? UrlDoc3 { get; set; }

        public DateTime FechaCartaFactura { get; set; }

        public string? Placa { get; set; }

        public string? Propietario { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long IdUserRegistro { get; set; }

        public List<RefrendoFiles>? Files { get; set; }
        public DateTime FechaEmisionRef { get; set; }
        public DateTime FechaPago { get; set; }
    }

    public class RefrendoAutocompletRequest
    {
        [JsonProperty("term")]
        public string Term { get; set; }

        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("end")]
        public int End { get; set; }

		[JsonProperty("placa")]
		public bool Placa { get; set; }
	}

    public class RefrendoFiles
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }
}
