using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vInventario
{
    public long Id { get; set; }

    public long IdAlmacen { get; set; }

    public int CantidadStock { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int? FolioInicial { get; set; }

    public int? FolioFinal { get; set; }

    public string NombreCertificado { get; set; } = null!;

    public long? IdVerificentro { get; set; }
}
