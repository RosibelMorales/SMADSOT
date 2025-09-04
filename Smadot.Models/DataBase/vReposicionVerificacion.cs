using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vReposicionVerificacion
{
    public long? IdVerificacion { get; set; }

    public long FolioFormaValorada { get; set; }

    public long IdReposicion { get; set; }

    public DateTime Fecha { get; set; }

    public string? Placa { get; set; }

    public string? Serie { get; set; }

    public string? FolioCertificado { get; set; }

    public DateTime? Vigencia { get; set; }

    public string TipoCertificado { get; set; } = null!;

    public string Semestre { get; set; } = null!;

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public string Combustible { get; set; } = null!;

    public string? TarjetaCirculacion { get; set; }

    public string TipoCertificadoFV { get; set; } = null!;

    public int? Anio { get; set; }

    public string NumeroReferencia { get; set; } = null!;

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }

    public long FolioFoliosFormaValoradaVerificentro { get; set; }

    public long IdFoliosFormaValoradaVerificentro { get; set; }

    public string EntidadProcedencia { get; set; } = null!;
}
