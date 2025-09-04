using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Rol
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Alias { get; set; }

    public bool AccesoTotalVerificentros { get; set; }

    public virtual ICollection<Permiso> IdPermisos { get; } = new List<Permiso>();

    public virtual ICollection<User> IdUsers { get; } = new List<User>();
}
