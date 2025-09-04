using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVerificacionInformacionGeneral
{
    public long IdVerificacion { get; set; }

    public long? IdResultadosVerificacion { get; set; }

    public long? IdParametrosTablaMaestra { get; set; }

    public long? IdCitaVerificacion { get; set; }

    public string? Clave { get; set; }

    public string? Linea { get; set; }

    public DateTime Fecha { get; set; }

    public DateTime Vigencia { get; set; }

    public string? MotivoVerificacion { get; set; }

    public string PlacaVerificacion { get; set; } = null!;

    public string SerieVerificacion { get; set; } = null!;

    public int ModeloVerificacion { get; set; }

    public int? Resultado { get; set; }

    public int? RESULTADO_PROVEEDOR { get; set; }

    public string? C_RECHAZO { get; set; }

    public int? C_RECHAZO_OBD { get; set; }

    public int? EstatusPrueba { get; set; }

    public string? Placa { get; set; }

    public string? Serie { get; set; }

    public short? Anio { get; set; }

    public DateTime? FechaFacturacion { get; set; }

    public bool? Poblano { get; set; }

    public string? Estado { get; set; }

    public string? Propietario { get; set; }

    public string Marca { get; set; } = null!;

    public string Submarca { get; set; } = null!;

    public bool? CambioPlacas { get; set; }

    public int? IdCatTipoServicio { get; set; }

    public int? DOBLECERO { get; set; }

    public int? REF_00 { get; set; }

    public int? PROTOCOLOTM { get; set; }

    public int? ANO_DESDE { get; set; }

    public int? ANO_HASTA { get; set; }

    public long? FolioActual { get; set; }
}
