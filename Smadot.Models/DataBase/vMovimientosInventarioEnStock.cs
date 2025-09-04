using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vMovimientosInventarioEnStock
{
    public long Id { get; set; }

    public long IdAlmacen { get; set; }

    public long? IdIngresoFV { get; set; }

    public long? IdVerificentro { get; set; }

    public long IdInventario { get; set; }

    public long? IdIngresoCertificado { get; set; }

    public long? IdVentaCertificado { get; set; }

    public int? FolioInicial { get; set; }

    public int? IdCatTipoCertificado { get; set; }

    public int? FolioFinal { get; set; }
}
