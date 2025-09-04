using Newtonsoft.Json;

namespace Smadot.Models.Entities.ReposicionCredencial.Request
{
    public class UserPuestoAutoCompleteRequest
    {
        [JsonProperty("term")]
        public string Term { get; set; }

        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("end")]
        public int End { get; set; }

    }
}
