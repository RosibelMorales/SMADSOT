namespace Smadot.Utilities.Modelos.Documentos
{
    public class ConsultaStockDVRFDocument
    {
        public string TipoCertificado { get; set; }
        public int? Cantidad { get; set; }
        public int? Caja { get; set; }
        public int? FolioInicial { get; set; }
        public int? FolioFinal { get; set; }
        public string ClaveCertificado { get; set; }
    }
}
