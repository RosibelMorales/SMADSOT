using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.FoliosUsadosEnVentanilla.Request
{
    public class FoliosUsadosEnVentanillaRequest : RequestList
    {
        [JsonProperty("pagination")]
        public bool Pagination { get; set; }
    }
}
