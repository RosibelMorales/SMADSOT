using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.IngresoFormaValorada.Response;
using Smadot.Models.Entities.ReporteMensual.Request;
using Smadot.Models.Entities.ReporteMensual.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using Smadot.Utilities.BlobStorage;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using NPOI.HPSF;
using Smadot.Models.Entities.SeguimientoCVV.Response;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Smadot.Models.Dicts.ProveedorDicts;

namespace Smadot.DirectorioCentrosVerificacion.Model.Negocio
{
    public class ReporteMensualNegocio : IReporteMensualNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;

        private string MensajeGenerico = "Error al realizar el registro, solo se puede realizar un registro por mes";
        public ReporteMensualNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public async Task<ResponseGeneric<List<ReporteMensualResponse>>> Consulta(ReporteMensualListRequest request)
        {
            try
            {
                var seguimiento = _context.vReportes.AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {

                    seguimiento = seguimiento.Where(x => x.NumeroReporte.ToString().Contains(request.Busqueda.ToLower()) || x.FechaModificacionReporte.ToString().Contains(request.Busqueda.ToLower()) || x.FechaRegistroReporte.ToString().Contains(request.Busqueda.ToLower()) || x.NombreCatEstatusReporte.ToLower().Contains(request.Busqueda.ToLower()) || x.NombreUsuario.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var tot = seguimiento.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    seguimiento = seguimiento.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    seguimiento = seguimiento.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }


                DateTime now = DateTime.Now;
                var result = await seguimiento.Select(x => new ReporteMensualResponse
                {
                    IdReporte = x.IdReporte,
                    NumeroReporte = x.NumeroReporte,
                    FechaModificacionReporte = x.FechaModificacionReporte,
                    FechaRegistroReporte = x.FechaRegistroReporte,
                    IdCatEstatusReporte = x.IdCatEstatusReporte,
                    NombreCatEstatusReporte = x.NombreCatEstatusReporte,
                    ActivoCatEstatusReporte = x.ActivoCatEstatusReporte,
                    IdUsuario = (long)x.IdUsuario,
                    NombreUsuario = x.NombreUsuario,
                    UrlEntrega = x.UrlEntrega,
                    UrlExcel = x.UrlExcel,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<ReporteMensualResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<ReporteMensualResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<ReporteMensualResponse>>> GetById(long? Id)
        {
            try
            {
                var user = _userResolver.GetUser();
                var result = new List<ReporteMensualResponse>();
                var certificado = _context.ReporteFVCancelada.Where(x => x.IdReporte == Id);
                if (Id != 0)
                {
                    certificado = _context.ReporteFVCancelada.Where(x => x.IdReporte == Id);
                    var solicitud = _context.vReportes.Where(x => x.IdReporte == Id);
                    if (solicitud != null)
                    {
                        string r = JsonConvert.SerializeObject(solicitud);
                        result = JsonConvert.DeserializeObject<List<ReporteMensualResponse>>(r);



                        var result2 = await certificado.Select(x => new ReporteMensualResponse
                        {
                            IdCatTipoCertificado = x.IdCatTipoCertificado,
                            IdReporte = x.IdReporte,
                            Cantidad = x.Cantidad,
                            FolioInicial = x.FolioInicial,
                            FolioFinal = x.FolioFinal
                        }).ToListAsync();

                        int sumaCantidad = (int)result2.Sum(x => x.Cantidad);
                        result2[0].Total = sumaCantidad;

                        result.AddRange(result2);
                    }
                }
                else
                {
                    var FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                    var FechaAux = FechaInicio.AddMonths(1).AddDays(-1);
                    var FechaFin = new DateTime(FechaAux.Year, FechaAux.Month, FechaAux.Day, 23, 59, 59);
                    var registros = _context.FoliosFormaValoradaVerificentros.Where(x => x.IdCatTipoTramite != null && x.FechaRegistro >= FechaInicio && x.FechaRegistro <= FechaFin && x.IdVerificentro == user.IdVerificentro);
                    var cancelados = await registros.CountAsync(x => x.Cancelado);
                    var entregados = await registros.CountAsync(x => x.Impreso);
                    // var totalPruebaRechazada = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.PruebaRechazada);
                    // var totalPruebaAprobada = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.PruebaAprobada);
                    // var totalPruebaEscapada = await indicadores.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.PruebaSinProcesar);
                    var verificaciones = _context.vIndicadoresDashboards.Where(x => x.FechaRegistro >= FechaInicio && x.FechaRegistro < FechaFin && (user.IdVerificentro == x.IdVerificentro));
                    var verificacionTipo = _context.vVerificacionesTipos.Where(x => x.Fecha >= FechaInicio && x.Fecha < FechaFin && (user.IdVerificentro == x.IdVerificentro));
                    var numeroPublicos = await verificacionTipo.CountAsync(x => x.Publico == true);
                    var numeroMercantil = await verificacionTipo.CountAsync(x => x.Mercantil == true);
                    var pruebasAprobadas = await verificaciones.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.PruebaAprobada);
                    var pruebasRechazo = await verificaciones.CountAsync(x => x.Origen == DictFiltroOrigenIndicadores.PruebaRechazada);
                    var pruebasRealizadas = pruebasAprobadas + pruebasRechazo;
                    // var inventarios = _context.Almacens.Include(x=> x.Inventarios).Where(x=> x.IdVerificentro== user.IdVerificentro).Sum(x=> x.Inventarios.);
                    var sinUsar = _context.vAlmacenStockReantantes.Where(x => x.IdVerificentro == user.IdVerificentro).FirstOrDefault();
                    result.Add(new ReporteMensualResponse
                    {
                        PruebasRealizadas = pruebasRealizadas,
                        PruebasAprobadas = pruebasAprobadas,
                        PruebasReprobadas = pruebasRechazo,
                        Entregados = entregados,
                        Cancelados = cancelados,
                        SinUsar = sinUsar?.Total ?? 0,
                        ServicioTransportePublico = numeroPublicos,
                        ServicioTransporteMercantil = numeroMercantil,
                    });
                    var foliosForma = _context.FoliosFormaValoradaVerificentros.Where(x => x.IdCatTipoTramite != null && x.FechaRegistro >= FechaInicio && x.FechaRegistro <= FechaFin && x.IdVerificentro == user.IdVerificentro);
                    foreach (var item in TipoCertificado.DictNombreCertificado)
                    {
                        var foliosTipo = foliosForma.Where(x => x.IdCatTipoCertificado == item.Key);
                        var folioInicial = foliosTipo.FirstOrDefault();
                        var folioFinal = foliosTipo.OrderByDescending(x => x.Id).FirstOrDefault();
                        result.Add(new ReporteMensualResponse
                        {
                            IdCatTipoCertificado = item.Key,
                            Cantidad = foliosTipo.Count(),
                            FolioInicial = folioInicial?.Folio ?? 0,
                            FolioFinal = folioFinal?.Folio ?? 0
                        });
                    }
                }


                return new ResponseGeneric<List<ReporteMensualResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<ReporteMensualResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> Registro(List<ReporteMensualResponse> request)
        {
            try
            {
                var reporteCancelado = new ReporteFVCanceladum();
                var reporte = new Reporte();
                var user = _userResolver.GetUser();

                if (request[0].IdReporte > 0 && request[0].NumeroReporte > 0)
                {
                    reporte = _context.Reportes.FirstOrDefault(x => x.Id == (long)request[0].IdReporte);
                    reporte.PruebasRealizadas = (int)request[0].PruebasRealizadas;
                    reporte.PruebasAprobadas = (int)request[0].PruebasAprobadas;
                    reporte.PruebasReprobadas = (int)request[0].PruebasReprobadas;
                    reporte.Entregados = (int)request[0].Entregados;
                    reporte.Cancelados = (int)request[0].Cancelados;
                    reporte.SinUsar = (int)request[0].SinUsar;
                    reporte.ServicioTransportePublico = (int)request[0].ServicioTransportePublico;
                    reporte.ServicioTransporteMercantil = (int)request[0].ServicioTransporteMercantil;
                    reporte.UltimaModificacion = DateTime.Now;
                    reporte.IdUserModifico = request[0].IdUsuario;
                    reporte.IdCatEstatusReporte = CatEstatusReporteDic.EnviadoDVRF;
                    reporte.IdVerificentro = user.IdVerificentro;
                    _context.Update(reporte);
                    var result = await _context.SaveChangesAsync() > 0;

                    if (request[0].Files != null)
                    {
                        foreach (var file in request[0].Files)
                        {
                            var url = await _blobStorage.UploadFileAsync(new byte[0], "ReporteMensual/" + reporte.Numero + "/" + file.Nombre, file.Base64);
                            if (!string.IsNullOrEmpty(url))
                            {
                                string extension = Path.GetExtension(file.Nombre);
                                switch (extension)
                                {
                                    case ".xlsx":
                                        reporte.UrlExcel = url; break;
                                    case ".pdf":
                                        reporte.UrlEntrega = url; break;
                                    case ".png":
                                        reporte.UrlEntrega = url; break;
                                    case ".jpg":
                                        reporte.UrlEntrega = url; break;
                                    case ".jpeg":
                                        reporte.UrlEntrega = url; break;
                                }
                            }
                        }
                        await _context.SaveChangesAsync();
                    }


                    foreach (var c in request)
                    {
                        reporteCancelado = _context.ReporteFVCancelada.FirstOrDefault(x => x.IdReporte == request[0].IdReporte && x.IdCatTipoCertificado == c.IdCatTipoCertificado);

                        reporteCancelado.IdCatTipoCertificado = (int)c.IdCatTipoCertificado;
                        reporteCancelado.Cantidad = (int)c.Cantidad;
                        reporteCancelado.FolioInicial = (int)c.FolioInicial;
                        reporteCancelado.FolioFinal = (int)c.FolioFinal;

                        _context.Update(reporteCancelado);
                        result = await _context.SaveChangesAsync() > 0;
                    }

                    return result ? new ResponseGeneric<long>(reporte.Numero) : new ResponseGeneric<long>();
                }
                else if (request[0].IdReporte > 0 && request[0].IdCatEstatusReporte != null)
                {
                    reporte = _context.Reportes.FirstOrDefault(x => x.Id == (long)request[0].IdReporte);
                    var alertadb = new Alertum();
                    switch (request[0].IdCatEstatusReporte)
                    {
                        case CatEstatusReporteDic.ValidadoDVRF:
                            reporte.IdCatEstatusReporte = CatEstatusReporteDic.ValidadoDVRF;
                            reporte.IdUserModifico = request[0].IdUsuario;
                            reporte.UltimaModificacion = (DateTime)request[0].FechaModificacionReporte;
                            alertadb = _context.Alerta.FirstOrDefault(x => x.TableId == reporte.Id && x.TableName == DictAlertas.Reportes && string.IsNullOrEmpty(x.MovimientoFinal));
                            if (alertadb != null)
                            {
                                alertadb.IdEstatusFinal = reporte.IdCatEstatusReporte;
                                alertadb.FechaModificacion = DateTime.Now;
                                alertadb.MovimientoFinal = string.Format(MovimientosDicts.DictAlertaReporteMensual[reporte.IdCatEstatusReporte], reporte.Numero, reporte.FechaRegistro.ToString("MMMM"), reporte.FechaRegistro.ToString("yyyy"));
                            }
                            _context.Update(reporte);
                            break;
                        case CatEstatusReporteDic.RechazadoDVRF:
                            reporte.IdCatEstatusReporte = CatEstatusReporteDic.RechazadoDVRF;
                            reporte.IdUserModifico = request[0].IdUsuario;
                            reporte.UltimaModificacion = (DateTime)request[0].FechaModificacionReporte;
                            alertadb = _context.Alerta.FirstOrDefault(x => x.TableId == reporte.Id && x.TableName == DictAlertas.Reportes && string.IsNullOrEmpty(x.MovimientoFinal));
                            if (alertadb != null)
                            {
                                alertadb.IdEstatusFinal = reporte.IdCatEstatusReporte;
                                alertadb.FechaModificacion = DateTime.Now;
                                alertadb.MovimientoFinal = string.Format(MovimientosDicts.DictAlertaReporteMensual[reporte.IdCatEstatusReporte], reporte.Numero, reporte.FechaRegistro.ToString("MMMM"), reporte.FechaRegistro.ToString("yyyy"));
                            }
                            _context.Update(reporte);
                            break;
                        case CatEstatusReporteDic.SolicitaModificar:
                            reporte.IdCatEstatusReporte = CatEstatusReporteDic.SolicitaModificar;
                            reporte.IdUserModifico = request[0].IdUsuario;
                            reporte.UltimaModificacion = (DateTime)request[0].FechaModificacionReporte;
                            _context.Update(reporte);
                            Alertum alerta = new()
                            {
                                TableName = DictAlertas.Reportes,
                                TableId = reporte.Id,
                                IdVerificentro = _userResolver.GetUser().IdVerificentro ?? 1,
                                Data = JsonConvert.SerializeObject(reporte, new JsonSerializerSettings
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                                    NullValueHandling = NullValueHandling.Ignore
                                }),
                                IdUser = _userResolver.GetUser().IdUser,
                                MovimientoInicial = string.Format(MovimientosDicts.DictAlertaReporteMensual[reporte.IdCatEstatusReporte], reporte.Numero, reporte.FechaRegistro.ToString("MMMM"), reporte.FechaRegistro.ToString("yyyy")),
                                Fecha = DateTime.Now,
                                IdEstatusInicial = CalibracionEstatus.SolicitaModificar,
                                MovimientoFinal = null,
                                FechaModificacion = null,
                                Leido = false,
                                Procesada = false
                            };
                            _context.Alerta.Add(alerta);
                            break;
                        case CatEstatusReporteDic.PermiteModificar:
                            reporte.IdCatEstatusReporte = CatEstatusReporteDic.PermiteModificar;
                            reporte.IdUserModifico = request[0].IdUsuario;
                            reporte.UltimaModificacion = (DateTime)request[0].FechaModificacionReporte;
                            alertadb = _context.Alerta.FirstOrDefault(x => x.TableId == reporte.Id && x.TableName == DictAlertas.Reportes && string.IsNullOrEmpty(x.MovimientoFinal));
                            if (alertadb != null)
                            {
                                alertadb.IdEstatusFinal = reporte.IdCatEstatusReporte;
                                alertadb.FechaModificacion = DateTime.Now;
                                alertadb.MovimientoFinal = string.Format(MovimientosDicts.DictAlertaReporteMensual[reporte.IdCatEstatusReporte], reporte.Numero, reporte.FechaRegistro.ToString("MMMM"), reporte.FechaRegistro.ToString("yyyy"));
                            }
                            _context.Update(reporte);
                            break;
                        case CatEstatusReporteDic.RechazaModificar:
                            reporte.IdCatEstatusReporte = CatEstatusReporteDic.RechazaModificar;
                            reporte.IdUserModifico = request[0].IdUsuario;
                            reporte.UltimaModificacion = (DateTime)request[0].FechaModificacionReporte;
                            alertadb = _context.Alerta.FirstOrDefault(x => x.TableId == reporte.Id && x.TableName == DictAlertas.Reportes && string.IsNullOrEmpty(x.MovimientoFinal));
                            if (alertadb != null)
                            {
                                alertadb.IdEstatusFinal = reporte.IdCatEstatusReporte;
                                alertadb.FechaModificacion = DateTime.Now;
                                alertadb.MovimientoFinal = string.Format(MovimientosDicts.DictAlertaReporteMensual[reporte.IdCatEstatusReporte], reporte.Numero, reporte.FechaRegistro.ToString("MMMM"), reporte.FechaRegistro.ToString("yyyy"));
                            }
                            _context.Update(reporte);
                            break;
                        default:
                            break;
                    }
                    var result = await _context.SaveChangesAsync() > 0;
                    return result ? new ResponseGeneric<long>(reporte.Numero) : new ResponseGeneric<long>();

                }

                else
                {
                    var lista = new List<ReporteMensualResponse>();
                    var lista2 = new List<ReporteMensualResponse>();
                    var mesSistema = DateTime.Now.Month;
                    var reporteValidacion = _context.vReportes.Where(x => x.FechaRegistroReporte.Month == mesSistema);
                    var numeroReporte = _context.vReportes.Where(x => x.NumeroReporte == request[0].NumeroReporte);

                    string r = JsonConvert.SerializeObject(reporteValidacion);
                    lista = JsonConvert.DeserializeObject<List<ReporteMensualResponse>>(r);

                    string r2 = JsonConvert.SerializeObject(numeroReporte);
                    lista2 = JsonConvert.DeserializeObject<List<ReporteMensualResponse>>(r2);

                    if (lista.Count == 0 && lista2.Count == 0)
                    {
                        reporte = new Reporte
                        {
                            Numero = (long)request[0].NumeroReporte,
                            FechaRegistro = DateTime.Now,
                            IdUserRegistro = request[0].IdUsuario,
                            PruebasRealizadas = (int)request[0].PruebasRealizadas,
                            PruebasAprobadas = (int)request[0].PruebasAprobadas,
                            PruebasReprobadas = (int)request[0].PruebasReprobadas,
                            Entregados = (int)request[0].Entregados,
                            Cancelados = (int)request[0].Cancelados,
                            SinUsar = (int)request[0].SinUsar,
                            ServicioTransportePublico = (int)request[0].ServicioTransportePublico,
                            ServicioTransporteMercantil = (int)request[0].ServicioTransporteMercantil,
                            UrlEntrega = " ",
                            UrlExcel = " ",
                            UltimaModificacion = DateTime.Now,
                            IdUserModifico = request[0].IdUsuario,
                            IdCatEstatusReporte = CatEstatusReporteDic.EnviadoDVRF,
                            IdVerificentro = user.IdVerificentro
                        };
                        _context.Reportes.Add(reporte);
                        var result = await _context.SaveChangesAsync() > 0;
                        Alertum alerta = new()
                        {
                            TableName = DictAlertas.Reportes,
                            TableId = reporte.Id,
                            IdVerificentro = _userResolver.GetUser().IdVerificentro ?? 1,
                            Data = JsonConvert.SerializeObject(reporte, new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                PreserveReferencesHandling = PreserveReferencesHandling.None,
                                NullValueHandling = NullValueHandling.Ignore
                            }),
                            IdUser = _userResolver.GetUser().IdUser,
                            MovimientoInicial = string.Format(MovimientosDicts.DictAlertaReporteMensual[reporte.IdCatEstatusReporte], reporte.Numero, reporte.FechaRegistro.ToString("MMMM"), reporte.FechaRegistro.ToString("yyyy")),
                            Fecha = DateTime.Now,
                            IdEstatusInicial = CalibracionEstatus.SolicitaModificar,
                            MovimientoFinal = null,
                            FechaModificacion = null,
                            Leido = false,
                            Procesada = false
                        };

                        _context.Alerta.Add(alerta);
                        var j = 0;
                        foreach (var file in request[0].Files)
                        {
                            var url = _blobStorage.UploadFileAsync(new byte[0], "ReporteMensual/" + reporte.Numero + "/" + file.Nombre, file.Base64).Result;
                            if (!string.IsNullOrEmpty(url))
                            {
                                switch (j)
                                {
                                    case 0:
                                        reporte.UrlEntrega = url; break;
                                    case 1:
                                        reporte.UrlExcel = url; break;
                                }
                            }
                            j++;
                        }
                        await _context.SaveChangesAsync();

                        foreach (var c in request)
                        {
                            reporteCancelado = new ReporteFVCanceladum
                            {
                                IdReporte = reporte.Id,
                                IdCatTipoCertificado = (int)c.IdCatTipoCertificado,
                                Cantidad = (int)c.Cantidad,
                                FolioInicial = (int)c.FolioInicial,
                                FolioFinal = (int)c.FolioFinal,
                            };
                            _context.ReporteFVCancelada.Add(reporteCancelado);
                            await _context.SaveChangesAsync();
                        }
                        return result ? new ResponseGeneric<long>(reporte.Id) : new ResponseGeneric<long>();
                    }
                    else
                    {
                        return new ResponseGeneric<long>(0);
                    }
                }

            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex);
            }
        }
    }
}

public interface IReporteMensualNegocio
{
    public Task<ResponseGeneric<List<ReporteMensualResponse>>> Consulta(ReporteMensualListRequest request);

    public Task<ResponseGeneric<List<ReporteMensualResponse>>> GetById(long? Id);

    Task<ResponseGeneric<long>> Registro(List<ReporteMensualResponse> request);

}