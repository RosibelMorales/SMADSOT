using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class ReporteFVCanceladum
{
    public long Id { get; set; }

    public long IdReporte { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int Cantidad { get; set; }

    public int FolioInicial { get; set; }

    public int FolioFinal { get; set; }

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;

    public virtual Reporte IdReporteNavigation { get; set; } = null!;
}
