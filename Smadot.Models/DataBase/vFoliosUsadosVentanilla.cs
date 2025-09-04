using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vFoliosUsadosVentanilla
{
    public DateTime Fecha { get; set; }

    public string ClaveTramite { get; set; } = null!;

    public bool Cancelado { get; set; }

    public long FolioTramite { get; set; }

    public string TipoTramite { get; set; } = null!;

    public string? NombreUsuario { get; set; }

    public string CorreoUsuario { get; set; } = null!;

    public string DatosVehiculo { get; set; } = null!;

    public string FolioCertificado { get; set; } = null!;

    public string? Razon { get; set; }

    public string? PersonaGeneroTramite { get; set; }

    public decimal MontoTramite { get; set; }

    public string ReferenciaBancaria { get; set; } = null!;

    public long IdFormaValorada { get; set; }
}
