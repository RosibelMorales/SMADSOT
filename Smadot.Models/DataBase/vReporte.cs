using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vReporte
{
    public long IdReporte { get; set; }

    public long NumeroReporte { get; set; }

    public DateTime FechaRegistroReporte { get; set; }

    public long? IdUsuario { get; set; }

    public string? NombreUsuario { get; set; }

    public int PruebasRealizadas { get; set; }

    public int PruebasAprobadas { get; set; }

    public int PruebasReprobadas { get; set; }

    public int Entregados { get; set; }

    public int Cancelados { get; set; }

    public int SinUsar { get; set; }

    public int ServicioTransportePublico { get; set; }

    public int ServicioTransporteMercantil { get; set; }

    public string UrlEntrega { get; set; } = null!;

    public string UrlExcel { get; set; } = null!;

    public DateTime FechaModificacionReporte { get; set; }

    public int? IdCatEstatusReporte { get; set; }

    public string? NombreCatEstatusReporte { get; set; }

    public bool? ActivoCatEstatusReporte { get; set; }
}
