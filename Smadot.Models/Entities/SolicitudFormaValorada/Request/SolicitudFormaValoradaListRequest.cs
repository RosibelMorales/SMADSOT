using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.SolicitudFormaValorada.Request
{
    public class SolicitudFormaValoradaListRequest : RequestList
    {
        [JsonProperty("siguienteFolio")]
        public bool? SiguienteFolio { get; set; }

        [JsonProperty("idAlmacen")]
        public long? IdAlmacen { get; set; }
    }


}
