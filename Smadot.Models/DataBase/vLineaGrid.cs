using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vLineaGrid
{
    public long Id { get; set; }

    public long IdVerificentro { get; set; }

    public string NombreVerificentro { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public long IdUserRegistro { get; set; }

    public string? NombreUsuario { get; set; }

    public string NombreLinea { get; set; } = null!;

    public DateTime FechaRegistroLinea { get; set; }

    public long? IdLineaMotivo { get; set; }

    public string Estatus { get; set; } = null!;

    public DateTime? FechaRegistroLineaMotivo { get; set; }

    public string Motivo { get; set; } = null!;

    public string? Notas { get; set; }

    public bool? Activa { get; set; }

    public string? UrlDocumento { get; set; }
}
