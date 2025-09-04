using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Citum
{
    public long Id { get; set; }

    public long IdVerificentro { get; set; }

    public bool SinRestriccionHorario { get; set; }

    public DateTime Fecha { get; set; }

    public string Placa { get; set; } = null!;

    public string Serie { get; set; } = null!;

    public int Modelo { get; set; }

    public string Marca { get; set; } = null!;

    public string Submarca { get; set; } = null!;

    public string ColorVehiculo { get; set; } = null!;

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;
}
