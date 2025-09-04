using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vAsignacionStock
{
    public long Id { get; set; }

    public DateTime FechaEntrega { get; set; }

    public string NombreRecibio { get; set; } = null!;

    public long IdUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string? UserAprobo { get; set; }

    public long IdAlmacen { get; set; }

    public string? UserRecibe { get; set; }

    public string? Nombre { get; set; }
}
