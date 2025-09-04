using NPOI.SS.Formula.Functions;
using Smadot.Models.DataBase;

namespace Smadot.Models.Dicts
{
    public struct CapacitacionEstatus
    {
        public const int SolicitaCapacitacion = 1;
        public const int AutorizarCapacitacion = 2;
        public const int RechazarCapacitacion = 3;
    }
    public struct AlertaVerificacionEstatus
    {
        public const int SolicitaPermisoIngresoManual = 1;
        public const int ApruebaIngresoManual = 2;
        public const int RechazaIngresoManual = 3;
    }
    public struct AcreditacionEstatus
    {
        public const int SolicitaAcreditacion = 1;
        public const int AutorizarAcreditacion = 2;
        public const int RechazaAcreditacion = 3;
    }
    public struct CalibracionEstatus
    {
        public const int DocumentacionSinRevisar = 1;
        public const int DocumentacionRechazada = 2;
        public const int DocumentacionValidada = 3;
        public const int SolicitaModificar = 4;
        public const int RechazaModificar = 5;
        public const int PermiteModificar = 6;
        public const int PendienteCalibracion = 7;
        public const int CalibracionFallida = 8;
        public static List<string> Nombres = new()
        {
            "Documentación sin revisar","Rechazado DVRF", "Validado DVRF", "Solicitar Modificar", "Rechaza Modificar", "Permite Modificar",
            "Equipo pendiente de calibración","Calibración Fallida"
        };

    }
    public struct LineaEstatus
    {

        public const int Cierre = 2;
        public const int Apertura = 1;
    }
    public struct ProgramacionCalibracionEstatus
    {

        public const int Pendiente = 1;
        public const int Rechazado = 2;
        public const int Autorizado = 3;
    }
    public struct PuestoEstatus
    {

        public const int Pendiente = 1;
        public const int Autorizado = 2;
        public const int Rechazado = 3;
        public const int SolicitarModificar = 4;
        public const int RechazaModificar = 5;
        public const int PermiteModificar = 6;
    }
    public struct VerificacionIngresoManualEstatus
    {

        public const int Pendiente = 1;
        public const int Autorizado = 2;
        public const int Rechazado = 3;
    }
    public struct CitaEstatus
    {

        public const int SoloNotificacion = 0;

    }
    public struct MovimientosDicts
    {

        public static Dictionary<int, string> DictMovimientoCapacitacion = new()
        {
                            {CapacitacionEstatus.SolicitaCapacitacion,"Solicita Autorización Capacitación {0}" },
                            {CapacitacionEstatus.AutorizarCapacitacion,"Autorizó Capacitación {0}" },
                            {CapacitacionEstatus.RechazarCapacitacion,"Solicitud Capacitación {0} Rechazada" },
        };
        public static Dictionary<int, string> DictMovimientoAcreditacion = new()
        {
                            {AcreditacionEstatus.SolicitaAcreditacion,"Solicita Autorización Acreditación {0}" },
                            {AcreditacionEstatus.AutorizarAcreditacion,"Autorizó Acreditación {0}" },
                            {AcreditacionEstatus.RechazaAcreditacion,"Solicitud Acreditación {0} Rechazada" },
        };
        public static Dictionary<int, string> DictMovimientoCalibracion = new()
        {
                            { CalibracionEstatus.DocumentacionSinRevisar,"Documentación pendiente de revisar de la calibración de {0}. Número de calibración: {1}, Fecha de remisión: {2}" },
                            { CalibracionEstatus.DocumentacionValidada,"Valida la Documentación de la Calibración de {0}" },
                            { CalibracionEstatus.DocumentacionRechazada,"Rechaza la Documentación de la Calibración de {0}" },
                            { CalibracionEstatus.SolicitaModificar,"Solicita Modificar la Calibración número {0} de {1}. Fecha de calibración:{2} " },
                            { CalibracionEstatus.PermiteModificar,"Permite Modificar la Calibración número {0} de {1}. Fecha de registro de la calibracion: {2}" },
                            { CalibracionEstatus.RechazaModificar,"Rechaza Modificar la Calibración  número {0} de {1}." },
                            { CalibracionEstatus.PendienteCalibracion,"{0} Pendiente de Calibración {1}. Fecha programada: {2}" },
        };
        public static Dictionary<int, string> DictMovimientoLinea = new()
        {
                            { LineaEstatus.Cierre,"Cierre de {0}. Fecha: {1}" },
                            { LineaEstatus.Apertura,"Apertura de {0}. Fecha: {1}" },
        };
        public static Dictionary<int, string> DictMovimientoProgramacionCalibracion = new()
        {
                             { ProgramacionCalibracionEstatus.Rechazado,"Se rechaza el cambio de período de clalibración a {0} al {1}. Fecha Registro {2}"},
                             { ProgramacionCalibracionEstatus.Autorizado, "Se acepta el cambio de período de clalibración a {0} al {1}. Fecha Registro {2}" },
                             { ProgramacionCalibracionEstatus.Pendiente, "Se solicita el cambio en el período del {0} a {1}. Fecha Registro {2}" },
        };
        public static Dictionary<int, string> DictMovimientoPuesto = new Dictionary<int, string>
        {
                                 { PuestoEstatus.Pendiente,"Se {1} la Información del empleado {0}." },
                                 { PuestoEstatus.Autorizado,"Se aprueba la información del empleado {0}." },
                                 { PuestoEstatus.Rechazado,"Se rechaza la información del empleado {0}." },
                                 { PuestoEstatus.SolicitarModificar,"Solicita Modificar la información del empleado {0}." },
                                 { PuestoEstatus.RechazaModificar,"Rechaza Modificar la información del empleado {0}." },
                                 { PuestoEstatus.PermiteModificar,"Permite Modificar la información del empleado {0}." },
        };
        public static Dictionary<int, string> DictMovimientoVerificacion = new()
        {
                                 { VerificacionIngresoManualEstatus.Pendiente,"Se solicita la autorización en {0} para un ingreso manual de placas para el vehículo con placas {1} y vin {2}." },
                                 { VerificacionIngresoManualEstatus.Rechazado,"Se rechaza la solicitud de ingreso manual en {0} para el vehículo con placas {1} y vin {2}." },
                                 { VerificacionIngresoManualEstatus.Autorizado,"Se acepta la solicitud de ingreso manual en {0} para el vehículo con placas {1} y vin {2}." },
        };

