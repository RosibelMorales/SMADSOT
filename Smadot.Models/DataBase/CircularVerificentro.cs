using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CircularVerificentro
{
    public long Id { get; set; }

    public long IdCircular { get; set; }

    public long IdVerificentro { get; set; }

    public bool Leido { get; set; }

    public string? AcuseLeido { get; set; }

    public virtual Circular IdCircularNavigation { get; set; } = null!;

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;
}
