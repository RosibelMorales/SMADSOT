using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class DocumentosCitum
{
    public int Id { get; set; }

    public long IdCitaVerificacion { get; set; }

    public int IdCatTipoServicio { get; set; }

    public string URLIdentificacion { get; set; } = null!;

    public string FolioTarjetaCirculacion { get; set; } = null!;

    public string URLFactura { get; set; } = null!;

    public string URLCartaFactura { get; set; } = null!;

    public string URLValidacionCertificado { get; set; } = null!;

    public bool CambioPlacas { get; set; }

    public string? URLBajaPlacas { get; set; }

    public string? URLAltaPlacas { get; set; }

    public DateTime FechaRecepcion { get; set; }

    public long IdUserRegistro { get; set; }

    public DateTime? FechaFacturacion { get; set; }

    public virtual CatTipoServicioCitum IdCatTipoServicioNavigation { get; set; } = null!;

    public virtual CitaVerificacion IdCitaVerificacionNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;
}
