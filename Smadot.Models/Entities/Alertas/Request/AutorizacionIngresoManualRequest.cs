using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Alertas.Request
{
    public class AutorizacionIngresoManualRequest
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("estatus")]
        public bool Estatus { get; set; }
    }
}
