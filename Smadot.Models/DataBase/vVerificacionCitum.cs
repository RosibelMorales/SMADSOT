using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVerificacionCitum
{
    public long Id { get; set; }

    public long IdCita { get; set; }

    public DateTime Fecha { get; set; }

    public string Placa { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string Serie { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public long IdLinea { get; set; }

    public string Linea { get; set; } = null!;

    public long IdVerificentro { get; set; }

    public string? NombrePropietario { get; set; }

    public string CitaSerie { get; set; } = null!;

    public int? Orden { get; set; }

    public bool IngresoManual { get; set; }

    public long? IdResultadosVerificacion { get; set; }

    public int? EstatusPrueba { get; set; }

    public int? C_RECHAZO { get; set; }

    public int? RESULTADO { get; set; }

    public bool? PruebaObd { get; set; }

    public bool? PruebaEmisiones { get; set; }

    public bool? PruebaOpacidad { get; set; }

    public int? IdTipoCertificado { get; set; }

    public string Folio { get; set; } = null!;

    public long? IdFolioFormaValoradaVerificentro { get; set; }

    public long? FolioAsignado { get; set; }

    public string ClaveLinea { get; set; } = null!;
}
