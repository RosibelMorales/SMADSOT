using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vCatSubMarcaVehiculo
{
    public string Marca { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public int Id { get; set; }

    public int IdCatMarcaVehiculo { get; set; }

    public int Clave { get; set; }
}
