using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.ConsultaPruebaVerificacion.Request
{
    public class ConsultaPruebaVerificacionRequest : RequestList
    {
        [JsonProperty("placa")]
        public bool placa { get; set; }

        [JsonProperty("placaSerie")]
        public string? placaSerie { get; set; }
    }
}
