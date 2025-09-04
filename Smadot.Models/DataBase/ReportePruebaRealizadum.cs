using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class ReportePruebaRealizadum
{
    public long Id { get; set; }

    public long IdReporte { get; set; }

    public int Gasolina { get; set; }

    public int Diesel { get; set; }

    public int Aprobadas { get; set; }

    public int Reprobadas { get; set; }

    public string TipoServicio { get; set; } = null!;

    public virtual Reporte IdReporteNavigation { get; set; } = null!;
}
