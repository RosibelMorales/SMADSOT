using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Puesto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdCatTipoPuesto { get; set; }

    public bool Activo { get; set; }

    public virtual CatTipoPuesto IdCatTipoPuestoNavigation { get; set; } = null!;

    public virtual ICollection<UserPuestoVerificentro> UserPuestoVerificentros { get; } = new List<UserPuestoVerificentro>();
}
