using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vIndicadoresDashboard
{
    public string Origen { get; set; } = null!;

    public long? ReporteId { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public long? IdVerificentro { get; set; }
}
