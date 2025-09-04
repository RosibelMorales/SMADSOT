using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class LogPruebaVerificacion
{
    public long Id { get; set; }

    public long? IdVerificacion { get; set; }

    public DateTime? FechaUltimaModificacion { get; set; }

    public string? NumSerie { get; set; }

    public string? Placa { get; set; }

    public int? Combustible { get; set; }

    public DateTime? Fecha { get; set; }

    public string? NumeroConstanciaOtorgada { get; set; }

    public string? ClaveCentro { get; set; }

    public string? RazonSocial { get; set; }

    public string? Propietario { get; set; }

    public string? TecnicoVerificador { get; set; }

    public string? Linea { get; set; }

    public int? IdEstatus { get; set; }

    public int? SubMarca { get; set; }

    public int? Marca { get; set; }

    public int? Anio { get; set; }

    public decimal? Pot_5024 { get; set; }

    public decimal? Pot_2540 { get; set; }

    public int? RPM_GOB { get; set; }

    public int? Protocolo { get; set; }

    public int? Cilindros { get; set; }

    public int? Cilindrada { get; set; }

    public int? Motor_DSL { get; set; }

    public int? PBV { get; set; }

    public int? PBV_Equivalente { get; set; }

    public int? PBV_ASM { get; set; }

    public int? C_ABS { get; set; }

    public int? ConvertidorCatalitico { get; set; }

    public int? ClaveCombustible { get; set; }

    public int? Combustible_POTMAX_RPM { get; set; }

    public int? RAL_FAB { get; set; }

    public int? GOB_FAB { get; set; }

    public long? OBD { get; set; }

    public long? DobleCero { get; set; }

    public long? CERO_GASOL { get; set; }

    public long? CERO_GASLP { get; set; }

    public long? CERO_GASNC { get; set; }

    public long? CERO_DSL { get; set; }

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

    public int? POTMAX_RPM { get; set; }

    public int? TEMP_GAS { get; set; }

    public int? TEMP_CAM { get; set; }

    public int? RESULTADO { get; set; }

    public int? C_RECHAZO { get; set; }

    public int? C_RECHAZO_OBD { get; set; }

    public bool? PruebaObd { get; set; }

    public bool? PruebaEmisiones { get; set; }

    public bool? PruebaDiesel { get; set; }
}