        public static Dictionary<int, string> DictAlertaCita = new()
        {
                                 { CitaEstatus.SoloNotificacion,"Rechazo de la documentación del Vehículo con Placas {0}, VIN {1}, marca {2} y modelo {3} debido a falta de información en la tabla maestra de la Secreataría de Planeación y Finanzas en el {4}. La solicitud se hizo a nombre de {5} y correo de contacto {6}" },
        };

        public static Dictionary<int, string> DictAlertaReporteMensual = new()
        {
                                 { CatEstatusReporteDic.EnviadoDVRF,"Se ha generado el reporte mensual número {0} correspondiente al mes de {1} del año {2}." },
                                 { CatEstatusReporteDic.RechazadoDVRF,"Se rechaza el reporte mensual número {0} correspondiente al mes de {1} del año {2}." },
                                 { CatEstatusReporteDic.ValidadoDVRF,"Se valida el reporte mensual número {0} correspondiente al mes de {1} del año {2}." },
                                 { CatEstatusReporteDic.RechazaModificar,"Se rechaza la modificación del reporte mensual número {0} correspondiente al mes de {1} del año {2}." },
                                 { CatEstatusReporteDic.PermiteModificar,"Se permite la modificación del reporte mensual número {0} correspondiente al mes de {1} del año {2}." },
                                 { CatEstatusReporteDic.SolicitaModificar,"Se ha solicitado modificar el reporte mensual número {0} correspondiente al mes de {1} del año {2}." },
        };

    }
    public struct DictAlertas
    {
        public const string Circular = "Circular";
        public const string Linea = "Linea";
        public const string Capacitacion = "Capacitacion";
        public const string Reportes = "Reportes";
        public const string Calibracion = "Calibracion";
        public const string Equipo = "Equipo";
        public const string ConfiguradorCita = "ConfiguradorCita";
        public const string UserPuestoVerificentro = "UserPuestoVerificentro";
        public const string EquipoTipoCalibracion = "EquipoTipoCalibracion";
        public const string Verificacion = "Verificacion";
        public const string Cita = "Cita";
        public const string ConfiguradorCitas = "ConfiguradorCita";
        public const string ActualizacionPrueba = "ActualizacionPrueba";
        public static readonly Dictionary<string, string> EquivAlertaTitle = new()
        {
            {Circular,"Circular" },
            {Cita,"Documentación de la Cita" },
            {ConfiguradorCitas,"Configurador de Citas" },
            {Linea,"Línea" },
            {Capacitacion,"Capcitación" },
            {Reportes,"Reportes" },
            {Calibracion,"Calibración" },
            {Equipo,"Equipo" },
            {UserPuestoVerificentro,"Cambio de Puesto" },
            {EquipoTipoCalibracion,"Cambio Frecuencia Calibración" },
            {Verificacion,"Verificación" }
        };
    }
}
