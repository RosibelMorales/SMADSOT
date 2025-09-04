using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CargaMasiva
{
    public long Id { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string NombreArchivo { get; set; } = null!;

    public int NumeroRegistros { get; set; }

    public long IdUserRegistro { get; set; }

    public string Base64String { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;
}
