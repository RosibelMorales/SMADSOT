using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Inventario
{
    public long Id { get; set; }

    public long IdAlmacen { get; set; }

    public int CantidadStock { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int? FolioInicial { get; set; }

    public int? FolioFinal { get; set; }

    public virtual Almacen IdAlmacenNavigation { get; set; } = null!;

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;

    public virtual ICollection<MovimientosInventario> MovimientosInventarios { get; } = new List<MovimientosInventario>();
}
