using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vUsuario
{
    public long IdUsuario { get; set; }

    public string? NombreUsuario { get; set; }

    public string? CorreoElectronico { get; set; }

    public long? IdRol { get; set; }

    public string? NombreRol { get; set; }

    public long? IdVerificentro { get; set; }

    public bool LockoutEnabled { get; set; }
}
