using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Permiso
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public long? IdPermisoPadre { get; set; }

    public virtual Permiso? IdPermisoPadreNavigation { get; set; }

    public virtual ICollection<Permiso> InverseIdPermisoPadreNavigation { get; } = new List<Permiso>();

    public virtual ICollection<Rol> IdRols { get; } = new List<Rol>();
}
