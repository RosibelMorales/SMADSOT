using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vSolicitudFormaValoradum
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

    public long IdSC { get; set; }

    public int IdCatTipoCertificadoSC { get; set; }

    public string TipoCertificadoSC { get; set; } = null!;

    public string? ClaveCertificadoSC { get; set; }

    public int CantidadSC { get; set; }

    public DateTime? FechaEntregaIFV { get; set; }

    public string? NombreRecibioIFV { get; set; }

    public long? IdIngresoFV { get; set; }

    public bool? HasCaja { get; set; }

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }
}
