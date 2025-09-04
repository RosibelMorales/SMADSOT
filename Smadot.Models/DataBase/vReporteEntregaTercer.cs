using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vReporteEntregaTercer
{
    public long IdReporte { get; set; }

    public long NumeroReporte { get; set; }

    public DateTime FechaInicial { get; set; }

    public DateTime FechaFinal { get; set; }

    public string UrlEntrega { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public string? UserRegistro { get; set; }

    public int? IdCatTipoReporte { get; set; }

    public string? NombreCatTipoReporte { get; set; }

    public int? Cantidad { get; set; }

    public int? SerieInicial { get; set; }

    public int? SerieFinal { get; set; }

    public long? IdVerificentro { get; set; }

    public string? NombreVerificentro { get; set; }

    public string? ClaveCertificado { get; set; }
}
