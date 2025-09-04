using Smadot.Models.DataBase;

namespace Smadot.Models.Entities.Alertas.Response
{
    public class AlertaGridResponse : vAlertaResponse
    {
        public string FechaStr { get { return Fecha.ToString("dd/MM/yyyy hh:mm tt"); } }
        public string FechaModificacionStr
        {
            get
            {
                return FechaModificacion != null ? FechaModificacion.Value.ToString("dd/MM/yyyy hh:mm tt") : "-/-/-";
            }
        }
        public string AtendidaStr
        {
            get
            {
                return string.IsNullOrEmpty(MovimientoFinal) ? "No" : "Sí";
            }
        }
        public string LeidoStatus { get; set; }
        public string Acciones { get; set; }
    }
    public class vAlertaResponse : vAlertum
    {
        public int Total { get; set; }
    }
}
