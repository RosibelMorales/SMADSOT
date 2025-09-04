using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVerificacionLinea
{
    public long Id { get; set; }

    public long IdVerificentro { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdCatEstatusLinea { get; set; }

    public long? IdVerificacion { get; set; }

    public DateTime? Fecha { get; set; }

    public DateTime? FechaVerificacion { get; set; }

    public DateTime? Vigencia { get; set; }

    public string? Placa { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public long? IdResultadosVerificacion { get; set; }

    public int? Orden { get; set; }
}
