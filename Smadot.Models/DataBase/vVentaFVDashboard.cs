using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVentaFVDashboard
{
    public long Id { get; set; }

    public long? IdVerificentro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime FechaVenta { get; set; }

    public long VentaCId { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int Cantidad { get; set; }

    public decimal ImporteTotal { get; set; }
}
