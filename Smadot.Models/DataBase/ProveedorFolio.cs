using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class ProveedorFolio
{
    public long Id { get; set; }

    public int? FolioInicial { get; set; }

    public int? FolioFinal { get; set; }

    public long IdProveedor { get; set; }

    public long? Folio { get; set; }

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;
}
