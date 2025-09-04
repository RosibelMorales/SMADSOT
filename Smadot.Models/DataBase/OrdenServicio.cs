using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class OrdenServicio
{
    public long Id { get; set; }

    public int IdCatTipoOrdenServicio { get; set; }

    public long FolioOS { get; set; }

    public string UrlOS { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public long IdUserRegistro { get; set; }

    public long IdEquipo { get; set; }

    public string Motivo { get; set; } = null!;

    public string Solucion { get; set; } = null!;

    public string? NumeroSerieActual { get; set; }

    public string? NumeroSerieAnterior { get; set; }

    public long? IdFolioProveedor { get; set; }

    public virtual CatTipoOrdenServicio IdCatTipoOrdenServicioNavigation { get; set; } = null!;

    public virtual Equipo IdEquipoNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;
}
