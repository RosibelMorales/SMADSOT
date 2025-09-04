using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.IngresoFormaValorada.Response;
using Smadot.Models.Entities.Motivos.Response;
using Smadot.Models.Entities.SeguimientoCVV.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Smadot.Models.Entities.SeguimientoCVV.Response
{
	public class SeguimientoCVVResponse
	{
		public long Id { get; set; }

		public long? IdLineaMotivo { get; set; }

		public string? Nombre { get; set; }

		public long? IdVerificentro { get; set; }

		public string? NombreVerificentro { get; set; }

		public long? IdUserRegistro { get; set; }

		public string? NombreUsuario { get; set; }

		public string? NombreLinea { get; set; }

		public DateTime FechaRegistroLinea { get; set; }

		public DateTime FechaRegistroLineaMotivo { get; set; }

        public int? IdEstatus { get; set; }

        public string? Estatus { get; set; }

        public int IdMotivo { get; set; }

        public string? Motivo { get; set; }

        public int? Total { get; set; }

		public string? Notas { get; set; }

		public string? NotasMotivo { get; set; }


		public string? NombreMotivo { get; set; }


		public bool Activo { get; set; }

		public string? UrlDocumento { get; set; }

		public List<object>? MotivosXD { get; set; }

		public List<SeguimientoCVVFiles>? Files { get; set; }

        public List<MotivosReponse>? Motivos { get; set; }

		public string? UrlDocumentoBase64 { get; set; }
		public string? Clave { get; set; }
		public string? UserRegistroMotivo { get; set; }
    }
    public class SeguimientoCVVResponseGrid : SeguimientoCVVResponse
	{
		public string Acciones { get; set; }
	}

	public class SeguimientoCVVFiles
	{
		public string? Base64 { get; set; }

		public string? Nombre { get; set; }

        public string? Tipo { get; set; }

    }

}
