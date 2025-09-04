using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CalendarioVerificacion
{
    public long Id { get; set; }

    public string Digitos { get; set; } = null!;

    public int Mes { get; set; }

    public int Semestre { get; set; }
}
