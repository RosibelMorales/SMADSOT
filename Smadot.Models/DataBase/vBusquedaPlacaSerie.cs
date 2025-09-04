using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vBusquedaPlacaSerie
{
    public long? IdFv { get; set; }

    public long Id { get; set; }

    public string? Placa { get; set; }

    public string Serie { get; set; } = null!;

    public bool Exento { get; set; }

    public bool Administrativa { get; set; }

    public bool Testificacion { get; set; }

    public bool Refrendo { get; set; }
}
