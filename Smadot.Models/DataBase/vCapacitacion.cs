using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vCapacitacion
{
    public long? IdCapacitacion { get; set; }

    public long? IdVerificentro { get; set; }

    public DateTime? FechaCapacitacion { get; set; }

    public string? TemaCapacitacion { get; set; }

    public int? IdCatEstatusCapacitacion { get; set; }

    public string? NombreCatEstatusCapacitacion { get; set; }

    public bool? ActivoCatEstatusCapacitacion { get; set; }

    public int? TotalAsistentes { get; set; }
}
