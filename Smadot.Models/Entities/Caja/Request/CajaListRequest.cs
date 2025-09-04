using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.Caja.Request
{
    public class CajaListRequest : RequestList
    {
        [JsonProperty("siguienteFolio")]
        public bool? SiguienteFolio { get; set; }
    }
}
