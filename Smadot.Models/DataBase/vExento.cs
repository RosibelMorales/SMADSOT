using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vExento
{
    public long Id { get; set; }

    public string Serie { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string Submarca { get; set; } = null!;

    public int Modelo { get; set; }

    public string? EntidadProcedencia { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdUser { get; set; }

    public string Combustible { get; set; } = null!;

    public string? NombresUser { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public string? TipoCertificado { get; set; }

    public long? Folio { get; set; }

    public string? ClaveTramite { get; set; }

    public long? IdFolioFormaValoradaVerificentro { get; set; }

    public DateTime? Vigencia { get; set; }

    public string? Placa { get; set; }

    public string? NumeroReferencia { get; set; }

    public string? Propietario { get; set; }
}
