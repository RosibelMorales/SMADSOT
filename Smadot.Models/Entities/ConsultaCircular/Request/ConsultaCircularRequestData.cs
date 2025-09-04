using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.ConsultaCircular.Request
{
	public class ConsultaCircularRequestData
	{
		public class ConsultaCircularRequest
		{
			[JsonProperty("idUserRegistro")]
			public long IdUserRegistro { get; set; }

			[JsonProperty("mensaje")]
			public string? Mensaje { get; set; }

			[JsonProperty("logo")]
			public string? Logo { get; set; }

		}

		public class ConfirmarCircularRequest
		{
            [JsonProperty("idV")]
            public long IdV { get; set; }

            [JsonProperty("idC")]
            public long IdC { get; set; }
        }
	}
}
