using Newtonsoft.Json;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.DirectorioCentrosVerificacion.Request
{
    public class DirectorioCentrosVerificacionListRequest : RequestList
    {
        [JsonProperty("siguienteFolio")]
        public bool? SiguienteFolio { get; set; }
        [JsonProperty("idAlmacen")]
        public long? IdAlmacen { get; set; }
    }
    public class ConfiguradorCitaRequestList : RequestList
    {
        [JsonProperty("idCVV")]
        public long IdCVV { get; set; }
    }
}
