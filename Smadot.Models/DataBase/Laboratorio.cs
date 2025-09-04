using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Laboratorio
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string CorreoElectronico { get; set; } = null!;

    public byte[] NumeroTelefono { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Empresa { get; set; } = null!;

    public bool Autorizado { get; set; }
}
