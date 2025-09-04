using System.Globalization;

namespace Smadot.Models.Entities.FoliosUsadosEnVentanilla.Response
{
    public class FoliosUsadosEnVentanillaResponse
    {
        public DateTime Fecha { get; set; }
        public string FolioTramite { get; set; }
        public string TipoTramite { get; set; }
        public string NombreUsuario { get; set; }
        public string TelefonoUsuario { get; set; }
        public string CorreoUsuario { get; set; }
        public string DatosVehiculo { get; set; }
        public string FolioCertificado { get; set; }
        public string Razon { get; set; }
        public string PersonaGeneroTramite { get; set; }
        public decimal? MontoTramite { get; set; }
        public string ReferenciaBancaria { get; set; }
        public int Total { get; set; }
        public long IdFormaValorada { get; set; }
        public string? ClaveTramite { get; set; }
    }

    public class FoliosUsadosEnVentanillaResponseGrid : FoliosUsadosEnVentanillaResponse
    {
        public string Acciones { get; set; }
        public string MontoString
        {
            get
            {
                return MontoTramite?.ToString("C2", new CultureInfo("es-MX"));
            }
        }
    }
}
