using Newtonsoft.Json;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Verificacion.Request
{
    public class VerificacionCitaRequest : RequestList
    {
        [JsonProperty("idVerificentro")]
        public long IdVerificentro { get; set; }
    }
}
