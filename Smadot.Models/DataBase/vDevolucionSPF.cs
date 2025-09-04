using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vDevolucionSPF
{
    public long Id { get; set; }

    public string NumeroDevolucion { get; set; } = null!;

    public string ResponsableEntrega { get; set; } = null!;

    public long IdUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string UserAprobo { get; set; } = null!;

    public long IdAlmacen { get; set; }

    public string? RecibioSPF { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public string? NumeroSolicitud { get; set; }

    public string? UrlNombreDocumento1 { get; set; }

    public string? UrlNombreDocumento2 { get; set; }

    public string? UrlNombreDocumento3 { get; set; }

    public string? Nombre { get; set; }

    public int? Cantidad { get; set; }
}
