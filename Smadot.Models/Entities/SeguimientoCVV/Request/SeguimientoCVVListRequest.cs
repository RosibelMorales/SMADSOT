using Newtonsoft.Json;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.SeguimientoCVV.Request
{
	public class SeguimientoCVVListRequest : RequestList
	{
		[JsonProperty("siguienteFolio")]
		public bool? SiguienteFolio { get; set; }
		[JsonProperty("idAlmacen")]
		public long? IdAlmacen { get; set; }
	
	}
}
