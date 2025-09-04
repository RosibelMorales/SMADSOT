using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class FoliosFormaValoradaActuale
{
    public int Id { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public long Folio { get; set; }

    public long IdVerificentro { get; set; }

    public long? IdFolioFormaValoradaVerificentro { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;
}
