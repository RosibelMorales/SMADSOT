using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVerificacionesTipo
{
    public bool? Publico { get; set; }

    public bool? Mercantil { get; set; }

    public long ReporteId { get; set; }

    public DateTime Fecha { get; set; }

    public long IdVerificentro { get; set; }
}
