using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVerificentro
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? NombreCorto { get; set; }

    public bool Activo { get; set; }

    public string? Clave { get; set; }

    public string? Direccion { get; set; }

    public string? Rfc { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? GerenteTecnico { get; set; }

    public string? RepresentanteLegal { get; set; }

    public decimal? Longitud { get; set; }

    public decimal? Latitud { get; set; }

    public string ApiKey { get; set; } = null!;

    public string ApiEndPoint { get; set; } = null!;

    public string DirectorGestionCalidadAire { get; set; } = null!;

    public long? IdAutorizadoVerificentro { get; set; }

    public string? NombreAutorizadoVerificentro { get; set; }

    public string? TelefonoAutorizadoVerificentro { get; set; }

    public string? CorreoAutorizadoVerificentro { get; set; }
}
