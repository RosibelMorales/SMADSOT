using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vEquipoCalibracion
{
    public long? IdCalibracion { get; set; }

    public DateTime? FechaCalibracion { get; set; }

    public DateTime? FechaProgramada { get; set; }

    public DateTime? FechaProximaCalibracion { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public string? Nota { get; set; }

    public int? NumeroCertificado { get; set; }

    public DateTime? FechaEmisionCertificad { get; set; }

    public long IdEquipo { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdCatEstatusEquipo { get; set; }

    public long IdVerificentro { get; set; }

    public long? IdVerificentroLinea { get; set; }

    public long? IdEquipoTipoCalibracion { get; set; }

    public DateTime? PrimeraFechaCalibracion { get; set; }

    public int? IdCatTipoCalibracion { get; set; }

    public long? IdUserValido { get; set; }

    public long? IdUserRegistro { get; set; }
}
