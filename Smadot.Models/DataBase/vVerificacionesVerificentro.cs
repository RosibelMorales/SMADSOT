using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVerificacionesVerificentro
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Clave { get; set; }

    public DateTime? Fecha { get; set; }

    public long? IdVerificacion { get; set; }

    public long? Folio { get; set; }
}
