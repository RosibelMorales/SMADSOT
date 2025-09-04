using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class ConfiguradorCitum
{
    public long Id { get; set; }

    public long IdCVV { get; set; }

    public DateTime Fecha { get; set; }

    public TimeSpan HoraInicio { get; set; }

    public TimeSpan HoraFin { get; set; }

    public int Capacidad { get; set; }

    public int IntervaloMin { get; set; }

    public bool Habilitado { get; set; }

    public virtual Verificentro IdCVVNavigation { get; set; } = null!;
}
