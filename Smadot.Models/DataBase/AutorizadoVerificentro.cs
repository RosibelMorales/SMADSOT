using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class AutorizadoVerificentro
{
    public long Id { get; set; }

    public long IdVerificentro { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;
}
