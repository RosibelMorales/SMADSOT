using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Smadot.Models.DataBase;

public partial class SmadotDbContext : DbContext
{
    public SmadotDbContext(DbContextOptions<SmadotDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AceditacionTecnicoSolicitud> AceditacionTecnicoSolicituds { get; set; }

    public virtual DbSet<AcreditacionTecnicoEvidenciaPuestoVerificentro> AcreditacionTecnicoEvidenciaPuestoVerificentros { get; set; }

    public virtual DbSet<AcreditacionTecnicoEvidencium> AcreditacionTecnicoEvidencia { get; set; }

    public virtual DbSet<Administrativa> Administrativas { get; set; }

    public virtual DbSet<Alertum> Alerta { get; set; }

    public virtual DbSet<Almacen> Almacens { get; set; }

    public virtual DbSet<AsignacionCertificado> AsignacionCertificados { get; set; }

    public virtual DbSet<AsignacionSV> AsignacionSVs { get; set; }

    public virtual DbSet<AutorizadoVerificentro> AutorizadoVerificentros { get; set; }

    public virtual DbSet<Caja> Cajas { get; set; }

    public virtual DbSet<CalendarioVerificacion> CalendarioVerificacions { get; set; }

    public virtual DbSet<Calibracion> Calibracions { get; set; }

    public virtual DbSet<Capacitacion> Capacitacions { get; set; }

    public virtual DbSet<CapacitacionEmpleado> CapacitacionEmpleados { get; set; }

    public virtual DbSet<CargaMasiva> CargaMasivas { get; set; }

    public virtual DbSet<CatEstatusAcreditacion> CatEstatusAcreditacions { get; set; }

    public virtual DbSet<CatEstatusCalibracion> CatEstatusCalibracions { get; set; }

    public virtual DbSet<CatEstatusCapacitacion> CatEstatusCapacitacions { get; set; }

    public virtual DbSet<CatEstatusEquipo> CatEstatusEquipos { get; set; }

    public virtual DbSet<CatEstatusFV> CatEstatusFVs { get; set; }

    public virtual DbSet<CatEstatusLinea> CatEstatusLineas { get; set; }

    public virtual DbSet<CatEstatusPuesto> CatEstatusPuestos { get; set; }

    public virtual DbSet<CatEstatusReporte> CatEstatusReportes { get; set; }

    public virtual DbSet<CatEstatusReporteCredencial> CatEstatusReporteCredencials { get; set; }

    public virtual DbSet<CatEstatusSolicitud> CatEstatusSolicituds { get; set; }

    public virtual DbSet<CatMarcaVehiculo> CatMarcaVehiculos { get; set; }

    public virtual DbSet<CatMotivoCancelacion> CatMotivoCancelacions { get; set; }

    public virtual DbSet<CatMotivoCancelacionCitum> CatMotivoCancelacionCita { get; set; }

    public virtual DbSet<CatMotivoLinea> CatMotivoLineas { get; set; }

    public virtual DbSet<CatMotivoReporteCredencial> CatMotivoReporteCredencials { get; set; }

    public virtual DbSet<CatMotivoTramite> CatMotivoTramites { get; set; }

    public virtual DbSet<CatSubMarcaVehiculo> CatSubMarcaVehiculos { get; set; }

    public virtual DbSet<CatSubdieselVehiculo> CatSubdieselVehiculos { get; set; }

    public virtual DbSet<CatTipoAcreditacion> CatTipoAcreditacions { get; set; }

    public virtual DbSet<CatTipoCalibracion> CatTipoCalibracions { get; set; }

    public virtual DbSet<CatTipoCausaRechazo> CatTipoCausaRechazos { get; set; }

    public virtual DbSet<CatTipoCertificado> CatTipoCertificados { get; set; }

    public virtual DbSet<CatTipoDiesel> CatTipoDiesels { get; set; }

    public virtual DbSet<CatTipoEquipo> CatTipoEquipos { get; set; }

    public virtual DbSet<CatTipoOrdenServicio> CatTipoOrdenServicios { get; set; }

    public virtual DbSet<CatTipoProtocolo> CatTipoProtocolos { get; set; }

    public virtual DbSet<CatTipoPuesto> CatTipoPuestos { get; set; }

    public virtual DbSet<CatTipoServicioCitum> CatTipoServicioCita { get; set; }

    public virtual DbSet<CatTipoTramite> CatTipoTramites { get; set; }

    public virtual DbSet<CicloVerificacion> CicloVerificacions { get; set; }

    public virtual DbSet<Circular> Circulars { get; set; }

    public virtual DbSet<CircularVerificentro> CircularVerificentros { get; set; }

    public virtual DbSet<CitaVerificacion> CitaVerificacions { get; set; }

    public virtual DbSet<ConfiguradorCitum> ConfiguradorCita { get; set; }

    public virtual DbSet<ConstanciaUltimaVerificacion> ConstanciaUltimaVerificacions { get; set; }

    public virtual DbSet<DevolucionCertificado> DevolucionCertificados { get; set; }

    public virtual DbSet<DevolucionSPF> DevolucionSPFs { get; set; }

    public virtual DbSet<DocumentosCitum> DocumentosCita { get; set; }

    public virtual DbSet<Equipo> Equipos { get; set; }

    public virtual DbSet<EquipoTipoCalibracion> EquipoTipoCalibracions { get; set; }

    public virtual DbSet<Error> Errors { get; set; }

    public virtual DbSet<Exento> Exentos { get; set; }

    public virtual DbSet<FoliosFormaValoradaActuale> FoliosFormaValoradaActuales { get; set; }

    public virtual DbSet<FoliosFormaValoradaVerificentro> FoliosFormaValoradaVerificentros { get; set; }

    public virtual DbSet<FormaValoradum> FormaValorada { get; set; }

    public virtual DbSet<HorarioUserPuestoVerificentro> HorarioUserPuestoVerificentros { get; set; }

    public virtual DbSet<IngresoCertificado> IngresoCertificados { get; set; }

    public virtual DbSet<IngresoFV> IngresoFVs { get; set; }

    public virtual DbSet<Instalacion> Instalacions { get; set; }

    public virtual DbSet<Inventario> Inventarios { get; set; }

    public virtual DbSet<LimiteVerificacion> LimiteVerificacions { get; set; }

    public virtual DbSet<LimiteVerificacionParametro> LimiteVerificacionParametros { get; set; }

    public virtual DbSet<Linea> Lineas { get; set; }

    public virtual DbSet<LineaMotivo> LineaMotivos { get; set; }

    public virtual DbSet<LogPruebaVerificacion> LogPruebaVerificacions { get; set; }

    public virtual DbSet<MotivoVerificacion> MotivoVerificacions { get; set; }

    public virtual DbSet<MovimientosInventario> MovimientosInventarios { get; set; }

    public virtual DbSet<NormaAcreditacion> NormaAcreditacions { get; set; }

    public virtual DbSet<OrdenServicio> OrdenServicios { get; set; }

    public virtual DbSet<ParametrosTablaMaestraVerificacion> ParametrosTablaMaestraVerificacions { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<ProveedorFolio> ProveedorFolios { get; set; }

    public virtual DbSet<Puesto> Puestos { get; set; }

    public virtual DbSet<Refrendo> Refrendos { get; set; }

    public virtual DbSet<Reporte> Reportes { get; set; }

    public virtual DbSet<ReporteCredencial> ReporteCredencials { get; set; }

    public virtual DbSet<ReporteEntregaTercer> ReporteEntregaTercers { get; set; }

    public virtual DbSet<ReporteEntregaTercerCertificado> ReporteEntregaTercerCertificados { get; set; }

    public virtual DbSet<ReporteFVCanceladum> ReporteFVCancelada { get; set; }

    public virtual DbSet<ReportePruebaRealizadum> ReportePruebaRealizada { get; set; }

    public virtual DbSet<Reposicion> Reposicions { get; set; }

    public virtual DbSet<ResultadosVerificacion> ResultadosVerificacions { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<SolicitudCertificado> SolicitudCertificados { get; set; }

    public virtual DbSet<SolicitudFV> SolicitudFVs { get; set; }

    public virtual DbSet<StockCertificado> StockCertificados { get; set; }

    public virtual DbSet<StockMinimo> StockMinimos { get; set; }

    public virtual DbSet<TablaMaestra> TablaMaestras { get; set; }

    public virtual DbSet<Testificacion> Testificacions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPuestoVerificentro> UserPuestoVerificentros { get; set; }

    public virtual DbSet<UsoSuministro> UsoSuministros { get; set; }

    public virtual DbSet<VentaCertificado> VentaCertificados { get; set; }

    public virtual DbSet<VentaFV> VentaFVs { get; set; }

    public virtual DbSet<Verificacion> Verificacions { get; set; }

    public virtual DbSet<Verificentro> Verificentros { get; set; }

    public virtual DbSet<vAcreditacionTecnicoEvidencium> vAcreditacionTecnicoEvidencia { get; set; }

    public virtual DbSet<vAcreditacionTecnicoSolicitud> vAcreditacionTecnicoSolicituds { get; set; }

    public virtual DbSet<vAdministrativa> vAdministrativas { get; set; }

    public virtual DbSet<vAlertum> vAlerta { get; set; }

    public virtual DbSet<vAlmacenStockReantante> vAlmacenStockReantantes { get; set; }

    public virtual DbSet<vAsignacionStock> vAsignacionStocks { get; set; }

    public virtual DbSet<vBusquedaPlacaSerie> vBusquedaPlacaSeries { get; set; }

    public virtual DbSet<vCALinea> vCALineas { get; set; }

    public virtual DbSet<vCajaFV> vCajaFVs { get; set; }

    public virtual DbSet<vCalibracion> vCalibracions { get; set; }

    public virtual DbSet<vCapacitacion> vCapacitacions { get; set; }

    public virtual DbSet<vCapacitacionEmpleado> vCapacitacionEmpleados { get; set; }

    public virtual DbSet<vCatMarcaVehiculoNombre> vCatMarcaVehiculoNombres { get; set; }

    public virtual DbSet<vCatSubMarcaVehiculo> vCatSubMarcaVehiculos { get; set; }

    public virtual DbSet<vCitaVerificacion> vCitaVerificacions { get; set; }

    public virtual DbSet<vConfiguradorCitum> vConfiguradorCita { get; set; }

    public virtual DbSet<vConstanciaUltimaVerificacion> vConstanciaUltimaVerificacions { get; set; }

    public virtual DbSet<vConsultaCirculare> vConsultaCirculares { get; set; }

    public virtual DbSet<vConsultaStockDVRF> vConsultaStockDVRves { get; set; }

    public virtual DbSet<vConteoVerificacione> vConteoVerificaciones { get; set; }

    public virtual DbSet<vDataTramitesVentanilla> vDataTramitesVentanillas { get; set; }

    public virtual DbSet<vDevolucionSPF> vDevolucionSPFs { get; set; }

    public virtual DbSet<vDevolucionesSolicitudFecha> vDevolucionesSolicitudFechas { get; set; }

    public virtual DbSet<vDieselVehiculo> vDieselVehiculos { get; set; }

    public virtual DbSet<vDocumentosCitum> vDocumentosCita { get; set; }

    public virtual DbSet<vEquipo> vEquipos { get; set; }

    public virtual DbSet<vEquipoCalibracion> vEquipoCalibracions { get; set; }

    public virtual DbSet<vEquipoOrdenServicio> vEquipoOrdenServicios { get; set; }

    public virtual DbSet<vExento> vExentos { get; set; }

    public virtual DbSet<vFoliosCancelado> vFoliosCancelados { get; set; }

    public virtual DbSet<vFoliosFormaValoradaExentosImpresion> vFoliosFormaValoradaExentosImpresions { get; set; }

    public virtual DbSet<vFoliosFormaValoradaImpresion> vFoliosFormaValoradaImpresions { get; set; }

    public virtual DbSet<vFoliosFormaValoradaTipo> vFoliosFormaValoradaTipos { get; set; }

    public virtual DbSet<vFoliosFormaValoradaVerificentro> vFoliosFormaValoradaVerificentros { get; set; }

    public virtual DbSet<vFoliosFormaValoradaVerificentroActuale> vFoliosFormaValoradaVerificentroActuales { get; set; }

    public virtual DbSet<vFoliosRegresadosSPF> vFoliosRegresadosSPFs { get; set; }

    public virtual DbSet<vFoliosUsadosVentanilla> vFoliosUsadosVentanillas { get; set; }

    public virtual DbSet<vFoliosVendidosCentrosVerificacion> vFoliosVendidosCentrosVerificacions { get; set; }

    public virtual DbSet<vHistorialCitum> vHistorialCita { get; set; }

    public virtual DbSet<vHistoricoCitum> vHistoricoCita { get; set; }

    public virtual DbSet<vIndicadoresDashboard> vIndicadoresDashboards { get; set; }

    public virtual DbSet<vIngresoFV> vIngresoFVs { get; set; }

    public virtual DbSet<vIngresosTotalFecha> vIngresosTotalFechas { get; set; }

    public virtual DbSet<vInstalacion> vInstalacions { get; set; }

    public virtual DbSet<vIntervalosCita> vIntervalosCitas { get; set; }

    public virtual DbSet<vInventario> vInventarios { get; set; }

    public virtual DbSet<vLineaEquipo> vLineaEquipos { get; set; }

    public virtual DbSet<vLineaGrid> vLineaGrids { get; set; }

    public virtual DbSet<vMovimientosInventarioEnStock> vMovimientosInventarioEnStocks { get; set; }

    public virtual DbSet<vMovimientosInventarioFueraStock> vMovimientosInventarioFueraStocks { get; set; }

    public virtual DbSet<vNumeroCitasHorario> vNumeroCitasHorarios { get; set; }

    public virtual DbSet<vOrdenServicio> vOrdenServicios { get; set; }

    public virtual DbSet<vPendientesImprimir> vPendientesImprimirs { get; set; }

    public virtual DbSet<vPersonal> vPersonals { get; set; }

    public virtual DbSet<vPersonalAutorizacion> vPersonalAutorizacions { get; set; }

    public virtual DbSet<vPlacaSerieBusquedaVerificacione> vPlacaSerieBusquedaVerificaciones { get; set; }

    public virtual DbSet<vPlacaSerieBusquedum> vPlacaSerieBusqueda { get; set; }

    public virtual DbSet<vProgramacionCalibracion> vProgramacionCalibracions { get; set; }

    public virtual DbSet<vProveedorFolio> vProveedorFolios { get; set; }

    public virtual DbSet<vRefrendo> vRefrendos { get; set; }

    public virtual DbSet<vReporte> vReportes { get; set; }

    public virtual DbSet<vReporteEntregaTercer> vReporteEntregaTercers { get; set; }

    public virtual DbSet<vReposicion> vReposicions { get; set; }

    public virtual DbSet<vReposicionCredencial> vReposicionCredencials { get; set; }

    public virtual DbSet<vReposicionVerificacion> vReposicionVerificacions { get; set; }

    public virtual DbSet<vSeguimientoSolicitud> vSeguimientoSolicituds { get; set; }

    public virtual DbSet<vSolicitudFormaValoradum> vSolicitudFormaValorada { get; set; }

    public virtual DbSet<vStockMinimo> vStockMinimos { get; set; }

    public virtual DbSet<vTablaMaestra> vTablaMaestras { get; set; }

    public virtual DbSet<vTestificacion> vTestificacions { get; set; }

    public virtual DbSet<vUsoSuministro> vUsoSuministros { get; set; }

    public virtual DbSet<vUsuario> vUsuarios { get; set; }

    public virtual DbSet<vVentaCertificado> vVentaCertificados { get; set; }

    public virtual DbSet<vVentaFV> vVentaFVs { get; set; }

    public virtual DbSet<vVentaFVDashboard> vVentaFVDashboards { get; set; }

    public virtual DbSet<vVerificacion> vVerificacions { get; set; }

    public virtual DbSet<vVerificacionCitum> vVerificacionCita { get; set; }

    public virtual DbSet<vVerificacionDashboard> vVerificacionDashboards { get; set; }

    public virtual DbSet<vVerificacionDatosLimite> vVerificacionDatosLimites { get; set; }

    public virtual DbSet<vVerificacionDatosProveedor> vVerificacionDatosProveedors { get; set; }

    public virtual DbSet<vVerificacionDatosProveedorUserDatum> vVerificacionDatosProveedorUserData { get; set; }

    public virtual DbSet<vVerificacionExpediente> vVerificacionExpedientes { get; set; }

    public virtual DbSet<vVerificacionInformacionGeneral> vVerificacionInformacionGenerals { get; set; }

    public virtual DbSet<vVerificacionLinea> vVerificacionLineas { get; set; }

    public virtual DbSet<vVerificacionReposicion> vVerificacionReposicions { get; set; }

    public virtual DbSet<vVerificacionSinFolio> vVerificacionSinFolios { get; set; }

    public virtual DbSet<vVerificacionesTipo> vVerificacionesTipos { get; set; }

    public virtual DbSet<vVerificacionesVerificentro> vVerificacionesVerificentros { get; set; }

    public virtual DbSet<vVerificentro> vVerificentros { get; set; }

    public virtual DbSet<vVigenciaVerificacion> vVigenciaVerificacions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AceditacionTecnicoSolicitud>(entity =>
        {
            entity.ToTable("AceditacionTecnicoSolicitud");

            entity.Property(e => e.IdVerificentro).HasDefaultValueSql("((17))");
            entity.Property(e => e.NumeroSolicitud)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlAprobacion)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusAcreditacionNavigation).WithMany(p => p.AceditacionTecnicoSolicituds)
                .HasForeignKey(d => d.IdCatEstatusAcreditacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AceditacionTecnicoSolicitud_CatEstatusAcreditacion");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.AceditacionTecnicoSolicituds)
                .HasForeignKey(d => d.IdVerificentro)
                .HasConstraintName("FK_AceditacionTecnicoSolicitud_Verificentro");

            entity.HasMany(d => d.IdUserPuestoVerificentros).WithMany(p => p.IdAcreditacionTecnicoSolicituds)
                .UsingEntity<Dictionary<string, object>>(
                    "AcreditacionTecnicoSolicitudPuestoVerificentro",
                    r => r.HasOne<UserPuestoVerificentro>().WithMany()
                        .HasForeignKey("IdUserPuestoVerificentro")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AcreditacionTecnicoSolicitudPuestoVerificentro_UserPuestoVerificentro"),
                    l => l.HasOne<AceditacionTecnicoSolicitud>().WithMany()
                        .HasForeignKey("IdAcreditacionTecnicoSolicitud")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AcreditacionTecnicoSolicitudPuestoVerificentro_AceditacionTecnicoSolicitud"),
                    j =>
                    {
                        j.HasKey("IdAcreditacionTecnicoSolicitud", "IdUserPuestoVerificentro");
                        j.ToTable("AcreditacionTecnicoSolicitudPuestoVerificentro");
                    });
        });

        modelBuilder.Entity<AcreditacionTecnicoEvidenciaPuestoVerificentro>(entity =>
        {
            entity.HasKey(e => new { e.IdAcreditacionTecnicoEvidencia, e.IdUserPuestoVerificentro });

            entity.ToTable("AcreditacionTecnicoEvidenciaPuestoVerificentro");

            entity.HasOne(d => d.IdAcreditacionTecnicoEvidenciaNavigation).WithMany(p => p.AcreditacionTecnicoEvidenciaPuestoVerificentros)
                .HasForeignKey(d => d.IdAcreditacionTecnicoEvidencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AcreditacionTecnicoEvidenciaPuestoVerificentro_AcreditacionTecnicoEvidencia");

            entity.HasOne(d => d.IdNormaAcreditacionNavigation).WithMany(p => p.AcreditacionTecnicoEvidenciaPuestoVerificentros)
                .HasForeignKey(d => d.IdNormaAcreditacion)
                .HasConstraintName("FK_AcreditacionTecnicoEvidenciaPuestoVerificentro_NormaAcreditacion");

            entity.HasOne(d => d.IdTipoAcreditacionNavigation).WithMany(p => p.AcreditacionTecnicoEvidenciaPuestoVerificentros)
                .HasForeignKey(d => d.IdTipoAcreditacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AcreditacionTecnicoEvidenciaPuestoVerificentro_CatTipoAcreditacion");

            entity.HasOne(d => d.IdUserPuestoVerificentroNavigation).WithMany(p => p.AcreditacionTecnicoEvidenciaPuestoVerificentros)
                .HasForeignKey(d => d.IdUserPuestoVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AcreditacionTecnicoEvidenciaPuestoVerificentro_UserPuestoVerificentro");
        });

        modelBuilder.Entity<AcreditacionTecnicoEvidencium>(entity =>
        {
            entity.Property(e => e.FechaAcreditacion).HasColumnType("datetime");
            entity.Property(e => e.FechaAmpliacion).HasColumnType("datetime");
            entity.Property(e => e.FechaEmision).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.NumeroAcreditacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSolicitud)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.TipoTramite)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UrlAcreditacion)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusAcreditacionNavigation).WithMany(p => p.AcreditacionTecnicoEvidencia)
                .HasForeignKey(d => d.IdCatEstatusAcreditacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AcreditacionTecnicoEvidencia_CatEstatusAcreditacion");
        });

        modelBuilder.Entity<Administrativa>(entity =>
        {
            entity.ToTable("Administrativa");

            entity.Property(e => e.Combustible)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.FolioAsignado).HasMaxLength(50);
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NombrePropietario)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc4)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");

            entity.HasOne(d => d.IdCatMotivoTramiteNavigation).WithMany(p => p.Administrativas)
                .HasForeignKey(d => d.IdCatMotivoTramite)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Administrativa_CatMotivoTramite");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.Administrativas)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Administrativa_Users");
        });

        modelBuilder.Entity<Alertum>(entity =>
        {
            entity.Property(e => e.Data).IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.MovimientoFinal).IsUnicode(false);
            entity.Property(e => e.MovimientoInicial).IsUnicode(false);
            entity.Property(e => e.TableName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Alerta)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_Alerta_Users");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.Alerta)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Alerta_Verificentro");
        });

        modelBuilder.Entity<Almacen>(entity =>
        {
            entity.ToTable("Almacen");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.Almacens)
                .HasForeignKey(d => d.IdVerificentro)
                .HasConstraintName("FK_Almacen_Verificentro");
        });

        modelBuilder.Entity<AsignacionCertificado>(entity =>
        {
            entity.ToTable("AsignacionCertificado");

            entity.HasOne(d => d.IdAsignacionSVNavigation).WithMany(p => p.AsignacionCertificados)
                .HasForeignKey(d => d.IdAsignacionSV)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AsignacionCertificado_AsignacionSV");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.AsignacionCertificados)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AsignacionCertificado_CatTipoCertificado");
        });

        modelBuilder.Entity<AsignacionSV>(entity =>
        {
            entity.ToTable("AsignacionSV");

            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NombreRecibio)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserAprobo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserRecibe)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAlmacenNavigation).WithMany(p => p.AsignacionSVs)
                .HasForeignKey(d => d.IdAlmacen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AsignacionSV_Almacen");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.AsignacionSVs)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AsignacionSV_Users");
        });

        modelBuilder.Entity<AutorizadoVerificentro>(entity =>
        {
            entity.ToTable("AutorizadoVerificentro");

            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.AutorizadoVerificentros)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AutorizadoVerificentro_Verificentro");
        });

        modelBuilder.Entity<Caja>(entity =>
        {
            entity.ToTable("Caja");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.Cajas)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Caja_CatTipoCertificado");

            entity.HasOne(d => d.IdIngresoFVNavigation).WithMany(p => p.Cajas)
                .HasForeignKey(d => d.IdIngresoFV)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Caja_IngresoFV");
        });

        modelBuilder.Entity<CalendarioVerificacion>(entity =>
        {
            entity.ToTable("CalendarioVerificacion");

            entity.Property(e => e.Digitos)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Calibracion>(entity =>
        {
            entity.ToTable("Calibracion");

            entity.Property(e => e.FechaCalibracion).HasColumnType("datetime");
            entity.Property(e => e.FechaEmisionCertificad).HasColumnType("datetime");
            entity.Property(e => e.FechaProgramada).HasColumnType("datetime");
            entity.Property(e => e.FechaProximaCalibracion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Laboratorio)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nota)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.UrlResultado)
                .HasMaxLength(250)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusCalibracionNavigation).WithMany(p => p.Calibracions)
                .HasForeignKey(d => d.IdCatEstatusCalibracion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Calibracion_CatEstatusCalibracion");

            entity.HasOne(d => d.IdEquipoTipoCalibracionNavigation).WithMany(p => p.Calibracions)
                .HasForeignKey(d => d.IdEquipoTipoCalibracion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Calibracion_EquipoTipoCalibracion");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.CalibracionIdUserRegistroNavigations)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Calibracion_Users");

            entity.HasOne(d => d.IdUserValidoNavigation).WithMany(p => p.CalibracionIdUserValidoNavigations)
                .HasForeignKey(d => d.IdUserValido)
                .HasConstraintName("FK_Calibracion_Users1");
        });

        modelBuilder.Entity<Capacitacion>(entity =>
        {
            entity.ToTable("Capacitacion");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Tema)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusCapacitacionNavigation).WithMany(p => p.Capacitacions)
                .HasForeignKey(d => d.IdCatEstatusCapacitacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Capacitacion_CatEstatusCapacitacion");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.Capacitacions)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Capacitacion_Verificentro");
        });

        modelBuilder.Entity<CapacitacionEmpleado>(entity =>
        {
            entity.ToTable("CapacitacionEmpleado");

            entity.Property(e => e.UrlEvaluacion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlFotografia)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCapacitacionNavigation).WithMany(p => p.CapacitacionEmpleados)
                .HasForeignKey(d => d.IdCapacitacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CapacitacionEmpleado_Capacitacion");

            entity.HasOne(d => d.IdUserPuestoVerificentroNavigation).WithMany(p => p.CapacitacionEmpleados)
                .HasForeignKey(d => d.IdUserPuestoVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CapacitacionEmpleado_UserPuestoVerificentro");
        });

        modelBuilder.Entity<CargaMasiva>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CargaMas__3214EC071184E29C");

            entity.ToTable("CargaMasiva");

            entity.Property(e => e.Base64String)
                .IsUnicode(false)
                .HasDefaultValueSql("(' ')");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NombreArchivo)
                .HasMaxLength(250)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.CargaMasivas)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CargaMasi__IdUse__1C0818FF");
        });

        modelBuilder.Entity<CatEstatusAcreditacion>(entity =>
        {
            entity.ToTable("CatEstatusAcreditacion");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatEstatusCalibracion>(entity =>
        {
            entity.ToTable("CatEstatusCalibracion");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatEstatusCapacitacion>(entity =>
        {
            entity.ToTable("CatEstatusCapacitacion");

            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatEstatusEquipo>(entity =>
        {
            entity.ToTable("CatEstatusEquipo");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatEstatusFV>(entity =>
        {
            entity.ToTable("CatEstatusFV");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatEstatusLinea>(entity =>
        {
            entity.ToTable("CatEstatusLinea");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatEstatusPuesto>(entity =>
        {
            entity.ToTable("CatEstatusPuesto");

            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatEstatusReporte>(entity =>
        {
            entity.ToTable("CatEstatusReporte");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatEstatusReporteCredencial>(entity =>
        {
            entity.ToTable("CatEstatusReporteCredencial");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatEstatusSolicitud>(entity =>
        {
            entity.ToTable("CatEstatusSolicitud");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatMarcaVehiculo>(entity =>
        {
            entity.ToTable("CatMarcaVehiculo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatMotivoCancelacion>(entity =>
        {
            entity.ToTable("CatMotivoCancelacion");

            entity.Property(e => e.Nombre)
                .HasMaxLength(525)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatMotivoCancelacionCitum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CatMotiv__3214EC07EADDCC1D");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatMotivoLinea>(entity =>
        {
            entity.ToTable("CatMotivoLinea");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatMotivoReporteCredencial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MotivoReporteCredencial");

            entity.ToTable("CatMotivoReporteCredencial");

            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatMotivoTramite>(entity =>
        {
            entity.ToTable("CatMotivoTramite");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatSubMarcaVehiculo>(entity =>
        {
            entity.ToTable("CatSubMarcaVehiculo");

            entity.Property(e => e.Alias)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatMarcaVehiculoNavigation).WithMany(p => p.CatSubMarcaVehiculos)
                .HasForeignKey(d => d.IdCatMarcaVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatSubMarcaVehiculo_CatMarcaVehiculo");
        });

        modelBuilder.Entity<CatSubdieselVehiculo>(entity =>
        {
            entity.ToTable("CatSubdieselVehiculo");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatTipoAcreditacion>(entity =>
        {
            entity.ToTable("CatTipoAcreditacion");

            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatTipoCalibracion>(entity =>
        {
            entity.ToTable("CatTipoCalibracion");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatTipoCausaRechazo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CatTipoC__3214EC0736E68169");

            entity.ToTable("CatTipoCausaRechazo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre).HasMaxLength(255);
        });

        modelBuilder.Entity<CatTipoCertificado>(entity =>
        {
            entity.ToTable("CatTipoCertificado");

            entity.Property(e => e.ClaveCertificado)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatTipoDiesel>(entity =>
        {
            entity.ToTable("CatTipoDiesel");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<CatTipoEquipo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CatTipoE__3214EC0743B426D3");

            entity.ToTable("CatTipoEquipo");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatTipoOrdenServicio>(entity =>
        {
            entity.ToTable("CatTipoOrdenServicio");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatTipoProtocolo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CatTipoP__3214EC07D84B9B32");

            entity.ToTable("CatTipoProtocolo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre).HasMaxLength(255);
        });

        modelBuilder.Entity<CatTipoPuesto>(entity =>
        {
            entity.ToTable("CatTipoPuesto");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatTipoServicioCitum>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatTipoTramite>(entity =>
        {
            entity.ToTable("CatTipoTramite");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CicloVerificacion>(entity =>
        {
            entity.ToTable("CicloVerificacion");

            entity.Property(e => e.FechaFin).HasColumnType("datetime");
            entity.Property(e => e.FechaInicio).HasColumnType("datetime");
            entity.Property(e => e.ImporteAdministrativo).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.ImporteConstanciaUltimaVer).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.ImporteExento).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.ImporteFv).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.ImporteReposicion).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.ImporteTestificacion).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Circular>(entity =>
        {
            entity.ToTable("Circular");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Mensaje).IsUnicode(false);
            entity.Property(e => e.NumeroCircular)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.Circulars)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Circular_Users");
        });

        modelBuilder.Entity<CircularVerificentro>(entity =>
        {
            entity.ToTable("CircularVerificentro");

            entity.Property(e => e.AcuseLeido)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCircularNavigation).WithMany(p => p.CircularVerificentros)
                .HasForeignKey(d => d.IdCircular)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CircularVerificentro_Circular");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.CircularVerificentros)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CircularVerificentro_Verificentro");
        });

        modelBuilder.Entity<CitaVerificacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Citas");

            entity.ToTable("CitaVerificacion");

            entity.HasIndex(e => e.Fecha, "IX_CitaVerificacion_IdCVV_IdCatMarcaVehiculo");

            entity.HasIndex(e => new { e.IdCatMarcaVehiculo, e.Fecha }, "IX_CitaVerificacion_IdCatMarcaVehiculo_Fecha");

            entity.HasIndex(e => e.IdCatMarcaVehiculo, "idx_CitaVerficacion");

            entity.Property(e => e.ColorVehiculo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaCancelacion).HasColumnType("datetime");
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaReinicio).HasColumnType("datetime");
            entity.Property(e => e.Folio)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NombreGeneraCita)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Placa).HasMaxLength(10);
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlComprobante)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCVVNavigation).WithMany(p => p.CitaVerificacions)
                .HasForeignKey(d => d.IdCVV)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Citas_Verificentro");

            entity.HasOne(d => d.IdCatMotivoCancelacionCitaNavigation).WithMany(p => p.CitaVerificacions)
                .HasForeignKey(d => d.IdCatMotivoCancelacionCita)
                .HasConstraintName("FK__CitaVerif__IdCat__59B045BD");

            entity.HasOne(d => d.IdUserCanceloNavigation).WithMany(p => p.CitaVerificacions)
                .HasForeignKey(d => d.IdUserCancelo)
                .HasConstraintName("FK__CitaVerif__IdUse__57C7FD4B");
        });

        modelBuilder.Entity<ConfiguradorCitum>(entity =>
        {
            entity.Property(e => e.Fecha).HasColumnType("date");

            entity.HasOne(d => d.IdCVVNavigation).WithMany(p => p.ConfiguradorCita)
                .HasForeignKey(d => d.IdCVV)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConfiguradorCita_Verificentro");
        });

        modelBuilder.Entity<ConstanciaUltimaVerificacion>(entity =>
        {
            entity.ToTable("ConstanciaUltimaVerificacion");

            entity.Property(e => e.ClaveTramite)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CodigoQr)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Combustible)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaEmisionRef).HasColumnType("datetime");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.FechaVerificacion).HasColumnType("datetime");
            entity.Property(e => e.FolioVerificacion).HasMaxLength(50);
            entity.Property(e => e.ImporteActual).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.Marca)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombrePropietario)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ServidorPublico)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.ConstanciaUltimaVerificacions)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConstanciaUltimaVerificacion_CatTipoCertificado");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.ConstanciaUltimaVerificacions)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConstanciaUltimaVerificacion_Users");

            entity.HasOne(d => d.IdVerificacionNavigation).WithMany(p => p.ConstanciaUltimaVerificacions)
                .HasForeignKey(d => d.IdVerificacion)
                .HasConstraintName("FK_ConstanciaUltimaVerificacion_Verificacion");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.ConstanciaUltimaVerificacions)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConstanciaUltimaVerificacion_Verificentro");
        });

        modelBuilder.Entity<DevolucionCertificado>(entity =>
        {
            entity.ToTable("DevolucionCertificado");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.DevolucionCertificados)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DevolucionCertificado_CatTipoCertificado");

            entity.HasOne(d => d.IdDevolucionSPFNavigation).WithMany(p => p.DevolucionCertificados)
                .HasForeignKey(d => d.IdDevolucionSPF)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DevolucionCertificado_DevolucionSPF");
        });

        modelBuilder.Entity<DevolucionSPF>(entity =>
        {
            entity.ToTable("DevolucionSPF");

            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NumeroDevolucion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSolicitud)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RecibioSPF)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ResponsableEntrega)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlNombreDocumento1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlNombreDocumento2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlNombreDocumento3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserAprobo)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAlmacenNavigation).WithMany(p => p.DevolucionSPFs)
                .HasForeignKey(d => d.IdAlmacen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DevolucionSPF_Almacen");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.DevolucionSPFs)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DevolucionSPF_Users");
        });

        modelBuilder.Entity<DocumentosCitum>(entity =>
        {
            entity.HasIndex(e => e.IdCitaVerificacion, "Index_DocumentosCita_1").IsUnique();

            entity.Property(e => e.FechaFacturacion).HasColumnType("datetime");
            entity.Property(e => e.FechaRecepcion).HasColumnType("datetime");
            entity.Property(e => e.FolioTarjetaCirculacion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.URLAltaPlacas).IsUnicode(false);
            entity.Property(e => e.URLBajaPlacas).IsUnicode(false);
            entity.Property(e => e.URLCartaFactura).IsUnicode(false);
            entity.Property(e => e.URLFactura).IsUnicode(false);
            entity.Property(e => e.URLIdentificacion).IsUnicode(false);
            entity.Property(e => e.URLValidacionCertificado).IsUnicode(false);

            entity.HasOne(d => d.IdCatTipoServicioNavigation).WithMany(p => p.DocumentosCita)
                .HasForeignKey(d => d.IdCatTipoServicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DocumentosCita_CatTipoServicioCita");

            entity.HasOne(d => d.IdCitaVerificacionNavigation).WithOne(p => p.DocumentosCitum)
                .HasForeignKey<DocumentosCitum>(d => d.IdCitaVerificacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DocumentosCita_CitaVerificacion");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.DocumentosCita)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DocumentosCita_UsoSuministro");
        });

        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.ToTable("Equipo");

            entity.Property(e => e.Comentarios)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.IdCatTipoEquipo).HasDefaultValueSql("((1))");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSerie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UrlEspecificacionTecnica)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlFactura)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlManualUsuario)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlRecomendacionServicio)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusEquipoNavigation).WithMany(p => p.Equipos)
                .HasForeignKey(d => d.IdCatEstatusEquipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Table_1_CatEstatusEquipo");

            entity.HasOne(d => d.IdCatTipoEquipoNavigation).WithMany(p => p.Equipos)
                .HasForeignKey(d => d.IdCatTipoEquipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Equipo__IdCatTip__60E75331");

            entity.HasOne(d => d.IdLineaNavigation).WithMany(p => p.Equipos)
                .HasForeignKey(d => d.IdLinea)
                .HasConstraintName("FK_Equipo_Linea");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.EquipoIdUserRegistroNavigations)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Table_1_Users");

            entity.HasOne(d => d.IdUserRevisoNavigation).WithMany(p => p.EquipoIdUserRevisoNavigations)
                .HasForeignKey(d => d.IdUserReviso)
                .HasConstraintName("FK_Table_1_Users1");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.Equipos)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Equipo_Verificentro");
        });

        modelBuilder.Entity<EquipoTipoCalibracion>(entity =>
        {
            entity.ToTable("EquipoTipoCalibracion");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Nota)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PrimeraFechaCalibracion).HasColumnType("datetime");
            entity.Property(e => e.UrlDocumento1)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdEquipoNavigation).WithMany(p => p.EquipoTipoCalibracions)
                .HasForeignKey(d => d.IdEquipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EquipoTipoCalibracion_Equipo");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.EquipoTipoCalibracionIdUserRegistroNavigations)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EquipoTipoCalibracion_Users");

            entity.HasOne(d => d.IdUserValidoNavigation).WithMany(p => p.EquipoTipoCalibracionIdUserValidoNavigations)
                .HasForeignKey(d => d.IdUserValido)
                .HasConstraintName("FK_EquipoTipoCalibracion_Users1");
        });

        modelBuilder.Entity<Error>(entity =>
        {
            entity.ToTable("Error");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Values).HasColumnType("text");
        });

        modelBuilder.Entity<Exento>(entity =>
        {
            entity.ToTable("Exento");

            entity.Property(e => e.Combustible)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FechaCartaFactura).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IdUser).HasDefaultValueSql("((1))");
            entity.Property(e => e.Marca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NumTarjetaCirculacion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UltimoFolio).HasMaxLength(50);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc4)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
            entity.Property(e => e.VigenciaHoloAnterior).HasColumnType("datetime");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.Exentos)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exento_CatTipoCertificado");
        });

        modelBuilder.Entity<FoliosFormaValoradaActuale>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.FoliosFormaValoradaActuales)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FoliosFormaValoradaActuales_CatTipoCertificado_1");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.FoliosFormaValoradaActuales)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FoliosFormaValoradaActuales_FoliosFormaValoradaActuales");
        });

        modelBuilder.Entity<FoliosFormaValoradaVerificentro>(entity =>
        {
            entity.ToTable("FoliosFormaValoradaVerificentro");

            entity.HasIndex(e => new { e.Cancelado, e.IdCatTipoTramite }, "idx_Vista_FoliosFormaValoradaVerificentro");

            entity.HasIndex(e => e.IdVerificentro, "idx_vFoliosFormaValoradaVerificentro");

            entity.HasIndex(e => new { e.Cancelado, e.IdCatTipoTramite }, "idx_vhistorialCita");

            entity.Property(e => e.ClaveTramite)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ClaveTramiteCancelado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaCancelacion).HasColumnType("datetime");
            entity.Property(e => e.FechaEmisionRef).HasColumnType("datetime");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.ImporteActual).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.OtroMotivo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ServidorPublico)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAdministrativaNavigation).WithMany(p => p.FoliosFormaValoradaVerificentros)
                .HasForeignKey(d => d.IdAdministrativa)
                .HasConstraintName("FK_FoliosFormaValoradaVerificentro_Administrativa");

            entity.HasOne(d => d.IdCatMotivoCancelacionNavigation).WithMany(p => p.FoliosFormaValoradaVerificentros)
                .HasForeignKey(d => d.IdCatMotivoCancelacion)
                .HasConstraintName("FK_FoliosFormaValoradaVerificentro_CatMotivoCancelacion");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.FoliosFormaValoradaVerificentros)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .HasConstraintName("FK_FoliosFormaValoradaVerificentro_CatTipoCertificado");

            entity.HasOne(d => d.IdCatTipoTramiteNavigation).WithMany(p => p.FoliosFormaValoradaVerificentros)
                .HasForeignKey(d => d.IdCatTipoTramite)
                .HasConstraintName("FK_FoliosFormaValoradaVerificentro_CatTipoTramite");

            entity.HasOne(d => d.IdExentoNavigation).WithMany(p => p.FoliosFormaValoradaVerificentros)
                .HasForeignKey(d => d.IdExento)
                .HasConstraintName("FK_FoliosFormaValoradaVerificentro_Exento");

            entity.HasOne(d => d.IdTestificacionNavigation).WithMany(p => p.FoliosFormaValoradaVerificentros)
                .HasForeignKey(d => d.IdTestificacion)
                .HasConstraintName("FK_FoliosFormaValoradaVerificentro_Testificacion");

            entity.HasOne(d => d.IdUserCanceloNavigation).WithMany(p => p.FoliosFormaValoradaVerificentroIdUserCanceloNavigations)
                .HasForeignKey(d => d.IdUserCancelo)
                .HasConstraintName("FK_FoliosFormaValoradaVerificentro_Users");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.FoliosFormaValoradaVerificentroIdUserRegistroNavigations)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FoliosFormaValoradaVerificentro_Users2");

            entity.HasOne(d => d.IdVerificacionNavigation).WithMany(p => p.FoliosFormaValoradaVerificentros)
                .HasForeignKey(d => d.IdVerificacion)
                .HasConstraintName("FK_FoliosFormaValoradaVerificentro_Verificacion");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.FoliosFormaValoradaVerificentros)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FoliosFormaValoradaVerificentro_Verificentro");
        });

        modelBuilder.Entity<FormaValoradum>(entity =>
        {
            entity.Property(e => e.AproboCancelacion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ClaveCertificado)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ClaveDevolucion)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ClaveSolicitud)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ClaveVenta)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CorreoUsuario)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Cvv)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaCancelacion).HasColumnType("datetime");
            entity.Property(e => e.Folio)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FolioCertificado)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FolioFinal)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FolioInicial)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MontoTramite).HasColumnType("money");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OtroMotivo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.PersonaGeneroTramite)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PersonaRecibeCertificados)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PersonaValido)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Razon)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ReferenciaBancaria)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.ResponsableEntrega)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TelefonoUsuario)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusFVNavigation).WithMany(p => p.FormaValorada)
                .HasForeignKey(d => d.IdCatEstatusFV)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FormaValorada_CatEstatusFV");

            entity.HasOne(d => d.IdCatMotivoCancelacionNavigation).WithMany(p => p.FormaValorada)
                .HasForeignKey(d => d.IdCatMotivoCancelacion)
                .HasConstraintName("FK_FormaValorada_CatMotivoCancelacion");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.FormaValorada)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FormaValorada_CatTipoCertificado");

            entity.HasOne(d => d.IdCatTipoTramiteNavigation).WithMany(p => p.FormaValorada)
                .HasForeignKey(d => d.IdCatTipoTramite)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FormaValorada_CatTipoTramite");

            entity.HasOne(d => d.IdCicloVerificacionNavigation).WithMany(p => p.FormaValorada)
                .HasForeignKey(d => d.IdCicloVerificacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FormaValorada_CicloVerificacion");

            entity.HasOne(d => d.IdUserCanceloNavigation).WithMany(p => p.FormaValorada)
                .HasForeignKey(d => d.IdUserCancelo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FormaValorada_Users");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.FormaValorada)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FormaValorada_Verificentro");
        });

        modelBuilder.Entity<HorarioUserPuestoVerificentro>(entity =>
        {
            entity.ToTable("HorarioUserPuestoVerificentro");

            entity.Property(e => e.Dia)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUserPuestoVerificentroNavigation).WithMany(p => p.HorarioUserPuestoVerificentros)
                .HasForeignKey(d => d.IdUserPuestoVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HorarioUserPuestoVerificentro_UserPuestoVerificentro");
        });

        modelBuilder.Entity<IngresoCertificado>(entity =>
        {
            entity.ToTable("IngresoCertificado");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.IngresoCertificados)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IngresoCertificado_CatTipoCertificado");

            entity.HasOne(d => d.IdIngresoFVNavigation).WithMany(p => p.IngresoCertificados)
                .HasForeignKey(d => d.IdIngresoFV)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IngresoCertificado_IngresoFV");
        });

        modelBuilder.Entity<IngresoFV>(entity =>
        {
            entity.ToTable("IngresoFV");

            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NombreRecibio)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAlmacenNavigation).WithMany(p => p.IngresoFVs)
                .HasForeignKey(d => d.IdAlmacen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IngresoFV_Almacen");

            entity.HasOne(d => d.IdSolicitudFVNavigation).WithMany(p => p.IngresoFVs)
                .HasForeignKey(d => d.IdSolicitudFV)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IngresoFV_SolicitudFV");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.IngresoFVs)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IngresoFV_Users");
        });

        modelBuilder.Entity<Instalacion>(entity =>
        {
            entity.ToTable("Instalacion");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.UrlDocumento)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Instalacions)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Instalacion_Proveedor");

            entity.HasOne(d => d.IdReporteNavigation).WithMany(p => p.Instalacions)
                .HasForeignKey(d => d.IdReporte)
                .HasConstraintName("FK_Instalacion_Reporte");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.Instalacions)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Instalacion_Users");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.Instalacions)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Instalacion_Verificentro");
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.ToTable("Inventario");

            entity.HasOne(d => d.IdAlmacenNavigation).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.IdAlmacen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_Almacen");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_CatTipoCertificado");
        });

        modelBuilder.Entity<LimiteVerificacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LimiteVe__3214EC07AD5076B4");

            entity.ToTable("LimiteVerificacion");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Co).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Coco2_Max).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Coco2_Min).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Coeficiente_Absorcion_Luz).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Factor_Lamda).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.O2).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Opacidad).HasColumnType("decimal(4, 2)");
        });

        modelBuilder.Entity<LimiteVerificacionParametro>(entity =>
        {
            entity.HasKey(e => e.IdVerificacion).HasName("PK__tmp_ms_x__FB609034F71F8AEC");

            entity.ToTable("LimiteVerificacionParametro");

            entity.Property(e => e.IdVerificacion).ValueGeneratedNever();
            entity.Property(e => e.Co).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Coco2_Max).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Coco2_Min).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Coeficiente_Absorcion_Luz).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Factor_Lamda).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.O2).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Opacidad).HasColumnType("decimal(4, 2)");

            entity.HasOne(d => d.IdVerificacionNavigation).WithOne(p => p.LimiteVerificacionParametro)
                .HasForeignKey<LimiteVerificacionParametro>(d => d.IdVerificacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LimiteVerificacionParametro_Verificacion");
        });

        modelBuilder.Entity<Linea>(entity =>
        {
            entity.ToTable("Linea");

            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusLineaNavigation).WithMany(p => p.Lineas)
                .HasForeignKey(d => d.IdCatEstatusLinea)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Linea_CatEstatusLinea");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.Lineas)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Linea_Users");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.Lineas)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Linea_Verificentro");
        });

        modelBuilder.Entity<LineaMotivo>(entity =>
        {
            entity.ToTable("LineaMotivo");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Notas)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDocumento)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatMotivoLineaNavigation).WithMany(p => p.LineaMotivos)
                .HasForeignKey(d => d.IdCatMotivoLinea)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Table_1_CatMotivoLinea");

            entity.HasOne(d => d.IdLineaNavigation).WithMany(p => p.LineaMotivos)
                .HasForeignKey(d => d.IdLinea)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Table_1_Linea");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.LineaMotivos)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Table_1_Users3");
        });

        modelBuilder.Entity<LogPruebaVerificacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC073E7D0FD7");

            entity.ToTable("LogPruebaVerificacion");

            entity.Property(e => e.BHP).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.CO).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.CO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.ClaveCentro).HasMaxLength(10);
            entity.Property(e => e.Etapa).HasMaxLength(255);
            entity.Property(e => e.FCDIL).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FUERZA).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaUltimaModificacion).HasColumnType("datetime");
            entity.Property(e => e.HR_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.HR_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.KPH).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.LAMDA).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.Linea).HasMaxLength(4);
            entity.Property(e => e.NumSerie).HasMaxLength(255);
            entity.Property(e => e.NumeroConstanciaOtorgada).HasMaxLength(20);
            entity.Property(e => e.O2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.OBD_TIPO_SDB).HasMaxLength(255);
            entity.Property(e => e.OPACIDADK).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.OPACIDADP).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.PAR_TOR).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.POT_FRENO).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.Placa).HasMaxLength(255);
            entity.Property(e => e.Pot_2540).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Pot_5024).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Propietario).HasMaxLength(255);
            entity.Property(e => e.RazonSocial).HasMaxLength(255);
            entity.Property(e => e.TEMP_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.TEMP_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.THP_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.THP_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.TecnicoVerificador).HasMaxLength(15);
            entity.Property(e => e.VEL_ANG).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VEL_LIN).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VOLTS_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.VOLTS_5024).HasColumnType("decimal(9, 3)");
        });

        modelBuilder.Entity<MotivoVerificacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MotivoVe__3214EC07CCB40872");

            entity.ToTable("MotivoVerificacion");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MovimientosInventario>(entity =>
        {
            entity.ToTable("MovimientosInventario");

            entity.HasOne(d => d.IdIngresoCertificadoNavigation).WithMany(p => p.MovimientosInventarios)
                .HasForeignKey(d => d.IdIngresoCertificado)
                .HasConstraintName("FK_MovimientosInventario_IngresoCertificado");

            entity.HasOne(d => d.IdInventarioNavigation).WithMany(p => p.MovimientosInventarios)
                .HasForeignKey(d => d.IdInventario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MovimientosInventario_Inventario");

            entity.HasOne(d => d.IdVentaCertificadoNavigation).WithMany(p => p.MovimientosInventarios)
                .HasForeignKey(d => d.IdVentaCertificado)
                .HasConstraintName("FK_MovimientosInventario_VentaCertificado");
        });

        modelBuilder.Entity<NormaAcreditacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_NomaAcreditacion");

            entity.ToTable("NormaAcreditacion");

            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion).IsUnicode(false);
        });

        modelBuilder.Entity<OrdenServicio>(entity =>
        {
            entity.ToTable("OrdenServicio");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Motivo)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSerieActual)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSerieAnterior)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Solucion)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.UrlOS)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatTipoOrdenServicioNavigation).WithMany(p => p.OrdenServicios)
                .HasForeignKey(d => d.IdCatTipoOrdenServicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenServicio_CatTipoOrdenServicio");

            entity.HasOne(d => d.IdEquipoNavigation).WithMany(p => p.OrdenServicios)
                .HasForeignKey(d => d.IdEquipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenServicio_Equipo");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.OrdenServicios)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenServicio_Users");
        });

        modelBuilder.Entity<ParametrosTablaMaestraVerificacion>(entity =>
        {
            entity.HasKey(e => e.IdVerificacion);

            entity.ToTable("ParametrosTablaMaestraVerificacion");

            entity.Property(e => e.IdVerificacion).ValueGeneratedNever();
            entity.Property(e => e.LAMDA_MAX).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.O2_MAX).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_5024).HasColumnType("decimal(5, 1)");

            entity.HasOne(d => d.IdVerificacionNavigation).WithOne(p => p.ParametrosTablaMaestraVerificacion)
                .HasForeignKey<ParametrosTablaMaestraVerificacion>(d => d.IdVerificacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ParametrosTablaMaestraVerificacion_Verificacion");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.ToTable("Permiso");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Descripcion).IsUnicode(false);
            entity.Property(e => e.Nombre).IsUnicode(false);

            entity.HasOne(d => d.IdPermisoPadreNavigation).WithMany(p => p.InverseIdPermisoPadreNavigation)
                .HasForeignKey(d => d.IdPermisoPadre)
                .HasConstraintName("FK_Permiso_Permiso");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.ToTable("Proveedor");

            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Empresa)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTelefono)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProveedorFolio>(entity =>
        {
            entity.HasIndex(e => e.Folio, "Index_ProveedorFolios_1").IsUnique();

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.ProveedorFolios)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProveedorFolios_Proveedor");
        });

        modelBuilder.Entity<Puesto>(entity =>
        {
            entity.ToTable("Puesto");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatTipoPuestoNavigation).WithMany(p => p.Puestos)
                .HasForeignKey(d => d.IdCatTipoPuesto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Puesto_CatTipoPuesto");
        });

        modelBuilder.Entity<Refrendo>(entity =>
        {
            entity.ToTable("Refrendo");

            entity.Property(e => e.FechaCartaFactura).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
            entity.Property(e => e.VigenciaHoloAnterior).HasColumnType("datetime");

            entity.HasOne(d => d.IdExentoNavigation).WithMany(p => p.Refrendos)
                .HasForeignKey(d => d.IdExento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Refrendo_Exento");

            entity.HasOne(d => d.IdFolioFormaValoradaVerificentroNavigation).WithMany(p => p.Refrendos)
                .HasForeignKey(d => d.IdFolioFormaValoradaVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Refrendo_FoliosFormaValoradaVerificentro");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.Refrendos)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Refrendo_Users");
        });

        modelBuilder.Entity<Reporte>(entity =>
        {
            entity.ToTable("Reporte");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.UltimaModificacion).HasColumnType("datetime");
            entity.Property(e => e.UrlEntrega)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlExcel)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusReporteNavigation).WithMany(p => p.Reportes)
                .HasForeignKey(d => d.IdCatEstatusReporte)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporte_CatEstatusReporte");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.Reportes)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporte_Users");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.Reportes)
                .HasForeignKey(d => d.IdVerificentro)
                .HasConstraintName("FK_Reporte_Verificentro");
        });

        modelBuilder.Entity<ReporteCredencial>(entity =>
        {
            entity.ToTable("ReporteCredencial");

            entity.Property(e => e.Denuncia)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.UrlCredencial)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusReporteCredencialNavigation).WithMany(p => p.ReporteCredencials)
                .HasForeignKey(d => d.IdCatEstatusReporteCredencial)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteCredencial_CatEstatusReporteCredencial");

            entity.HasOne(d => d.IdCatMotivoReporteCredencialNavigation).WithMany(p => p.ReporteCredencials)
                .HasForeignKey(d => d.IdCatMotivoReporteCredencial)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteCredencial_CatMotivoReporteCredencial");

            entity.HasOne(d => d.IdUserPuestoVerificentroNavigation).WithMany(p => p.ReporteCredencials)
                .HasForeignKey(d => d.IdUserPuestoVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteCredencial_UserPuestoVerificentro");
        });

        modelBuilder.Entity<ReporteEntregaTercer>(entity =>
        {
            entity.ToTable("ReporteEntregaTercer");

            entity.Property(e => e.FechaFinal).HasColumnType("datetime");
            entity.Property(e => e.FechaInicial).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.UrlEntrega)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.ReporteEntregaTercers)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteTercerTanto_Verificentro");
        });

        modelBuilder.Entity<ReporteEntregaTercerCertificado>(entity =>
        {
            entity.ToTable("ReporteEntregaTercerCertificado");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.ReporteEntregaTercerCertificados)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteEntregaTercerCertificado_CatTipoCertificado");

            entity.HasOne(d => d.IdReporteNavigation).WithMany(p => p.ReporteEntregaTercerCertificados)
                .HasForeignKey(d => d.IdReporte)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteEntregaTercerCertificado_ReporteEntregaTercer");
        });

        modelBuilder.Entity<ReporteFVCanceladum>(entity =>
        {
            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.ReporteFVCancelada)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteFVCancelada_CatTipoCertificado");

            entity.HasOne(d => d.IdReporteNavigation).WithMany(p => p.ReporteFVCancelada)
                .HasForeignKey(d => d.IdReporte)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteFVCancelada_Reporte");
        });

        modelBuilder.Entity<ReportePruebaRealizadum>(entity =>
        {
            entity.Property(e => e.TipoServicio)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.IdReporteNavigation).WithMany(p => p.ReportePruebaRealizada)
                .HasForeignKey(d => d.IdReporte)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReportePruebaRealizada_Reporte");
        });

        modelBuilder.Entity<Reposicion>(entity =>
        {
            entity.ToTable("Reposicion");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdFoliosFormaValoradaVerificentroNavigation).WithMany(p => p.Reposicions)
                .HasForeignKey(d => d.IdFoliosFormaValoradaVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reposicion_FoliosFormaValoradaVerificentro");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.Reposicions)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reposicion_Users");
        });

        modelBuilder.Entity<ResultadosVerificacion>(entity =>
        {
            entity.ToTable("ResultadosVerificacion");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BHP).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.CO).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.CO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.C_RECHAZO).HasDefaultValueSql("((0))");
            entity.Property(e => e.C_RECHAZO_OBD).HasDefaultValueSql("((0))");
            entity.Property(e => e.Etapa)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FCDIL).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FUERZA).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.FinalizacionPruebas).HasColumnType("datetime");
            entity.Property(e => e.HR_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.HR_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.InicioPruebas).HasColumnType("datetime");
            entity.Property(e => e.KPH).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.LAMDA).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.O2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.OBD_TIPO_SDB)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OPACIDADK).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.OPACIDADP).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.PAR_TOR).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.POT_FRENO).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.RESULTADO).HasDefaultValueSql("((0))");
            entity.Property(e => e.TEMP_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.TEMP_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.THP_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.THP_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.URLEvidenciaFalla).IsUnicode(false);
            entity.Property(e => e.VEL_ANG).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VEL_LIN).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VOLTS_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.VOLTS_5024).HasColumnType("decimal(9, 3)");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ResultadosVerificacion)
                .HasForeignKey<ResultadosVerificacion>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResultadosVerificacion_Verificacion");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rol__3214EC07385752A3");

            entity.ToTable("Rol");

            entity.Property(e => e.Alias)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .IsUnicode(false);

            entity.HasMany(d => d.IdPermisos).WithMany(p => p.IdRols)
                .UsingEntity<Dictionary<string, object>>(
                    "RolPermiso",
                    r => r.HasOne<Permiso>().WithMany()
                        .HasForeignKey("IdPermiso")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolPermiso_Permiso"),
                    l => l.HasOne<Rol>().WithMany()
                        .HasForeignKey("IdRol")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolPermiso_Rol"),
                    j =>
                    {
                        j.HasKey("IdRol", "IdPermiso");
                        j.ToTable("RolPermiso");
                    });
        });

        modelBuilder.Entity<SolicitudCertificado>(entity =>
        {
            entity.ToTable("SolicitudCertificado");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.SolicitudCertificados)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudCertificado_CatTipoCertificado");

            entity.HasOne(d => d.IdSolicitudFVNavigation).WithMany(p => p.SolicitudCertificados)
                .HasForeignKey(d => d.IdSolicitudFV)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudCertificado_SolicitudFV");
        });

        modelBuilder.Entity<SolicitudFV>(entity =>
        {
            entity.ToTable("SolicitudFV");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.FechaSolicitud).HasColumnType("datetime");

            entity.HasOne(d => d.IdAlmacenNavigation).WithMany(p => p.SolicitudFVs)
                .HasForeignKey(d => d.IdAlmacen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudFV_Almacen");

            entity.HasOne(d => d.IdCatEstatusSolicitudNavigation).WithMany(p => p.SolicitudFVs)
                .HasForeignKey(d => d.IdCatEstatusSolicitud)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudFV_CatEstatusSolicitud");

            entity.HasOne(d => d.IdUserSolicitaNavigation).WithMany(p => p.SolicitudFVs)
                .HasForeignKey(d => d.IdUserSolicita)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudFV_Users");
        });

        modelBuilder.Entity<StockCertificado>(entity =>
        {
            entity.ToTable("StockCertificado");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.StockCertificados)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockCertificado_CatTipoCertificado");

            entity.HasOne(d => d.IdStockMinimoNavigation).WithMany(p => p.StockCertificados)
                .HasForeignKey(d => d.IdStockMinimo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockCertificado_StockMinimo");
        });

        modelBuilder.Entity<StockMinimo>(entity =>
        {
            entity.ToTable("StockMinimo");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

            entity.HasOne(d => d.IdAlmacenNavigation).WithMany(p => p.StockMinimos)
                .HasForeignKey(d => d.IdAlmacen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockMinimo_Almacen");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.StockMinimos)
                .HasForeignKey(d => d.IdUserRegistro)
                .HasConstraintName("FK_StockMinimo_Users");
        });

        modelBuilder.Entity<TablaMaestra>(entity =>
        {
            entity.ToTable("TablaMaestra");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.LAMDA_MAX).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.O2_MAX).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_5024).HasColumnType("decimal(5, 1)");
        });

        modelBuilder.Entity<Testificacion>(entity =>
        {
            entity.ToTable("Testificacion");

            entity.Property(e => e.Combustible)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasColumnType("date");
            entity.Property(e => e.FolioOrigen).HasMaxLength(50);
            entity.Property(e => e.Marca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NumeroRef)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PersonaTramite)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SubMarca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UrlCertificado).IsUnicode(false);
            entity.Property(e => e.UrlIdentificacion).IsUnicode(false);
            entity.Property(e => e.UrlTarjetaCirculacion).IsUnicode(false);
            entity.Property(e => e.VigenciaOrigen).HasColumnType("date");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC074AE0379E");

            entity.Property(e => e.Curp)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FechaNacimiento).HasColumnType("datetime");
            entity.Property(e => e.Genero)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.LockoutEnd).HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(256);
            entity.Property(e => e.PersonGroupId)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PersonId)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Rfc)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UrlFirma)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlFoto)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlIne)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlSeguroSocial)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdVerificentro)
                .HasConstraintName("FK_Users_Verificentro");

            entity.HasMany(d => d.IdRols).WithMany(p => p.IdUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuarioRol",
                    r => r.HasOne<Rol>().WithMany()
                        .HasForeignKey("IdRol")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UsuarioRol_Rol"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UsuarioRol_Users"),
                    j =>
                    {
                        j.HasKey("IdUser", "IdRol").HasName("PK__UsuarioR__656DB3BC530CB369");
                        j.ToTable("UsuarioRol");
                    });
        });

        modelBuilder.Entity<UserPuestoVerificentro>(entity =>
        {
            entity.ToTable("UserPuestoVerificentro");

            entity.Property(e => e.FechaAcreditacionNorma).HasColumnType("datetime");
            entity.Property(e => e.FechaCapacitacionFinal).HasColumnType("datetime");
            entity.Property(e => e.FechaCapacitacionInicio).HasColumnType("datetime");
            entity.Property(e => e.FechaIncorpacion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.FechaSeparacion).HasColumnType("datetime");
            entity.Property(e => e.NumeroTrabajador)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlContrato)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusPuestoNavigation).WithMany(p => p.UserPuestoVerificentros)
                .HasForeignKey(d => d.IdCatEstatusPuesto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPuestoVerificentro_CatEstatusPuesto");

            entity.HasOne(d => d.IdPuestoNavigation).WithMany(p => p.UserPuestoVerificentros)
                .HasForeignKey(d => d.IdPuesto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPuestoVerificentro_Puesto");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserPuestoVerificentroIdUserNavigations)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPuestoVerificentro_Users");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.UserPuestoVerificentroIdUserRegistroNavigations)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPuestoVerificentro_Users1");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.UserPuestoVerificentros)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPuestoVerificentro_Verificentro");
        });

        modelBuilder.Entity<UsoSuministro>(entity =>
        {
            entity.ToTable("UsoSuministro");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nota)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Proveedor)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UrlFactura)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.UsoSuministros)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsoSuministro_Users");

            entity.HasOne(d => d.IdVerificentroNavigation).WithMany(p => p.UsoSuministros)
                .HasForeignKey(d => d.IdVerificentro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsoSuministro_Verificentro");
        });

        modelBuilder.Entity<VentaCertificado>(entity =>
        {
            entity.ToTable("VentaCertificado");

            entity.Property(e => e.Folios)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ImporteTotal).HasColumnType("numeric(12, 2)");

            entity.HasOne(d => d.IdCatTipoCertificadoNavigation).WithMany(p => p.VentaCertificados)
                .HasForeignKey(d => d.IdCatTipoCertificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VentaCertificado_CatTipoCertificado");

            entity.HasOne(d => d.IdVentaFVNavigation).WithMany(p => p.VentaCertificados)
                .HasForeignKey(d => d.IdVentaFV)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VentaCertificado_VentaFV");
        });

        modelBuilder.Entity<VentaFV>(entity =>
        {
            entity.ToTable("VentaFV");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.FechaVenta).HasColumnType("datetime");
            entity.Property(e => e.NumeroCompra)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.VentaFV)
                .HasForeignKey<VentaFV>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VentaFV_IngresoFV");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.VentaFVs)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VentaFV_Users");
        });

        modelBuilder.Entity<Verificacion>(entity =>
        {
            entity.ToTable("Verificacion");

            entity.HasIndex(e => e.Fecha, "idx_VpendientesImprimir");

            entity.HasIndex(e => e.IdCitaVerificacion, "idx_vista_verificacion");

            entity.Property(e => e.Combustible)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaVerificacion).HasColumnType("datetime");
            entity.Property(e => e.FolioCertificado).HasMaxLength(50);
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.REF00).HasDefaultValueSql("((0))");
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.URLFotoTecnico).IsUnicode(false);
            entity.Property(e => e.URLPlacaCamara).IsUnicode(false);
            entity.Property(e => e.URLPlacaCamaraTrasera).IsUnicode(false);
            entity.Property(e => e.URLPlacaDelantera).IsUnicode(false);
            entity.Property(e => e.URLPlacaTrasera).IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");

            entity.HasOne(d => d.IdCatSubDieselNavigation).WithMany(p => p.Verificacions)
                .HasForeignKey(d => d.IdCatSubDiesel)
                .HasConstraintName("FK_Verificacion_CatSubdieselVehiculo");

            entity.HasOne(d => d.IdCicloVerificacionNavigation).WithMany(p => p.Verificacions)
                .HasForeignKey(d => d.IdCicloVerificacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Verificacion_CicloVerificacion");

            entity.HasOne(d => d.IdCitaVerificacionNavigation).WithMany(p => p.Verificacions)
                .HasForeignKey(d => d.IdCitaVerificacion)
                .HasConstraintName("FK_Verificacion_CitaVerificacion");

            entity.HasOne(d => d.IdEquipoVerificacionNavigation).WithMany(p => p.Verificacions)
                .HasForeignKey(d => d.IdEquipoVerificacion)
                .HasConstraintName("FK_Verificacion_Equipo");

            entity.HasOne(d => d.IdLineaNavigation).WithMany(p => p.Verificacions)
                .HasForeignKey(d => d.IdLinea)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Verificacion_Linea");

            entity.HasOne(d => d.IdMotivoVerificacionNavigation).WithMany(p => p.Verificacions)
                .HasForeignKey(d => d.IdMotivoVerificacion)
                .HasConstraintName("FK__Verificac__IdMot__278EDA44");

            entity.HasOne(d => d.IdTipoCertificadoNavigation).WithMany(p => p.Verificacions)
                .HasForeignKey(d => d.IdTipoCertificado)
                .HasConstraintName("FK_Verificacion_CatTipoCertificado");

            entity.HasOne(d => d.IdUserTecnicoNavigation).WithMany(p => p.Verificacions)
                .HasForeignKey(d => d.IdUserTecnico)
                .HasConstraintName("FK_Verificacion_Users");
        });

        modelBuilder.Entity<Verificentro>(entity =>
        {
            entity.ToTable("Verificentro");

            entity.Property(e => e.ApiEndPoint).IsUnicode(false);
            entity.Property(e => e.ApiKey).IsUnicode(false);
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DirectorGestionCalidadAire)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.GerenteTecnico)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Latitud).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.Longitud).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.Municipio)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RepresentanteLegal)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Rfc)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vAcreditacionTecnicoEvidencium>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vAcreditacionTecnicoEvidencia");

            entity.Property(e => e.FechaAcreditacion).HasColumnType("datetime");
            entity.Property(e => e.NumeroAcreditacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSolicitud)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.TipoTramite)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UrlAcreditacion)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vAcreditacionTecnicoSolicitud>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vAcreditacionTecnicoSolicitud");

            entity.Property(e => e.Estatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSolicitud)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlAprobacion)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vAdministrativa>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vAdministrativa");

            entity.Property(e => e.ApiEndPoint).IsUnicode(false);
            entity.Property(e => e.ApiKey).IsUnicode(false);
            entity.Property(e => e.ClaveTramite)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Combustible)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaEmisionRef).HasColumnType("datetime");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.FolioAnterior).HasMaxLength(50);
            entity.Property(e => e.FolioAsignado).HasMaxLength(50);
            entity.Property(e => e.ImporteActual).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MotivoTramite)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombrePropietario)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTrabajadorCapturista)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificadoFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc4)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioRegistro).HasMaxLength(256);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vAlertum>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vAlerta");

            entity.Property(e => e.Data).IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.MovimientoFinal).IsUnicode(false);
            entity.Property(e => e.MovimientoInicial).IsUnicode(false);
            entity.Property(e => e.Nombre).HasMaxLength(256);
            entity.Property(e => e.TableName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Verificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vAlmacenStockReantante>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vAlmacenStockReantante");
        });

        modelBuilder.Entity<vAsignacionStock>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vAsignacionStock");

            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(256);
            entity.Property(e => e.NombreRecibio)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserAprobo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserRecibe)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vBusquedaPlacaSerie>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vBusquedaPlacaSerie");

            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vCALinea>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vCALinea");

            entity.Property(e => e.FechaRegistroLineaMotivo).HasColumnType("datetime");
            entity.Property(e => e.Motivo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Notas)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDocumento)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserRegistroMotivo).HasMaxLength(256);
        });

        modelBuilder.Entity<vCajaFV>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vCajaFV");

            entity.Property(e => e.ClaveCertificadoCatTipoCertificado)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.NombreCatTipoCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreRecibio)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vCalibracion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vCalibracion");

            entity.Property(e => e.FechaCalibracion).HasColumnType("datetime");
            entity.Property(e => e.FechaEmisionCertificad).HasColumnType("datetime");
            entity.Property(e => e.FechaProgramada).HasColumnType("datetime");
            entity.Property(e => e.FechaProximaCalibracion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NombreEquipo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreEstatusCalibracion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreLinea)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NombreUserRegistro).HasMaxLength(256);
            entity.Property(e => e.NombreUserValido).HasMaxLength(256);
            entity.Property(e => e.Nota)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.UrlResultado)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vCapacitacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vCapacitacion");

            entity.Property(e => e.FechaCapacitacion).HasColumnType("datetime");
            entity.Property(e => e.NombreCatEstatusCapacitacion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TemaCapacitacion)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vCapacitacionEmpleado>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vCapacitacionEmpleado");

            entity.Property(e => e.FechaCapacitacion).HasColumnType("date");
            entity.Property(e => e.Nombre).HasMaxLength(256);
            entity.Property(e => e.NumeroTrabajador)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TemaCapacitacion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UrlEvaluacion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlFotografia)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vCatMarcaVehiculoNombre>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vCatMarcaVehiculoNombres");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vCatSubMarcaVehiculo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vCatSubMarcaVehiculo");

            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vCitaVerificacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vCitaVerificacion");

            entity.Property(e => e.ColorVehiculo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DireccionVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Folio)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.LineaNombre)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NombreGeneraCita)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NombrePropietario)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Placa).HasMaxLength(10);
            entity.Property(e => e.RazonSocial).IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlComprobante)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Verificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vConfiguradorCitum>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vConfiguradorCita");

            entity.Property(e => e.Fecha).HasColumnType("date");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<vConstanciaUltimaVerificacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vConstanciaUltimaVerificacion");

            entity.Property(e => e.ClaveTramite)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CodigoQr)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Combustible)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DirectorGestionCalidadAire)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaEmisionRef).HasColumnType("datetime");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Folio)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.FolioCertificado)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Marca)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre).HasMaxLength(256);
            entity.Property(e => e.NombreRazonSocial)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PlacaSerie)
                .HasMaxLength(153)
                .IsUnicode(false);
            entity.Property(e => e.PlacaVerificacion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Semestre)
                .HasMaxLength(21)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SerieVerificacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ServidorPublico)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vConsultaCirculare>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vConsultaCirculares");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Mensaje).IsUnicode(false);
            entity.Property(e => e.NumeroCircular)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vConsultaStockDVRF>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vConsultaStockDVRF");

            entity.Property(e => e.Almacen)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ClaveCertificado)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombreTipoCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vConteoVerificacione>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vConteoVerificaciones");

            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vDataTramitesVentanilla>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vDataTramitesVentanilla");

            entity.Property(e => e.ApiEndPoint).IsUnicode(false);
            entity.Property(e => e.ApiKey).IsUnicode(false);
            entity.Property(e => e.CausaRechazo).HasMaxLength(255);
            entity.Property(e => e.ClaveTramite)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Combustible)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FolioAnterior).HasMaxLength(50);
            entity.Property(e => e.ImporteActual).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.Marca)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MotivoVerificacion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTrabajadorCapturista)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTrabajadorTecnico)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificadoFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vDevolucionSPF>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vDevolucionSPF");

            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(256);
            entity.Property(e => e.NumeroDevolucion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSolicitud)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RecibioSPF)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ResponsableEntrega)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlNombreDocumento1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlNombreDocumento2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlNombreDocumento3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserAprobo)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vDevolucionesSolicitudFecha>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vDevolucionesSolicitudFecha");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.NumeroSolicitud)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vDieselVehiculo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vDieselVehiculo");

            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vDocumentosCitum>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vDocumentosCita");

            entity.Property(e => e.ColorVehiculo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FolioTarjetaCirculacion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MarcaDiesel).HasMaxLength(50);
            entity.Property(e => e.SubMarca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Subdiesel)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.URLAltaPlacas).IsUnicode(false);
            entity.Property(e => e.URLBajaPlacas).IsUnicode(false);
            entity.Property(e => e.URLCartaFactura).IsUnicode(false);
            entity.Property(e => e.URLFactura).IsUnicode(false);
            entity.Property(e => e.URLIdentificacion).IsUnicode(false);
            entity.Property(e => e.URLValidacionCertificado).IsUnicode(false);
        });

        modelBuilder.Entity<vEquipo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vEquipo");

            entity.Property(e => e.Comentarios)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Estatus)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.FechaProximaCalibracion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NombreEquipo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreEstatusEquipo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreLinea)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NombreUsuarioRegistro).HasMaxLength(256);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSerie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UrlEspecificacionTecnica)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlFactura)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlManualUsuario)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlRecomendacionServicio)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vEquipoCalibracion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vEquipoCalibracion");

            entity.Property(e => e.FechaCalibracion).HasColumnType("datetime");
            entity.Property(e => e.FechaEmisionCertificad).HasColumnType("datetime");
            entity.Property(e => e.FechaProgramada).HasColumnType("datetime");
            entity.Property(e => e.FechaProximaCalibracion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nota)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.PrimeraFechaCalibracion).HasColumnType("datetime");
        });

        modelBuilder.Entity<vEquipoOrdenServicio>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vEquipoOrdenServicio");

            entity.Property(e => e.NombreEquipo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreEstatusEquipo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreLinea)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NombreUsuarioRegistro).HasMaxLength(256);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSerie)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vExento>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vExento");

            entity.Property(e => e.ClaveTramite)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Combustible)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Marca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NombresUser).HasMaxLength(256);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vFoliosCancelado>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vFoliosCancelados");

            entity.Property(e => e.DatosVehiculo)
                .HasMaxLength(203)
                .IsUnicode(false);
            entity.Property(e => e.EstatusFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaCancelacion).HasColumnType("datetime");
            entity.Property(e => e.Folio)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Motivo)
                .HasMaxLength(525)
                .IsUnicode(false);
            entity.Property(e => e.OtroMotivo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.PersonaRealizoTramite)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.TipoTramite)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioAprobo)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vFoliosFormaValoradaExentosImpresion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vFoliosFormaValoradaExentosImpresion");

            entity.Property(e => e.ApiEndPoint).IsUnicode(false);
            entity.Property(e => e.ApiKey).IsUnicode(false);
            entity.Property(e => e.Combustible)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Marca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NombreTecnico)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vFoliosFormaValoradaImpresion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vFoliosFormaValoradaImpresion");

            entity.Property(e => e.ApiEndPoint).IsUnicode(false);
            entity.Property(e => e.ApiKey).IsUnicode(false);
            entity.Property(e => e.CO2_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.CO2_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.COCO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ClaveTramite)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Combustible)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Etapa)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.FinalizacionPruebas).HasColumnType("datetime");
            entity.Property(e => e.FolioCertificadoAnterior).HasMaxLength(50);
            entity.Property(e => e.ImporteActual).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.InicioPruebas).HasColumnType("datetime");
            entity.Property(e => e.KPH_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.LAMDA_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.Linea)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Marca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NombreCapturista).HasMaxLength(256);
            entity.Property(e => e.NombreEquipo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreTecnico).HasMaxLength(256);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NumeroCapturista)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSerieEquipo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTecnico)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.O2_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.OBD_TIPO_SDB)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OPACIDADK).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.OPACIDADP).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TEMP_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.TEMP_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.THP_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.THP_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificadoFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.VOLTS_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.VOLTS_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vFoliosFormaValoradaTipo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vFoliosFormaValoradaTipo");

            entity.Property(e => e.ClaveCertificado)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NombreCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vFoliosFormaValoradaVerificentro>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vFoliosFormaValoradaVerificentro");

            entity.Property(e => e.CatMotivoCancelacion)
                .HasMaxLength(525)
                .IsUnicode(false);
            entity.Property(e => e.CatTipoCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ColorVehiculo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaCancelacion).HasColumnType("datetime");
            entity.Property(e => e.FechaCitaVerificacion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.ImporteActual).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombreDueñoVeh)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.OtroMotivo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserCancelo).HasMaxLength(256);
            entity.Property(e => e.Verificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vFoliosFormaValoradaVerificentroActuale>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vFoliosFormaValoradaVerificentroActuales");

            entity.Property(e => e.CatTipoCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ClaveCertificado)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Verificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vFoliosRegresadosSPF>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vFoliosRegresadosSPF");

            entity.Property(e => e.ClaveCertificado)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ClaveDevolucion)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ClaveSolicitud)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ClaveVenta)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FolioFinal)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FolioInicial)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PersonaValido)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ResponsableEntrega)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vFoliosUsadosVentanilla>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vFoliosUsadosVentanilla");

            entity.Property(e => e.ClaveTramite)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CorreoUsuario)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.DatosVehiculo)
                .HasMaxLength(333)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FolioCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MontoTramite).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PersonaGeneroTramite).HasMaxLength(256);
            entity.Property(e => e.Razon)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ReferenciaBancaria)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TipoTramite)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vFoliosVendidosCentrosVerificacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vFoliosVendidosCentrosVerificacion");

            entity.Property(e => e.ClaveVenta)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Cvv)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.IdFormaValorada).ValueGeneratedOnAdd();
            entity.Property(e => e.Monto).HasColumnType("money");
            entity.Property(e => e.PersonaRecibeCertificados)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ReferenciaBancaria)
                .HasMaxLength(40)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vHistorialCitum>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vHistorialCita");

            entity.Property(e => e.CitaSerie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ClaveLinea)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Folio)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Linea)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NombreCentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombrePersona)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Placa).HasMaxLength(10);
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlComprobante)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vHistoricoCitum>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vHistoricoCita");

            entity.Property(e => e.CitaSerie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ColorVehiculo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaCancelacion).HasColumnType("datetime");
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaReinicio).HasColumnType("datetime");
            entity.Property(e => e.Folio)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Linea)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NombreCentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombrePersona)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Placa).HasMaxLength(10);
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlComprobante)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vIndicadoresDashboard>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vIndicadoresDashboard");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Origen)
                .HasMaxLength(24)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vIngresoFV>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vIngresoFV");

            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NombreRecibio)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreUser).HasMaxLength(256);
        });

        modelBuilder.Entity<vIngresosTotalFecha>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vIngresosTotalFecha");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
        });

        modelBuilder.Entity<vInstalacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vInstalacion");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NombreProveedor)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreUsuario).HasMaxLength(256);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDocumento)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vIntervalosCita>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vIntervalosCitas");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
        });

        modelBuilder.Entity<vInventario>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vInventario");

            entity.Property(e => e.NombreCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vLineaEquipo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vLineaEquipos");

            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vLineaGrid>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vLineaGrid");

            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Estatus)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistroLinea).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistroLineaMotivo).HasColumnType("datetime");
            entity.Property(e => e.Motivo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreLinea)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NombreUsuario).HasMaxLength(256);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Notas)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDocumento)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vMovimientosInventarioEnStock>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vMovimientosInventarioEnStock");
        });

        modelBuilder.Entity<vMovimientosInventarioFueraStock>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vMovimientosInventarioFueraStock");
        });

        modelBuilder.Entity<vNumeroCitasHorario>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vNumeroCitasHorario");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaDia).HasColumnType("date");
        });

        modelBuilder.Entity<vOrdenServicio>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vOrdenServicio");

            entity.Property(e => e.CatNombreTipoOrden)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Empresa)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Motivo)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NombreEquipo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreLinea)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NombreTecnico)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreUsuario).HasMaxLength(256);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSerieActual)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSerieAnterior)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Solucion)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.UrlOS)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vPendientesImprimir>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vPendientesImprimir");

            entity.Property(e => e.ApiEndPoint).IsUnicode(false);
            entity.Property(e => e.ApiKey).IsUnicode(false);
            entity.Property(e => e.Combustible)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FolioAnterior).HasMaxLength(50);
            entity.Property(e => e.Marca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NombreCapturista).HasMaxLength(256);
            entity.Property(e => e.NombreTecnico).HasMaxLength(256);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTrabajadorCapturista)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTrabajadorTecnico)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario).IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vPersonal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vPersonal");

            entity.Property(e => e.CorreoUsuario).HasMaxLength(256);
            entity.Property(e => e.Curp)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FechaAcreditacionNorma).HasColumnType("datetime");
            entity.Property(e => e.FechaCapacitacionFinal).HasColumnType("datetime");
            entity.Property(e => e.FechaCapacitacionInicio).HasColumnType("datetime");
            entity.Property(e => e.FechaIncorporacionPuesto).HasColumnType("datetime");
            entity.Property(e => e.FechaNacimiento).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.FechaSeparacionPuesto).HasColumnType("datetime");
            entity.Property(e => e.Genero)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Nombre).HasMaxLength(256);
            entity.Property(e => e.NombrePuesto)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTrabajador)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Rfc)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TelefonoUsuario).HasMaxLength(20);
            entity.Property(e => e.TipoPuesto)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlContrato)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlFirma)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlFoto)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlIne)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlSeguroSocial)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vPersonalAutorizacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vPersonalAutorizacion");

            entity.Property(e => e.CorreoUsuario).HasMaxLength(256);
            entity.Property(e => e.Curp)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.EstatusPuesto)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FechaAcreditacionNorma).HasColumnType("datetime");
            entity.Property(e => e.FechaCapacitacionFinal).HasColumnType("datetime");
            entity.Property(e => e.FechaCapacitacionInicio).HasColumnType("datetime");
            entity.Property(e => e.FechaIncorporacionPuesto).HasColumnType("datetime");
            entity.Property(e => e.FechaNacimiento).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.FechaSeparacionPuesto).HasColumnType("datetime");
            entity.Property(e => e.Genero)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Nombre).HasMaxLength(256);
            entity.Property(e => e.NombrePuesto)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTrabajador)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Rfc)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TelefonoUsuario).HasMaxLength(20);
            entity.Property(e => e.TipoPuesto)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlContrato)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlFirma)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlFoto)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlIne)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlSeguroSocial)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vPlacaSerieBusquedaVerificacione>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vPlacaSerieBusquedaVerificaciones");

            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vPlacaSerieBusquedum>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vPlacaSerieBusqueda");

            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vProgramacionCalibracion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vProgramacionCalibracion");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NombreEquipo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreTipoCalibracion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreUser).HasMaxLength(256);
            entity.Property(e => e.NombreValido).HasMaxLength(256);
            entity.Property(e => e.Nota)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PrimeraFechaCalibracion).HasColumnType("datetime");
            entity.Property(e => e.UrlDocumento1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<vProveedorFolio>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vProveedorFolios");

            entity.Property(e => e.Equipo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Motivo)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Proveedor)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ProveedorEmpresa)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vRefrendo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vRefrendo");

            entity.Property(e => e.CatTipoCcertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ClaveTramite)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Combustible)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaCartaFactura).HasColumnType("datetime");
            entity.Property(e => e.FechaCartaFacturaExento).HasColumnType("datetime");
            entity.Property(e => e.FechaEmisionRef).HasColumnType("datetime");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Marca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre).HasMaxLength(256);
            entity.Property(e => e.NumTarjetaCirculacion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferenciaExento)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PlacaExento)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PropietarioExento)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ServidorPublico)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UltimoFolio).HasMaxLength(50);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1Exento)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2Exento)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3Exento)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
            entity.Property(e => e.VigenciaHoloAnterior).HasColumnType("datetime");
            entity.Property(e => e.VigenciaHoloAnteriorExento).HasColumnType("datetime");
        });

        modelBuilder.Entity<vReporte>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vReporte");

            entity.Property(e => e.FechaModificacionReporte).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistroReporte).HasColumnType("datetime");
            entity.Property(e => e.NombreCatEstatusReporte)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreUsuario).HasMaxLength(256);
            entity.Property(e => e.UrlEntrega)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlExcel)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vReporteEntregaTercer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vReporteEntregaTercer");

            entity.Property(e => e.ClaveCertificado)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaFinal).HasColumnType("datetime");
            entity.Property(e => e.FechaInicial).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NombreCatTipoReporte)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlEntrega)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserRegistro).HasMaxLength(256);
        });

        modelBuilder.Entity<vReposicion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vReposicion");

            entity.Property(e => e.ClaveTramite)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioRegistro).HasMaxLength(256);
        });

        modelBuilder.Entity<vReposicionCredencial>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vReposicionCredencial");

            entity.Property(e => e.Denuncia)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EstatusReporteCredencial)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.MotivoReporteCredencial)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre).HasMaxLength(256);
            entity.Property(e => e.NumeroTrabajador)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlCredencial)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vReposicionVerificacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vReposicionVerificacion");

            entity.Property(e => e.Combustible)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FolioCertificado).HasMaxLength(50);
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Semestre)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificadoFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vSeguimientoSolicitud>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vSeguimientoSolicitud");

            entity.Property(e => e.AlmacenFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EstatusFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FechaEntregaIFV).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistroFV).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistroVFV).HasColumnType("datetime");
            entity.Property(e => e.FechaSolicitudFV).HasColumnType("datetime");
            entity.Property(e => e.FechaVentaVFV).HasColumnType("datetime");
            entity.Property(e => e.ImporteTotal).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.NombreRecibioIFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroCompraVFV)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferenciaVFV)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1IFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1VFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2IFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2VFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3IFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3VFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserSolicitaFV).HasMaxLength(256);
        });

        modelBuilder.Entity<vSolicitudFormaValoradum>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vSolicitudFormaValorada");

            entity.Property(e => e.AlmacenFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ClaveCertificadoSC)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EstatusFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FechaEntregaIFV).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistroFV).HasColumnType("datetime");
            entity.Property(e => e.FechaSolicitudFV).HasColumnType("datetime");
            entity.Property(e => e.NombreRecibioIFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificadoSC)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserSolicitaFV).HasMaxLength(256);
        });

        modelBuilder.Entity<vStockMinimo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vStockMinimo");

            entity.Property(e => e.ClaveCertificado)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreAlmacen)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vTablaMaestra>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vTablaMaestra");

            entity.Property(e => e.LAMDA_MAX).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.O2_MAX).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.SubMarca)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vTestificacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vTestificacion");

            entity.Property(e => e.Combustible)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaEmisionRef).HasColumnType("datetime");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("date");
            entity.Property(e => e.FolioOrigen).HasMaxLength(50);
            entity.Property(e => e.Marca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NumeroRef)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PersonaTramite)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SubMarca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UrlCertificado).IsUnicode(false);
            entity.Property(e => e.UrlIdentificacion).IsUnicode(false);
            entity.Property(e => e.UrlTarjetaCirculacion).IsUnicode(false);
            entity.Property(e => e.VigenciaOrigen).HasColumnType("date");
        });

        modelBuilder.Entity<vUsoSuministro>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vUsoSuministro");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.NombreUser).HasMaxLength(256);
            entity.Property(e => e.NombreUsoSuministro)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nota)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Proveedor)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UrlFactura)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vUsuario>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vUsuarios");

            entity.Property(e => e.CorreoElectronico).HasMaxLength(256);
            entity.Property(e => e.NombreRol)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NombreUsuario).HasMaxLength(256);
        });

        modelBuilder.Entity<vVentaCertificado>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVentaCertificado");

            entity.Property(e => e.Folios)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ImporteTotal).HasColumnType("numeric(12, 2)");
        });

        modelBuilder.Entity<vVentaFV>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVentaFV");

            entity.Property(e => e.Certificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ClaveCertificado)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaVenta).HasColumnType("datetime");
            entity.Property(e => e.Folios)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ImporteTotal).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.NombreAlmacen)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroCompra)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Rfc)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserRegistro).HasMaxLength(256);
            entity.Property(e => e.Verificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vVentaFVDashboard>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVentaFVDashboard");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.FechaVenta).HasColumnType("datetime");
            entity.Property(e => e.ImporteTotal).HasColumnType("numeric(12, 2)");
        });

        modelBuilder.Entity<vVerificacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacion");

            entity.Property(e => e.Combustible)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaVerificacion).HasColumnType("datetime");
            entity.Property(e => e.FinalizacionPruebas).HasColumnType("datetime");
            entity.Property(e => e.FolioCertificado)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.InicioPruebas).HasColumnType("datetime");
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NombreDueñoVeh)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Semestre)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vVerificacionCitum>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacionCita");

            entity.Property(e => e.CitaSerie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ClaveLinea)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Folio)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Linea)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NombrePropietario)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vVerificacionDashboard>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacionDashboard");

            entity.Property(e => e.BHP).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.CO).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.CO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CausaRechazo).HasMaxLength(255);
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ClaveTramite)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Combustible)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Etapa)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FCDIL).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FUERZA).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.FechaFacturacion).HasColumnType("datetime");
            entity.Property(e => e.FechaVerificacion).HasColumnType("datetime");
            entity.Property(e => e.FinalizacionPruebas).HasColumnType("datetime");
            entity.Property(e => e.FolioCertificadoAnterior).HasMaxLength(50);
            entity.Property(e => e.HR_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.HR_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.InicioPruebas).HasColumnType("datetime");
            entity.Property(e => e.KPH).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.LAMDA).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_MAX).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MotivoVerificacion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombrePropietario)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.O2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_MAX).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.OBD_TIPO_SDB)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OPACIDADK).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.OPACIDADP).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.PAR_TOR).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.POT_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_FRENO).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SubMarca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TEMP_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.TEMP_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.THP_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.THP_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TipoServicio)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.VEL_ANG).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VEL_LIN).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VOLTS_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.VOLTS_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vVerificacionDatosLimite>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacionDatosLimites");

            entity.Property(e => e.BHP).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.CO).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.CO2_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.CO2_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.COCO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CorreoPropietario)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Etapa)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FCDIL).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FUERZA).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaCita).HasColumnType("datetime");
            entity.Property(e => e.FechaFacturacion).HasColumnType("datetime");
            entity.Property(e => e.FinalizacionPruebas).HasColumnType("datetime");
            entity.Property(e => e.HR_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.HR_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.InicioPruebas).HasColumnType("datetime");
            entity.Property(e => e.KPH).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.LAMDA).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_MAX).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NombreTecnico).HasMaxLength(256);
            entity.Property(e => e.O2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_MAX).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.OBD_TIPO_SDB)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OPACIDADK).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.OPACIDADP).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.PAR_TOR).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.POT_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_FRENO).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TEMP_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.TEMP_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.THP_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.THP_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.URLAltaPlacas).IsUnicode(false);
            entity.Property(e => e.URLBajaPlacas).IsUnicode(false);
            entity.Property(e => e.URLCartaFactura).IsUnicode(false);
            entity.Property(e => e.URLEvidenciaFalla).IsUnicode(false);
            entity.Property(e => e.URLFactura).IsUnicode(false);
            entity.Property(e => e.URLFotoTecnico).IsUnicode(false);
            entity.Property(e => e.URLIdentificacion).IsUnicode(false);
            entity.Property(e => e.URLPlacaCamara).IsUnicode(false);
            entity.Property(e => e.URLPlacaDelantera).IsUnicode(false);
            entity.Property(e => e.URLPlacaTrasera).IsUnicode(false);
            entity.Property(e => e.URLValidacionCertificado).IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.Property(e => e.VEL_ANG).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VEL_LIN).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VOLTS_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.VOLTS_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vVerificacionDatosProveedor>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacionDatosProveedor");

            entity.Property(e => e.BHP).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.CO).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.CO2_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.CO2_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.COCO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CorreoPropietario)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Etapa)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FCDIL).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FUERZA).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaCita).HasColumnType("datetime");
            entity.Property(e => e.FechaFacturacion).HasColumnType("datetime");
            entity.Property(e => e.FinalizacionPruebas).HasColumnType("datetime");
            entity.Property(e => e.HR_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.HR_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.InicioPruebas).HasColumnType("datetime");
            entity.Property(e => e.KPH).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.LAMDA).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_MAX).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NombreTecnico).HasMaxLength(256);
            entity.Property(e => e.O2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_MAX).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.OBD_TIPO_SDB)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OPACIDADK).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.OPACIDADP).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.PAR_TOR).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.POT_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_FRENO).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario).IsUnicode(false);
            entity.Property(e => e.RazonSocial).IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TEMP_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.TEMP_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.THP_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.THP_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.URLAltaPlacas).IsUnicode(false);
            entity.Property(e => e.URLBajaPlacas).IsUnicode(false);
            entity.Property(e => e.URLCartaFactura).IsUnicode(false);
            entity.Property(e => e.URLEvidenciaFalla).IsUnicode(false);
            entity.Property(e => e.URLFactura).IsUnicode(false);
            entity.Property(e => e.URLFotoTecnico).IsUnicode(false);
            entity.Property(e => e.URLIdentificacion).IsUnicode(false);
            entity.Property(e => e.URLPlacaCamara).IsUnicode(false);
            entity.Property(e => e.URLPlacaDelantera).IsUnicode(false);
            entity.Property(e => e.URLPlacaTrasera).IsUnicode(false);
            entity.Property(e => e.URLValidacionCertificado).IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.Property(e => e.VEL_ANG).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VEL_LIN).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VOLTS_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.VOLTS_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vVerificacionDatosProveedorUserDatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacionDatosProveedorUserData");

            entity.Property(e => e.BHP).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.CO).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.CO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CorreoPropietario)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Etapa)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FCDIL).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FUERZA).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaCita).HasColumnType("datetime");
            entity.Property(e => e.FechaFacturacion).HasColumnType("datetime");
            entity.Property(e => e.FinalizacionPruebas).HasColumnType("datetime");
            entity.Property(e => e.HR_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.HR_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.InicioPruebas).HasColumnType("datetime");
            entity.Property(e => e.KPH).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.LAMDA).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_MAX).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MotivoVerificacion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTrabajador)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.O2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_MAX).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.OBD_TIPO_SDB)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OPACIDADK).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.OPACIDADP).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.PAR_TOR).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.POT_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_FRENO).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TEMP_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.TEMP_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.THP_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.THP_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TipoServicio)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.URLAltaPlacas).IsUnicode(false);
            entity.Property(e => e.URLBajaPlacas).IsUnicode(false);
            entity.Property(e => e.URLCartaFactura).IsUnicode(false);
            entity.Property(e => e.URLEvidenciaFalla).IsUnicode(false);
            entity.Property(e => e.URLFactura).IsUnicode(false);
            entity.Property(e => e.URLFotoTecnico).IsUnicode(false);
            entity.Property(e => e.URLIdentificacion).IsUnicode(false);
            entity.Property(e => e.URLPlacaCamara).IsUnicode(false);
            entity.Property(e => e.URLPlacaCamaraTrasera).IsUnicode(false);
            entity.Property(e => e.URLPlacaDelantera).IsUnicode(false);
            entity.Property(e => e.URLPlacaTrasera).IsUnicode(false);
            entity.Property(e => e.URLValidacionCertificado).IsUnicode(false);
            entity.Property(e => e.VEL_ANG).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VEL_LIN).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VOLTS_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.VOLTS_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vVerificacionExpediente>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacionExpediente");

            entity.Property(e => e.BHP).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.CO).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.CO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.COCO2_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.CO_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CorreoPropietario)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Etapa)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FCDIL).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCDIL_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_2540).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FCNOX_5024).HasColumnType("decimal(8, 3)");
            entity.Property(e => e.FUERZA).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaCita).HasColumnType("datetime");
            entity.Property(e => e.FechaFacturacion).HasColumnType("datetime");
            entity.Property(e => e.FinalizacionPruebas).HasColumnType("datetime");
            entity.Property(e => e.HR_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.HR_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.InicioPruebas).HasColumnType("datetime");
            entity.Property(e => e.KPH).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.KPH_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.LAMDA).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.LAMDA_MAX).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MotivoVerificacion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTrabajador)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.O2).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.O2_MAX).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.OBD_TIPO_SDB)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OPACIDADK).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.OPACIDADP).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.PAR_TOR).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.POT_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.POT_FRENO).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TEMP_2540).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.TEMP_5024).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.THP_2540).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.THP_5024).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TipoServicio)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.URLAltaPlacas).IsUnicode(false);
            entity.Property(e => e.URLBajaPlacas).IsUnicode(false);
            entity.Property(e => e.URLCartaFactura).IsUnicode(false);
            entity.Property(e => e.URLEvidenciaFalla).IsUnicode(false);
            entity.Property(e => e.URLFactura).IsUnicode(false);
            entity.Property(e => e.URLFotoTecnico).IsUnicode(false);
            entity.Property(e => e.URLIdentificacion).IsUnicode(false);
            entity.Property(e => e.URLPlacaCamara).IsUnicode(false);
            entity.Property(e => e.URLPlacaCamaraTrasera).IsUnicode(false);
            entity.Property(e => e.URLPlacaDelantera).IsUnicode(false);
            entity.Property(e => e.URLPlacaTrasera).IsUnicode(false);
            entity.Property(e => e.URLValidacionCertificado).IsUnicode(false);
            entity.Property(e => e.VEL_ANG).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VEL_LIN).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.VOLTS_2540).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.VOLTS_5024).HasColumnType("decimal(9, 3)");
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vVerificacionInformacionGeneral>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacionInformacionGeneral");

            entity.Property(e => e.C_RECHAZO).HasMaxLength(255);
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaFacturacion).HasColumnType("datetime");
            entity.Property(e => e.Linea)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MotivoVerificacion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Placa).HasMaxLength(10);
            entity.Property(e => e.PlacaVerificacion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SerieVerificacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vVerificacionLinea>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacionLinea");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaVerificacion).HasColumnType("datetime");
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vVerificacionReposicion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacionReposicion");

            entity.Property(e => e.ApiEndPoint).IsUnicode(false);
            entity.Property(e => e.ApiKey).IsUnicode(false);
            entity.Property(e => e.ClaveTramite)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Combustible)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EntidadProcedencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.FolioCertificadoAnterior).HasMaxLength(50);
            entity.Property(e => e.ImporteActual).HasColumnType("numeric(12, 2)");
            entity.Property(e => e.Marca)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MotivoTramite)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreTecnico).HasMaxLength(256);
            entity.Property(e => e.NombreVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Propietario).IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TarjetaCirculacion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TipoCertificadoFV)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlDoc3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        modelBuilder.Entity<vVerificacionSinFolio>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacionSinFolio");
        });

        modelBuilder.Entity<vVerificacionesTipo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacionesTipo");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
        });

        modelBuilder.Entity<vVerificacionesVerificentro>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificacionesVerificentro");

            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vVerificentro>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVerificentro");

            entity.Property(e => e.ApiEndPoint).IsUnicode(false);
            entity.Property(e => e.ApiKey).IsUnicode(false);
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CorreoAutorizadoVerificentro)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DirectorGestionCalidadAire)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.GerenteTecnico)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Latitud).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.Longitud).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreAutorizadoVerificentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreCorto)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.RepresentanteLegal)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Rfc)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TelefonoAutorizadoVerificentro)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vVigenciaVerificacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVigenciaVerificacion");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Marca)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
