using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Caja
{
    public long Id { get; set; }

    public long IdIngresoFV { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int NumeroCaja { get; set; }

    public int FolioInicial { get; set; }

    public int FolioFinal { get; set; }

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;

    public virtual IngresoFV IdIngresoFVNavigation { get; set; } = null!;
}
