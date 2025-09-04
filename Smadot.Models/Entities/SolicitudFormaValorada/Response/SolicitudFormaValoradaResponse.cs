using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.SolicitudFormaValorada.Response
{
	public class SolicitudFormaValoradaResponse
	{
		public long IdSolicitudFV { get; set; }
		public long? IdIngresoFV { get; set; }

		public DateTime FechaSolicitudFV { get; set; }

		public string? FechaSolicitud { get; set; }

		public long IdUserSolicitaFV { get; set; }

		public string? UserSolicitaFV { get; set; }

		public int IdCatEstatusSolicitudFV { get; set; }

		public string EstatusFV { get; set; } = null!;

		public bool ActivoFV { get; set; }

		public DateTime FechaRegistroFV { get; set; }

		public long IdAlmacenFV { get; set; }

		public string NombreUser { get; set; }


		public string AlmacenFV { get; set; } = null!;

		public long IdSC { get; set; }

		public int IdCatTipoCertificadoSC { get; set; }

		public string TipoCertificadoSC { get; set; } = null!;

		public string? ClaveCertificadoSC { get; set; }

		public int CantidadSC { get; set; }

		public int FolioInicialSC { get; set; }

		public int FolioFinalSC { get; set; }

		public DateTime? FechaEntregaIFV { get; set; }

		public string? NombreRecibioIFV { get; set; }

		public int? Total { get; set; }

		public int CantidadTotal { get; set; }

		public List<AlmacenResponse>? Almacenes { get; set; }
		public long? IdVerificentro { get; set; }

		public bool? HasCaja { get; set; }

		public string? UrlDoc1 { get; set; }

		public string? UrlDoc2 { get; set; }

		public string? UrlDoc3 { get; set; }
	}

	public class SolicitudFormaValoradaResponseGrid : SolicitudFormaValoradaResponse
	{
		public string Acciones { get; set; }
	}
}
