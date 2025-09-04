using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class StockCertificado
{
    public long Id { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int CantidadMinima { get; set; }

    public int CantidadMedia { get; set; }

    public long IdStockMinimo { get; set; }

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;

    public virtual StockMinimo IdStockMinimoNavigation { get; set; } = null!;
}
