using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.ProgramacionCalibracion.Request
{
    public class ProgramacionCalibracionRequest : RequestList
    {
        [JsonProperty("idEquipo")]
        public long IdEquipo { get; set; }
    }
}
