using Smadot.Models.DataBase;

namespace Smadot.Models.Dicts
{
    public class PermisosMenu
    {
        public const long SolicitudesdeFormasValoradas = 1;
        public const long IngresodeFormasValoradas = 2;
        public const long AsignacionStockdeVentanilla = 3;
        public const long DevolucionaSPF = 4;
        public const long FoliosCancelados = 5;
        public const long DeterminaciondeStockMinimo = 6;
        public const long ConsultaStockDVRF = 7;
        public const long FoliosUsadosenVentanilla = 8;
        public const long FoliosVendidosaCentrosdeVerificacion = 9;
        public const long FoliosRegresadosaSPF = 10;
        public const long ConsultadeCirculares = 11;
        public const long Dashboard = 12;
        public const long DashboardLineas = 13;
        public const long VentadeFormasValoradasaCVV = 14;
        public const long Reposiciones = 15;
        public const long Exentos = 16;
        public const long Administrativas = 17;
        public const long ConstanciasdeUltimaVerificacion = 18;
        public const long ConsultadePruebasdeVerificacion = 19;
        public const long ConsultadeTablaMaestra = 20;
        public const long DirectoriodeVerificentros = 21;
        public const long AccesoaCamarasCVV = 22;
        public const long InventariodeEquipos = 23;
        public const long Calibraciones = 24;
        public const long Proveedores = 25;
        public const long Laboratorios = 26;
        public const long ProveedoresFolio = 27;
        public const long OrdenesdeServicio = 28;
        public const long Instalaciones = 29;
        public const long UsodeSuministros = 30;
        public const long Lineas = 31;
        public const long ReporteMensual = 32;
        public const long ReporteTercerTanto = 33;
        public const long Personal = 34;
        public const long Acreditaciones = 35;
        public const long Capacitaciones = 36;
        public const long ReposiciondeCredenciales = 37;
        public const long AutorizacionesyValidacionesDVRF = 38;
        public const long Roles = 39;
        public const long Usuarios = 40;
        public const long Permisos = 41;
        public const long Configuracion = 42;
        public const long AlertasNotificacion = 43;
        public const long RecepcionDocumentos = 44;
        public const long Impresiones = 45;
        public const long HistorialCitas = 46;
        public const long Testificacion = 47;
        public const long ConfiguradorCita = 48;

    }
    public class MenuSolicitudesFormasValoradas
    {
        public const long Registrar = 1001;
        public const long Editar = 1002;
        public const long PermisoMenuPadre = PermisosMenu.SolicitudesdeFormasValoradas;
    }
    public class MenuIngresoFormasValoradas
    {
        public const long RegistrarIngreso = 2001;
        public const long RegistrarCajas = 2002;
        public const long PermisoMenuPadre = PermisosMenu.IngresodeFormasValoradas;
    }
    public class MenuAsignacionStockdeVentanilla
    {
        public const long Registrar = 3001;
        public const long Detalle = 3002;
        public const long PermisoMenuPadre = PermisosMenu.AsignacionStockdeVentanilla;
    }
    public class MenuDevolucionaSPF
    {
        public const long Registrar = 4001;
        public const long Detalle = 4002;
        public const long Exportar = 4003;
        public const long PermisoMenuPadre = PermisosMenu.DevolucionaSPF;
    }
    public class MenuFoliosCancelados
    {
        public const long Registrar = 5001;
        public const long PermisoMenuPadre = PermisosMenu.FoliosCancelados;
    }
    public class MenuDeterminacionStockMinimo
    {
        public const long Registrar = 6001;
        public const long PermisoMenuPadre = PermisosMenu.DeterminaciondeStockMinimo;
    }
    public class MenuConsultaStockDVRF
    {
        public const long Exportar = 7001;
        public const long PermisoMenuPadre = PermisosMenu.ConsultaStockDVRF;
    }
    public class MenuFoliosUsadosVentanilla
    {
        public const long Exportar = 8001;
        public const long Detalle = 8002;
        public const long PermisoMenuPadre = PermisosMenu.FoliosUsadosenVentanilla;
    }
    public class MenuFoliosVendidosCentrosdeVerificacion
    {
        public const long Exportar = 9001;
        public const long Detalle = 9002;
        public const long PermisoMenuPadre = PermisosMenu.FoliosVendidosaCentrosdeVerificacion;
    }
    public class MenuFoliosRegresadosSPF
    {
        public const long Exportar = 10001;
        public const long Detalle = 10002;
        public const long PermisoMenuPadre = PermisosMenu.FoliosRegresadosaSPF;
    }
    public class MenuConsultaCirculares
    {
        public const long Registrar = 11001;
        public const long Detalle = 11002;
        public const long PermisoMenuPadre = PermisosMenu.ConsultadeCirculares;
    }
    public class MenuDashboard
    {
        public const long PermisoMenuPadre = PermisosMenu.Dashboard;
    }
    public class MenuDashboardLineas
    {
        public const long PermisoMenuPadre = PermisosMenu.DashboardLineas;
    }
    public class MenuVentaFormasValoradasCVV
    {
        public const long Registrar = 14001;
        public const long Detalle = 14002;
        public const long GenerarRecibo = 14003;
        public const long PermisoMenuPadre = PermisosMenu.VentadeFormasValoradasaCVV;
    }
    public class MenuReposiciones
    {
        public const long Registrar = 15001;
        public const long Detalle = 15002;
        public const long GenerarCertificado = 15003;
        public const long Eliminar = 15004;
        public const long PermisoMenuPadre = PermisosMenu.Reposiciones;
    }
    public class MenuExentos
    {
        public const long Registrar = 16001;
        public const long Detalle = 16002;
        public const long Refrendar = 16003;
        public const long DetalleRefrendo = 16004;
        public const long CertificadoRefrendo = 16005;
        public const long CertificadoExento = 16006;
        public const long Eliminar = 16007;
        public const long PermisoMenuPadre = PermisosMenu.Exentos;
    }
    public class MenuAdministrativas
    {
        public const long Registrar = 17001;
        public const long Detalle = 17002;
        public const long GenerarCertificado = 17003;
        public const long Eliminar = 17004;
        public const long PermisoMenuPadre = PermisosMenu.Administrativas;
    }
    public class MenuTestificacion
    {
        public const long Registrar = 47001;
        public const long Detalle = 47002;
        public const long GenerarCertificado = 47003;
        public const long Eliminar = 47004;
        public const long PermisoMenuPadre = PermisosMenu.Testificacion;
    }
    public class MenuConstanciasUltimaVerificacion
    {
        public const long Registrar = 18001;
        public const long Detalle = 18002;
        public const long GenerarOficio = 18003;
        public const long PermisoMenuPadre = PermisosMenu.ConstanciasdeUltimaVerificacion;
    }
    public class MenuConsultaPruebasVerificacion
    {
        public const long PermisoMenuPadre = PermisosMenu.ConsultadePruebasdeVerificacion;
    }
    public class MenuConsultaTablaMaestra
    {
        public const long Exportar = 20001;
        public const long Actualizar = 20002;
        public const long Editar = 20003;
        public const long PermisoMenuPadre = PermisosMenu.ConsultadeTablaMaestra;
    }
    public class MenuDirectorioVerificentros
    {
        public const long Detalles = 21001;
        public const long ConfiguradordeCitas = 21002;
        public const long DiasnoLaborales = 21003;
        public const long Registrar = 21004;
        public const long Editar = 21005;


