using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vCALinea
{
    public long Id { get; set; }

    public long IdLinea { get; set; }

    public long IdLineaMotivo { get; set; }

    public DateTime FechaRegistroLineaMotivo { get; set; }

    public bool Estatus { get; set; }

    public string? Motivo { get; set; }

    public string Notas { get; set; } = null!;

    public string? UrlDocumento { get; set; }

    public long? IdUserRegistroMotivo { get; set; }

    public string? UserRegistroMotivo { get; set; }
}
