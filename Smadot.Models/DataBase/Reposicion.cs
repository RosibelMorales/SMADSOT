using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Reposicion
{
    public long Id { get; set; }

    public long IdFoliosFormaValoradaVerificentro { get; set; }

    public string NumeroReferencia { get; set; } = null!;

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdUserRegistro { get; set; }

    public virtual FoliosFormaValoradaVerificentro IdFoliosFormaValoradaVerificentroNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;
}
