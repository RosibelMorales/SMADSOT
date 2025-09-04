using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class FormaValoradum
{
    public long Id { get; set; }

    public string Folio { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public int IdCatTipoTramite { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public int? IdCatMotivoCancelacion { get; set; }

    public string? AproboCancelacion { get; set; }

    public int IdCatEstatusFV { get; set; }

    public long IdUserCancelo { get; set; }

    public long IdCicloVerificacion { get; set; }

    public long IdVerificentro { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public string? Marca { get; set; }

    public string? Submarca { get; set; }

    public string? Modelo { get; set; }

    public string? Color { get; set; }

    public string? OtroMotivo { get; set; }

    public string? NombreUsuario { get; set; }

    public string? TelefonoUsuario { get; set; }

    public string? CorreoUsuario { get; set; }

    public string? FolioCertificado { get; set; }

    public string? Razon { get; set; }

    public string? PersonaGeneroTramite { get; set; }

    public decimal? MontoTramite { get; set; }

    public string? ReferenciaBancaria { get; set; }

    public string? Cvv { get; set; }

    public string? PersonaRecibeCertificados { get; set; }

    public string? ClaveVenta { get; set; }

    public int? Total { get; set; }

    public int? FoliosStock { get; set; }

    public int? FoliosUsados { get; set; }

    public string? ClaveCertificado { get; set; }

    public string? ClaveDevolucion { get; set; }

    public string? FolioInicial { get; set; }

    public string? ResponsableEntrega { get; set; }

    public string? PersonaValido { get; set; }

    public int? FolioVenta { get; set; }

    public string? ClaveSolicitud { get; set; }

    public string? FolioFinal { get; set; }

    public int? FoliosFV { get; set; }

    public virtual CatEstatusFV IdCatEstatusFVNavigation { get; set; } = null!;

    public virtual CatMotivoCancelacion? IdCatMotivoCancelacionNavigation { get; set; }

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;

    public virtual CatTipoTramite IdCatTipoTramiteNavigation { get; set; } = null!;

    public virtual CicloVerificacion IdCicloVerificacionNavigation { get; set; } = null!;

    public virtual User IdUserCanceloNavigation { get; set; } = null!;

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;
}
