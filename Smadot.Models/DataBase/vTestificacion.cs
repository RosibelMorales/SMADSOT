using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vTestificacion
{
    public long Id { get; set; }

    public string NumeroRef { get; set; } = null!;

    public string Propietario { get; set; } = null!;

    public string Serie { get; set; } = null!;

    public string Placa { get; set; } = null!;

    public int Modelo { get; set; }

    public string Marca { get; set; } = null!;

    public string SubMarca { get; set; } = null!;

    public string FolioOrigen { get; set; } = null!;

    public DateTime VigenciaOrigen { get; set; }

    public string PersonaTramite { get; set; } = null!;

    public string? UrlCertificado { get; set; }

    public string? UrlIdentificacion { get; set; }

    public string? UrlTarjetaCirculacion { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string Combustible { get; set; } = null!;

    public string TarjetaCirculacion { get; set; } = null!;

    public int IdTipoCombustible { get; set; }

    public int IdTipoCertificadoOrigen { get; set; }

    public long IdFolio { get; set; }

    public string EntidadProcedencia { get; set; } = null!;

    public DateTime FechaPago { get; set; }

    public DateTime FechaEmisionRef { get; set; }

    public long Folio { get; set; }
}
