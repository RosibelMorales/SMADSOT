using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vAcreditacionTecnicoEvidencium
{
    public long? ATecnicoEvidenciaId { get; set; }

    public string? NumeroSolicitud { get; set; }

    public string? TipoTramite { get; set; }

    public string? NumeroAcreditacion { get; set; }

    public DateTime? FechaAcreditacion { get; set; }

    public string? UrlAcreditacion { get; set; }

    public long IdVerificentro { get; set; }

    public long IdUser { get; set; }
}
