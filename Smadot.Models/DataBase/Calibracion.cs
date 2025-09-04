using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Calibracion
{
    public long Id { get; set; }

    public long IdEquipoTipoCalibracion { get; set; }

    public int Consecutivo { get; set; }

    public DateTime FechaCalibracion { get; set; }

    public DateTime FechaProgramada { get; set; }

    public DateTime FechaProximaCalibracion { get; set; }

    public long IdUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long? IdUserValido { get; set; }

    public string? Nota { get; set; }

    public int IdCatEstatusCalibracion { get; set; }

    public int? NumeroCertificado { get; set; }

    public DateTime? FechaEmisionCertificad { get; set; }

    public string? UrlResultado { get; set; }

    public string? Laboratorio { get; set; }

    public virtual CatEstatusCalibracion IdCatEstatusCalibracionNavigation { get; set; } = null!;

    public virtual EquipoTipoCalibracion IdEquipoTipoCalibracionNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;

    public virtual User? IdUserValidoNavigation { get; set; }
}
