using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class LineaMotivo
{
    public long Id { get; set; }

    public long IdLinea { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdUserRegistro { get; set; }

    public int IdCatMotivoLinea { get; set; }

    public string? UrlDocumento { get; set; }

    public string Notas { get; set; } = null!;

    public bool Estatus { get; set; }

    public virtual CatMotivoLinea IdCatMotivoLineaNavigation { get; set; } = null!;

    public virtual Linea IdLineaNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;
}
