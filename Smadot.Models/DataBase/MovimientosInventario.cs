using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class MovimientosInventario
{
    public long Id { get; set; }

    public long IdInventario { get; set; }

    public long? IdDevolucionCertificado { get; set; }

    public long? IdAsignacionCertificado { get; set; }

    public long? IdIngresoCertificado { get; set; }

    public long? IdVentaCertificado { get; set; }

    public virtual IngresoCertificado? IdIngresoCertificadoNavigation { get; set; }

    public virtual Inventario IdInventarioNavigation { get; set; } = null!;

    public virtual VentaCertificado? IdVentaCertificadoNavigation { get; set; }
}
