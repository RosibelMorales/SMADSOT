using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Verificentro
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public string? Clave { get; set; }

    public string? Direccion { get; set; }

    public string? Rfc { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? GerenteTecnico { get; set; }

    public string? RepresentanteLegal { get; set; }

    public decimal? Longitud { get; set; }

    public decimal? Latitud { get; set; }

    public string DirectorGestionCalidadAire { get; set; } = null!;

    public string ApiEndPoint { get; set; } = null!;

    public string ApiKey { get; set; } = null!;

    public string Municipio { get; set; } = null!;

    public virtual ICollection<AceditacionTecnicoSolicitud> AceditacionTecnicoSolicituds { get; } = new List<AceditacionTecnicoSolicitud>();

    public virtual ICollection<Alertum> Alerta { get; } = new List<Alertum>();

    public virtual ICollection<Almacen> Almacens { get; } = new List<Almacen>();

    public virtual ICollection<AutorizadoVerificentro> AutorizadoVerificentros { get; } = new List<AutorizadoVerificentro>();

    public virtual ICollection<Capacitacion> Capacitacions { get; } = new List<Capacitacion>();

    public virtual ICollection<CircularVerificentro> CircularVerificentros { get; } = new List<CircularVerificentro>();

    public virtual ICollection<CitaVerificacion> CitaVerificacions { get; } = new List<CitaVerificacion>();

    public virtual ICollection<ConfiguradorCitum> ConfiguradorCita { get; } = new List<ConfiguradorCitum>();

    public virtual ICollection<ConstanciaUltimaVerificacion> ConstanciaUltimaVerificacions { get; } = new List<ConstanciaUltimaVerificacion>();

    public virtual ICollection<Equipo> Equipos { get; } = new List<Equipo>();

    public virtual ICollection<FoliosFormaValoradaActuale> FoliosFormaValoradaActuales { get; } = new List<FoliosFormaValoradaActuale>();

    public virtual ICollection<FoliosFormaValoradaVerificentro> FoliosFormaValoradaVerificentros { get; } = new List<FoliosFormaValoradaVerificentro>();

    public virtual ICollection<FormaValoradum> FormaValorada { get; } = new List<FormaValoradum>();

    public virtual ICollection<Instalacion> Instalacions { get; } = new List<Instalacion>();

    public virtual ICollection<Linea> Lineas { get; } = new List<Linea>();

    public virtual ICollection<ReporteEntregaTercer> ReporteEntregaTercers { get; } = new List<ReporteEntregaTercer>();

    public virtual ICollection<Reporte> Reportes { get; } = new List<Reporte>();

    public virtual ICollection<UserPuestoVerificentro> UserPuestoVerificentros { get; } = new List<UserPuestoVerificentro>();

    public virtual ICollection<User> Users { get; } = new List<User>();

    public virtual ICollection<UsoSuministro> UsoSuministros { get; } = new List<UsoSuministro>();
}
