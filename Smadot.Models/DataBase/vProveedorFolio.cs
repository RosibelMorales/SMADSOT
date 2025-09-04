using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vProveedorFolio
{
    public long? FolioOS { get; set; }

    public string ProveedorEmpresa { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }

    public string Proveedor { get; set; } = null!;

    public string? Motivo { get; set; }

    public string? Equipo { get; set; }

    public bool? EstatusFolio { get; set; }

    public long Id { get; set; }

    public long? FolioPF { get; set; }

    public bool EsLaboratorio { get; set; }

    public long? IdVerificentro { get; set; }
}
