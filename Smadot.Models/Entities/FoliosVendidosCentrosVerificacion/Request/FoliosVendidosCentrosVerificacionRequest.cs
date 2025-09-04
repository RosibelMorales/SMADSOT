using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.FoliosVendidosCentrosVerificacion.Request
{
    public class FoliosVendidosCentrosVerificacionRequest : RequestList
    {
        [JsonProperty("pagination")]
        public bool Pagination { get; set; }
    }
}
