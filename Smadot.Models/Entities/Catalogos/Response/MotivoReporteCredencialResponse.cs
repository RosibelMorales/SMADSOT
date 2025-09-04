using Newtonsoft.Json;

namespace Smadot.Models.Entities.Catalogos.Response
{
    public class MotivoReporteCredencialResponse
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("nombre")]
        public string? Nombre { get; set; } = null!;

        [JsonProperty("activo")]
        public bool? Activo { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
	}
}
