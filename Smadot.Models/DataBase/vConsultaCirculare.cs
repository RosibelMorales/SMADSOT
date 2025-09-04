using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vConsultaCirculare
{
    public long? IdCircular { get; set; }

    public string? NumeroCircular { get; set; }

    public DateTime? Fecha { get; set; }

    public int? Leido { get; set; }

    public int? NoLeido { get; set; }

    public string? Mensaje { get; set; }
}
