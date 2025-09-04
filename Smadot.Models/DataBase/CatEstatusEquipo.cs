using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CatEstatusEquipo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Equipo> Equipos { get; } = new List<Equipo>();
}
