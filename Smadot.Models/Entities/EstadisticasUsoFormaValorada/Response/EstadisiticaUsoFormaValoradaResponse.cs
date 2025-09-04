using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Dicts.ProveedorDicts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.SolicitudFormaValorada.Response
{
    public class EstadisiticaUsoFormaValoradaResponse
    {
        public long Id { get; set; }

        public string Folio { get; set; }
        public string TipoCertificado { get; set; }

        public string ClaveTramiteRealizado { get; set; }

    }

    public class EstadisiticaUsoFormaValoradaResponseGrid : EstadisiticaUsoFormaValoradaResponse
    {
        public string Acciones { get; set; }
    }
    public class DashboardIndicadores
    {
        public DashboardIndicadores(List<Indicador> totales)
        {
            Totales = totales;
        }

        public List<Indicador> Totales { get; set; }
    }
    public class Indicador
    {
        public Indicador(string nombre, string total, string cssClass)
        {
            Nombre = nombre;
            Total = total;
            CssClass = cssClass;
        }

        public string Nombre { get; set; }
        public string CssClass { get; set; }
        public string Total { get; set; }
    }

    // public class DashboardEstadisticasResponse
    // {
    //     public long TotalReportes { get; set; }
    //     public long TotalReportesTerce { get; set; }
    //     public long TotalPruebas { get; set; }
    //     public long TotalPruebasRechazadas { get; set; }
    //     public long TotalPersonalCapacitacion { get; set; }
    //     public long TotalPersonalAdministrativo { get; set; }
    //     public long TotalTecnicosAcreditados { get; set; }
    //     public long TotalCalibracionesLaboratorios { get; set; }
    //     public long TotalCalibracionesProveedores { get; set; }
    //     public long TotalNoCertificaciones { get; set; }
    //     public long TotalCitas { get; set; }
    //     public long TotalCitasNoAtendidas { get; set; }
    //     public long TotalCitasAtendidas { get; set; }
    //     public string FolioInicialDobleCero { get; set; }
    //     public string FolioFinalDobleCero { get; set; }
    //     public decimal MontoTramite { get; set; }
    //     public long VentaCvv { get; set; }

    //     public string FolioInicialCero { get; set; }
    //     public string FolioFinalCero { get; set; }

    //     public string FolioInicialUno { get; set; }
    //     public string FolioFinalUno { get; set; }

    //     public string FolioInicialDos { get; set; }
    //     public string FolioFinalDos { get; set; }

    //     public string FolioInicialCna { get; set; }
    //     public string FolioFinalCna { get; set; }

    //     public string FolioInicialExento { get; set; }
    //     public string FolioFinalExento { get; set; }

    //     public string FolioInicialTestificacion { get; set; }
    //     public string FolioFinalTestificacion { get; set; }

    //     //public List<DataGaugeChart> DataChart { get; set; }
    // }

    public class DataGaugeChart
    {
        public string Category { get; set; }
        public long Value { get; set; }
        public long Full { get; set; }
    }

    public class DataPieChart
    {
        public string Category { get; set; }
        public long Value { get; set; }
    }

    public class EstadisticaUsoFormaValoradaGridResponse : vVerificacionDashboard
    {
        public string Aprobado
        {
            get
            {
                var title = "";
                var icon1 = "";
                var badge = "";
                if (EstatusPrueba == EstatusVerificacion.VerificacionAbortada)
                {
                    title = "Verificación Abortada";
                    icon1 = "bi bi-exclamation-circle text-white";
                    badge = "badge-danger";
                }
                else if (EstatusPrueba == EstatusVerificacion.VerificacionFinalizada || EstatusPrueba == EstatusVerificacion.FolioImpreso)
                {
                    title = "Verificación Finalizada";
                    icon1 = "bi bi-check-circle text-white";
                    badge = "badge-success";
                }
                else if (EstatusPrueba == 0 || EstatusPrueba == null)
                {
                    title = "Verificación No Iniciada";
                    icon1 = "bi bi-circle text-white";
                    badge = "badge-light";
                }
                else if (EstatusPrueba >= EstatusVerificacion.TerminaPruebaVisual)
                {
                    title = "Verificación En Proceso";
                    icon1 = "bi bi-slash-circle text-white";
                    badge = "badge-warning";
                }
                return $"<span class=\"badge {badge} py-3 px-4 fs-7\"  title=\"{title}\" >" +
                $"<i class=\"{icon1}\"></i>" +
                        "</span> ";
            }
        }

        public string TipoCertificadoString => EstatusPrueba != EstatusVerificacion.VerificacionAbortada && EstatusPrueba != EstatusVerificacion.VerificacionFinalizada && EstatusPrueba != EstatusVerificacion.FolioImpreso ? "-" : IdTipoCertificado != null ? TipoCertificado.DictNombreCertificado.FirstOrDefault(x => x.Key == IdTipoCertificado).Value.ToString() : "-";

        public bool VerificacionActiva => Vigencia.Date > DateTime.Now.Date;

        public TimeSpan DiffTime => (TimeSpan)((InicioPruebas != null && FinalizacionPruebas != null) ? FinalizacionPruebas - InicioPruebas : new TimeSpan());

        public string IntervaloMinutos => EstatusPrueba != EstatusVerificacion.VerificacionAbortada && EstatusPrueba != EstatusVerificacion.VerificacionFinalizada && EstatusPrueba != EstatusVerificacion.FolioImpreso ? "-" :
        (InicioPruebas != null && FinalizacionPruebas != null) ? DiffTime.Minutes == 1 ? string.Format("{0} minuto", DiffTime.Minutes.ToString()) : string.Format("{0} minutos", DiffTime.Minutes.ToString()) : "-";
    }

    public class EstadisiticaUsoFormaValoradaResponseGridAcciones : EstadisticaUsoFormaValoradaGridResponse
    {
        public string Acciones { get; set; }
    }

    public class VerificacionCVVRangoFecha
    {
        public long IdVerificentro { get; set; }
        public string Clave { get; set; }
        public int Aprobadas { get; set; }
        public int Rechazadas { get; set; }
        public int NoProcesadas { get; set; }
        public int Todas { get; set; }
    }
    public class CertificadoMapeo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        [JsonIgnore]
        public string Clave { get; set; }
        [JsonIgnore]
        public string Simbolo => TipoCertificado.DictNombreCertificadoSimbolo[Id];
        [JsonIgnore]
        public string ClassNames => TipoCertificado.ColorCertificado[Id];
        public int Total { get; set; }
    }
}
