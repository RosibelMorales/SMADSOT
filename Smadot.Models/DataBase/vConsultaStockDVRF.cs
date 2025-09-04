using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vConsultaStockDVRF
{
    public long? NumeroSolucitud { get; set; }

    public long? IdIngresoFV { get; set; }

    public string NombreTipoCertificado { get; set; } = null!;

    public int CantidadStock { get; set; }

    public long IdAlmacen { get; set; }

    public string Almacen { get; set; } = null!;

    public int NumeroCaja { get; set; }

    public int? FolioInicial { get; set; }

    public int? FolioFinal { get; set; }

    public string? ClaveCertificado { get; set; }

    public int? CantidadMinima { get; set; }

    public int? CantidadMedia { get; set; }
}
