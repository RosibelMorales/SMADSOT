using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class IngresoCertificado
{
    public long Id { get; set; }

    public long IdIngresoFV { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int CantidadRecibida { get; set; }

    public int FolioInicial { get; set; }

    public int FolioFinal { get; set; }

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;

    public virtual IngresoFV IdIngresoFVNavigation { get; set; } = null!;

    public virtual ICollection<MovimientosInventario> MovimientosInventarios { get; } = new List<MovimientosInventario>();
}
