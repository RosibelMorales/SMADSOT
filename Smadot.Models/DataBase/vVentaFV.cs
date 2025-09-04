using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVentaFV
{
    public long Id { get; set; }

    public long NumeroVenta { get; set; }

    public DateTime FechaVenta { get; set; }

    public string Verificentro { get; set; } = null!;

    public string? UserRegistro { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int Cantidad { get; set; }

    public string Folios { get; set; } = null!;

    public decimal ImporteTotal { get; set; }

    public string Certificado { get; set; } = null!;

    public string? ClaveCertificado { get; set; }

    public long IdAlmacen { get; set; }

    public string NumeroReferencia { get; set; } = null!;

    public string NumeroCompra { get; set; } = null!;

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }

    public string NombreAlmacen { get; set; } = null!;

    public string? Clave { get; set; }

    public string? Rfc { get; set; }

    public long IdVerificentro { get; set; }
}
