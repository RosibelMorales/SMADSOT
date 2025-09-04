using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class UsoSuministro
{
    public long Id { get; set; }

    public long IdVerificentro { get; set; }

    public string Nombre { get; set; } = null!;

    public int Cantidad { get; set; }

    public DateTime Fecha { get; set; }

    public string Nota { get; set; } = null!;

    public string UrlFactura { get; set; } = null!;

    public string Proveedor { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public long IdUserRegistro { get; set; }

    public virtual User IdUserRegistroNavigation { get; set; } = null!;

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;
}
