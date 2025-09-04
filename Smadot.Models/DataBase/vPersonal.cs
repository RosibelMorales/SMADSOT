using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vPersonal
{
    public long Id { get; set; }

    public string? Nombre { get; set; }

    public string Genero { get; set; } = null!;

    public string Rfc { get; set; } = null!;

    public string Curp { get; set; } = null!;

    public string? NombrePuesto { get; set; }

    public string? TipoPuesto { get; set; }

    public DateTime FechaIncorporacionPuesto { get; set; }

    public string NumeroTrabajador { get; set; } = null!;

    public DateTime? FechaSeparacionPuesto { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime FechaCapacitacionInicio { get; set; }

    public DateTime FechaCapacitacionFinal { get; set; }

    public DateTime? FechaAcreditacionNorma { get; set; }

    public int IdPuesto { get; set; }

    public DateTime FechaNacimiento { get; set; }

    public string UrlIne { get; set; } = null!;

    public string UrlFoto { get; set; } = null!;

    public string UrlSeguroSocial { get; set; } = null!;

    public string UrlFirma { get; set; } = null!;

    public long IdPuestoVerificentro { get; set; }

    public string? CorreoUsuario { get; set; }

    public string UrlContrato { get; set; } = null!;

    public long IdVerificentro { get; set; }

    public string? TelefonoUsuario { get; set; }

    public int IdCatEstatusPuesto { get; set; }

    public int? IdNormaAcreditacion { get; set; }
}
