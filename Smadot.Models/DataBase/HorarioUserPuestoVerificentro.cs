using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class HorarioUserPuestoVerificentro
{
    public long Id { get; set; }

    public long IdUserPuestoVerificentro { get; set; }

    public string Dia { get; set; } = null!;

    public TimeSpan HoraInicio { get; set; }

    public TimeSpan HoraFin { get; set; }

    public virtual UserPuestoVerificentro IdUserPuestoVerificentroNavigation { get; set; } = null!;
}
