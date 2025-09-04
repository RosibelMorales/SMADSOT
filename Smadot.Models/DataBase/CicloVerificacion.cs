using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CicloVerificacion
{
    public long Id { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public decimal ImporteFv { get; set; }

    public decimal ImporteExento { get; set; }

    public decimal ImporteTestificacion { get; set; }

    public decimal ImporteAdministrativo { get; set; }

    public decimal ImporteReposicion { get; set; }

    public decimal ImporteConstanciaUltimaVer { get; set; }

    public virtual ICollection<FormaValoradum> FormaValorada { get; } = new List<FormaValoradum>();

    public virtual ICollection<Verificacion> Verificacions { get; } = new List<Verificacion>();
}
