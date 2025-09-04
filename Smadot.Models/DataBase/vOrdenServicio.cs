using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vOrdenServicio
{
    public long Id { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long FolioOS { get; set; }

    public string Solucion { get; set; } = null!;

    public string UrlOS { get; set; } = null!;

    public long IdUserRegistro { get; set; }

    public string Motivo { get; set; } = null!;

    public string? NumeroSerieActual { get; set; }

    public string? NumeroSerieAnterior { get; set; }

    public string? NombreUsuario { get; set; }

    public long IdVerificentro { get; set; }

    public string NombreVerificentro { get; set; } = null!;

    public long IdEquipo { get; set; }

    public string NombreEquipo { get; set; } = null!;

    public long? IdLinea { get; set; }

    public string? NombreLinea { get; set; }

    public int IdCatTipoOrdenServicio { get; set; }

    public string CatNombreTipoOrden { get; set; } = null!;

    public string NombreTecnico { get; set; } = null!;

    public string Empresa { get; set; } = null!;
}
