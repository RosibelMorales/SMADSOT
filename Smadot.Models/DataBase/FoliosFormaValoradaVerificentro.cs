using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class FoliosFormaValoradaVerificentro
{
    public long Id { get; set; }

    public long Folio { get; set; }

    public long IdVerificentro { get; set; }

    public decimal ImporteActual { get; set; }

    public long? IdVerificacion { get; set; }

    public bool Cancelado { get; set; }

    public int? IdCatTipoCertificado { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public int? IdCatMotivoCancelacion { get; set; }

    public string? OtroMotivo { get; set; }

    public long? IdUserCancelo { get; set; }

    public int? ConsecutivoTramiteCancelado { get; set; }

    public string? ClaveTramiteCancelado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdUserRegistro { get; set; }

    public int? IdCatTipoTramite { get; set; }

    public bool Impreso { get; set; }

    public int ConsecutivoTramite { get; set; }

    public DateTime FechaEmisionRef { get; set; }

    public DateTime FechaPago { get; set; }

    public string ClaveTramite { get; set; } = null!;

    public string EntidadProcedencia { get; set; } = null!;

    public string ServidorPublico { get; set; } = null!;

    public bool Reposicion { get; set; }

    public long? IdExento { get; set; }

    public long? IdAdministrativa { get; set; }

    public long? IdTestificacion { get; set; }

    public virtual Administrativa? IdAdministrativaNavigation { get; set; }

    public virtual CatMotivoCancelacion? IdCatMotivoCancelacionNavigation { get; set; }

    public virtual CatTipoCertificado? IdCatTipoCertificadoNavigation { get; set; }

    public virtual CatTipoTramite? IdCatTipoTramiteNavigation { get; set; }

    public virtual Exento? IdExentoNavigation { get; set; }

    public virtual Testificacion? IdTestificacionNavigation { get; set; }

    public virtual User? IdUserCanceloNavigation { get; set; }

    public virtual User IdUserRegistroNavigation { get; set; } = null!;

    public virtual Verificacion? IdVerificacionNavigation { get; set; }

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;

    public virtual ICollection<Refrendo> Refrendos { get; } = new List<Refrendo>();

    public virtual ICollection<Reposicion> Reposicions { get; } = new List<Reposicion>();
}
