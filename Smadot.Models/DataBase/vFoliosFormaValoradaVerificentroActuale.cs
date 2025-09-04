using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vFoliosFormaValoradaVerificentroActuale
{
    public int Id { get; set; }

    public long Folio { get; set; }

    public long IdVerificentro { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CatTipoCertificado { get; set; } = null!;

    public string? ClaveCertificado { get; set; }

    public string Verificentro { get; set; } = null!;
}
