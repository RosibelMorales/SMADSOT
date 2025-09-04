using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Exento
{
    public long Id { get; set; }

    public string Marca { get; set; } = null!;

    public string Submarca { get; set; } = null!;

    public int Modelo { get; set; }

    public string Placa { get; set; } = null!;

    public string Serie { get; set; } = null!;

    public int IdCatTipoCertificado { get; set; }

    public DateTime Vigencia { get; set; }

    public DateTime FechaCartaFactura { get; set; }

    public string Propietario { get; set; } = null!;

    public string Combustible { get; set; } = null!;

    public string NumTarjetaCirculacion { get; set; } = null!;

    public string UltimoFolio { get; set; } = null!;

    public DateTime? VigenciaHoloAnterior { get; set; }

    public string NumeroReferencia { get; set; } = null!;

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }

    public string? UrlDoc4 { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdUser { get; set; }

    public virtual ICollection<FoliosFormaValoradaVerificentro> FoliosFormaValoradaVerificentros { get; } = new List<FoliosFormaValoradaVerificentro>();

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;

    public virtual ICollection<Refrendo> Refrendos { get; } = new List<Refrendo>();
}
