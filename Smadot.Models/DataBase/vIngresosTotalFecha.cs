using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vIngresosTotalFecha
{
    public DateTime? Fecha { get; set; }

    public int? Total { get; set; }

    public long IdSolicitudFV { get; set; }
}
