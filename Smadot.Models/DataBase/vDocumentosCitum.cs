using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vDocumentosCitum
{
    public long IdCitaVerificacion { get; set; }

    public int Id { get; set; }

    public string URLIdentificacion { get; set; } = null!;

    public string FolioTarjetaCirculacion { get; set; } = null!;

    public int IdCatTipoServicio { get; set; }

    public string URLFactura { get; set; } = null!;

    public string URLCartaFactura { get; set; } = null!;

    public string? URLAltaPlacas { get; set; }

    public string? URLBajaPlacas { get; set; }

    public string URLValidacionCertificado { get; set; } = null!;

    public bool CambioPlacas { get; set; }

    public string ColorVehiculo { get; set; } = null!;

    public int IdCatMarcaVehiculo { get; set; }

    public string? Marca { get; set; }

    public int IdCatSubmarcaVehiculo { get; set; }

    public string? SubMarca { get; set; }

    public int CILINDRADA { get; set; }

    public int CILINDROS { get; set; }

    public long? IdCatSubDiesel { get; set; }

    public int? IdMarcaDiesel { get; set; }

    public string? MarcaDiesel { get; set; }

    public string? Subdiesel { get; set; }
}
