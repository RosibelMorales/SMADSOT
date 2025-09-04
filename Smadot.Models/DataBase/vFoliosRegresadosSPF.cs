using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vFoliosRegresadosSPF
{
    public DateTime Fecha { get; set; }

    public string? ClaveCertificado { get; set; }

    public string? ClaveDevolucion { get; set; }

    public string? ClaveSolicitud { get; set; }

    public string? ClaveVenta { get; set; }

    public string? TipoCertificado { get; set; }

    public string? FolioInicial { get; set; }

    public string? FolioFinal { get; set; }

    public string? ResponsableEntrega { get; set; }

    public string? PersonaValido { get; set; }

    public long IdFormaValorada { get; set; }
}
