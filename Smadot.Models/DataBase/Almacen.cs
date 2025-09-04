using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Almacen
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public long? IdVerificentro { get; set; }

    public virtual ICollection<AsignacionSV> AsignacionSVs { get; } = new List<AsignacionSV>();

    public virtual ICollection<DevolucionSPF> DevolucionSPFs { get; } = new List<DevolucionSPF>();

    public virtual Verificentro? IdVerificentroNavigation { get; set; }

    public virtual ICollection<IngresoFV> IngresoFVs { get; } = new List<IngresoFV>();

    public virtual ICollection<Inventario> Inventarios { get; } = new List<Inventario>();

    public virtual ICollection<SolicitudFV> SolicitudFVs { get; } = new List<SolicitudFV>();

    public virtual ICollection<StockMinimo> StockMinimos { get; } = new List<StockMinimo>();
}
