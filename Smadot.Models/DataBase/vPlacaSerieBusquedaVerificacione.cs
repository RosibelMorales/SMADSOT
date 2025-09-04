using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vPlacaSerieBusquedaVerificacione
{
    public string? Placa { get; set; }

    public string? Serie { get; set; }

    public long IdVerificentro { get; set; }
}
