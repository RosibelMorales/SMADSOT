using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.Entities.EstadisticasUsoFormaValorada.Request;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;

namespace Smadot.SolicitudFormaValorada.Model.Negocio
{
    public class DashboardNegocio : IDashboardNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        public DashboardNegocio(SmadotDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }
        public async Task<ResponseGeneric<DashboardIndicadores>> EstadisticasDashboard(EstadisticaUsoFormaValoradaRequest request)
        {
            try
            {
                request.FechaFinal = request.FechaFinal.AddDays(1);

                var indicadores = _context.vIndicadoresDashboards.Where(x => x.FechaRegistro >= request.FechaInicial && x.FechaRegistro < request.FechaFinal && (request.IdVerificentro == null || request.IdVerificentro == x.IdVerificentro));
                var personal = _context.vIndicadoresDashboards.Where(x => request.IdVerificentro == null || request.IdVerificentro == x.IdVerificentro);
                // Realizamos todas las consultas a la base de datos de los indicadores agrupados en la misma vista
                var totalCertificadosUsados = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.CertificadosUsados);
                var totalReporte = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.Reporte);
                var totalReporteTercer = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.ReporteTercer);
                var totalPruebaRechazada = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.PruebaRechazada);
                var totalPruebaAprobada = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.PruebaAprobada);
                var totalPruebaEscapada = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.PruebaSinProcesar);
                var totalPrueba = totalPruebaAprobada + totalPruebaEscapada + totalPruebaRechazada;
                var totalPersonalTecnico = await personal.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.PersonalTecnico);
                var totalPersonalAdministrativo = await personal.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.PersonalAdministrativo);
                var totalPersonalCapacitacion = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.PersonalCapacitacion);
                var totalCalibracionLaboratorio = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.CalibracionLaboratorio);
                var totalCalibracionProveedor = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.CalibracionProveedor);
                var totalCitaAtendida = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.CitaAtendida);
                var totalCitaNoAtendida = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.CitaNoAtendida);
                var totalCita = totalCitaNoAtendida + totalCitaAtendida;

                //Montos decimales
                var dbVentaCvv = await _context.vVentaFVDashboards.Where(x => x.FechaRegistro >= request.FechaInicial && x.FechaRegistro < request.FechaFinal).ToListAsync();
                // var countVentaCvv = dbVentaCvv.Count();
                var totalVenta = dbVentaCvv.Sum(x => x.ImporteTotal);
                var listTotales = new List<Indicador>{
                    new("Pruebas Aprobadas",totalPruebaAprobada.ToString("0"),"bg-success"),
                    new("Pruebas Rechazadas",totalPruebaRechazada.ToString("0"),"bg-danger"),
                    new("Pruebas Sin Procesar",totalPruebaEscapada.ToString("0"),"bg-warning"),
                    new("Pruebas Totales",totalPrueba.ToString("0"),"bg-gray"),

                    new("Citas Atendidas",totalCitaAtendida.ToString("0"),"bg-success"),
                    new("Citas No Atendidas",totalCitaNoAtendida.ToString("0"),"bg-danger"),
                    new("Total de Citas",totalCita.ToString("0"),"bg-gray"),
                    new("Monto de Ventas",totalVenta.ToString("C2", new CultureInfo("es-MX")),"bg-info"),

                    new("No. de Certificados Usados",totalCertificadosUsados.ToString("0"),"bg-dark"),
                    new("Personal Administrativo",totalPersonalAdministrativo.ToString("0"),"bg-primary-smadsot"),
                    new("Personal Técnico",totalPersonalTecnico.ToString("0"),"bg-primary"),
                    new("Personal En Capacitación",totalPersonalCapacitacion.ToString("0"),"bg-gray"),

                    new("Calibraciones Laboratorio",totalCalibracionLaboratorio.ToString("0"),"bg-secondary"),
                    new("Calibraciones Proveedor",totalCalibracionProveedor.ToString("0"),"bg-secondary"),
                    new("Reportes Mensuales",totalReporte.ToString("0"),"bg-secondary"),
                    new("Resportes Tercer",totalReporteTercer.ToString("0"),"bg-secondary"),
                };
                var result = new DashboardIndicadores(listTotales);
                return new ResponseGeneric<DashboardIndicadores>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DashboardIndicadores>(ex);
            }
        }

        public async Task<ResponseGeneric<ResponseGrid<EstadisiticaUsoFormaValoradaResponseGridAcciones>>> ConsultaEstadisticasDashboard(EstadisticaUsoFormaValoradaGridRequest request)
        {
            try
            {

                request.FechaFinal = request.FechaFinal.AddDays(1);

                var registros = _context.vVerificacionDashboards.Where(x => x.FechaVerificacion >= request.FechaInicial && x.FechaVerificacion < request.FechaFinal
                && (request.IdVerificentro == null || request.IdVerificentro == x.IdVerificentro));

                var tot = await registros.CountAsync();

                if (request.TipoHolograma != null && request.TipoHolograma > 0)
                {
                    registros = registros.Where(x => x.IdTipoCertificado == request.TipoHolograma);
                }
                if (request.ModeloMin != null && request.ModeloMin > 1920)
                {
                    registros = registros.Where(x => x.Modelo >= request.ModeloMin);
                }
                if (request.ModeloMax != null && request.ModeloMax > 1920)
                {
                    registros = registros.Where(x => x.Modelo <= request.ModeloMax);
                }
                if (request.CausaRechazo != null && request.CausaRechazo > 0)
                {
                    registros = registros.Where(x => x.C_RECHAZO == request.CausaRechazo);
                }
                if (request.IdMotivoVerificacion != null && request.IdMotivoVerificacion > 0)
                {
                    registros = registros.Where(x => x.IdMotivoVerificacion == request.IdMotivoVerificacion);
                }
                if (request.IdCatTipoServicio != null && request.IdCatTipoServicio > 0)
                {
                    registros = registros.Where(x => x.IdCatTipoServicio == request.IdCatTipoServicio);
                }

                // if (!string.IsNullOrEmpty(request.Busqueda))
                // {
                //     registros = registros.Where(x =>
                //         x.Combustible.ToLower().Contains(request.Busqueda.ToLower()));

                //     // filtered = registros.Count();
                // }
                var filtered = await registros.CountAsync();
                if (request.EsGrid)
                {

                    if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                    {
                        registros = registros.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                    }

                    if (request.Pagina > 0 && request.Registros > 0)
                    {
                        registros = registros.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                    }
                }
                DateTime now = DateTime.Now;
                var list = await registros.ToListAsync();
                var result = new ResponseGrid<EstadisiticaUsoFormaValoradaResponseGridAcciones>
                {
                    RecordsTotal = tot,
                    RecordsFiltered = filtered,
                    Data = JsonConvert.DeserializeObject<List<EstadisiticaUsoFormaValoradaResponseGridAcciones>>(JsonConvert.SerializeObject(list)) ?? new()
                };
                return new ResponseGeneric<ResponseGrid<EstadisiticaUsoFormaValoradaResponseGridAcciones>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<EstadisiticaUsoFormaValoradaResponseGridAcciones>>(ex);
            }
        }
        public async Task<ResponseGeneric<List<VerificacionCVVRangoFecha>>> EstadisticasDashboardGraphBar(EstadisticaUsoFormaValoradaRequest request)
        {
            try
            {
                request.FechaFinal = request.FechaFinal.AddDays(1);

                var conteoVerificaciones = await _context.vConteoVerificaciones
                                            .Where(v => v.Fecha >= request.FechaInicial && v.Fecha < request.FechaFinal) // Aplicar filtro por rango de fechas
                                            .GroupBy(v => new { v.IdVerificentro, v.NombreVerificentro, v.Clave })
                                            .Select(g => new VerificacionCVVRangoFecha
                                            {
                                                IdVerificentro = g.Key.IdVerificentro,
                                                Clave = g.Key.Clave,
                                                Aprobadas = g.Sum(v => v.Aprobadas),
                                                Rechazadas = g.Sum(v => v.Rechazadas),
                                                NoProcesadas = g.Sum(v => v.NoProcesadas),
                                                Todas = g.Sum(v => v.Todas)
                                            })
                                            .ToListAsync();
                return new ResponseGeneric<List<VerificacionCVVRangoFecha>>(conteoVerificaciones);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<VerificacionCVVRangoFecha>>(ex);
            }
        }
        public async Task<ResponseGeneric<List<CertificadoMapeo>>> GetConteoFolios(EstadisticaUsoFormaValoradaRequest request)
        {
            try
            {
                request.FechaFinal = request.FechaFinal.AddDays(1);

                var conteoVerificaciones = await _context.vFoliosFormaValoradaTipos
                                            .Where(v => v.FechaRegistro >= request.FechaInicial && v.FechaRegistro < request.FechaFinal && (request.IdVerificentro == null || request.IdVerificentro == v.IdVerificentro)) // Aplicar filtro por rango de fechas
                                            .GroupBy(v => new { v.IdCatTipoCertificado, v.NombreCertificado, v.ClaveCertificado })
                                            .Select(g => new CertificadoMapeo
                                            {
                                                Id = g.Key.IdCatTipoCertificado,
                                                Nombre = g.Key.NombreCertificado,
                                                Clave = g.Key.ClaveCertificado,
                                                Total = g.Count(),
                                            })
                                            .ToListAsync();
                var certificadosFaltantes = (request.IdVerificentro == 18 || request.IdVerificentro == null) ? TipoCertificado.DictNombreCertificado.Where(x => !conteoVerificaciones.Any(c => x.Key == c.Id)) : TipoCertificado.DictNombreCertificadoCVV.Where(x => !conteoVerificaciones.Any(c => x.Key == c.Id));
                foreach (var item in certificadosFaltantes)
                {
                    conteoVerificaciones.Add(new()
                    {
                        Id = item.Key,
                        Nombre = item.Value
                    });
                }
                conteoVerificaciones = conteoVerificaciones.OrderBy(x => x.Id).ToList();

                return new ResponseGeneric<List<CertificadoMapeo>>(conteoVerificaciones);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<CertificadoMapeo>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<DataPieChart>>> EstadisticasDashboardGraphPie(EstadisticaUsoFormaValoradaRequest request)
        {
            try
            {
                request.FechaFinal = request.FechaFinal.AddDays(1);

                var _dbAdmin = await _context.Administrativas.Where(x => x.FechaRegistro >= request.FechaInicial && x.FechaRegistro < request.FechaFinal).CountAsync();

                var _dbExentos = await _context.Exentos.Include(x => x.FoliosFormaValoradaVerificentros).Where(x => x.FoliosFormaValoradaVerificentros.Any(y => y.FechaRegistro >= request.FechaInicial && y.FechaRegistro < request.FechaFinal)).CountAsync();

                var _dbVerificacion = await _context.Verificacions.Where(x => x.Fecha >= request.FechaInicial && x.Fecha < request.FechaFinal).CountAsync();
                var _dbTestificacion = await _context.Testificacions.Where(x => x.FechaRegistro >= request.FechaInicial && x.FechaRegistro < request.FechaFinal).CountAsync();

                var result = new List<DataPieChart>{

                        new DataPieChart
                        {
                            Category = "Administrativas",
                            Value = _dbAdmin
                        },new DataPieChart
                        {
                            Category = "Exentos",
                            Value = _dbExentos
                        },
                        new DataPieChart
                        {
                            Category = "Verificaciones",
                            Value = _dbVerificacion
                        },
                        new DataPieChart
                        {
                            Category = "Testificacion",
                            Value = _dbTestificacion
                        }
                };

                return new ResponseGeneric<List<DataPieChart>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<DataPieChart>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<DataGaugeChart>>> EstadisticasDashboardGraphGauge(EstadisticaUsoFormaValoradaRequest request)
        {
            try
            {
                request.FechaFinal = request.FechaFinal.AddDays(1);


                var ffvv = _context.FoliosFormaValoradaVerificentros.Where(x => x.FechaRegistro >= request.FechaInicial && x.FechaRegistro < request.FechaFinal && x.IdCatTipoTramite != null).AsQueryable();

                var _dbDobleCero = await ffvv.Where(x => x.IdCatTipoCertificado == TipoCertificado.DobleCero).CountAsync();
                var _dbCero = await ffvv.Where(x => x.IdCatTipoCertificado == TipoCertificado.Cero).CountAsync();
                var _dbUno = await ffvv.Where(x => x.IdCatTipoCertificado == TipoCertificado.Uno).CountAsync();
                var _dbDos = await ffvv.Where(x => x.IdCatTipoCertificado == TipoCertificado.Dos).CountAsync();
                var _dbCNoAprobado = await ffvv.Where(x => x.IdCatTipoCertificado == TipoCertificado.ConstanciasNoAprobado).CountAsync();

                var _dbExentos = await _context.Exentos.Include(x => x.FoliosFormaValoradaVerificentros).Where(x => x.FoliosFormaValoradaVerificentros.Any(y => y.FechaRegistro >= request.FechaInicial && y.FechaRegistro < request.FechaFinal && y.IdCatTipoCertificado == TipoCertificado.Exentos && y.IdCatTipoTramite != null)).CountAsync();
                var _dbTestificacion = await _context.Testificacions.Where(x => x.FechaRegistro >= request.FechaInicial && x.FechaRegistro < request.FechaFinal).CountAsync();
                //var full = dataDobleCero + dataCero + dataUno + dataDos + dataCNoAprobado + dataExentos;
                var result = new List<DataGaugeChart>{
                    new DataGaugeChart
                    {
                        Category = "Doble Cero 00",
                        Full = 100,
                        Value = _dbDobleCero
                    },
                    new DataGaugeChart
                    {
                        Category = "Cero 0",
                        Full = 100,
                        Value = _dbCero
                    },
                    new DataGaugeChart
                    {
                        Category = "Uno 1",
                        Full = 100,
                        Value = _dbUno
                    },
                    new DataGaugeChart
                    {
                        Category = "Dos 2",
                        Full = 100,
                        Value = _dbDos
                    },
                    new DataGaugeChart
                    {
                        Category = "Constancias de No Aprobado",
                        Full = 100,
                        Value = _dbCNoAprobado
                    },
                    new DataGaugeChart
                    {
                        Category = "Exentos",
                        Full = 100,
                        Value = _dbExentos
                    },
                    new DataGaugeChart
                    {
                        Category = "Testificación",
                        Full = 100,
                        Value = _dbTestificacion
                    }
                };
                return new ResponseGeneric<List<DataGaugeChart>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<DataGaugeChart>>(ex);
            }
        }

    }
    public interface IDashboardNegocio
    {
        Task<ResponseGeneric<DashboardIndicadores>> EstadisticasDashboard(EstadisticaUsoFormaValoradaRequest request);
        Task<ResponseGeneric<ResponseGrid<EstadisiticaUsoFormaValoradaResponseGridAcciones>>> ConsultaEstadisticasDashboard(EstadisticaUsoFormaValoradaGridRequest request);
        Task<ResponseGeneric<List<CertificadoMapeo>>> GetConteoFolios(EstadisticaUsoFormaValoradaRequest request);
        Task<ResponseGeneric<List<VerificacionCVVRangoFecha>>> EstadisticasDashboardGraphBar(EstadisticaUsoFormaValoradaRequest request);
        Task<ResponseGeneric<List<DataPieChart>>> EstadisticasDashboardGraphPie(EstadisticaUsoFormaValoradaRequest request);
        Task<ResponseGeneric<List<DataGaugeChart>>> EstadisticasDashboardGraphGauge(EstadisticaUsoFormaValoradaRequest request);
    }
}