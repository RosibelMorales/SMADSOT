using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.Catalogos.Request
{
	public class AlmacenRequest : RequestList
	{
        [JsonProperty("idVerificentroDestino")]
        public long? IdVerificentroDestino { get; set; }
        [JsonProperty("verificentroNulos")]
        public bool VerificentrosNulos { get; set; }
    }
}
