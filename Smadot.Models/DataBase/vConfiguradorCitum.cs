using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vConfiguradorCitum
{
    public int Capacidad { get; set; }

    public DateTime Fecha { get; set; }

    public bool Habilitado { get; set; }

    public TimeSpan HoraInicio { get; set; }

    public TimeSpan HoraFin { get; set; }

    public long Id { get; set; }

    public long IdCVV { get; set; }

    public int IntervaloMin { get; set; }
}
