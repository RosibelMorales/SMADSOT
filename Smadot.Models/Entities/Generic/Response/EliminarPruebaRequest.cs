using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Smadot.Models.Entities.Generic.Response
{
    public class EliminarPruebaRequest
    {
        [JsonProperty("idPrueba")]
        public long IdPrueba { get; set; }

    }
}
