using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vPendientesImprimir
{
    public long? IdVerificacion { get; set; }

    public long? IdAdministrativa { get; set; }

    public long? IdExento { get; set; }

    public long? IdRefrendoExento { get; set; }

    public long? IdTestificacion { get; set; }

    public long? IdFolioFormaValorada { get; set; }

    public long? FolioFoliosFormaValoradaVerificentro { get; set; }

    public string? NombreVerificentro { get; set; }

    public string? ApiEndPoint { get; set; }

    public string? ApiKey { get; set; }

    public long? IdVerificentro { get; set; }

    public bool? Impreso { get; set; }

    public string? EntidadProcedencia { get; set; }

    public string NumeroReferencia { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public string? Placa { get; set; }

    public string Serie { get; set; } = null!;

    public DateTime Vigencia { get; set; }

    public string Marca { get; set; } = null!;

    public string Submarca { get; set; } = null!;

    public string Combustible { get; set; } = null!;

    public string TarjetaCirculacion { get; set; } = null!;

    public int Modelo { get; set; }

    public string? FolioAnterior { get; set; }

    public string? NombreCapturista { get; set; }

    public long IdUserCapturista { get; set; }

    public string? NumeroTrabajadorCapturista { get; set; }

    public string? NombreTecnico { get; set; }

    public long? IdUserTecnico { get; set; }

    public string? NumeroTrabajadorTecnico { get; set; }

    public string? Propietario { get; set; }
}
