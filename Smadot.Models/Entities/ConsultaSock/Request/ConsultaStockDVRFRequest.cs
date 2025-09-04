using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.ConsultaSock.Request
{
	public class ConsultaStockDVRFRequest : RequestList
	{
        [JsonProperty("idAlmacen")]
        public long? IdAlmacen { get; set; }
    }
}
