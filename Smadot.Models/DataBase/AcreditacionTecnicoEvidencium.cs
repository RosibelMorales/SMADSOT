using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class AcreditacionTecnicoEvidencium
{
    public long Id { get; set; }

    public string NumeroSolicitud { get; set; } = null!;

    public string TipoTramite { get; set; } = null!;

    public string NumeroAcreditacion { get; set; } = null!;

    public string NumeroReferencia { get; set; } = null!;

    public DateTime FechaAcreditacion { get; set; }

    public DateTime? FechaAmpliacion { get; set; }

    public DateTime FechaEmision { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string? UrlAcreditacion { get; set; }

    public int IdCatEstatusAcreditacion { get; set; }

    public virtual ICollection<AcreditacionTecnicoEvidenciaPuestoVerificentro> AcreditacionTecnicoEvidenciaPuestoVerificentros { get; } = new List<AcreditacionTecnicoEvidenciaPuestoVerificentro>();

    public virtual CatEstatusAcreditacion IdCatEstatusAcreditacionNavigation { get; set; } = null!;
}
