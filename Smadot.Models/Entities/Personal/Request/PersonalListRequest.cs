using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.Personal.Request
{
    public class PersonalListRequest : RequestList
    {
        [JsonProperty("idVerificentro")]
        public long? IdVerificentro { get; set; }
    }
}
