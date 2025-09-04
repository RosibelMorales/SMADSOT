using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Catalogos.Response
{
	public class AlmacenResponse
	{
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("nombre")]
        public string? Nombre { get; set; } = null!;

        [JsonProperty("activo")]
        public bool? Activo { get; set; }

        [JsonProperty("idVerificentro")]
        public long? IdVerificentro { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
	}
}
