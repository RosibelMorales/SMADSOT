using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVerificacionDatosLimite
{
    public long Id { get; set; }

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

    public long IdVerificacion { get; set; }

    public string Modelo { get; set; } = null!;

    public string Placa { get; set; } = null!;

    public string TarjetaCirculacion { get; set; } = null!;

    public string Serie { get; set; } = null!;

    public int Anio { get; set; }

    public DateTime Fecha { get; set; }

    public long? IdUserTecnico { get; set; }

    public string? UserName { get; set; }

    public long IdLinea { get; set; }

    public string Clave { get; set; } = null!;

    public long IdVerificentro { get; set; }

    public int? RAL_FAB { get; set; }

    public int? GOB_FAB { get; set; }

    public int? ClaveCombustible { get; set; }

    public int? RPM_GOB { get; set; }

    public long? IdResultadosVerificacion { get; set; }

    public DateTime? InicioPruebas { get; set; }

    public DateTime? FinalizacionPruebas { get; set; }

    public int? EstatusPrueba { get; set; }

    public bool? FugasSistemaEscape { get; set; }

    public bool? PortafiltroAire { get; set; }

    public bool? FiltroAire { get; set; }

    public bool? TaponDispositivoAceite { get; set; }

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

    public int? POTMIN_RPM { get; set; }

    public int? RPOTMAX_RPM { get; set; }

    public int? TEMP_GAS { get; set; }

    public int? TEMP_CAM { get; set; }

    public int? RESULTADO { get; set; }

    public int? C_RECHAZO { get; set; }

    public int? C_RECHAZO_OBD { get; set; }

    public bool? PruebaObd { get; set; }

    public bool? PruebaEmisiones { get; set; }

    public bool? PruebaOpacidad { get; set; }

    public string? URLEvidenciaFalla { get; set; }

    public string? Propietario { get; set; }

    public string? RazonSocial { get; set; }

    public int? MarcaProveedor { get; set; }

    public DateTime? FechaCita { get; set; }

    public string? CorreoPropietario { get; set; }

    public string? Estado { get; set; }

    public int? IdTipoCombustible { get; set; }

    public string? URLFotoTecnico { get; set; }

    public string? URLPlacaCamara { get; set; }

    public string? URLPlacaDelantera { get; set; }

    public string? URLPlacaTrasera { get; set; }

    public string? NombreTecnico { get; set; }

    public long? FolioFormaValorada { get; set; }

    public DateTime Vigencia { get; set; }

    public bool IngresoManual { get; set; }

    public string Marca { get; set; } = null!;

    public int? IdMotivoVerificacion { get; set; }

    public DateTime? FechaFacturacion { get; set; }

    public string? URLIdentificacion { get; set; }

    public string? URLFactura { get; set; }

    public string? URLCartaFactura { get; set; }

    public string? URLValidacionCertificado { get; set; }

    public string? URLBajaPlacas { get; set; }

    public string? URLAltaPlacas { get; set; }

    public bool? CambioPlacas { get; set; }

    public int? IdCatTipoServicio { get; set; }

    public long? IdLimiteVerificacionParametro { get; set; }
}
