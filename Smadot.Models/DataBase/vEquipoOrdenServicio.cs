using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vEquipoOrdenServicio
{
    public long Id { get; set; }

    public string NombreEquipo { get; set; } = null!;

    public string NumeroSerie { get; set; } = null!;

    public string? NombreEstatusEquipo { get; set; }

    public long IdUserRegistro { get; set; }

    public string? NombreUsuarioRegistro { get; set; }

    public long? IdLinea { get; set; }

    public string? NombreLinea { get; set; }

    public long IdVerificentro { get; set; }

    public int IdCatTipoEquipo { get; set; }

    public string? NombreVerificentro { get; set; }

    public int? IdEstatusEquipo { get; set; }
}