        public const long ConfiguradordeCitasCargarHorario = 21201;
        public const long ConfiguradordeCitasEditarHorario = 21202;
        public const long PermisoMenuPadre = PermisosMenu.DirectoriodeVerificentros;
    }
    public class MenuAccesoCamarasCVV
    {
        public const long PermisoMenuPadre = PermisosMenu.AccesoaCamarasCVV;
    }
    public class MenuInventarioEquipos
    {
        public const long Registrar = 23001;
        public const long ProgramaciondeCalibracion = 23002;
        public const long Detalle = 23003;
        public const long Activar = 23004;
        public const long Rechazar = 23005;
        public const long Desactivar = 23006;
        public const long Editar = 23007;

        public const long ProgramaciondeCalibracionRegistrar = 23101;
        public const long ProgramaciondeCalibracionDetalle = 23102;
        public const long ProgramaciondeCalibracionValidar = 23103;
        public const long ProgramaciondeCalibracionRechazar = 23104;

        public const long PermisoMenuPadre = PermisosMenu.InventariodeEquipos;
    }
    public class MenuCalibraciones
    {
        public const long Registrar = 24001;
        public const long Modificar = 24002;
        public const long Detalle = 24003;
        public const long PermisoMenuPadre = PermisosMenu.Calibraciones;
    }
    public class MenuProveedores
    {
        public const long Registrar = 25001;
        public const long Detalle = 25002;
        public const long Autorizar = 25003;
        public const long Desautorizar = 25004;
        public const long PermisoMenuPadre = PermisosMenu.Proveedores;
    }
    public class MenuLaboratorios
    {
        public const long Registrar = 26001;
        public const long Detalle = 26002;
        public const long Autorizar = 26003;
        public const long Desautorizar = 26004;
        public const long PermisoMenuPadre = PermisosMenu.Laboratorios;
    }
    public class MenuProveedoresFolio
    {
        public const long Registrar = 27001;
        public const long Detalle = 27002;
        public const long PermisoMenuPadre = PermisosMenu.ProveedoresFolio;
    }
    public class MenuOrdenesServicio
    {
        public const long Registrar = 28001;
        public const long Detalle = 28002;
        public const long PermisoMenuPadre = PermisosMenu.OrdenesdeServicio;
    }
    public class MenuInstalaciones
    {
        public const long Registrar = 29001;
        public const long Detalle = 29002;
        public const long PermisoMenuPadre = PermisosMenu.Instalaciones;
    }
    public class MenuUsoSuministros
    {
        public const long Registrar = 30001;
        public const long Detalle = 30002;
        public const long PermisoMenuPadre = PermisosMenu.UsodeSuministros;
    }
    public class MenuLineas
    {
        public const long Registrar = 31001;
        public const long Detalle = 31002;
        public const long Abrir = 31003;
        public const long Cerrar = 31004;
        public const long PermisoMenuPadre = PermisosMenu.Lineas;
    }
    public class MenuReporteMensual
    {
        public const long Nuevo = 32001;
        public const long Detalle = 32002;
        public const long Editar = 32003;
        public const long PermiteModificar = 32004;
        public const long RechazaModificar = 32005;
        public const long ValidadoDVRF = 32006;
        public const long RechazadoDVRF = 32007;
        public const long PermisoMenuPadre = PermisosMenu.ReporteMensual;
    }
    public class MenuReporteTercerTanto
    {
        public const long Registrar = 33001;
        public const long Detalle = 33002;
        public const long PermisoMenuPadre = PermisosMenu.ReporteTercerTanto;
    }
    public class MenuPersonal
    {
        public const long Registrar = 34001;
        public const long Editar = 34002;
        public const long Detalle = 34003;
        public const long Movimiento = 34004;
        public const long Baja = 34005;
        public const long AprobarDVRF = 34006;
        public const long RechazarDVRF = 34007;
        public const long PermiteModificar = 34008;
        public const long RechazaModificar = 34009;
        public const long PermisoMenuPadre = PermisosMenu.Personal;
    }
    public class MenuAcreditaciones
    {
        public const long Registrar = 35001;
        public const long Autorizar = 35002;
        public const long Rechazar = 35003;
        public const long Evidencia = 35004;
        public const long PermisoMenuPadre = PermisosMenu.Acreditaciones;
    }
    public class MenuCapacitaciones
    {
        public const long Registrar = 36001;
        public const long Detalle = 36002;
        public const long Autorizar = 36003;
        public const long Rechazar = 36004;
        public const long SubirFoto = 36005;
        public const long SubirEvaluacion = 36006;
        public const long PermisoMenuPadre = PermisosMenu.Capacitaciones;
    }
    public class MenuReposicionCredenciales
    {
        public const long Registrar = 37001;
        public const long Detalle = 37002;
        public const long PermisoMenuPadre = PermisosMenu.ReposiciondeCredenciales;
    }
    public class MenuAutorizacionesValidacionesDVRF
    {
        public const long Detalle = 38001;
        public const long Atender = 38002;
        public const long PermisoMenuPadre = PermisosMenu.AutorizacionesyValidacionesDVRF;
    }
    public class MenuRoles
    {
        public const long Registrar = 39001;
        public const long Editar = 39002;
        public const long PermisoMenuPadre = PermisosMenu.Roles;
    }
    public class MenuUsuarios
    {
        public const long Registrar = 40001;
        public const long Editar = 40002;
        public const long ActivarDesactivar = 40003;
        public const long PermisoMenuPadre = PermisosMenu.Usuarios;
    }
    public class MenuPermisos
    {
        public const long Registrar = 41001;
        public const long PermisoMenuPadre = PermisosMenu.Permisos;
    }
    public class MenuConfiguracion
    {
        public const long ModificarImportes = 42001;
        public const long CargarFolios = 42002;
        public const long PermisoMenuPadre = PermisosMenu.Configuracion;
    }
    public class MenuRecepcionDocumentos
    {
        public const long RegistroRecepcionDocs = 44001;
        public const long ReagendarCita = 44002;
        public const long CambiarLinea = 44003;
        public const long CalcularResultados = 44004;

