using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Proveedor
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string CorreoElectronico { get; set; } = null!;

    public string NumeroTelefono { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Empresa { get; set; } = null!;

    public bool Autorizado { get; set; }

    public bool EsLaboratorio { get; set; }

    public virtual ICollection<Instalacion> Instalacions { get; } = new List<Instalacion>();

    public virtual ICollection<ProveedorFolio> ProveedorFolios { get; } = new List<ProveedorFolio>();
}
