using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class SolicitudCertificado
{
    public long Id { get; set; }

    public long IdSolicitudFV { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int Cantidad { get; set; }

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;

    public virtual SolicitudFV IdSolicitudFVNavigation { get; set; } = null!;
}
