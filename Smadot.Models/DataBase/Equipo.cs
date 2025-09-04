using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Equipo
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string NumeroSerie { get; set; } = null!;

    public string? UrlFactura { get; set; }

    public string? UrlManualUsuario { get; set; }

    public string? UrlEspecificacionTecnica { get; set; }

    public string? UrlRecomendacionServicio { get; set; }

    public int IdCatEstatusEquipo { get; set; }

    public long IdUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long? IdUserReviso { get; set; }

    public string? Comentarios { get; set; }

    public long? IdLinea { get; set; }

    public long IdVerificentro { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int IdCatTipoEquipo { get; set; }

    public virtual ICollection<EquipoTipoCalibracion> EquipoTipoCalibracions { get; } = new List<EquipoTipoCalibracion>();

    public virtual CatEstatusEquipo IdCatEstatusEquipoNavigation { get; set; } = null!;

    public virtual CatTipoEquipo IdCatTipoEquipoNavigation { get; set; } = null!;

    public virtual Linea? IdLineaNavigation { get; set; }

    public virtual User IdUserRegistroNavigation { get; set; } = null!;

    public virtual User? IdUserRevisoNavigation { get; set; }

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;

    public virtual ICollection<OrdenServicio> OrdenServicios { get; } = new List<OrdenServicio>();

    public virtual ICollection<Verificacion> Verificacions { get; } = new List<Verificacion>();
}
