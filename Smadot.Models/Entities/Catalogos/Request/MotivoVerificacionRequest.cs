using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.Catalogos.Request
{
    public class MotivoVerificacionRequest : RequestList
    {
        [JsonProperty("activo")]
        public bool? Activo { get; set; }
    }
}
