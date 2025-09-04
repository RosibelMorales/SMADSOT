using Smadot.Models.DataBase;
namespace Smadot.Models.Entities.Reportes.Response
{
    public class ReporteTercerTantoResponseData
    {
        public class ReporteTercerTantoResponse
        {
            public long? Id { get; set; }
            public string? NumeroEntrega { get; set; }
            public string? FechaInicial { get; set; }
            public string? FechaFinal { get; set; }
            public string? UrlDocumento { get; set; }
            public string? FechaRegistro { get; set; }
            public string? UsuarioRegistro { get; set; }
            public int Total { get; set; }
            public List<ReporteTercerTantoCertificados> reporteTercerTantoCertificados { get; set; }
        }

        public class ReporteTercerTantoResponseGrid : ReporteTercerTantoResponse
        {
            public string Acciones { get; set; }
        }

        public class ReporteTercerTantoCertificados
        {
            public string ClaveCertificado { get; set; }
            public string NombreCatTipoReporte { get; set; }
            public int Cantidad { get; set; }
            public int SerieInicial { get; set; }
            public int SerieFinal { get; set; }
        }
    }
}
