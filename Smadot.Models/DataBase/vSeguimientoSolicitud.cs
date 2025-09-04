using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vSeguimientoSolicitud
{
    public long IdSolicitudFV { get; set; }

    public DateTime FechaSolicitudFV { get; set; }

    public long IdUserSolicitaFV { get; set; }

    public string? UserSolicitaFV { get; set; }

    public int IdCatEstatusSolicitudFV { get; set; }

    public string EstatusFV { get; set; } = null!;

    public bool ActivoFV { get; set; }

    public DateTime FechaRegistroFV { get; set; }

    public long IdAlmacenFV { get; set; }

    public string AlmacenFV { get; set; } = null!;

    public long? IdVerificentro { get; set; }

    public long? IdUserRegistroIFV { get; set; }

    public DateTime? FechaEntregaIFV { get; set; }

    public string? NombreRecibioIFV { get; set; }

    public long? IdIngresoFV { get; set; }

    public string? UrlDoc1IFV { get; set; }

    public string? UrlDoc2IFV { get; set; }

    public string? UrlDoc3IFV { get; set; }

    public long? IdVentaFV { get; set; }

    public long? IdUserRegistroVFV { get; set; }

    public DateTime? FechaRegistroVFV { get; set; }

    public DateTime? FechaVentaVFV { get; set; }

    public string? NumeroCompraVFV { get; set; }

    public string? NumeroReferenciaVFV { get; set; }

    public string? UrlDoc1VFV { get; set; }

    public string? UrlDoc2VFV { get; set; }

    public string? UrlDoc3VFV { get; set; }

    public decimal? ImporteTotal { get; set; }
}
