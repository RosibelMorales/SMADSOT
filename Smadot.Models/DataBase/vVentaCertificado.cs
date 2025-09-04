using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVentaCertificado
{
    public long Id { get; set; }

    public long IdVentaFV { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int Cantidad { get; set; }

    public int FolioInicial { get; set; }

    public int FolioFinal { get; set; }

    public string Folios { get; set; } = null!;

    public decimal ImporteTotal { get; set; }
}
