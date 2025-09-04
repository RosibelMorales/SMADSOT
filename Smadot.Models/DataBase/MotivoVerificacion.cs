using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class MotivoVerificacion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Verificacion> Verificacions { get; } = new List<Verificacion>();
}
