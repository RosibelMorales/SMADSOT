using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class UsuarioRol
{
    public long IdUser { get; set; }

    public long IdRol { get; set; }

    public virtual User IdUser1 { get; set; } = null!;

    public virtual Rol IdUserNavigation { get; set; } = null!;
}
