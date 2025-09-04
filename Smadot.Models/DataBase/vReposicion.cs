using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vReposicion
{
    public long IdReposicion { get; set; }

    public string? ClaveTramite { get; set; }

    public long? IdVerificacion { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string NumeroReferencia { get; set; } = null!;

    public long IdUserRegistro { get; set; }

    public string? UsuarioRegistro { get; set; }

    public string? Placa { get; set; }

    public string? Serie { get; set; }

    public long? Folio { get; set; }

    public long? IdFoliosFormaValoradaVerificentro { get; set; }
}
