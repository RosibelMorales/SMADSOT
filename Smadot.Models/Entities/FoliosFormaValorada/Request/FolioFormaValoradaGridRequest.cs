using Newtonsoft.Json;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.FoliosFormaValorada.Request
{
    public class FolioFormaValoradaGridRequest : RequestList
    {
        [JsonProperty("tipoCertificado")]
        public long? TipoCertificado { get; set; }
        [JsonProperty("tipoTramite")]
        public int? TipoTramite { get; set; }
    }
}
