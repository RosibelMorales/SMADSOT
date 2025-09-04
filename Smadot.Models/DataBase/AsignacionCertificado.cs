using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class AsignacionCertificado
{
    public long Id { get; set; }

    public long IdAsignacionSV { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int Cantidad { get; set; }

    public int FolioInicial { get; set; }

    public int FolioFinal { get; set; }

    public virtual AsignacionSV IdAsignacionSVNavigation { get; set; } = null!;

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;
}
