using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vFoliosCancelado
{
    public long IdFormaValorada { get; set; }

    public string Folio { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public int? IdCatMotivoCancelacion { get; set; }

    public string? Motivo { get; set; }

    public long IdUserCancelo { get; set; }

    public string? UsuarioAprobo { get; set; }

    public int IdCatTipoTramite { get; set; }

    public string? TipoTramite { get; set; }

    public string DatosVehiculo { get; set; } = null!;

    public int IdCatEstatusFV { get; set; }

    public string? EstatusFV { get; set; }

    public string? OtroMotivo { get; set; }

    public string PersonaRealizoTramite { get; set; } = null!;
}
