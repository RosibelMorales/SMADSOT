using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vStockMinimo
{
    public long? IdStockMinimo { get; set; }

    public long? IdAlmacen { get; set; }

    public string? NombreAlmacen { get; set; }

    public int? CantidadMinima { get; set; }

    public int? CantidadMedia { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public string Nombre { get; set; } = null!;

    public string? ClaveCertificado { get; set; }

    public long? IdVerificentro { get; set; }
}
