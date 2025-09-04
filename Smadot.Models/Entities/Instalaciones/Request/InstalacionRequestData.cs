using Newtonsoft.Json;

namespace Smadot.Models.Entities.Instalaciones.Request
{
    public class InstalacionRequestData
    {
        [JsonProperty("idProveedor")]
        public long IdProveedor { get; set; }

        [JsonProperty("idUserRegistro")]
        public long IdUserRegistro { get; set; }
        public string? FileString { get; set; }
    }

    public class InstalacionApiRequestData
    {
        [JsonProperty("idProveedor")]
        public long IdProveedor { get; set; }

        [JsonProperty("idUserRegistro")]
        public long IdUserRegistro { get; set; }
        public long? IdVerificentro { get; set; }
        public List<InstalacionDoc> File { get; set; }
    }

    public class InstalacionDoc
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }
}
