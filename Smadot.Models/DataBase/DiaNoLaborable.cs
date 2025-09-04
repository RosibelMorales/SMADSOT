using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class DiaNoLaborable
{
    public long Id { get; set; }

    public DateTime Fecha { get; set; }

    public long IdUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdCVV { get; set; }

    public virtual Verificentro IdCVVNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;
}
