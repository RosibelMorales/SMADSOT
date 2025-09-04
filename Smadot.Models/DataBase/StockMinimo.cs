using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class StockMinimo
{
    public long Id { get; set; }

    public long IdAlmacen { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long? IdUserRegistro { get; set; }

    public virtual Almacen IdAlmacenNavigation { get; set; } = null!;

    public virtual User? IdUserRegistroNavigation { get; set; }

    public virtual ICollection<StockCertificado> StockCertificados { get; } = new List<StockCertificado>();
}
