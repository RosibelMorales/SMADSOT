using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos.Documentos
{
	public class CertificadoAdministrativaResponse
	{
		public long? IdAdministrativa { get; set; }

		public int? IdCatMotivoTramite { get; set; }

		public string? MotivoTramite { get; set; }

		public string? Placa { get; set; }

		public string? Serie { get; set; }

		public string? NumeroReferencia { get; set; }

		public string? UrlDoc1 { get; set; }

		public string? UrlDoc2 { get; set; }

		public string? UrlDoc3 { get; set; }

		public string? FolioAsignado { get; set; }

		public long? IdUserRegistro { get; set; }

		public string? UsuarioRegistro { get; set; }

		public string? FechaRegistro { get; set; }

	}
}
