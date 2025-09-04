using Newtonsoft.Json;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Catalogos.Request
{
    public class SubMarcaVehiculoRequest : RequestList
    {
        [JsonProperty("marca")]
        public string? Marca { get; set; }
        [JsonProperty("submarca")]
        public string? SubMarcaVehiculo { get; set; }
        [JsonProperty("anio")]
        public int? Anio { get; set; }
        [JsonProperty("submarcaClave")]
        public bool SubmarcaClave { get; set; }
    }
}
