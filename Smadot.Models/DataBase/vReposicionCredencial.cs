using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vReposicionCredencial
{
    public long Id { get; set; }

    public long IdUserPuestoVerificentro { get; set; }

    public int IdCatMotivoReporteCredencial { get; set; }

    public string Denuncia { get; set; } = null!;

    public string? UrlCredencial { get; set; }

    public int IdCatEstatusReporteCredencial { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public string? Nombre { get; set; }

    public string? NumeroTrabajador { get; set; }

    public string? MotivoReporteCredencial { get; set; }

    public string? EstatusReporteCredencial { get; set; }

    public long? IdVerificentro { get; set; }
}
