using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vProgramacionCalibracion
{
    public long? Id { get; set; }

    public long? IdEquipo { get; set; }

    public string NombreEquipo { get; set; } = null!;

    public int? IdCatTipoCalibracion { get; set; }

    public string NombreTipoCalibracion { get; set; } = null!;

    public string? UrlDocumento1 { get; set; }

    public DateTime? PrimeraFechaCalibracion { get; set; }

    public string? Nota { get; set; }

    public long? IdUserRegistro { get; set; }

    public string? NombreUser { get; set; }

    public string? UserName { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public long? IdUserValido { get; set; }

    public string? NombreValido { get; set; }

    public long? IdVerificentro { get; set; }
}
