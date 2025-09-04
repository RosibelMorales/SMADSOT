using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class SolicitudFormaValoradum
{
    public long Id { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public long IdUserSolicita { get; set; }

    public string Estatus { get; set; } = null!;

    public DateTime? FechaEntrega { get; set; }

    public long? IdUserRecibe { get; set; }

    public decimal DobleCeroCantidad { get; set; }

    public decimal DobleCeroFolioInicial { get; set; }

    public decimal DobleCeroFolioFinal { get; set; }

    public string DobleCeroClaveCertificado { get; set; } = null!;

    public decimal CeroCantidad { get; set; }

    public decimal CeroFolioInicial { get; set; }

    public decimal CeroFolioFinal { get; set; }

    public string CeroClaveCertificado { get; set; } = null!;

    public decimal UnoCantidad { get; set; }

    public decimal UnoFolioInicial { get; set; }

    public decimal UnoFolioFinal { get; set; }

    public string UnoClaveCertificado { get; set; } = null!;

    public decimal DosCantidad { get; set; }

    public decimal DosFolioInicial { get; set; }

    public decimal DosFolioFinal { get; set; }

    public string DosClaveCertificado { get; set; } = null!;

    public decimal ConstanciaNoAprobadoCantidad { get; set; }

    public decimal ConstanciaNoAprobadoFolioInicial { get; set; }

    public decimal ConstanciaNoAprobadoFolioFinal { get; set; }

    public string ConstanciaNoAprobadoClaveCertificado { get; set; } = null!;

    public decimal ExentosCantidad { get; set; }

    public decimal ExentosFolioInicial { get; set; }

    public decimal ExentosFolioFinal { get; set; }

    public string ExentosClaveCertificado { get; set; } = null!;

    public virtual User? IdUserRecibeNavigation { get; set; }

    public virtual User IdUserSolicitaNavigation { get; set; } = null!;
}
