using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vAcreditacionTecnicoSolicitud
{
    public long Id { get; set; }

    public string NumeroSolicitud { get; set; } = null!;

    public int IdCatEstatusAcreditacion { get; set; }

    public string? Estatus { get; set; }

    public string? UrlAprobacion { get; set; }

    public long? IdVerificentro { get; set; }
}
