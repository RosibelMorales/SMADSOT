using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vInstalacion
{
    public long Id { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string? NombreProveedor { get; set; }

    public string UrlDocumento { get; set; } = null!;

    public string? NombreUsuario { get; set; }

    public long? IdVerificentro { get; set; }

    public string? NombreVerificentro { get; set; }
}
