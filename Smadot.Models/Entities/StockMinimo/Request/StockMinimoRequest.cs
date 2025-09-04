using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.StockMinimo.Request
{
    public class StockMinimoRequest : RequestList
    {
        [JsonProperty("idAlmacen")]
        public long? IdAlmacen { get; set; }
    }
}
