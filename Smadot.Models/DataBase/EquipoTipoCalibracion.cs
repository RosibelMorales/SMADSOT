using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class EquipoTipoCalibracion
{
    public long Id { get; set; }

    public long IdEquipo { get; set; }

    public int IdCatTipoCalibracion { get; set; }

    public string? UrlDocumento1 { get; set; }

    public DateTime PrimeraFechaCalibracion { get; set; }

    public string? Nota { get; set; }

    public long IdUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long? IdUserValido { get; set; }

    public virtual ICollection<Calibracion> Calibracions { get; } = new List<Calibracion>();

    public virtual Equipo IdEquipoNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;

    public virtual User? IdUserValidoNavigation { get; set; }
}
