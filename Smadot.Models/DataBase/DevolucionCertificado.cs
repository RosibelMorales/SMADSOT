using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class DevolucionCertificado
{
    public long Id { get; set; }

    public long IdDevolucionSPF { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int Cantidad { get; set; }

    public int FolioInicial { get; set; }

    public int FolioFinal { get; set; }

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;

    public virtual DevolucionSPF IdDevolucionSPFNavigation { get; set; } = null!;
}
