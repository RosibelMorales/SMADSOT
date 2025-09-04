using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Instalacion
{
    public long Id { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdUserRegistro { get; set; }

    public long? IdReporte { get; set; }

    public long IdProveedor { get; set; }

    public string UrlDocumento { get; set; } = null!;

    public long IdVerificentro { get; set; }

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;

    public virtual Reporte? IdReporteNavigation { get; set; }

    public virtual User IdUserRegistroNavigation { get; set; } = null!;

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;
}
