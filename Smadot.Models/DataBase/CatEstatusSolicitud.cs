using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CatEstatusSolicitud
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<SolicitudFV> SolicitudFVs { get; } = new List<SolicitudFV>();
}
