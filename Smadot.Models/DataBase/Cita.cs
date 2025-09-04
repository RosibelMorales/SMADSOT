using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Cita
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? RazonSocial { get; set; }

    public string? Correo { get; set; }

    public DateTime Fecha { get; set; }

    public long IdCVV { get; set; }

    public string Placa { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public short Anio { get; set; }

    public bool Acepto { get; set; }

    public virtual Verificentro IdCVVNavigation { get; set; } = null!;
}
