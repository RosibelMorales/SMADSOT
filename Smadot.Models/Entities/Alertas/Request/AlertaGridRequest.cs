using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.Alertas.Request
{
    public class AlertaGridRequest : RequestList
    {
        [JsonProperty("sinAtender")]
        public bool SinAtender { get; set; } = false;
        [JsonProperty("leido")]
        public bool? Leido { get; set; }
    }
}
