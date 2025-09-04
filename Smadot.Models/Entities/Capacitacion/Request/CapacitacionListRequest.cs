using Newtonsoft.Json;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Capacitacion.Request
{
    public class CapacitacionListRequest : RequestList
    {
        [JsonProperty("siguienteFolio")]
        public bool? SiguienteFolio { get; set; }

        public int? IdEmpleado { get; set; }

		public int? Id { get; set; }
	}
}
