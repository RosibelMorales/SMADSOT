using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Smadot.Models.Entities.Verificacion
{
    public class EventoEntradaRequest
    {
        [JsonProperty("evento")]
        public EventoEntrada Evento { get; set; }
        [JsonProperty("prueba")]
        public PruebaInicio? Prueba { get; set; } = null;
    }
    public class PruebaInicio
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("claveHolograma")]
        public string ClaveHolograma { get; set; }
        [JsonProperty("motivoVerificacion")]
        public string MotivoVerificacion { get; set; }
        [JsonProperty("claveLinea")]
        public string ClaveLinea { get; set; }
        [JsonProperty("numeroSerie")]
        public string NumeroSerie { get; set; }
        [JsonProperty("tecnicoVerificador")]
        public string TecnicoVerificador { get; set; }
        [JsonProperty("marca")]
        public int? Marca { get; set; }
        [JsonProperty("submarca")]
        public int? Submarca { get; set; }
        [JsonProperty("anio")]
        public int? Anio { get; set; }
        [JsonProperty("placa")]
        public string Placa { get; set; }
        [JsonProperty("claveCombustible")]
        public int? ClaveCombustible { get; set; }
        [JsonProperty("combustible")]
        public int? Combustible { get; set; }
        [JsonProperty("combustible_POTMAX_RPM")]
        public int? Combustible_POTMAX_RPM { get; set; }
        [JsonProperty("RAL_FAB")]
        public int? RAL_FAB { get; set; }
        [JsonProperty("GOB_FAB")]
        public int? GOB_FAB { get; set; }
        [JsonProperty("Pot_5024")]
        public decimal Pot_5024 { get; set; }
        [JsonProperty("Pot_2540")]
        public decimal Pot_2540 { get; set; }
        [JsonProperty("PotMax_RPM")]
        public int? PotMax_RPM { get; set; }
        [JsonProperty("Protocolo")]
        public int? Protocolo { get; set; }
        [JsonProperty("Cilindros")]
        public int? Cilindros { get; set; }
        [JsonProperty("Cilindrada")]
        public int? Cilindrada { get; set; }
        [JsonProperty("Motor_DSL")]
        public int? Motor_DSL { get; set; }
        [JsonProperty("PBV")]
        public ulong? PBV { get; set; }
        [JsonProperty("PBV_Equivalente")]
        public int? PBV_Equivalente { get; set; }
        [JsonProperty("PBV_ASM")]
        public int? PBV_ASM { get; set; }
        [JsonProperty("C_ABS")]
        public int? C_ABS { get; set; }
        [JsonProperty("ConvertidorCatalitico")]
        public int? ConvertidorCatalitico { get; set; }
        [JsonProperty("OBD")]
        public ulong? OBD { get; set; }
        [JsonProperty("DobleCero")]
        public ulong? DobleCero { get; set; }
        [JsonProperty("CERO_GASOL")]
        public ulong? CERO_GASOL { get; set; }
        [JsonProperty("CERO_GASLP")]
        public ulong? CERO_GASLP { get; set; }
        [JsonProperty("CERO_GASNC")]
        public ulong? CERO_GASNC { get; set; }
        [JsonProperty("CERO_DSL")]
        public ulong? CERO_DSL { get; set; }
        [JsonProperty("IdEstatus")]
        public int? IdEstatus { get; set; }
        [JsonProperty("Resultado")]
        public int? Resultado { get; set; }

        [JsonProperty("CRechazo")]
        public int? CRechazo { get; set; }
        [JsonProperty("revolucionesMaxGobernador")]
        public int? RevolucionesMaxGobernador { get; set; }

        [JsonProperty("nombrePropietario")]
        public string? NombrePropietario { get; set; }
        [JsonProperty("razonSocial")]
        public string? RazonSocial { get; set; }
        [JsonProperty("tarjetaCirculacion")]
        public string? TarjetaCirculacion { get; set; }

    }
    public class EventoEntrada
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("IdTipo")]
        public int IdTipo { get; set; }

        [JsonProperty("Fecha")]
        public DateTime Fecha { get; set; }

        [JsonProperty("TecnicoVerificador")]
        public string TecnicoVerificador { get; set; } = null!;

        [JsonProperty("IdIdentificador")]
        public string IdIdentificador { get; set; } = null!;
        [JsonProperty("Nota")]

        public string Nota { get; set; } = null!;

    }
    public class Prueba
    {
        public long Id { get; set; }

        public DateTime FechaUltimaModificacion { get; set; }

        public string? NumSerie { get; set; }

        public string? Placa { get; set; }

        public int? Combustible { get; set; }

        public DateTime? Fecha { get; set; }

        public string? NumeroConstanciaOtorgada { get; set; }

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

        public ulong? OBD { get; set; }

        public ulong? DobleCero { get; set; }

        public ulong? CERO_GASOL { get; set; }

        public ulong? CERO_GASLP { get; set; }

        public ulong? CERO_GASNC { get; set; }

        public ulong? CERO_DSL { get; set; }

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
    public partial class EventoSalida
    {
        public long Id { get; set; }
        public int IdTipo { get; set; }
        public DateTime Fecha { get; set; }
        public string Nota { get; set; } = null!;
        public string TecnicoVerificador { get; set; } = null!;
        public string IdIdentificador { get; set; } = null!;
        public string? Data { get; set; }
    }
}
