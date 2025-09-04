using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class User
{
    public long Id { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public string? Nombre { get; set; }

    public DateTime FechaNacimiento { get; set; }

    public string Genero { get; set; } = null!;

    public string Rfc { get; set; } = null!;

    public string Curp { get; set; } = null!;

    public string UrlIne { get; set; } = null!;

    public string UrlFoto { get; set; } = null!;

    public string UrlSeguroSocial { get; set; } = null!;

    public string UrlFirma { get; set; } = null!;

    public long? IdVerificentro { get; set; }

    public string? PersonId { get; set; }

    public string? PersonGroupId { get; set; }

    public virtual ICollection<Administrativa> Administrativas { get; } = new List<Administrativa>();

    public virtual ICollection<Alertum> Alerta { get; } = new List<Alertum>();

    public virtual ICollection<AsignacionSV> AsignacionSVs { get; } = new List<AsignacionSV>();

    public virtual ICollection<Calibracion> CalibracionIdUserRegistroNavigations { get; } = new List<Calibracion>();

    public virtual ICollection<Calibracion> CalibracionIdUserValidoNavigations { get; } = new List<Calibracion>();

    public virtual ICollection<CargaMasiva> CargaMasivas { get; } = new List<CargaMasiva>();

    public virtual ICollection<Circular> Circulars { get; } = new List<Circular>();

    public virtual ICollection<CitaVerificacion> CitaVerificacions { get; } = new List<CitaVerificacion>();

    public virtual ICollection<ConstanciaUltimaVerificacion> ConstanciaUltimaVerificacions { get; } = new List<ConstanciaUltimaVerificacion>();

    public virtual ICollection<DevolucionSPF> DevolucionSPFs { get; } = new List<DevolucionSPF>();

    public virtual ICollection<DocumentosCitum> DocumentosCita { get; } = new List<DocumentosCitum>();

    public virtual ICollection<Equipo> EquipoIdUserRegistroNavigations { get; } = new List<Equipo>();

    public virtual ICollection<Equipo> EquipoIdUserRevisoNavigations { get; } = new List<Equipo>();

    public virtual ICollection<EquipoTipoCalibracion> EquipoTipoCalibracionIdUserRegistroNavigations { get; } = new List<EquipoTipoCalibracion>();

    public virtual ICollection<EquipoTipoCalibracion> EquipoTipoCalibracionIdUserValidoNavigations { get; } = new List<EquipoTipoCalibracion>();

    public virtual ICollection<FoliosFormaValoradaVerificentro> FoliosFormaValoradaVerificentroIdUserCanceloNavigations { get; } = new List<FoliosFormaValoradaVerificentro>();

    public virtual ICollection<FoliosFormaValoradaVerificentro> FoliosFormaValoradaVerificentroIdUserRegistroNavigations { get; } = new List<FoliosFormaValoradaVerificentro>();

    public virtual ICollection<FormaValoradum> FormaValorada { get; } = new List<FormaValoradum>();

    public virtual Verificentro? IdVerificentroNavigation { get; set; }

    public virtual ICollection<IngresoFV> IngresoFVs { get; } = new List<IngresoFV>();

    public virtual ICollection<Instalacion> Instalacions { get; } = new List<Instalacion>();

    public virtual ICollection<LineaMotivo> LineaMotivos { get; } = new List<LineaMotivo>();

    public virtual ICollection<Linea> Lineas { get; } = new List<Linea>();

    public virtual ICollection<OrdenServicio> OrdenServicios { get; } = new List<OrdenServicio>();

    public virtual ICollection<Refrendo> Refrendos { get; } = new List<Refrendo>();

    public virtual ICollection<Reporte> Reportes { get; } = new List<Reporte>();

    public virtual ICollection<Reposicion> Reposicions { get; } = new List<Reposicion>();

    public virtual ICollection<SolicitudFV> SolicitudFVs { get; } = new List<SolicitudFV>();

    public virtual ICollection<StockMinimo> StockMinimos { get; } = new List<StockMinimo>();

    public virtual ICollection<UserPuestoVerificentro> UserPuestoVerificentroIdUserNavigations { get; } = new List<UserPuestoVerificentro>();

    public virtual ICollection<UserPuestoVerificentro> UserPuestoVerificentroIdUserRegistroNavigations { get; } = new List<UserPuestoVerificentro>();

    public virtual ICollection<UsoSuministro> UsoSuministros { get; } = new List<UsoSuministro>();

    public virtual ICollection<VentaFV> VentaFVs { get; } = new List<VentaFV>();

    public virtual ICollection<Verificacion> Verificacions { get; } = new List<Verificacion>();

    public virtual ICollection<Rol> IdRols { get; } = new List<Rol>();
}
