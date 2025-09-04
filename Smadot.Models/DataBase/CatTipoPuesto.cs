using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CatTipoPuesto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Puesto> Puestos { get; } = new List<Puesto>();
}
