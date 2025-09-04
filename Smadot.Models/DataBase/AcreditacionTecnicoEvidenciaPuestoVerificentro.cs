using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class AcreditacionTecnicoEvidenciaPuestoVerificentro
{
    public long IdAcreditacionTecnicoEvidencia { get; set; }

    public long IdUserPuestoVerificentro { get; set; }

    public bool Eliminado { get; set; }

    public int IdTipoAcreditacion { get; set; }

    public int? IdNormaAcreditacion { get; set; }

    public virtual AcreditacionTecnicoEvidencium IdAcreditacionTecnicoEvidenciaNavigation { get; set; } = null!;

    public virtual NormaAcreditacion? IdNormaAcreditacionNavigation { get; set; }

    public virtual CatTipoAcreditacion IdTipoAcreditacionNavigation { get; set; } = null!;

    public virtual UserPuestoVerificentro IdUserPuestoVerificentroNavigation { get; set; } = null!;
}
