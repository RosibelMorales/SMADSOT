using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CatEstatusAcreditacion
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<AceditacionTecnicoSolicitud> AceditacionTecnicoSolicituds { get; } = new List<AceditacionTecnicoSolicitud>();

    public virtual ICollection<AcreditacionTecnicoEvidencium> AcreditacionTecnicoEvidencia { get; } = new List<AcreditacionTecnicoEvidencium>();
}
