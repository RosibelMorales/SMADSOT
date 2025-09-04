using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CatMarcaVehiculo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<CatSubMarcaVehiculo> CatSubMarcaVehiculos { get; } = new List<CatSubMarcaVehiculo>();
}
