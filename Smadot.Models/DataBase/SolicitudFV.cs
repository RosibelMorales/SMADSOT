using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class SolicitudFV
{
    public long Id { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public long IdUserSolicita { get; set; }

    public int IdCatEstatusSolicitud { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdAlmacen { get; set; }

    public virtual Almacen IdAlmacenNavigation { get; set; } = null!;

    public virtual CatEstatusSolicitud IdCatEstatusSolicitudNavigation { get; set; } = null!;

    public virtual User IdUserSolicitaNavigation { get; set; } = null!;

    public virtual ICollection<IngresoFV> IngresoFVs { get; } = new List<IngresoFV>();

    public virtual ICollection<SolicitudCertificado> SolicitudCertificados { get; } = new List<SolicitudCertificado>();
}
