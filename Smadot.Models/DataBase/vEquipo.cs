using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vEquipo
{
    public long Id { get; set; }

    public string NombreEquipo { get; set; } = null!;

    public string NumeroSerie { get; set; } = null!;

    public string? UrlFactura { get; set; }

    public string? UrlManualUsuario { get; set; }

    public string? UrlEspecificacionTecnica { get; set; }

    public string? UrlRecomendacionServicio { get; set; }

    public int IdCatEstatusEquipo { get; set; }

    public int IdCatTipoEquipo { get; set; }

    public string? NombreEstatusEquipo { get; set; }

    public long IdUserRegistro { get; set; }

    public string? NombreUsuarioRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public long? IdUserReviso { get; set; }

    public string? Comentarios { get; set; }

    public long? IdLinea { get; set; }

    public string? NombreLinea { get; set; }

    public long IdVerificentro { get; set; }

    public string? NombreVerificentro { get; set; }

    public string? Estatus { get; set; }

    public int? IdEstatusEquipo { get; set; }

    public long? IdEquipoTipoCalibracion { get; set; }

    public long? IdUltimaCalibracion { get; set; }

    public DateTime? FechaProximaCalibracion { get; set; }
}
