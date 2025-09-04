using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.ConsultaCircular.Response
{
	public class ConsultaCircularResponseData
	{
		public class ConsultaCircularResponse
		{
			public long? Id { get; set; }
			public string? NumeroCircular { get; set; }
			public DateTime? Fecha { get; set; }
			public int? Leidos { get; set; }
			public int? NoLeidos { get; set; }

			public string Mensaje { get; set; }

			public int Total { get; set; }
		}

		public class ConsultaCircularResponseGrid : ConsultaCircularResponse
		{
			public string Acciones { get; set; }
			public string FechaString
			{ get { return Fecha.Value.ToString("dd/MM/yyyy"); } }
		}
	}
}
