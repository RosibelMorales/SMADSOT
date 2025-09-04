using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vCajaFV
{
    public long Id { get; set; }

    public long IdIngresoFV { get; set; }

    public string NombreRecibio { get; set; } = null!;

    public DateTime FechaEntrega { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public string NombreCatTipoCertificado { get; set; } = null!;

    public string? ClaveCertificadoCatTipoCertificado { get; set; }

    public int NumeroCaja { get; set; }

    public int FolioInicial { get; set; }

    public int FolioFinal { get; set; }
}
