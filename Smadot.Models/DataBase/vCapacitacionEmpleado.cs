using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vCapacitacionEmpleado
{
    public long? Id { get; set; }

    public DateTime? FechaCapacitacion { get; set; }

    public int? IdCatEstatusCapacitacion { get; set; }

    public long? IdCapacitacion { get; set; }

    public long? IdUserPuestoVerificentro { get; set; }

    public bool Asistio { get; set; }

    public string? TemaCapacitacion { get; set; }

    public string? UrlFotografia { get; set; }

    public string? UrlEvaluacion { get; set; }

    public string? Nombre { get; set; }

    public string? NumeroTrabajador { get; set; }

    public long? IdVerificentro { get; set; }
}
