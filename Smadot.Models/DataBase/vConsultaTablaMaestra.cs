using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vConsultaTablaMaestra
{
    public long IdTablaMaestra { get; set; }

    public long IdCicloVerificacion { get; set; }

    public string Marca { get; set; } = null!;

    public string Submarca { get; set; } = null!;

    public string Linea { get; set; } = null!;

    public int Modelo { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public decimal? ImporteFv { get; set; }
}
