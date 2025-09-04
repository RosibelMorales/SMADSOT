using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CatTipoCausaRechazo
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public bool? Activo { get; set; }
}
