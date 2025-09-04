namespace Smadot.Utilities.Modelos.Documentos
{
    public class FoliosVendidosCentroVerificacionDocument
    {
        public int? FolioVenta { get; set; }
        public int? FolioFV { get; set; }
        public string ClaveVenta { get; set; }
        public int? FoliosStock { get; set; }
        public string ReferenciaBancaria { get; set; }
        public decimal? MontoCadaVenta { get; set; }
        public DateTime Fecha { get; set; }
    }
}
