using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CatSubdieselVehiculo
{
    public int? IdCatMarcaVehiculo { get; set; }

    public string Nombre { get; set; } = null!;

    public int Clave { get; set; }

    public int POTMAX_RPM { get; set; }

    public int RAL_FAB { get; set; }

    public int GOB_FAB { get; set; }

    public long Id { get; set; }

    public virtual ICollection<Verificacion> Verificacions { get; } = new List<Verificacion>();
}
