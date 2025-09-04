using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVigenciaVerificacion
{
    public long IdRegistro { get; set; }

    public DateTime Fecha { get; set; }

    public string? Placa { get; set; }

    public string Serie { get; set; } = null!;

    public DateTime? Vigencia { get; set; }

    public string Marca { get; set; } = null!;

    public string Submarca { get; set; } = null!;
}
