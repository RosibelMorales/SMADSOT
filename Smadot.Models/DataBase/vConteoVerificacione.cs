using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vConteoVerificacione
{
    public long IdVerificentro { get; set; }

    public string NombreVerificentro { get; set; } = null!;

    public string? Clave { get; set; }

    public DateTime? Fecha { get; set; }

    public int Aprobadas { get; set; }

    public int Rechazadas { get; set; }

    public int NoProcesadas { get; set; }

    public int Todas { get; set; }
}
