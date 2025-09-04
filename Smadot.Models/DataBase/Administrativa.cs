using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Administrativa
{
    public long Id { get; set; }

    public int IdCatMotivoTramite { get; set; }

    public string Placa { get; set; } = null!;

    public string Serie { get; set; } = null!;

    public string NumeroReferencia { get; set; } = null!;

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }

    public string FolioAsignado { get; set; } = null!;

    public long IdUserRegistro { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime Vigencia { get; set; }

    public string NombrePropietario { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string Submarca { get; set; } = null!;

    public int Modelo { get; set; }

    public string TarjetaCirculacion { get; set; } = null!;

    public int IdTipoCombustible { get; set; }

    public string Combustible { get; set; } = null!;

    public string? UrlDoc4 { get; set; }

    public virtual ICollection<FoliosFormaValoradaVerificentro> FoliosFormaValoradaVerificentros { get; } = new List<FoliosFormaValoradaVerificentro>();

    public virtual CatMotivoTramite IdCatMotivoTramiteNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;
}
