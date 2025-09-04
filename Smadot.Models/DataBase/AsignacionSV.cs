using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class AsignacionSV
{
    public long Id { get; set; }

    public DateTime FechaEntrega { get; set; }

    public string NombreRecibio { get; set; } = null!;

    public long IdUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string? UserAprobo { get; set; }

    public long IdAlmacen { get; set; }

    public string? UserRecibe { get; set; }

    public virtual ICollection<AsignacionCertificado> AsignacionCertificados { get; } = new List<AsignacionCertificado>();

    public virtual Almacen IdAlmacenNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;
}
