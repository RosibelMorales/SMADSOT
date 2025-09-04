using System.Globalization;

namespace Smadot.Models.Entities.FoliosVendidosCentrosVerificacion.Response
{
    public class FoliosVendidosCentrosVerificacionResponse
	{
        public DateTime Fecha { get; set; }
        public string CVV { get; set; }
        public int? FolioVenta { get; set; }
        public int? FolioFV { get; set; }
        public string PersonaRecibeCertificado { get; set; }
        public string ClaveVenta { get; set; }
        public int? Total { get; set; }
        public int? FoliosStock { get; set; }
        public int? FoliosUsados { get; set; }
        public string ReferenciaBancaria { get; set; }
        public decimal? MontoCadaVenta { get; set; }
        public int TotalRegistros { get; set; }
        public long IdFormaValorada { get; set; }

    }

    public class FoliosVendidosCentrosVerificacionsResponseGrid : FoliosVendidosCentrosVerificacionResponse
	{
		public string Acciones { get; set; }
        public string MontoString
        {
            get
            {
                return MontoCadaVenta?.ToString("C2", new CultureInfo("es-MX"));
            }
        }
    }
}