        public const long PermisoMenuPadre = PermisosMenu.RecepcionDocumentos;
    }
    public class MenuImpresiones
    {
        public const long RecalcularFolio = 45001;
        public const long PermisoMenuPadre = PermisosMenu.Impresiones;
    }
    public class ConfiguradorCita
    {
        public const long PermisoMenuPadre = PermisosMenu.ConfiguradorCita;
    }
    public class MenuHistorialCitas
    {
        public const long Expediente = 46001;
        public const long GenerarFolio = 46002;
        public const long ReimpresionFolio = 46003;
        public const long ReimpresionCertificado = 46004;
        public const long ReiniciarCita = 46005;
        public const long PermisoMenuPadre = PermisosMenu.HistorialCitas;
    }
    public class MenuAlertas
    {
        public const long Calibraciones = 43001;
        public const long Lineas = 43002;
        public const long Capacitaciones = 43003;
        public const long Acreditaciones = 43004;
        public const long Equipos = 43005;
        public const long CambioPuesto = 43006;
        public const long Verificaciones = 43007;
        public const long ProgramacionCalibracion = 43008;
        public const long ConfiguradorCitas = 43009;
        public static readonly Dictionary<int, string> DiccionarioAlertasPermiso = new Dictionary<int, string>()
        {
            { (int)Calibraciones,DictAlertas.Calibracion },
            { (int)Lineas,DictAlertas.Linea},
            { (int)Capacitaciones,DictAlertas.Capacitacion},
            { (int)Acreditaciones,DictAlertas.Reportes},
            { (int)Equipos,DictAlertas.Equipo},
            { (int)CambioPuesto,DictAlertas.UserPuestoVerificentro},
            { (int)Verificaciones,DictAlertas.Verificacion},
            { (int)ProgramacionCalibracion,DictAlertas.EquipoTipoCalibracion},
            { (int)ConfiguradorCitas,DictAlertas.ConfiguradorCitas}
        };
    }
}
