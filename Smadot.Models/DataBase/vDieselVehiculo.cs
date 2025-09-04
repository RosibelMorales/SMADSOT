using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vDieselVehiculo
{
    public string? Marca { get; set; }

    public int? IdCatMarcaVehiculo { get; set; }

    public string? Nombre { get; set; }

    public int? Clave { get; set; }

    public int? POTMAX_RPM { get; set; }

    public int? RAL_FAB { get; set; }

    public int? GOB_FAB { get; set; }

    public long? Id { get; set; }
}
