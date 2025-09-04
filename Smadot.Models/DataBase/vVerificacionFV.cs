using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVerificacionFV
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

    public long IdCicloVerificacion { get; set; }

    public long IdVerificacion { get; set; }

    public string Modelo { get; set; } = null!;

    public string Placa { get; set; } = null!;

    public string TarjetaCirculacion { get; set; } = null!;

    public string Serie { get; set; } = null!;

    public int Anio { get; set; }

    public DateTime Fecha { get; set; }

    public long? IdUserTecnico { get; set; }

    public string? Username { get; set; }

    public long IdLinea { get; set; }

    public string Clave { get; set; } = null!;

    public long IdVerificentro { get; set; }

    public int RAL_FAB { get; set; }

    public int GOB_FAB { get; set; }

    public int ClaveCombustible { get; set; }

    public long? IdResultadosVerificacion { get; set; }

    public DateTime? InicioPruebas { get; set; }

    public DateTime? FinalizacionPruebas { get; set; }

    public int? EstatusPrueba { get; set; }

    public int? NumeroEscapes { get; set; }

    public string? Etapa { get; set; }

    public int? SPS_Humo { get; set; }

    public int? SPS_5024 { get; set; }

    public int? SPS_2540 { get; set; }

    public string? OBD_TIPO_SDB { get; set; }

    public int? OBD_MIL { get; set; }

    public int? OBD_CATAL { get; set; }

    public int? OBD_CILIN { get; set; }

    public int? OBD_COMBU { get; set; }

    public int? OBD_INTEG { get; set; }

    public int? OBD_OXIGE { get; set; }

    public int? LAMDA_5024 { get; set; }

    public int? TEMP_5024 { get; set; }

    public int? HR_5024 { get; set; }

    public int? PSI_5024 { get; set; }

    public int? FCNOX_5024 { get; set; }

    public int? FCDIL_5024 { get; set; }

    public int? RPM_5024 { get; set; }

    public int? KPH_5024 { get; set; }

    public int? THP_5024 { get; set; }

    public int? VOLTS_5024 { get; set; }

    public int? HC_5024 { get; set; }

    public int? CO_5024 { get; set; }

    public int? CO2_5024 { get; set; }

    public int? COCO2_5024 { get; set; }

    public int? O2_5024 { get; set; }

    public int? NO_5024 { get; set; }

    public int? LAMDA_2540 { get; set; }

    public int? TEMP_2540 { get; set; }

    public int? HR_2540 { get; set; }

    public int? PSI_2540 { get; set; }

    public int? FCNOX_2540 { get; set; }

    public int? FCDIL_2540 { get; set; }

    public int? RPM_2540 { get; set; }

    public int? KPH_2540 { get; set; }

    public int? THP_2540 { get; set; }

    public int? VOLTS_2540 { get; set; }

    public int? HC_2540 { get; set; }

    public int? CO_2540 { get; set; }

    public int? CO2_2540 { get; set; }

    public int? COCO2_2540 { get; set; }

    public int? O2_2540 { get; set; }

    public int? NO_2540 { get; set; }

    public int? OPACIDADP { get; set; }

    public int? OPACIDADK { get; set; }

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

    public string? Propietario { get; set; }

    public DateTime? FechaCita { get; set; }

    public string? CorreoPropietario { get; set; }

    public string? NombreTecnico { get; set; }

    public long? FolioFormaValorada { get; set; }

    public DateTime Vigencia { get; set; }

    public string Marca { get; set; } = null!;
}
