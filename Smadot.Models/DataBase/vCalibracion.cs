using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vCalibracion
{
    public long Id { get; set; }

    public long IdEquipoTipoCalibracion { get; set; }

    public long IdEquipo { get; set; }

    public string? NombreEquipo { get; set; }

    public int Consecutivo { get; set; }

    public DateTime FechaCalibracion { get; set; }

    public DateTime FechaProgramada { get; set; }

    public DateTime FechaProximaCalibracion { get; set; }

    public long IdUserRegistro { get; set; }

    public string? NombreUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long? IdUserValido { get; set; }

    public string? NombreUserValido { get; set; }

    public string? Nota { get; set; }

    public int IdCatEstatusCalibracion { get; set; }

    public string? NombreEstatusCalibracion { get; set; }

    public int? NumeroCertificado { get; set; }

    public DateTime? FechaEmisionCertificad { get; set; }

    public string? UrlResultado { get; set; }

    public string? NombreLinea { get; set; }

    public long? IdVerificentro { get; set; }

    public int? IdCatEstatusEquipo { get; set; }

    public long? IdLinea { get; set; }
}
