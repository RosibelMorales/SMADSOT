using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Smadot.Models.Entities.RecepcionDocumentos.Request
{
    public class CambiarLineaRequest
    {
        [JsonProperty("idPrueba")]
        public long IdPrueba { get; set; }
        [JsonProperty("linea")]
        public string Linea { get; set; }
    }
}