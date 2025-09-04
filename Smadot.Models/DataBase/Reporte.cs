using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Reporte
{
    public long Id { get; set; }

    public long Numero { get; set; }

    public DateTime FechaRegistro { get; set; }

    public long IdUserRegistro { get; set; }

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

    public DateTime UltimaModificacion { get; set; }

    public long IdUserModifico { get; set; }

    public int IdCatEstatusReporte { get; set; }

    public long? IdVerificentro { get; set; }

    public virtual CatEstatusReporte IdCatEstatusReporteNavigation { get; set; } = null!;

    public virtual User IdUserRegistroNavigation { get; set; } = null!;

    public virtual Verificentro? IdVerificentroNavigation { get; set; }

    public virtual ICollection<Instalacion> Instalacions { get; } = new List<Instalacion>();

    public virtual ICollection<ReporteFVCanceladum> ReporteFVCancelada { get; } = new List<ReporteFVCanceladum>();

    public virtual ICollection<ReportePruebaRealizadum> ReportePruebaRealizada { get; } = new List<ReportePruebaRealizadum>();
}
