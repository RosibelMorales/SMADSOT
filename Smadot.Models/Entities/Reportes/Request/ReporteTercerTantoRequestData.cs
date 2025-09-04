using Newtonsoft.Json;

namespace Smadot.Models.Entities.Reportes.Request
{
    public class ReporteTercerTantoRequestData
    {
        public long NumeroEntrega { get; set; }
        public long IdUserRegistro { get; set; }
        public string? FileString { get; set; }
        public string FechaInicial { get; set; }
        public string FechaFinal { get; set; }
        public List<List<string>> DatosRegistro { get; set; }
    }

    public class ReporteCertificadoRequest
    {
        public long NumeroCuenta { get; set; }
        public long IdUserRegistro { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public List<ReporteCertificado> ReporteCertificados { get; set; }
        public List<ReporteCertificadoDoc> File { get; set; }
        public long? IdVerificentro { get; set; }
    }

    public class ReporteCertificado
    {
        public int IdCatTipoCertificado { get; set; }
        public int Cantidad { get; set; }

        public int SerieInicial { get; set; }

        public int SerieFinal { get; set; }
    }

    public class ReporteCertificadoDoc
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }
}
