using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vLineaEquipo
{
    public long IdLinea { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdCatEstatusEquipo { get; set; }

    public long Id { get; set; }

    public int IdCatEstatusLinea { get; set; }

    public int IdCatTipoEquipo { get; set; }
}
