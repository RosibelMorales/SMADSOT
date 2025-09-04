using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class LaboratorioTecnico
{
    public long Id { get; set; }

    public string NumeroEmpleado { get; set; } = null!;

    public string? Nombre { get; set; }

    public virtual ICollection<Calibracion> Calibracions { get; } = new List<Calibracion>();
}
