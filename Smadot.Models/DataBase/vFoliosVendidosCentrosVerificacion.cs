using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vFoliosVendidosCentrosVerificacion
{
    public DateTime Fecha { get; set; }

    public string? Cvv { get; set; }

    public string? PersonaRecibeCertificados { get; set; }

    public string? ClaveVenta { get; set; }

    public int? Total { get; set; }

    public int? FolioVenta { get; set; }

    public int? FoliosStock { get; set; }

    public int? FoliosUsados { get; set; }

    public string? ReferenciaBancaria { get; set; }

    public decimal? Monto { get; set; }

    public long IdFormaValorada { get; set; }

    public int? FoliosFV { get; set; }
}
