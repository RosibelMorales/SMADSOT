using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class ReporteEntregaTercer
{
    public long Id { get; set; }

    public long Numero { get; set; }

    public DateTime FechaInicial { get; set; }

    public DateTime FechaFinal { get; set; }

    public string UrlEntrega { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public long IdUserRegistro { get; set; }

    public long IdVerificentro { get; set; }

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;

    public virtual ICollection<ReporteEntregaTercerCertificado> ReporteEntregaTercerCertificados { get; } = new List<ReporteEntregaTercerCertificado>();
}
