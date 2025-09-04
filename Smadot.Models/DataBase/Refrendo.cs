using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Refrendo
{
    public long Id { get; set; }

    public long IdExento { get; set; }

    public string NumeroReferencia { get; set; } = null!;

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }

    public DateTime FechaCartaFactura { get; set; }

    public DateTime VigenciaHoloAnterior { get; set; }

    public string? Placa { get; set; }

    public string? Propietario { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdUserRegistro { get; set; }

    public long IdFolioFormaValoradaVerificentro { get; set; }

    public DateTime Vigencia { get; set; }

    public virtual Exento IdExentoNavigation { get; set; } = null!;

    public virtual FoliosFormaValoradaVerificentro IdFolioFormaValoradaVerificentroNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;
}
