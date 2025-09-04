using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class ConstanciaUltimaVerificacion
{
    public long Id { get; set; }

    public long? IdVerificacion { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdUserRegistro { get; set; }

    public string Placa { get; set; } = null!;

    public string Serie { get; set; } = null!;

    public string NumeroReferencia { get; set; } = null!;

    public string? UrlDoc1 { get; set; }

    public string? UrlDoc2 { get; set; }

    public string? UrlDoc3 { get; set; }

    public string? CodigoQr { get; set; }

    public int ConsecutivoTramite { get; set; }

    public DateTime FechaEmisionRef { get; set; }

    public DateTime FechaPago { get; set; }

    public string ClaveTramite { get; set; } = null!;

    public string EntidadProcedencia { get; set; } = null!;

    public string ServidorPublico { get; set; } = null!;

    public decimal ImporteActual { get; set; }

    public string Marca { get; set; } = null!;

    public string Submarca { get; set; } = null!;

    public DateTime Vigencia { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int Modelo { get; set; }

    public long IdVerificentro { get; set; }

    public string Combustible { get; set; } = null!;

    public string? TarjetaCirculacion { get; set; }

    public string? NombrePropietario { get; set; }

    public DateTime FechaVerificacion { get; set; }

    public string FolioVerificacion { get; set; } = null!;

    public virtual CatTipoCertificado IdCatTipoCertificadoNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;

    public virtual Verificacion? IdVerificacionNavigation { get; set; }

    public virtual Verificentro IdVerificentroNavigation { get; set; } = null!;
}
