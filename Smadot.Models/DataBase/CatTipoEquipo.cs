using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CatTipoEquipo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Estatus { get; set; }

    public virtual ICollection<Equipo> Equipos { get; } = new List<Equipo>();
}
