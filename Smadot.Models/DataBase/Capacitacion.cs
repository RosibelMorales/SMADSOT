using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Capacitacion
{
    public long Id { get; set; }

    public DateTime Fecha { get; set; }

    public string Tema { get; set; } = null!;

    public int IdCatEstatusCapacitacion { get; set; }

    public long IdVerificentro { get; set; }

    public virtual ICollection<CapacitacionEmpleado> CapacitacionEmpleados { get; } = new List<CapacitacionEmpleado>();

    public virtual CatEstatusCapacitacion IdCatEstatusCapacitacionNavigation { get; set; } = null!;

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;
}
