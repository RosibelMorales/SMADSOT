using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CapacitacionEmpleado
{
    public long Id { get; set; }

    public long IdUserPuestoVerificentro { get; set; }

    public long IdCapacitacion { get; set; }

    public string UrlFotografia { get; set; } = null!;

    public bool Asistio { get; set; }

    public string UrlEvaluacion { get; set; } = null!;

    public virtual Capacitacion IdCapacitacionNavigation { get; set; } = null!;

    public virtual UserPuestoVerificentro IdUserPuestoVerificentroNavigation { get; set; } = null!;
}
