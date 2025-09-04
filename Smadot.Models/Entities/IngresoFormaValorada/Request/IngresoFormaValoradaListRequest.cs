using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.IngresoFormaValorada.Request
{
    public class IngresoFormaValoradaListRequest : RequestList
    {
        [JsonProperty("siguienteFolio")]
        public bool? SiguienteFolio { get; set; }
    }
}
