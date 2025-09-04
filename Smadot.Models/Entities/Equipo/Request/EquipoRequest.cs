using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.Equipo.Request
{
    public class EquipoRequest : RequestList
    {
        [JsonProperty("idLinea")]
        public long? IdLinea { get; set; }

        [JsonProperty("idVerificentro")]
        public long? IdVerificentro { get; set; }
    }
}
