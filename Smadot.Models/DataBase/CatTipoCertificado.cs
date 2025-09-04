using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CatTipoCertificado
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? ClaveCertificado { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<AsignacionCertificado> AsignacionCertificados { get; } = new List<AsignacionCertificado>();

    public virtual ICollection<Caja> Cajas { get; } = new List<Caja>();

    public virtual ICollection<ConstanciaUltimaVerificacion> ConstanciaUltimaVerificacions { get; } = new List<ConstanciaUltimaVerificacion>();

    public virtual ICollection<DevolucionCertificado> DevolucionCertificados { get; } = new List<DevolucionCertificado>();

    public virtual ICollection<Exento> Exentos { get; } = new List<Exento>();

    public virtual ICollection<FoliosFormaValoradaActuale> FoliosFormaValoradaActuales { get; } = new List<FoliosFormaValoradaActuale>();

    public virtual ICollection<FoliosFormaValoradaVerificentro> FoliosFormaValoradaVerificentros { get; } = new List<FoliosFormaValoradaVerificentro>();

    public virtual ICollection<FormaValoradum> FormaValorada { get; } = new List<FormaValoradum>();

    public virtual ICollection<IngresoCertificado> IngresoCertificados { get; } = new List<IngresoCertificado>();

    public virtual ICollection<Inventario> Inventarios { get; } = new List<Inventario>();

    public virtual ICollection<ReporteEntregaTercerCertificado> ReporteEntregaTercerCertificados { get; } = new List<ReporteEntregaTercerCertificado>();

    public virtual ICollection<ReporteFVCanceladum> ReporteFVCancelada { get; } = new List<ReporteFVCanceladum>();

    public virtual ICollection<SolicitudCertificado> SolicitudCertificados { get; } = new List<SolicitudCertificado>();

    public virtual ICollection<StockCertificado> StockCertificados { get; } = new List<StockCertificado>();

    public virtual ICollection<VentaCertificado> VentaCertificados { get; } = new List<VentaCertificado>();

    public virtual ICollection<Verificacion> Verificacions { get; } = new List<Verificacion>();
}
