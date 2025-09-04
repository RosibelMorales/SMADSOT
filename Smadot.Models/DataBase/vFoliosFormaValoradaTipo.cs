using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vFoliosFormaValoradaTipo
{
    public long IdVerificentro { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public string NombreCertificado { get; set; } = null!;

    public string? ClaveCertificado { get; set; }

    public long Folio { get; set; }

    public DateTime FechaRegistro { get; set; }
}
