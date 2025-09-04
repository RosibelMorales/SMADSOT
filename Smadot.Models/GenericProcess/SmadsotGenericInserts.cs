using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.Entities.RecepcionDocumentos.Request;
using Smadot.Models.Entities.Verificacion;
using static Smadot.Models.Entities.Generic.Response.TablaFijaViewModel;

namespace Smadot.Models.GenericProcess
{
    public class SmadsotGenericInserts /*: ISmadsotGenericInserts*/
    {
        private readonly SmadotDbContext _context;
        private readonly IConfiguration _configuration;

        public SmadsotGenericInserts(SmadotDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<ResponseValidacion> ValidateFolio(int idCatTipoCertificado, long idVerificentro, int claveTramite, long idUser, string? estado, long? idVerificacion, long? idAdministrativa, long? idExento, long? idTestificacion = null, bool reposicion = false, long? FolioCerticado = null, decimal? importe = null, DateTime? fechaResgistro=null)
        {
            var validRecord = new ResponseValidacion();
            var certificado = TipoCertificado.DictNombreCertificado[idCatTipoCertificado];

            try
            {
                long numeroFolio = 0;
                FoliosFormaValoradaActuale ultimoFolio = new();
                if (FolioCerticado == null)
                {

                    var ultimoFolioAux = await _context.FoliosFormaValoradaActuales
                        .Where(x => x.IdCatTipoCertificado == idCatTipoCertificado && x.IdVerificentro == idVerificentro)
                        .FirstOrDefaultAsync();

                    if (ultimoFolioAux == null)
                    {
                        validRecord.IsSucces = false;
                        validRecord.Description = $"No se encontró el último folio {certificado} para continuar con el conteo.";
                        return validRecord;
                    }
                    ultimoFolio = ultimoFolioAux;
                    numeroFolio = ultimoFolio.Folio + 1;
                    numeroFolio = GenerarFolioSaltoCancelados(numeroFolio);
                    JObject jObject = new()
                    {
                        ["ultimoFolioRegistrado"] = ultimoFolio.Folio,
                        ["folioAsignado"] = numeroFolio,
                        ["tipoFolio"] = idCatTipoCertificado,
                        ["idVerificacion"] = idVerificacion,
                        ["idVerificentro"] = idVerificentro,
                        ["tipo"] = "LogCertificado"
                    };
                    await _context.Errors.AddAsync(new()
                    {
                        Created = DateTime.Now,
                        Values = JsonConvert.SerializeObject(jObject)
                    });
                }
                else
                {
                    numeroFolio = FolioCerticado.Value;
                }

                //var isInStock = _context.vMovimientosInventarioEnStocks
                //    .Any(x => x.IdVerificentro == idVerificentro && x.IdCatTipoCertificado == idCatTipoCertificado && x.FolioInicial <= numeroFolio && x.FolioFinal >= numeroFolio);

                //if (!isInStock)
                //{
                //    validRecord.IsSucces = false;
                //    validRecord.Description = $"No se encontró folio en stock {certificado}. No se puede registrar.";
                //    return validRecord;
                //}

                //var folioRegistrado = _context.vFoliosFormaValoradaVerificentros
                //    .Any(x => x.Folio == numeroFolio && x.IdCatTipoTramite != null);

                //if (folioRegistrado)
                //{
                //    validRecord.IsSucces = false;
                //    validRecord.Description = $"El folio que se intenta registrar de {certificado} ya se encuentra registrado.";
                //    return validRecord;
                //}
                // Se comenta para que no sea necesario un inventario al imprimir
                // var almacen = claveTramite != TipoTramite.CV ?
                //     await _context.Almacens.FirstOrDefaultAsync(x => x.IdVerificentro == null) :
                //     await _context.Almacens.FirstOrDefaultAsync(x => x.IdVerificentro == idVerificentro);

                // if (almacen == null)
                // {
                //     validRecord.IsSucces = false;
                //     validRecord.Description = $"No se encontró el almacén del centro.";
                //     return validRecord;
                // }

                // var inventarioDestinoDB = await _context.Inventarios
                //     .FirstOrDefaultAsync(x => x.IdAlmacen == almacen.Id && x.IdCatTipoCertificado == idCatTipoCertificado);

                // if (inventarioDestinoDB == null)
                // {
                //     validRecord.IsSucces = false;
                //     validRecord.Description = $"No se encontró el inventario del folio {certificado}.";
                //     return validRecord;
                // }

                // inventarioDestinoDB.CantidadStock -= 1;

                //if (inventarioDestinoDB.CantidadStock < 0)
                //{
                //    validRecord.IsSucces = false;
                //    validRecord.Description = $"Se ha agotado el stock de {certificado}.";
                //    return validRecord;
                //}
                if (importe == null)
                {

                    var ciclo = _context.CicloVerificacions
                        .FirstOrDefault(x => x.Activo && x.FechaInicio <= DateTime.Now && x.FechaFin >= DateTime.Now);

                    importe = claveTramite switch
                    {
                        TipoTramite.CV => ciclo.ImporteFv,
                        TipoTramite.CE => ciclo.ImporteExento,
                        TipoTramite.CA => ciclo.ImporteAdministrativo,
                        TipoTramite.CT => ciclo.ImporteTestificacion,
                        TipoTramite.CR => ciclo.ImporteReposicion,
                        _ => 0M
                    };
                }

                var count = _context.FoliosFormaValoradaVerificentros
                    .Count(x => x.ClaveTramite.Contains(TipoTramite.Dict[claveTramite]) && x.IdCatTipoTramite != null) + 1;

                var date = fechaResgistro ?? DateTime.Now;

                var newFolio = new FoliosFormaValoradaVerificentro
                {
                    Folio = numeroFolio,
                    IdVerificentro = idVerificentro,
                    ImporteActual = importe.Value,
                    Cancelado = false,
                    IdCatTipoCertificado = idCatTipoCertificado,
                    FechaRegistro = date,
                    IdUserRegistro = idUser,
                    IdCatTipoTramite = claveTramite == TipoTramite.CV ? TipoTramiteDict.CVV : TipoTramiteDict.Ventanilla,
                    ConsecutivoTramite = count,
                    FechaEmisionRef = date,
                    FechaPago = date,
                    ClaveTramite = $"{TipoTramite.Dict[claveTramite]}-{date.Year}/{count}",
                    EntidadProcedencia = estado ?? "Puebla",
                    ServidorPublico = "",
                    Reposicion = reposicion,
                    IdVerificacion = idVerificacion,
                    IdAdministrativa = idAdministrativa,
                    IdTestificacion = idTestificacion,
                    IdExento = idExento
                };
                await _context.AddAsync(newFolio);
                if (FolioCerticado == null)
                {

                    ultimoFolio.Folio = numeroFolio;
                    await _context.SaveChangesAsync();
                    ultimoFolio.IdFolioFormaValoradaVerificentro = newFolio.Id;
                    _context.Update(ultimoFolio);
                }
                else
                {
                    newFolio.Impreso = true;

                }
                await _context.SaveChangesAsync();

                validRecord.IsSucces = true;
                validRecord.recordId = newFolio.Id;
            }
            catch (Exception ex)
            {
                validRecord.IsSucces = false;
                validRecord.Description = $"Ocurrió un error al generar el folio de tipo {certificado}.";
            }

            return validRecord;
        }
        // public async Task<ResponseValidacion> ValidateFolio(int idCatTipoCertificado, long idVerificentro, int claveTramite, long idUser, string? estado, long? idVerificacion, long? idAdministrativa, long? idExento, bool reposicion = false)
        // {
        //     var validRecord = new ResponseValidacion();
        //     var certificado = TipoCertificado.DictNombreCertificado[idCatTipoCertificado];
        //     try
        //     {

        //         var ultimo = _context.FoliosFormaValoradaVerificentros.OrderByDescending(x => x.Id)
        //             .FirstOrDefault(x => x.IdCatTipoCertificado == idCatTipoCertificado && x.IdVerificentro == idVerificentro)?.Folio;
        //         if (ultimo == null)
        //         {
        //             validRecord.IsSucces = false;
        //             validRecord.Description = $"No se encontró el último folio {certificado} para continuar con el conteo.";
        //             return validRecord;
        //         }
        //         var numeroFolio = ultimo.Value + 1;
        //         numeroFolio = GenerarFolioSaltoCancelados(numeroFolio);
        //         var isInStock = _context.vMovimientosInventarioEnStocks.Any(x => x.IdVerificentro == idVerificentro && numeroFolio >= x.FolioInicial && numeroFolio <= x.FolioFinal && x.IdCatTipoCertificado == idCatTipoCertificado);
        //         if (!isInStock)
        //         {
        //             validRecord.IsSucces = false;
        //             validRecord.Description = $"No se encontró folio en stock {certificado}. No se puede registrar.";
        //             return validRecord;
        //         }
        //         var folioRegistrado = _context.vFoliosFormaValoradaVerificentros.Any(x => x.Folio == numeroFolio && x.IdCatTipoTramite == null);
        //         if (folioRegistrado)
        //         {
        //             validRecord.IsSucces = false;
        //             validRecord.Description = $"El folio que se intenta registrar de {certificado} ya se encuentra registrado.";
        //             return validRecord;
        //         }
        //         var inventarioDB = new Almacen();
        //         var almacen = new Almacen();
        //         if (claveTramite != TipoTramite.CV)
        //         {

        //             almacen = await _context.Almacens.FirstOrDefaultAsync(x => x.IdVerificentro == null);
        //         }
        //         else
        //         {
        //             almacen = await _context.Almacens.FirstOrDefaultAsync(x => x.IdVerificentro == idVerificentro);

        //         }
        //         if (almacen == null)
        //         {
        //             validRecord.IsSucces = false;
        //             validRecord.Description = $"No se encontró el almacen del centro.";
        //             return validRecord;
        //         }

        //         var inventarioDestinoDB = await _context.Inventarios.FirstOrDefaultAsync(x => x.IdAlmacen == almacen.Id && x.IdCatTipoCertificado == idCatTipoCertificado);
        //         if (inventarioDestinoDB == null)
        //         {
        //             validRecord.IsSucces = false;
        //             validRecord.Description = $"No se encontró el inventario del folio {certificado}.";
        //             return validRecord;
        //         }
        //         inventarioDestinoDB.CantidadStock -= 1;
        //         if (inventarioDestinoDB.CantidadStock < 0)
        //         {
        //             validRecord.IsSucces = false;
        //             validRecord.Description = $"Se ha agotado el stock de {certificado}.";
        //             return validRecord;
        //         }
        //         var ciclo = _context.CicloVerificacions.FirstOrDefault(x => x.Activo && x.FechaInicio <= DateTime.Now && x.FechaFin >= DateTime.Now);
        //         var importe = 0M;
        //         var claveTramiteString = TipoTramite.Dict[claveTramite];
        //         switch (claveTramite)
        //         {
        //             case TipoTramite.CV:
        //                 importe = ciclo.ImporteFv;
        //                 break;
        //             case TipoTramite.CE:
        //                 importe = ciclo.ImporteExento;
        //                 break;
        //             case TipoTramite.CA:
        //                 importe = ciclo.ImporteAdministrativo;
        //                 break;
        //             case TipoTramite.CT:
        //                 importe = ciclo.ImporteTestificacion;
        //                 break;
        //             case TipoTramite.CR:
        //                 importe = ciclo.ImporteReposicion;
        //                 break;
        //         }
        //         var count = _context.FoliosFormaValoradaVerificentros.Where(x => x.ClaveTramite.Contains(claveTramiteString) && x.IdCatTipoTramite != null).Count() + 1;
        //         var date = DateTime.Now;
        //         var newFolio = new FoliosFormaValoradaVerificentro
        //         {
        //             Folio = ultimo.Value + 1,
        //             IdVerificentro = idVerificentro,
        //             ImporteActual = importe,
        //             Cancelado = false,
        //             IdCatTipoCertificado = idCatTipoCertificado,
        //             FechaRegistro = DateTime.Now,
        //             IdUserRegistro = idUser,
        //             IdCatTipoTramite = claveTramite == TipoTramite.CV ? TipoTramiteDict.CVV : TipoTramiteDict.Ventanilla,
        //             ConsecutivoTramite = count,
        //             FechaEmisionRef = DateTime.Now,
        //             FechaPago = DateTime.Now,
        //             ClaveTramite = $"{claveTramiteString}-{date.Year}/{count}",
        //             EntidadProcedencia = estado ?? "Puebla",
        //             ServidorPublico = "",
        //             Reposicion = reposicion,
        //             IdVerificacion = idVerificacion,
        //             IdAdministrativa = idAdministrativa,
        //             IdExento = idExento,
        //         };
        //         _context.Add(newFolio);
        //         await _context.SaveChangesAsync();
        //         validRecord.IsSucces = true;
        //         validRecord.recordId = newFolio.Id;
        //     }
        //     catch (Exception ex)
        //     {
        //         validRecord.IsSucces = false;
        //         validRecord.Description = $"Ocurrió un error al generar el folio de tipo {certificado}.";
        //     }
        //     return validRecord;
        // }

        private long GenerarFolioSaltoCancelados(long numeroFolio)
        {
            var folioCancelado = _context.vFoliosFormaValoradaVerificentros.Any(x => x.Folio == numeroFolio && x.Cancelado);
            if (folioCancelado)
            {
                GenerarFolioSaltoCancelados(++numeroFolio);
            }
            return numeroFolio;
        }

        public async Task<ResponseValidacion> ValidateVenta(VentaCertificados venta, long idAlmacen)
        {
            var validRecord = new ResponseValidacion();
            var certificado = TipoCertificado.DictNombreCertificado[venta.IdTipoCertificado];
            try
            {

                // var CVVSecertaria = _context.vVerificentros.FirstOrDefaultAsync(x => x.Clave == "SMADSOT-00");
                var hayStock = await _context.vInventarios.FirstOrDefaultAsync(x => x.IdAlmacen == idAlmacen && x.IdCatTipoCertificado == x.IdCatTipoCertificado);
                var isInStock = _context.vMovimientosInventarioEnStocks.Any(x => x.IdAlmacen == idAlmacen && venta.FolioInicial >= x.FolioInicial && venta.FolioInicial <= x.FolioFinal &&
                venta.FolioFinal >= x.FolioInicial && venta.FolioFinal <= x.FolioFinal);
                var foliosRegistrados = _context.vFoliosFormaValoradaVerificentros.Where(x => x.Folio >= venta.FolioInicial && x.Folio <= venta.FolioFinal);
                if (hayStock == null)
                {
                    validRecord.IsSucces = false;
                    validRecord.Description = $"No se puede vender el rango de folios. La cantidad restante de Holograma {certificado} en Stock";
                }
                // else if (venta.Cantidad > hayStock.CantidadStock)
                // {
                //     validRecord.IsSucces = false;
                //     validRecord.Description = $"No se puede vender el rango de folios. La cantidad restante de Holograma {certificado} en Stock";
                // }
                // else if (foliosRegistrados.Any())
                // {
                //     validRecord.IsSucces = false;
                //     validRecord.Description = $"No se puede vender el rango de folios. Uno o varios de los folios que se intentan registrar de {certificado} ya fueron usados en otro trámite.";
                // }
                else if (isInStock)
                {
                    var OutStock = _context.vMovimientosInventarioFueraStocks.Any(x => venta.FolioInicial >= x.FolioInicial && venta.FolioInicial <= x.FolioFinal &&
                                    venta.FolioFinal >= x.FolioInicial && venta.FolioFinal <= x.FolioFinal);
                    // if (!OutStock)
                    // {
                    //     var inventarioDB = await _context.Inventarios.FirstOrDefaultAsync(x => x.Id == hayStock.Id);
                    //     inventarioDB.CantidadStock -= venta.Cantidad;
                    //     var inventarioDestinoDB = await _context.Inventarios.FirstOrDefaultAsync(x => x.IdAlmacen == idAlmacen && x.IdCatTipoCertificado == venta.IdTipoCertificado);
                    //     // if (inventarioDestinoDB == null)
                    //     // {
                    //     //     inventarioDestinoDB = new Inventario
                    //     //     {
                    //     //         CantidadStock = venta.Cantidad,
                    //     //         FolioInicial = venta.FolioInicial,
                    //     //         FolioFinal = venta.FolioFinal,
                    //     //         IdAlmacen = idAlmacen,
                    //     //         IdCatTipoCertificado = venta.IdTipoCertificado
                    //     //     };
                    //     // }
                    //     // else
                    //     // {
                    //     //     inventarioDB.CantidadStock += venta.Cantidad;
                    //     // }
                    //     await _context.SaveChangesAsync();
                    //     validRecord.recordId = inventarioDB.Id;
                    // }
                    validRecord.IsSucces = !OutStock;
                    validRecord.Description = !OutStock ? null : $"El rango de folios de {certificado} que se intenta vender interfiere con un rango asignado a otro trámite u otro centro de verificación.";
                }
                else
                {
                    validRecord.IsSucces = isInStock;
                    validRecord.Description = $"El rango de folios de {certificado} que se intenta vender no se encuentra en stock.";
                }
            }
            catch (Exception)
            {
                validRecord.IsSucces = false;
                validRecord.Description = $"Hubo un error al intentar validar el rango de folios que se intenta vender de Holograma {certificado}";
            }
            return validRecord;
        }

        public async Task<List<LineaPendientes>> PendientesLineas(long IdVerificentro, int PROTOCOLO)
        {
            DateTime fecha = DateTime.Now;
            var finDia = new DateTime(fecha.Year, fecha.Month, fecha.Day, 23, 59, 59);
            var inicioDia = new DateTime(fecha.Year, fecha.Month, fecha.Day, 0, 0, 0);
            var lineasQ = new List<LineaPendientes>();
            var verificacionesHoy = _context.vVerificacionLineas.Where(v => v.Fecha >= inicioDia && v.Fecha <= finDia && v.IdVerificentro == IdVerificentro).ToList();

            var lineas = _context.Lineas.Where(l => l.IdVerificentro == IdVerificentro && l.IdCatEstatusLinea == LineaEstatus.Apertura).OrderBy(x => x.Clave);
            foreach (var l in lineas)
            {
                var verificacionesEnCola = verificacionesHoy.Where(v => v.Id == l.Id).OrderByDescending(x => x.Orden);
                var pendientes = verificacionesEnCola.Count();
                var ultimaVerificacion = verificacionesEnCola.FirstOrDefault();
                var infoVerificentro = await _context.vVerificentros.FirstOrDefaultAsync(x => x.Id == IdVerificentro);
                //var testCamaras = await CapturarPlacas(infoVerificentro.ApiEndPoint, infoVerificentro.ApiKey, l.Clave);
                //if (testCamaras.Status == ResponseStatus.Success)
                //{
                bool equipada = false;
                vLineaEquipo? equipo = null;
                //if (testCamaras.Response.Delantera != null || testCamaras.Response.Trasera != null)
                //{
                if (Protocolos.ESTATICO != PROTOCOLO)
                {
                    switch (PROTOCOLO)
                    {
                        case Protocolos.ACELERACIONDIESEL:
                        case Protocolos.DIESELSINDATOS:
                            equipo = await _context.vLineaEquipos.FirstOrDefaultAsync(x => x.IdLinea == l.Id && x.IdCatTipoEquipo == EquiposDict.OPACIMETRO
                            && x.IdCatEstatusEquipo == EstatusEquipo.Activo);
                            equipada = equipo != null;
                            break;
                        case Protocolos.DINAMICO:
                            equipo = await _context.vLineaEquipos.FirstOrDefaultAsync(x => x.IdLinea == l.Id && x.IdCatTipoEquipo == EquiposDict.DINAMOMETRO
                            && x.IdCatEstatusEquipo == EstatusEquipo.Activo);
                            equipada = equipo != null;
                            break;
                            // case Protocolos.ESTATICO:
                            //     equipo = await _context.vLineaEquipos.FirstOrDefaultAsync(x => x.IdLinea == l.Id && x.Nombre.Contains(EquiposDict.Equipos[EquiposDict.TACOMETRONOCONTACTO])
                            //     && x.IdCatEstatusEquipo == EstatusEquipo.Activo);
                            //     equipada = equipo != null;
                            //     break;
                    }
                    if (equipada)
                        lineasQ.Add(new LineaPendientes { Linea = l, Pendientes = pendientes, UltimoEnCola = ultimaVerificacion != null ? ultimaVerificacion.Orden ?? 0 : 0, IdEquipo = equipo?.Id ?? 0 });
                }
                else
                {
                    equipo = equipo = await _context.vLineaEquipos.FirstOrDefaultAsync(x => (x.IdCatTipoEquipo == EquiposDict.TACOMETROENCENDEDOR || x.IdCatTipoEquipo == EquiposDict.TACOMETRONOCONTACTO || x.IdCatTipoEquipo == EquiposDict.TACOMETROPINZAS || x.IdCatTipoEquipo == EquiposDict.TACOMETROTRANSDUCTOR)
                            && x.IdCatEstatusEquipo == EstatusEquipo.Activo);
                    if (equipo != null)
                        lineasQ.Add(new LineaPendientes { Linea = l, Pendientes = pendientes, UltimoEnCola = ultimaVerificacion != null ? ultimaVerificacion.Orden ?? 0 : 0, IdEquipo = equipo?.Id ?? 0 });

                }
                //}
                //}
            }

            return lineasQ.OrderBy(x => x.Pendientes).ToList();
        }

        public async Task SaveLog(Exception ex, EventoSalida evento, string type)
        {
            try
            {
                JObject jsonEvento = JObject.FromObject(evento);
                JObject jsonException = JObject.FromObject(ex);
                JObject data = new()
                {
                    ["Exception"] = jsonException,
                    ["Type"] = type,
                    ["Evento"] = jsonEvento

                };
                _context.Errors.Add(new()
                {
                    Created = DateTime.Now,
                    Values = JsonConvert.SerializeObject(data)
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }
        public async Task SaveLog(object ex, EventoSalida evento, string type)
        {
            try
            {
                JObject jsonEvento = JObject.FromObject(evento);
                JObject jsonException = JObject.FromObject(ex);
                JObject data = new()
                {
                    ["Exception"] = jsonException,
                    ["Type"] = type,
                    ["Evento"] = jsonEvento

                };
                _context.Errors.Add(new()
                {
                    Created = DateTime.Now,
                    Values = JsonConvert.SerializeObject(data)
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }
        public async Task SaveLog(Exception ex, string type)
        {
            try
            {
                JObject exceptioJson = new()
                {
                    ["StackTrace"] = ex.StackTrace,
                    ["Source"] = ex.Source,
                    ["Message"] = ex.Message,
                    ["InnerException"] = ex.InnerException?.Message ?? "",
                    ["InnerExceptionData"] = JObject.FromObject(ex.InnerException?.Data ?? new object()),
                    ["InnerExceptionDataSource"] = ex.InnerException?.Source ?? "",
                };
                JObject data = new()
                {
                    ["Exception"] = exceptioJson,
                    ["Type"] = type
                };
                _context.Errors.Add(new()
                {
                    Created = DateTime.Now,
                    Values = JsonConvert.SerializeObject(data)
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        } public async Task SaveLog(object ex, string type)
        {
            try
            {
                JObject exceptioJson = JObject.FromObject(ex);
                JObject data = new()
                {
                    ["Exception"] = exceptioJson,
                    ["Type"] = type
                };
                _context.Errors.Add(new()
                {
                    Created = DateTime.Now,
                    Values = JsonConvert.SerializeObject(data)
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }
        public async Task SaveLog(Exception ex, int conteo, string type)
        {
            try
            {
                JObject exceptioJson = new()
                {
                    ["StackTrace"] = ex.StackTrace,
                    ["Source"] = ex.Source,
                    ["Message"] = ex.Message,
                    ["InnerException"] = ex.InnerException?.Message ?? "",
                    ["InnerExceptionData"] = JObject.FromObject(ex.InnerException?.Data ?? new object()),
                    ["InnerExceptionDataSource"] = ex.InnerException?.Source ?? "",
                    ["Conteo"] = conteo,
                };
                JObject data = new()
                {
                    ["Exception"] = exceptioJson,
                    ["Type"] = type
                };
                _context.Errors.Add(new()
                {
                    Created = DateTime.Now,
                    Values = JsonConvert.SerializeObject(data)
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }
        public async Task SaveLog(string jsonObj, string type)
        {
            try
            {
                JObject data = new JObject
                {
                    { "Data", JObject.Parse(jsonObj) },
                    { "Type", type }
                };
                _context.Errors.Add(new()
                {
                    Created = DateTime.Now,
                    Values = JsonConvert.SerializeObject(data)
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }
        public async Task SaveLogPrueba(Prueba prueba, string claveCentro)
        {
            try
            {
                var pruebaLog = new LogPruebaVerificacion()
                {
                    IdVerificacion = prueba.Id,
                    ClaveCentro = claveCentro,
                    FechaUltimaModificacion = prueba.FechaUltimaModificacion,
                    NumSerie = prueba.NumSerie,
                    Placa = prueba.Placa,
                    Combustible = prueba.Combustible,
                    Fecha = prueba.Fecha,
                    NumeroConstanciaOtorgada = prueba.NumeroConstanciaOtorgada,
                    RazonSocial = prueba.RazonSocial,
                    Propietario = prueba.Propietario,
                    TecnicoVerificador = prueba.TecnicoVerificador,
                    Linea = prueba.Linea,
                    IdEstatus = prueba.IdEstatus,
                    SubMarca = prueba.SubMarca,
                    Marca = prueba.Marca,
                    Anio = prueba.Anio,
                    Pot_5024 = prueba.Pot_5024,
                    Pot_2540 = prueba.Pot_2540,
                    RPM_GOB = prueba.RPM_GOB,
                    Protocolo = prueba.Protocolo,
                    Cilindros = prueba.Cilindros,
                    Cilindrada = prueba.Cilindrada,
                    Motor_DSL = prueba.Motor_DSL,
                    PBV = prueba.PBV,
                    PBV_Equivalente = prueba.PBV_Equivalente,
                    PBV_ASM = prueba.PBV_ASM,
                    C_ABS = prueba.C_ABS,
                    ConvertidorCatalitico = prueba.ConvertidorCatalitico,
                    ClaveCombustible = prueba.ClaveCombustible,
                    Combustible_POTMAX_RPM = prueba.Combustible_POTMAX_RPM,
                    RAL_FAB = prueba.RAL_FAB,
                    GOB_FAB = prueba.GOB_FAB,
                    OBD = (long)(prueba.OBD ?? 0),
                    DobleCero = (long)(prueba.DobleCero ?? 0),
                    CERO_GASOL = (long)(prueba.CERO_GASOL ?? 0),
                    CERO_GASLP = (long)(prueba.CERO_GASLP ?? 0),
                    CERO_GASNC = (long)(prueba.CERO_GASNC ?? 0),
                    CERO_DSL = (long)(prueba.CERO_DSL ?? 0),
                    Etapa = prueba.Etapa,
                    SPS_Humo = prueba.SPS_Humo,
                    SPS_5024 = prueba.SPS_5024,
                    SPS_2540 = prueba.SPS_2540,
                    HC = prueba.HC,
                    CO = prueba.CO,
                    CO2 = prueba.CO2,
                    O2 = prueba.O2,
                    NO = prueba.NO,
                    LAMDA = prueba.LAMDA,
                    FCNOX = prueba.FCNOX,
                    FCDIL = prueba.FCDIL,
                    RPM = prueba.RPM,
                    KPH = prueba.KPH,
                    VEL_LIN = prueba.VEL_LIN,
                    VEL_ANG = prueba.VEL_ANG,
                    BHP = prueba.BHP,
                    PAR_TOR = prueba.PAR_TOR,
                    FUERZA = prueba.FUERZA,
                    POT_FRENO = prueba.POT_FRENO,
                    TEMP = prueba.TEMP,
                    PRESION = prueba.PRESION,
                    HUMREL = prueba.HUMREL,
                    OBD_TIPO_SDB = prueba.OBD_TIPO_SDB,
                    OBD_MIL = prueba.OBD_MIL,
                    OBD_CATAL = prueba.OBD_CATAL,
                    OBD_CILIN = prueba.OBD_CILIN,
                    OBD_COMBU = prueba.OBD_COMBU,
                    OBD_INTEG = prueba.OBD_INTEG,
                    OBD_OXIGE = prueba.OBD_OXIGE,
                    LAMDA_5024 = prueba.LAMDA_5024,
                    TEMP_5024 = prueba.TEMP_5024,
                    HR_5024 = prueba.HR_5024,
                    PSI_5024 = prueba.PSI_5024,
                    FCNOX_5024 = prueba.FCNOX_5024,
                    FCDIL_5024 = prueba.FCDIL_5024,
                    RPM_5024 = prueba.RPM_5024,
                    KPH_5024 = prueba.KPH_5024,
                    THP_5024 = prueba.THP_5024,
                    VOLTS_5024 = prueba.VOLTS_5024,
                    HC_5024 = prueba.HC_5024,
                    CO_5024 = prueba.CO_5024,
                    CO2_5024 = prueba.CO2_5024,
                    COCO2_5024 = prueba.COCO2_5024,
                    O2_5024 = prueba.O2_5024,
                    NO_5024 = prueba.NO_5024,
                    LAMDA_2540 = prueba.LAMDA_2540,
                    TEMP_2540 = prueba.TEMP_2540,
                    HR_2540 = prueba.HR_2540,
                    PSI_2540 = prueba.PSI_2540,
                    FCNOX_2540 = prueba.FCNOX_2540,
                    FCDIL_2540 = prueba.FCDIL_2540,
                    RPM_2540 = prueba.RPM_2540,
                    KPH_2540 = prueba.KPH_2540,
                    THP_2540 = prueba.THP_2540,
                    VOLTS_2540 = prueba.VOLTS_2540,
                    HC_2540 = prueba.HC_2540,
                    CO_2540 = prueba.CO_2540,
                    CO2_2540 = prueba.CO2_2540,
                    COCO2_2540 = prueba.COCO2_2540,
                    O2_2540 = prueba.O2_2540,
                    NO_2540 = prueba.NO_2540,
                    OPACIDADP = prueba.OPACIDADP,
                    OPACIDADK = prueba.OPACIDADK,
                    TEMP_MOT = prueba.TEMP_MOT,
                    VEL_GOB = prueba.VEL_GOB,
                    POTMIN_RPM = prueba.POTMIN_RPM,
                    POTMAX_RPM = prueba.POTMAX_RPM,
                    TEMP_GAS = prueba.TEMP_GAS,
                    TEMP_CAM = prueba.TEMP_CAM,
                    RESULTADO = prueba.RESULTADO,
                    C_RECHAZO = prueba.C_RECHAZO,
                    C_RECHAZO_OBD = prueba.C_RECHAZO_OBD,
                    PruebaObd = prueba.PruebaObd,
                    PruebaEmisiones = prueba.PruebaEmisiones,
                    PruebaDiesel = prueba.PruebaDiesel,
                };
                await _context.LogPruebaVerificacions.AddAsync(pruebaLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }
        public async Task<CertificadoResultado> ValidarResultados(
            int protocolo, int pbv_equiv, int anio, decimal opacidadDK, bool pruebaOBD, bool pruebaEmisiones,
            bool pruebaOpacidad, int c_rechazo, int c_rechazo_obd, decimal LAMDA_2540, decimal LAMDA_5024,
            decimal HC_2540, decimal HC_5024, decimal CO_2540, decimal CO_5024, decimal NO_2540, decimal NO_5024,
            decimal O2_2540, decimal O2_5024, decimal COCO2_2540, decimal COCO2_5024, int tipoCombustible, bool candidatoDoblecero)
        {

            CertificadoResultado certificadoResultado = new() { ResultadoTipoCertificado = TipoCertificado.ConstanciasNoAprobado, CausaRechazo = CausaRechazo.NoAplica, CausaRechazoOBD = OBD.SinCodigoError, ResultadoPrueba = Resultados.Rechazo };
            var combustibleNat = tipoCombustible == Combustible.GasNat || tipoCombustible == Combustible.GasLp;

            // Se obtienen los límites máximos de la bd
            LimiteVerificacion? limiteVerificacion = await _context.LimiteVerificacions.FirstOrDefaultAsync(x => x.IdCatTipoProtolo == protocolo && (x.AnioInicio <= anio || x.AnioInicio == null) && (x.AnioFin >= anio || x.AnioFin == null)
            && x.CombustibleGas == combustibleNat && (x.PBVMin <= pbv_equiv || x.PBVMin == null) && (x.PBVMax >= pbv_equiv || x.PBVMax == null) && (x.IdCatTipoCertificado == TipoCertificado.Uno && (protocolo == Protocolos.DIESELSINDATOS || protocolo == Protocolos.ACELERACIONDIESEL) ? opacidadDK <= x.Coeficiente_Absorcion_Luz : true));

            certificadoResultado.LimiteVerificacion = limiteVerificacion;

            // Reregresa un error sí no hay datos para calcular
            if (limiteVerificacion == null)
            {

                certificadoResultado.CausaRechazo = CausaRechazo.Emisiones;
                certificadoResultado.CausaRechazoOBD = OBD.SinCodigoError;

                certificadoResultado.Error = true;
                certificadoResultado.Mensaje = $"No se encontraron los parametros en la lista de límites de emisiones Protocolo: {protocolo}. Tipo Combustible: {tipoCombustible}." + "IdVerifiacion: {0}";
                return certificadoResultado;
            }
            if (pruebaOBD && limiteVerificacion.IdCatTipoCertificado == TipoCertificado.Cero && (tipoCombustible != Combustible.Diesel || candidatoDoblecero) && (c_rechazo_obd != OBD.FallaConexion)) // Los vehículos candidatos a tipo 1 o 2 no pueden aprobar por obd
            {
                if ((c_rechazo_obd == OBD.SinCodigoError) || c_rechazo == CausaRechazo.NoAplica)
                {

                    certificadoResultado.CausaRechazo = CausaRechazo.NoAplica;
                    certificadoResultado.ResultadoPrueba = candidatoDoblecero ? Resultados.DictCertificadoEquivalencia[TipoCertificado.DobleCero] : Resultados.DictCertificadoEquivalencia[limiteVerificacion.IdCatTipoCertificado];
                    certificadoResultado.ResultadoTipoCertificado = candidatoDoblecero ? TipoCertificado.DobleCero : limiteVerificacion.IdCatTipoCertificado;
                    return certificadoResultado;
                }
                else
                {
                    certificadoResultado.CausaRechazo = CausaRechazo.OBD;
                    certificadoResultado.CausaRechazoOBD = OBD.CodigoError;
                }
                return certificadoResultado;
            }

            certificadoResultado.CausaRechazo = CausaRechazo.Emisiones;
            certificadoResultado.CausaRechazoOBD = OBD.SinCodigoError;

            // Comprobamos que se le haya aplicado prueba de OBD de acuerdo al año que corresponde segun el ppvo
            //if (pruebaOBD && anio < 2006)
            //{
            //    certificadoResultado.Error = true;
            //    certificadoResultado.Mensaje = "Se realizó prueba OBD a un vehiculo que no corresponde.";
            //    return certificadoResultado;
            //}

            // Comprobamos que se le haya aplicado o prueba de opcacidad o emisiones.
            if (!pruebaEmisiones && !pruebaOpacidad)
            {
                certificadoResultado.Error = true;
                certificadoResultado.Mensaje = "No se recibieron los resultados de parte del proveedor o se recibieron parcialmente.";
                return certificadoResultado;
            }

            // Sí es combustible diesel nos ahorramos todo el proceso y validamos directamente el resultado del opacímetro
            if (protocolo == Protocolos.DIESELSINDATOS || protocolo == Protocolos.ACELERACIONDIESEL)
            {
                if (opacidadDK < limiteVerificacion?.Coeficiente_Absorcion_Luz)
                {
                    certificadoResultado.CausaRechazo = CausaRechazo.NoAplica;
                    certificadoResultado.ResultadoPrueba = Resultados.DictCertificadoEquivalencia[candidatoDoblecero ? TipoCertificado.DobleCero : limiteVerificacion.IdCatTipoCertificado];
                    certificadoResultado.ResultadoTipoCertificado = candidatoDoblecero ? TipoCertificado.DobleCero : limiteVerificacion.IdCatTipoCertificado;
                }
                else
                {
                    certificadoResultado.CausaRechazo = CausaRechazo.Emisiones;
                }
            }
            else // Si no es diesele evaluamos los resultados que hayamos obtenido de limitesVerificacion para evaluarlo
            {
                if (LAMDA_2540 <= limiteVerificacion.Factor_Lamda && LAMDA_5024 <= limiteVerificacion.Factor_Lamda
                                    && HC_2540 <= limiteVerificacion.Hc && HC_5024 <= limiteVerificacion.Hc
                                    && CO_2540 <= limiteVerificacion.Co && CO_5024 <= limiteVerificacion.Co
                                    && ((NO_2540 <= limiteVerificacion.No && NO_5024 <= limiteVerificacion.No) || limiteVerificacion.No == null)
                                    && O2_2540 <= limiteVerificacion.O2 && O2_5024 <= limiteVerificacion.O2
                                    && COCO2_2540 >= limiteVerificacion.Coco2_Min && COCO2_2540 <= limiteVerificacion.Coco2_Max
                                    && COCO2_5024 >= limiteVerificacion.Coco2_Min && COCO2_5024 <= limiteVerificacion.Coco2_Max)
                {
                    certificadoResultado.CausaRechazo = CausaRechazo.NoAplica;

                    certificadoResultado.ResultadoPrueba = candidatoDoblecero && limiteVerificacion.IdCatTipoCertificado == TipoCertificado.Cero ? Resultados.DictCertificadoEquivalencia[TipoCertificado.DobleCero] : Resultados.DictCertificadoEquivalencia[limiteVerificacion.IdCatTipoCertificado];
                    certificadoResultado.ResultadoTipoCertificado = candidatoDoblecero && limiteVerificacion.IdCatTipoCertificado == TipoCertificado.Cero ? TipoCertificado.DobleCero : limiteVerificacion.IdCatTipoCertificado;
                }
                else
                {
                    certificadoResultado.CausaRechazo = CausaRechazo.Emisiones;
                    certificadoResultado.Mensaje = "Los resultados de emisiones de la prueba están fuera de los límites permisibles para gasolina.";
                }
            }
            return certificadoResultado;


        }

        public async Task<LimiteVerificacion?> ObtenerLimites(
                int protocolo, int pbv_equiv, int anio, int tipoCombustible, decimal opacidadDK)
        {
            var combustibleNat = tipoCombustible == Combustible.GasNat || tipoCombustible == Combustible.GasLp;
            LimiteVerificacion? limiteVerificacion = await _context.LimiteVerificacions.FirstOrDefaultAsync(x => x.IdCatTipoProtolo == protocolo && (x.AnioInicio <= anio || x.AnioInicio == null) && (x.AnioFin >= anio || x.AnioFin == null)
                && x.CombustibleGas == combustibleNat && (x.PBVMin <= pbv_equiv || x.PBVMin == null) && (x.PBVMax >= pbv_equiv || x.PBVMax == null) && (x.IdCatTipoCertificado == TipoCertificado.Uno && (protocolo == Protocolos.DIESELSINDATOS || protocolo == Protocolos.ACELERACIONDIESEL) ? opacidadDK <= x.Coeficiente_Absorcion_Luz : true));

            return limiteVerificacion;
        }
    }
}