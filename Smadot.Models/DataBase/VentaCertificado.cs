using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class VentaCertificado
{
    public long Id { get; set; }

    public long IdVentaFV { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int Cantidad { get; set; }

    public string Folios { get; set; } = null!;

    public decimal ImporteTotal { get; set; }

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;

    public virtual VentaFV IdVentaFVNavigation { get; set; } = null!;

    public virtual ICollection<MovimientosInventario> MovimientosInventarios { get; } = new List<MovimientosInventario>();
}
