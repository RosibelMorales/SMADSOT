using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vDevolucionesSolicitudFecha
{
    public long? IdVerificentro { get; set; }

    public long IdSolicitud { get; set; }

    public string? NumeroSolicitud { get; set; }

    public DateTime? Fecha { get; set; }

    public int? Total { get; set; }
}
