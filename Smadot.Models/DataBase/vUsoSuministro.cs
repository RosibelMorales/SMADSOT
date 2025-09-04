using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vUsoSuministro
{
    public long Id { get; set; }

    public long IdVerificentro { get; set; }

    public string NombreVerificentro { get; set; } = null!;

    public string NombreUsoSuministro { get; set; } = null!;

    public int Cantidad { get; set; }

    public DateTime Fecha { get; set; }

    public string Nota { get; set; } = null!;

    public string UrlFactura { get; set; } = null!;

    public string Proveedor { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public long IdUserRegistro { get; set; }

    public string? NombreUser { get; set; }
}
