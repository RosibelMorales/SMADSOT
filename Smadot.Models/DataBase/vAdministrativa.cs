using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vAdministrativa
{
    public long? Id { get; set; }

    public long IdAdministrativa { get; set; }

    public string? TipoCertificadoFV { get; set; }

    public long? FolioCertificado { get; set; }

    public string? ClaveTramite { get; set; }

    public string? NombreVerificentro { get; set; }

    public string? ApiEndPoint { get; set; }

    public string? ApiKey { get; set; }

    public long? IdVerificentro { get; set; }

    public decimal? ImporteActual { get; set; }

    public DateTime Fecha { get; set; }

    public string FolioAnterior { get; set; } = null!;

    public long IdUserCapturista { get; set; }

    public string? NumeroTrabajadorCapturista { get; set; }

    public int? IdUserTecnico { get; set; }

    public int? NumeroTrabajadorTecnico { get; set; }

    public string Propietario { get; set; } = null!;

    public int? CausaRechazo { get; set; }

    public int? MotivoVerificacion { get; set; }

    public int IdCatMotivoTramite { get; set; }

    public string? MotivoTramite { get; set; }

    public string Placa { get; set; } = null!;

    public string Serie { get; set; } = null!;

    public string NumeroReferencia { get; set; } = null!;

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }

    public string? UrlDoc4 { get; set; }

    public string FolioAsignado { get; set; } = null!;

    public long IdUserRegistro { get; set; }

    public string? UsuarioRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime Vigencia { get; set; }

    public string NombrePropietario { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string Submarca { get; set; } = null!;

    public int Modelo { get; set; }

    public string TarjetaCirculacion { get; set; } = null!;

    public int IdTipoCombustible { get; set; }

    public string Combustible { get; set; } = null!;

    public int? IdCatTipoCertificado { get; set; }

    public string? EntidadProcedencia { get; set; }

    public DateTime? FechaEmisionRef { get; set; }

    public DateTime? FechaPago { get; set; }
}
