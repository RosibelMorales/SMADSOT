using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.VentaCVV.Request
{
    public class VentaCVVListRequest : RequestList
    {
        [JsonProperty("idAlmacen")]
        public long? IdAlmacen { get; set; }
    }
}
