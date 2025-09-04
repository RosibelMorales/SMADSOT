using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos.Documentos
{
    public class EstadisticasDashboardGridDocumentResponse
    {
        public long Id { get; set; }

        public string Combustible { get; set; } = null!;

        public string Serie { get; set; } = null!;

        public string Placa { get; set; } = null!;

        public int Modelo { get; set; }

        public string Marca { get; set; } = null!;

        public string SubMarca { get; set; } = null!;

        public string TarjetaCirculacion { get; set; } = null!;

        public string FolioCertificado { get; set; } = null!;

        public DateTime FechaVerificacion { get; set; }

        public DateTime Vigencia { get; set; }

        public string NombreVerificentro { get; set; } = null!;

        public string MotivoVerificacion { get; set; } = null!;

        public int IdCatTipoServicio { get; set; }

        public int IdMotivoVerificacion { get; set; }

        public string TipoServicio { get; set; } = null!;

        public string? CausaRechazo { get; set; }

        public bool CambioPlacas { get; set; }

        public DateTime? FechaFacturacion { get; set; }

        public int NoIntentos { get; set; }

        public bool? SinMulta { get; set; }

        public long IdVerificacion { get; set; }

        public int IdCatMarcaVehiculo { get; set; }

        public int IdCatSubmarcaVehiculo { get; set; }

        public int Motor_DSL { get; set; }

        public int COMB_ORIG { get; set; }

        public int CARROCERIA { get; set; }

        public int ALIM_COMB { get; set; }

        public int CILINDROS { get; set; }

        public int CILINDRADA { get; set; }

        public int PBV { get; set; }

        public int PBV_EQUIV { get; set; }

        public int PBV_ASM { get; set; }

        public int CONV_CATAL { get; set; }

        public int OBD { get; set; }

        public int C_ABS { get; set; }

        public int T_TRACC { get; set; }

        public int C_TRACC { get; set; }

        public int T_PRUEBA { get; set; }

        public int PROTOCOLO { get; set; }

        public int POTMAX_RPM { get; set; }

        public int ANO_DESDE { get; set; }

        public int ANO_HASTA { get; set; }

        public decimal O2_MAX { get; set; }

        public decimal LAMDA_MAX { get; set; }

        public decimal POT_5024 { get; set; }

        public decimal POT_2540 { get; set; }

        public int DOBLECERO { get; set; }

        public int CERO_GASOL { get; set; }

        public int CERO_GASLP { get; set; }

        public int CERO_GASNC { get; set; }

        public int CERO_DSL { get; set; }

        public int REF_00 { get; set; }

        public DateTime? InicioPruebas { get; set; }

        public DateTime? FinalizacionPruebas { get; set; }

        public int? EstatusPrueba { get; set; }

        public bool? FugasSistemaEscape { get; set; }

        public bool? PortafiltroAire { get; set; }

        public bool? TaponDispositivoAceite { get; set; }

        public bool? FiltroAire { get; set; }

        public bool? TaponCombustible { get; set; }

        public bool? Bayoneta { get; set; }

        public bool? FugaAceiteMotor { get; set; }

        public bool? FugaAceiteTransmision { get; set; }

        public bool? FugaLiquidoRefrigerante { get; set; }

        public bool? DibujoNeumaticos { get; set; }

        public bool? DesperfectosNeumaticos { get; set; }

        public bool? DimensionesNeumaticoIncorrectas { get; set; }

        public bool? ControlEmisionDesconectados { get; set; }

        public bool? ControlEmisionAlterados { get; set; }

        public bool? PlacasCorrespondientes { get; set; }

        public bool? GobernadorBuenEstado { get; set; }

        public int? NumeroEscapes { get; set; }

        public string? Etapa { get; set; }

        public int? SPS_Humo { get; set; }

        public int? SPS_5024 { get; set; }

        public int? SPS_2540 { get; set; }

        public int? HC { get; set; }

        public decimal? CO { get; set; }

        public decimal? CO2 { get; set; }

        public decimal? O2 { get; set; }

        public int? NO { get; set; }

        public decimal? LAMDA { get; set; }

        public decimal? FCNOX { get; set; }

        public decimal? FCDIL { get; set; }

        public int? RPM { get; set; }

        public decimal? KPH { get; set; }

        public decimal? VEL_LIN { get; set; }

        public decimal? VEL_ANG { get; set; }

        public decimal? BHP { get; set; }

        public decimal? PAR_TOR { get; set; }

        public decimal? FUERZA { get; set; }

        public decimal? POT_FRENO { get; set; }

        public int? TEMP { get; set; }

        public int? PRESION { get; set; }

        public int? HUMREL { get; set; }

        public string? OBD_TIPO_SDB { get; set; }

        public int? OBD_MIL { get; set; }

        public int? OBD_CATAL { get; set; }

        public int? OBD_CILIN { get; set; }

        public int? OBD_COMBU { get; set; }

        public int? OBD_INTEG { get; set; }

        public int? OBD_OXIGE { get; set; }

        public decimal? LAMDA_5024 { get; set; }

        public decimal? TEMP_5024 { get; set; }

        public decimal? HR_5024 { get; set; }

        public int? PSI_5024 { get; set; }

        public decimal? FCNOX_5024 { get; set; }

        public decimal? FCDIL_5024 { get; set; }

        public int? RPM_5024 { get; set; }

        public decimal? KPH_5024 { get; set; }

        public decimal? THP_5024 { get; set; }

        public decimal? VOLTS_5024 { get; set; }

        public int? HC_5024 { get; set; }

        public decimal? CO_5024 { get; set; }

        public decimal? CO2_5024 { get; set; }

        public decimal? COCO2_5024 { get; set; }

        public decimal? O2_5024 { get; set; }

        public int? NO_5024 { get; set; }

        public decimal? LAMDA_2540 { get; set; }

        public decimal? TEMP_2540 { get; set; }

        public decimal? HR_2540 { get; set; }

        public int? PSI_2540 { get; set; }

        public decimal? FCNOX_2540 { get; set; }

        public decimal? FCDIL_2540 { get; set; }

        public int? RPM_2540 { get; set; }

        public decimal? KPH_2540 { get; set; }

        public decimal? THP_2540 { get; set; }

        public decimal? VOLTS_2540 { get; set; }

        public int? HC_2540 { get; set; }

        public decimal? CO_2540 { get; set; }

        public decimal? CO2_2540 { get; set; }

        public decimal? COCO2_2540 { get; set; }

        public decimal? O2_2540 { get; set; }

        public int? NO_2540 { get; set; }

        public decimal? OPACIDADP { get; set; }

        public decimal? OPACIDADK { get; set; }

        public int? TEMP_MOT { get; set; }

        public int? VEL_GOB { get; set; }

        public int? TEMP_GAS { get; set; }

        public int? TEMP_CAM { get; set; }

        public int? RESULTADO { get; set; }

        public int? C_RECHAZO { get; set; }

        public int? C_RECHAZO_OBD { get; set; }

        public bool? PruebaObd { get; set; }

        public bool? PruebaEmisiones { get; set; }

        public bool? PruebaOpacidad { get; set; }

        public string EntidadProcedencia { get; set; } = null!;

        public int? IdTipoCertificado { get; set; }

        public long FolioCertificadoActual { get; set; }

        public string ClaveTramite { get; set; } = null!;

        public long IdVerificentro { get; set; }

        public string? NombrePropietario { get; set; }

        public string UrlRoot { get; set; } = null!;
        public string TipoCertificadoString => EstatusPrueba != EstatusVerificacionUtilities.VerificacionAbortada && EstatusPrueba != EstatusVerificacionUtilities.VerificacionFinalizada && EstatusPrueba != EstatusVerificacionUtilities.FolioImpreso ? "-" : IdTipoCertificado != null ? TipoCertificadoUtil.DictNombreCertificado.FirstOrDefault(x => x.Key == IdTipoCertificado).Value.ToString() : "-";

        public bool VerificacionActiva => Vigencia.Date > DateTime.Now.Date;

        public TimeSpan DiffTime => (TimeSpan)((InicioPruebas != null && FinalizacionPruebas != null) ? FinalizacionPruebas - InicioPruebas : new TimeSpan());

        public string IntervaloMinutos => EstatusPrueba != EstatusVerificacionUtilities.VerificacionAbortada && EstatusPrueba != EstatusVerificacionUtilities.VerificacionFinalizada && EstatusPrueba != EstatusVerificacionUtilities.FolioImpreso ? "-" :
        (InicioPruebas != null && FinalizacionPruebas != null) ? DiffTime.Minutes == 1 ? string.Format("{0} minuto", DiffTime.Minutes.ToString()) : string.Format("{0} minutos", DiffTime.Minutes.ToString()) : "-";
    }

    public struct CausaRechazoUtil
    {
        public static Dictionary<int, string> DictClaveRechazo = new Dictionary<int, string>
                        {
                            {0, "No Aplica" },
                            {1, "Certificado Anterior Robado" },
                            {2, "Visual" },
                            {3, "Humo visible" },
                            {4, "rpm" },
                            {5, "Vehículo Fuera de especificaciones mecánicas" },
                            {6, "Dilución" },
                            {7, "Emisiones" },
                            {8, "Lamda durante la prueba." },
                            {10, "Falla del equipo analizador." },
                            {11, "OBD" },
                            {12, "Prueba Abortada por el Técnico Verificador" },
                        };
        public const int NoAplica = 0;
        public const int CertificadoAnteriorRobado = 1;
        public const int Visual = 2;
        public const int HumoCisible = 3;
        public const int Rpm = 4;
        public const int VehículoFueraEspecificacionesMecanicas = 5;
        public const int Dilucion = 6;
        public const int Emisiones = 7;
        public const int LamdaDurantePrueba = 8;
        public const int FallaEquipoAnalizador = 10;
        public const int OBD = 11;
        public const int AbortadaTecnico = 12;

    }

    public struct ResultadosUtil
    {
        public static Dictionary<int, string> DictResultados = new Dictionary<int, string>
                       {
                            {0, "Escapado" },
                            {1, "Tipo 0" },
                            {2, "Tipo 1" },
                            {3, "Tipo 2" },
                            {4, "Tipo 00" },
                            {5, "Rechazo" },
                       };


        public const int Escapado = 0;
        public const int Cero = 1;
        public const int Uno = 2;
        public const int Dos = 3;
        public const int DobleCero = 4;
        public const int Rechazo = 5;
    }

    public class TipoCertificadoUtil
    {
        public const int DobleCero = 1;
        public const int Cero = 2;
        public const int Uno = 3;
        public const int Dos = 4;
        public const int ConstanciasNoAprobado = 5;
        public const int Exentos = 6;
        public const int Testificacion = 7;

        public List<object> NombreCertificado = new List<object>()
        {
            "DOBLECERO","CERO","UNO","DOS","CONSTANCIASNOAPROBADO","EXENTOS", "TESTIFICACION"
        };
        public static Dictionary<int, string> DictNombreCertificado = new()
        {
            { DobleCero,"Tipo 00"},
            { Cero ,"Tipo 0"},
            { Uno ,"Tipo 1"},
            { Dos ,"Tipo 2"},
            { ConstanciasNoAprobado ,"Tipo CNA"},
            { Exentos ,"Tipo Exento"},
            { Testificacion ,"Tipo Testificación"},
        };


    }

}
