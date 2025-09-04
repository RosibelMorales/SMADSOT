using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Smadot.Models.Entities.Verificacion.Response
{
    //{2, "Estático (TSI)" },
    //{ 6, "Aceleración libre (Diesel)" },
    //{ 7, "dinámico (ASM)" }
    public class ResultadosPruebaOBD
    {
        public string Etapa { get; set; }
        public int? Protocolo { get; set; }
        public string OBD_TIPO_SDB { get; set; }
        public int? OBD_MIL { get; set; }
        public int? OBD_CATAL { get; set; }
        public int? OBD_CILIN { get; set; }
        public int? OBD_COMBU { get; set; }
        public int? OBD_INTEG { get; set; }
        public int? OBD_OXIGE { get; set; }
        public int? SPS_Humo { get; set; }
        public int? SPS_2540 { get; set; }
        public int? SPS_5024 { get; set; }
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
        public int? C_RECHAZO { get; set; }
        public int? C_Rechazo_OBD { get; set; }
        public int? RESULTADO { get; set; }
        public int? EstatusPrueba { get; set; }
    }
    public class ResultadosEmisiones
    {
        public string Etapa { get; set; }
        public int? Protocolo { get; set; }
        public int? SPS_Humo { get; set; }
        public int? SPS_2540 { get; set; }
        public int? SPS_5024 { get; set; }
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
        public int? C_RECHAZO { get; set; }
        public int? RESULTADO { get; set; }
        public int? EstatusPrueba { get; set; }
    }
    public class ResultadosOpacidad
    {
        public string Etapa { get; set; }
        public int? Protocolo { get; set; }
        public decimal? OPACIDADP { get; set; }
        public decimal? OPACIDADK { get; set; }
        public int? TEMP_MOT { get; set; }
        public int? VEL_GOB { get; set; }
        public int? POTMIN_RPM { get; set; }
        public int? POTMAX_RPM { get; set; }
        public int? TEMP_GAS { get; set; }
        public int? TEMP_CAM { get; set; }
        public int? C_RECHAZO { get; set; }
        public int? RESULTADO { get; set; }
        public int? EstatusPrueba { get; set; }
    }
    public class PruebaVisualResponse
    {
        public int TipoCombustible { get; set; }
        public int NumeroEscapes { get; set; }
        public bool FugasSistemaEscape { get; set; }
        public bool GobernadorBuenEstado { get; set; }
        public bool PortafiltroAire { get; set; }
        public bool FiltroAire { get; set; }
        public bool TaponDispositivoAceite { get; set; }
        public bool TaponCombustible { get; set; }
        public bool Bayoneta { get; set; }
        public bool FugaAceiteMotor { get; set; }
        public bool FugaAceiteTransmision { get; set; }
        public bool FugaLiquidoRefrigerante { get; set; }
        public bool DibujoNeumaticos { get; set; }
        public bool DesperfectosNeumaticos { get; set; }
        public bool DimensionesNeumaticoIncorrectas { get; set; }
        public bool ControlEmisionDesconectados { get; set; }
        public bool ControlEmisionAlterados { get; set; }
        public bool PlacasCorrespondientes { get; set; }
    }
}