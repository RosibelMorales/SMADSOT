using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class VentaFV
{
    public long Id { get; set; }

    public string NumeroReferencia { get; set; } = null!;

    public long IdUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }

    public DateTime FechaVenta { get; set; }

    public string NumeroCompra { get; set; } = null!;

    public virtual IngresoFV IdNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;

    public virtual ICollection<VentaCertificado> VentaCertificados { get; } = new List<VentaCertificado>();
}
