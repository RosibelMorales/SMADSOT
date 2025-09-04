using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Linea
{
    public long Id { get; set; }

    public long IdVerificentro { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdCatEstatusLinea { get; set; }

    public long IdUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string Clave { get; set; } = null!;

    public virtual ICollection<Equipo> Equipos { get; } = new List<Equipo>();

    public virtual CatEstatusLinea IdCatEstatusLineaNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;

    public virtual ICollection<LineaMotivo> LineaMotivos { get; } = new List<LineaMotivo>();

    public virtual ICollection<Verificacion> Verificacions { get; } = new List<Verificacion>();
}
