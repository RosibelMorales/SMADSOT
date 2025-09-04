using System;
using System.Collections.Generic;

namespace Smadsot.FotoMulta.Model.DataBase;

public partial class Rol
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Alias { get; set; }

    public bool AccesoTotalVerificentros { get; set; }

    public virtual ICollection<Permiso> IdPermisos { get; set; } = new List<Permiso>();

    public virtual ICollection<User> IdUsers { get; set; } = new List<User>();
}
