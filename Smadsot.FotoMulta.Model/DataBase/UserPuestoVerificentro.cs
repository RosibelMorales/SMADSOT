using System;
using System.Collections.Generic;

namespace Smadsot.FotoMulta.Model.DataBase;

public partial class UserPuestoVerificentro
{
    public long Id { get; set; }

    public long IdUser { get; set; }

    public long IdVerificentro { get; set; }

    public int IdPuesto { get; set; }

    public DateTime FechaCapacitacionInicio { get; set; }

    public DateTime FechaCapacitacionFinal { get; set; }

    public DateTime FechaIncorpacion { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdUserRegistro { get; set; }

    public string UrlContrato { get; set; } = null!;

    public DateTime? FechaAcreditacionNorma { get; set; }

    public string NumeroTrabajador { get; set; } = null!;

    public DateTime? FechaSeparacion { get; set; }

    public int IdCatEstatusPuesto { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;
}
