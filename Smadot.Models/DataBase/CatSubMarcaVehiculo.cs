using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CatSubMarcaVehiculo
{
    public int IdCatMarcaVehiculo { get; set; }

    public int Clave { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Alias { get; set; }

    public int Id { get; set; }

    public virtual CatMarcaVehiculo IdCatMarcaVehiculoNavigation { get; set; } = null!;
}
