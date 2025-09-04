using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vIngresoFV
{
    public long Id { get; set; }

    public DateTime FechaEntrega { get; set; }

    public string NombreRecibio { get; set; } = null!;

    public long IdSolicitudFV { get; set; }

    public long IdUsers { get; set; }

    public string? NombreUser { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdAlmacen { get; set; }
}
