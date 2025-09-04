using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vDataTramitesVentanilla
{
    public long? Id { get; set; }

    public long? IdVerificacion { get; set; }

    public long? IdAdministrativa { get; set; }

    public long? IdExento { get; set; }

    public long? IdTestificacion { get; set; }

    public long? IdConstanciaUltimaVerificacion { get; set; }

    public string? TipoCertificadoFV { get; set; }

    public long? FolioCertificado { get; set; }

    public string? ClaveTramite { get; set; }

    public int? IdCatTipoTramite { get; set; }

    public int? IdCatTipoCertificado { get; set; }

    public string? NombreVerificentro { get; set; }

    public string? ApiEndPoint { get; set; }

    public string? ApiKey { get; set; }

    public long? IdVerificentro { get; set; }

    public decimal? ImporteActual { get; set; }

    public string? EntidadProcedencia { get; set; }

    public string? NumeroReferencia { get; set; }

    public DateTime Fecha { get; set; }

    public string? Placa { get; set; }

    public string Serie { get; set; } = null!;

    public DateTime? Vigencia { get; set; }

    public string Marca { get; set; } = null!;

    public string Submarca { get; set; } = null!;

    public string Combustible { get; set; } = null!;

    public string? TarjetaCirculacion { get; set; }

    public int Modelo { get; set; }

    public string FolioAnterior { get; set; } = null!;

    public long? IdUserCapturista { get; set; }

    public string? NumeroTrabajadorCapturista { get; set; }

    public long? IdUserTecnico { get; set; }

    public string? NumeroTrabajadorTecnico { get; set; }

    public string? Propietario { get; set; }

    public string? CausaRechazo { get; set; }

    public string? MotivoVerificacion { get; set; }
}
