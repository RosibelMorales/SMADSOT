using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class ReporteCredencial
{
    public long Id { get; set; }

    public long IdUserPuestoVerificentro { get; set; }

    public int IdCatMotivoReporteCredencial { get; set; }

    public string Denuncia { get; set; } = null!;

    public string? UrlCredencial { get; set; }

    public int IdCatEstatusReporteCredencial { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual CatEstatusReporteCredencial IdCatEstatusReporteCredencialNavigation { get; set; } = null!;

    public virtual CatMotivoReporteCredencial IdCatMotivoReporteCredencialNavigation { get; set; } = null!;

    public virtual UserPuestoVerificentro IdUserPuestoVerificentroNavigation { get; set; } = null!;
}
