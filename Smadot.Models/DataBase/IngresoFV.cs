using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class IngresoFV
{
    public long Id { get; set; }

    public DateTime FechaEntrega { get; set; }

    public string NombreRecibio { get; set; } = null!;

    public long IdSolicitudFV { get; set; }

    public long IdUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdAlmacen { get; set; }

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }

    public virtual ICollection<Caja> Cajas { get; } = new List<Caja>();

    public virtual Almacen IdAlmacenNavigation { get; set; } = null!;

    public virtual SolicitudFV IdSolicitudFVNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;

    public virtual ICollection<IngresoCertificado> IngresoCertificados { get; } = new List<IngresoCertificado>();

    public virtual VentaFV? VentaFV { get; set; }
}
