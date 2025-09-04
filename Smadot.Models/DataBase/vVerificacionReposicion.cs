using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVerificacionReposicion
{
    public long Id { get; set; }

    public long? IdVerificacion { get; set; }

    public long? IdExento { get; set; }

    public long? IdRefrendoExento { get; set; }

    public long? IdAdministrativa { get; set; }

    public long? IdTestificacion { get; set; }

    public long? IdConstanciaUltimaVerificacion { get; set; }

    public string TipoCertificadoFV { get; set; } = null!;

    public string ClaveTramite { get; set; } = null!;

    public int? IdCatTipoTramite { get; set; }

    public int? IdCatTipoCertificado { get; set; }

    public bool? Cancelado { get; set; }

    public long? IdReposicion { get; set; }

    public string TipoCertificado { get; set; } = null!;

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }

    public long FolioFoliosFormaValoradaVerificentro { get; set; }

    public string NombreVerificentro { get; set; } = null!;

    public string ApiEndPoint { get; set; } = null!;

    public string ApiKey { get; set; } = null!;

    public long IdFoliosFormaValoradaVerificentro { get; set; }

    public long IdVerificentro { get; set; }

    public long FolioCertificado { get; set; }

    public decimal ImporteActual { get; set; }

    public bool? Impreso { get; set; }

    public string EntidadProcedencia { get; set; } = null!;

    public string? NumeroReferencia { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public string? Placa { get; set; }

    public string? Serie { get; set; }

    public DateTime? Vigencia { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public string? Combustible { get; set; }

    public string? TarjetaCirculacion { get; set; }

    public int? Anio { get; set; }

    public string? FolioCertificadoAnterior { get; set; }

    public string? NombreTecnico { get; set; }

    public long? IdUserTecnico { get; set; }

    public string? Propietario { get; set; }

    public int? IdCatMotivoTramite { get; set; }

    public string? MotivoTramite { get; set; }

    public int? IdMotivoVerificacion { get; set; }
}
