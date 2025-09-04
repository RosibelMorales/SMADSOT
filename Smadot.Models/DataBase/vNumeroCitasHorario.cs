using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vNumeroCitasHorario
{
    public long IdCVV { get; set; }

    public DateTime Fecha { get; set; }

    public DateTime? FechaDia { get; set; }

    public int? NumeroCitas { get; set; }

    public int Capacidad { get; set; }
}
