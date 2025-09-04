using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class NormaAcreditacion
{
    public int Id { get; set; }

    public string Clave { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<AcreditacionTecnicoEvidenciaPuestoVerificentro> AcreditacionTecnicoEvidenciaPuestoVerificentros { get; } = new List<AcreditacionTecnicoEvidenciaPuestoVerificentro>();
}
