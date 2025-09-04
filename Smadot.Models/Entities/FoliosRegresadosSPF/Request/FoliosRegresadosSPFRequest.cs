using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.FoliosRegresadosSPF.Request
{
    public class FoliosRegresadosSPFRequest: RequestList
    {
        [JsonProperty("pagination")]
        public bool Pagination { get; set; }
    }
}
