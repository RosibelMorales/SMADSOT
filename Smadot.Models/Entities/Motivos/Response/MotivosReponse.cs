using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Motivos.Response
{
	public class MotivosReponse
	{
		public int? Id { get; set; }

		public string? Nombre { get; set; }

		public bool? Activo { get; set; }

		//[JsonProperty("id")]
		//public long? Id { get; set; }

		//[JsonProperty("nombre")]
		//public string? Nombre { get; set; } = null!;

		//[JsonProperty("activo")]
		//public bool? Activo { get; set; }

		//[JsonProperty("total")]
		//public int? Total { get; set; }
	}
}
