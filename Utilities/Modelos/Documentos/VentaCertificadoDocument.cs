namespace Smadot.Utilities.Modelos.Documentos
{
    public class VentaCertificadoDocument
    {
        public string NombreAlmacen { get; set; }
        public string NombreVerificentro { get; set; }
        public string? NumeroReferencia { get; set; }
        public string? ReferenciaBancaria { get; set; }
        public string? NumeroCompra { get; set; }
        public DateTime FechaVenta { get; set; }
        public string? Clave { get; set; }
        public string? Rfc { get; set; }
        public Decimal? SumaImporteTotal { get; set; }
        public int? TotalCantidadHologramas { get; set; }
        public List<CertificadoDataTotal> certificadosTotalData { get; set; }

    }
    public class CertificadoDataTotal
    {
        public string NombreHolograma { get; set; }
        public string FolioInicial { get; set; }
        public string FolioFinal { get; set; }
        public int Cantidad { get; set; }
        public decimal ImporteTotal { get; set; }
    }
}