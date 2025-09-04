using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vAlertum
{
    public long Id { get; set; }

    public string TableName { get; set; } = null!;

    public long TableId { get; set; }

    public long IdVerificentro { get; set; }

    public string Verificentro { get; set; } = null!;

    public int? IdEstatusFinal { get; set; }

    public int IdEstatusInicial { get; set; }

    public DateTime Fecha { get; set; }

    public long? IdUser { get; set; }

    public string? Data { get; set; }

    public string MovimientoInicial { get; set; } = null!;

    public string? MovimientoFinal { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? Nombre { get; set; }

    public bool Leido { get; set; }

    public bool Procesada { get; set; }
}
