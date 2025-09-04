using Newtonsoft.Json;
namespace Smadot.Utilities.Modelos
{
    public class RequestList
    {
        [JsonProperty("pagina")]
        public int? Pagina { get; set; }

        [JsonProperty("registros")]
        public int? Registros { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; } = false;

        [JsonProperty("busqueda")]
        public string? Busqueda { get; set; }

        [JsonProperty("columnaOrdenamiento")]
        public string? ColumnaOrdenamiento { get; set; }

        [JsonProperty("ordenamiento")]
        public string? Ordenamiento { get; set; }

        [JsonProperty("pagination")]
        public bool? Pagination { get; set; }
    }
}
