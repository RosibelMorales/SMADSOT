using Newtonsoft.Json;

namespace Smadot.Utilities.Modelos
{
    public class RequestListLinea : RequestList
    {
        [JsonProperty("idLinea")]
        public long IdLinea{ get; set; }
    }
}
