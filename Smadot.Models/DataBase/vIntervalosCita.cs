using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vIntervalosCita
{
    public DateTime Fecha { get; set; }

    public long IdCVV { get; set; }

    public int? TotalIntervalo { get; set; }
}
