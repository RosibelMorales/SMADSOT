namespace Smadot.Models.Entities.FoliosRegresadosSPF.Response
{
	public class FoliosRegresadosSPFResponse
	{
		public string ClaveDevolucion { get; set; }
		public string ClaveSolicitud { get; set; }
		public string ClaveVenta { get; set; }
		public DateTime Fecha { get; set; }
		public string TipoCertificado { get; set; }
		public string ClaveCertificado { get; set; }
		public string FolioInicial { get; set; }
		public string FolioFinal { get; set; }
		public string ResponsableEntrega { get; set; }
		public string PersonaValido { get; set; }
		public long IdFormaValorada { get; set; }
		public int Total { get; set; }

	}

	public class FoliosRegresadosSPFResponseGrid : FoliosRegresadosSPFResponse
	{
		public string Acciones { get; set; }
	}
}
