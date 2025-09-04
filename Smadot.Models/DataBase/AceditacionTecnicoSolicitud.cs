using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class AceditacionTecnicoSolicitud
{
    public long Id { get; set; }

    public string NumeroSolicitud { get; set; } = null!;

    public int IdCatEstatusAcreditacion { get; set; }

    public string? UrlAprobacion { get; set; }

    public long? IdVerificentro { get; set; }

    public virtual CatEstatusAcreditacion IdCatEstatusAcreditacionNavigation { get; set; } = null!;

    public virtual Verificentro? IdVerificentroNavigation { get; set; }

    public virtual ICollection<UserPuestoVerificentro> IdUserPuestoVerificentros { get; } = new List<UserPuestoVerificentro>();
}
