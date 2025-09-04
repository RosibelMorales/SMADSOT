using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vFoliosFormaValoradaImpresion
{
    public long Id { get; set; }

    public long? IdVerificacion { get; set; }

    public long? IdExento { get; set; }

    public long? IdTestificacion { get; set; }

    public long? IdRefrendoExento { get; set; }

    public string TipoCertificadoFV { get; set; } = null!;

    public string ClaveTramite { get; set; } = null!;

    public int? IdCatTipoTramite { get; set; }

    public int? IdCatTipoCertificado { get; set; }

    public long? IdReposicion { get; set; }

    public string TipoCertificado { get; set; } = null!;

    public string? NumeroReferencia { get; set; }

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }

    public long FolioFoliosFormaValoradaVerificentro { get; set; }

    public string? NombreVerificentro { get; set; }

    public string ApiEndPoint { get; set; } = null!;

    public string ApiKey { get; set; } = null!;

    public long IdFoliosFormaValoradaVerificentro { get; set; }

    public long IdVerificentro { get; set; }

    public long FolioCertificado { get; set; }

    public decimal ImporteActual { get; set; }

    public bool Impreso { get; set; }

    public string EntidadProcedencia { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }

    public string? Placa { get; set; }

    public string? Serie { get; set; }

    public DateTime? Vigencia { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public string? Combustible { get; set; }

    public string? TarjetaCirculacion { get; set; }

    public int? Anio { get; set; }

    public string? FolioCertificadoAnterior { get; set; }

    public string? Propietario { get; set; }

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

    public decimal? LAMDA_5024 { get; set; }

    public decimal? TEMP_5024 { get; set; }

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

    public bool? PruebaObd { get; set; }

    public bool? PruebaEmisiones { get; set; }

    public bool? PruebaOpacidad { get; set; }

    public int? PROTOCOLO { get; set; }

    public int? PBV { get; set; }

    public string? Clave { get; set; }

    public string? Linea { get; set; }

    public int? RESULTADO { get; set; }

    public int? C_RECHAZO { get; set; }

    public int? C_RECHAZO_OBD { get; set; }

    public DateTime? InicioPruebas { get; set; }

    public DateTime? FinalizacionPruebas { get; set; }

    public long? IdUserCapturista { get; set; }

    public string? NombreCapturista { get; set; }

    public string? NumeroCapturista { get; set; }

    public long? IdUserTecnico { get; set; }

    public string? NombreTecnico { get; set; }

    public string? NumeroTecnico { get; set; }

    public long? IdEquipo { get; set; }

    public string? NumeroSerieEquipo { get; set; }

    public string? NombreEquipo { get; set; }

    public int? IdCatTipoEquipo { get; set; }

    public int? IdMotivoVerificacion { get; set; }
}
