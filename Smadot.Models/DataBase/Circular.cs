using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Circular
{
    public long Id { get; set; }

    public string NumeroCircular { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public string Mensaje { get; set; } = null!;

    public long IdUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<CircularVerificentro> CircularVerificentros { get; } = new List<CircularVerificentro>();

    public virtual User IdUserRegistroNavigation { get; set; } = null!;
}
