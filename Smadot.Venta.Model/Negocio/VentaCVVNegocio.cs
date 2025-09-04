using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Refrendo.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Models.Entities.VentaCVV.Request;
using Smadot.Models.Entities.VentaCVV.Response;
using Smadot.Models.GenericProcess;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Transactions;

namespace Smadot.Venta.Model.Negocio
{
    public class VentaCVVNegocio : IVentaCVVNegocio
    {
        private readonly SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        private readonly ILogger<VentaCVVNegocio> _logger;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;
        public VentaCVVNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration, ILogger<VentaCVVNegocio> logger/*, ISmadsotGenericInserts smadsotGenericInserts*/, SmadsotGenericInserts smadsotGenericInserts)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _logger = logger;
            _smadsotGenericInserts = smadsotGenericInserts;
            // _smadsotGenericInserts = smadsotGenericInserts;
        }

        public async Task<ResponseGeneric<ResponseGrid<SeguimientoSolicitudResponse>>> Consulta(SolicitudFormaValoradaListRequest request)
        {
            try
            {
                var solicitudes = _context.vSeguimientoSolicituds.Select(x => new SeguimientoSolicitudResponse
                {
                    IdSolicitudFV = x.IdSolicitudFV,
                    FechaSolicitudFV = x.FechaSolicitudFV,
                    IdUserSolicitaFV = x.IdUserSolicitaFV,
                    UserSolicitaFV = x.UserSolicitaFV ?? string.Empty,
                    IdCatEstatusSolicitudFV = x.IdCatEstatusSolicitudFV,
                    EstatusFV = x.EstatusFV,
                    ActivoFV = x.ActivoFV,
                    FechaRegistroFV = x.FechaRegistroFV,
                    IdAlmacenFV = x.IdAlmacenFV,
                    AlmacenFV = x.AlmacenFV,
                    IdVerificentro = x.IdVerificentro,
                    FechaEntregaIFV = x.FechaEntregaIFV,
                    NombreRecibioIFV = x.NombreRecibioIFV,
                    IdVentaFV = x.IdVentaFV,
                    IdIngresoFV = x.IdIngresoFV,
                    FechaVentaVFV = x.FechaVentaVFV,
                    ImporteTotal = x.ImporteTotal
                }).Where(x => x.IdCatEstatusSolicitudFV >= EstatusSolicitud.Entregado).AsQueryable();

                if (request.SiguienteFolio.HasValue && request.SiguienteFolio.Value == true)
                    solicitudes = solicitudes.Where(x => x.IdCatEstatusSolicitudFV == TipoCertificado.DobleCero).Take(1);

                if (request.IdAlmacen.HasValue)
                    solicitudes = solicitudes.Where(x => x.IdAlmacenFV == request.IdAlmacen);

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    var date = new DateTime();
                    var dateDef = new DateTime();
                    DateTime.TryParseExact(request.Busqueda, "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out date);
                    solicitudes = solicitudes.Where(x => x.IdSolicitudFV.ToString().Contains(request.Busqueda.ToLower()) || (date != dateDef && x.FechaSolicitudFV == date) || x.UserSolicitaFV.ToLower().Contains(request.Busqueda.ToLower()) || x.EstatusFV.ToLower().Contains(request.Busqueda.ToLower()) || (date != dateDef && x.FechaEntregaIFV == date) || x.NombreRecibioIFV.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var tot = solicitudes.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    solicitudes = solicitudes.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    solicitudes = solicitudes.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                DateTime now = DateTime.Now;
                var result = new ResponseGrid<SeguimientoSolicitudResponse>
                {
                    RecordsFiltered = tot,
                    RecordsTotal = tot,
                    Data = await solicitudes.ToListAsync()
                };
                return new ResponseGeneric<ResponseGrid<SeguimientoSolicitudResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<SeguimientoSolicitudResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<VerificentroResponse>>> ConsultaAutocomplete(string request)
        {
            try
            {
                var verificentros = _context.Verificentros.AsQueryable()
                                                    .Where(x => x.Nombre.ToLower().Contains(request.ToLower())
                                                            || x.Rfc.ToLower().Contains(request.ToLower())
                                                            || x.Direccion.ToLower().Contains(request.ToLower())
                                                            || x.Clave.ToLower().Contains(request.ToLower()))
                                                    .Take(10);

                var result = await verificentros.Select(x => new VerificentroResponse
                {
                    Nombre = x.Nombre,
                    Activo = x.Activo,
                    Id = x.Id
                }).ToListAsync();

                return new ResponseGeneric<List<VerificentroResponse>>(result);
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<List<VerificentroResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<InventarioVentaCVVResponse>> DetalleInventarioVentaCVV(long idIngreso)
        {
            try
            {
                InventarioVentaCVVResponse venta = new()
                {
                    FechaReporteRevalidado = DateTime.Now,
                    FechaReporteValidado = DateTime.Now
                };

                var ciclo = _context.CicloVerificacions.FirstOrDefault(x => x.Activo && x.FechaInicio <= DateTime.Now && x.FechaFin >= DateTime.Now)
                    ?? throw new ValidationException("No existe el importe.");
                venta.ImporteFV = ciclo.ImporteFv;

                var certificados = await _context.IngresoCertificados.Include(x => x.IdIngresoFVNavigation).Where(x => x.IdIngresoFVNavigation.IdSolicitudFV == idIngreso).ToListAsync();
                if (certificados.Count == 0)
                    throw new ValidationException("No existe el ingreso seleccionado.");

                var inventarioDobleCero = certificados.FirstOrDefault(x => x.IdCatTipoCertificado == TipoCertificado.DobleCero);
                if (inventarioDobleCero is not null)
                {
                    venta.DobleCeroCantidadStock = inventarioDobleCero.CantidadRecibida;
                    venta.DobleCeroFolioInicial = inventarioDobleCero.FolioInicial;
                    venta.DobleCeroFolioFinal = inventarioDobleCero.FolioFinal;
                }

                var inventarioCero = certificados.FirstOrDefault(x => x.IdCatTipoCertificado == TipoCertificado.Cero);
                if (inventarioCero is not null)
                {
                    venta.CeroCantidadStock = inventarioCero.CantidadRecibida;
                    venta.CeroFolioInicial = inventarioCero.FolioInicial;
                    venta.CeroFolioFinal = inventarioCero.FolioFinal;
                }

                var inventarioUno = certificados.FirstOrDefault(x => x.IdCatTipoCertificado == TipoCertificado.Uno);
                if (inventarioUno is not null)
                {
                    venta.UnoCantidadStock = inventarioUno.CantidadRecibida;
                    venta.UnoFolioInicial = inventarioUno.FolioInicial;
                    venta.UnoFolioFinal = inventarioUno.FolioFinal;
                }

                var inventarioDos = certificados.FirstOrDefault(x => x.IdCatTipoCertificado == TipoCertificado.Dos);
                if (inventarioDos is not null)
                {
                    venta.DosCantidadStock = inventarioDos.CantidadRecibida;
                    venta.DosFolioInicial = inventarioDos.FolioInicial;
                    venta.DosFolioFinal = inventarioDos.FolioFinal;
                }

                var inventarioConstanciasNoAprobado = certificados.FirstOrDefault(x => x.IdCatTipoCertificado == TipoCertificado.ConstanciasNoAprobado);
                if (inventarioConstanciasNoAprobado is not null)
                {
                    venta.ConstanciasNoAprobadosCantidadStock = inventarioConstanciasNoAprobado.CantidadRecibida;
                    venta.ConstanciasNoAprobadosFolioInicial = inventarioConstanciasNoAprobado.FolioInicial;
                    venta.ConstanciasNoAprobadosFolioFinal = inventarioConstanciasNoAprobado.FolioFinal;
                }

                // var inventarioExentos = _context.vInventarios.Where(x => x.IdAlmacen == idAlmacen && x.IdCatTipoCertificado == TipoCertificado.Exentos).OrderByDescending(x => x.Id).LastOrDefault();
                // if (inventarioExentos is not null)
                // {
                //     venta.ExentosCantidadStock = inventarioExentos.CantidadStock;
                //     venta.ExentosFolioInicial = inventarioExentos.FolioInicial.HasValue ? inventarioExentos.FolioInicial.Value : 0;
                //     venta.ExentosFolioFinal = inventarioExentos.FolioFinal.HasValue ? inventarioExentos.FolioFinal.Value : 0;
                // }

                return new ResponseGeneric<InventarioVentaCVVResponse>(venta);
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<InventarioVentaCVVResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<VentaCertificadoResponse>> DetalleVentaCertificado(long idAlmacen, long idVenta)
        {
            try
            {
                VentaCertificadoResponse venta = new();
                venta.FechaReporteRevalidado = DateTime.Now;
                venta.FechaReporteValidado = DateTime.Now;

                var totales = _context.vVentaFVs.Where(x => x.Id == idVenta).OrderBy(x => x.NumeroCompra);
                var ventaFV = totales.FirstOrDefault();
                venta.NombreVerificentro = ventaFV.Verificentro;
                venta.NombreAlmacen = ventaFV.NombreAlmacen;
                venta.NumeroReferencia = ventaFV.NumeroReferencia;
                venta.NumeroCompra = ventaFV.NumeroCompra;
                venta.FechaVenta = ventaFV.FechaVenta;
                venta.Clave = ventaFV.Clave;
                venta.Rfc = ventaFV.Rfc;
                venta.UrlDoc1 = ventaFV.UrlDoc1;
                venta.UrlDoc2 = ventaFV.UrlDoc2;
                venta.UrlDoc3 = ventaFV.UrlDoc3;
                venta.SumaImporteTotal = totales.Sum(x => x.ImporteTotal);
                venta.Cantidad = totales.Sum(x => x.Cantidad);
                var listTotales = new List<CertificadoDataTotal>();
                foreach (var t in totales)
                {
                    var folios = t.Folios.Split("-");
                    var folioInicial = folios[0].Trim();
                    var folioFinal = folios[1].Trim();
                    listTotales.Add(new()
                    {
                        Cantidad = t.Cantidad,
                        FolioFinal = folioFinal,
                        FolioInicial = folioInicial,
                        NombreHolograma = t.Certificado,
                        ImporteTotal = t.ImporteTotal
                    });
                }
                venta.certificadosTotalData = listTotales;
                return new ResponseGeneric<VentaCertificadoResponse>(venta);
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<VentaCertificadoResponse>(ex);
            }
        }

        //public async Task<ResponseGeneric<long>> Registro(VentaApiRequest venta)
        //{
        //    try
        //    {
        //        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //        {
        //            //Registramos VentaFV
        //            var ventaFV = new VentaFV
        //            {
        //                NumeroReferencia = venta.NumeroReferencia?.ToUpper(),
        //                FechaRegistro = DateTime.Now,
        //                FechaVenta = venta.FechaVenta.Value,
        //                NumeroCompra = venta.NumeroCompra?.ToUpper(),
        //                IdUserRegistro = _userResolver.GetUser().IdUser,
        //                Id = 0
        //            };

        //            _context.VentaFVs.Add(ventaFV);
        //            await _context.SaveChangesAsync();

        //            if (ventaFV.Id == 0)
        //                throw new ValidationException("No se pudo registrar la ventaFV.");

        //            var i = 0;
        //            foreach (var file in venta.Files)
        //            {
        //                var url = await _blobStorage.UploadFileAsync(new byte[0], "VentaCVV/" + ventaFV.Id + "/" + file.Nombre, file.Base64);
        //                if (!string.IsNullOrEmpty(url))
        //                {
        //                    switch (i)
        //                    {
        //                        case 0:
        //                            ventaFV.UrlDoc1 = url; break;
        //                        case 1:
        //                            ventaFV.UrlDoc2 = url; break;
        //                        case 2:
        //                            ventaFV.UrlDoc3 = url; break;
        //                    }
        //                }
        //                i++;
        //            }
        //            await _context.SaveChangesAsync();
        //            //Registros todos los registros de venta.
        //            foreach (var item in venta.VentaCertificados)
        //            {
        //                var validado = await _smadsotGenericInserts.ValidateVenta(item, venta.IdAlmacen);
        //                if (!validado.IsSucces)
        //                {
        //                    throw new ValidationException(validado.Description);
        //                }

        //                var ciclo = _context.CicloVerificacions.FirstOrDefault(x => x.Activo && x.FechaInicio <= DateTime.Now && x.FechaFin >= DateTime.Now) ?? throw new ValidationException("No existe un ciclo de verificación registrado para el período actual");
        //                // var RangosInsertados = _context.IngresoCertificados.Any(x => (item.FolioInicial <= x.FolioFinal && item.FolioFinal >= x.FolioInicial) || (x.FolioInicial <= item.FolioFinal && x.FolioFinal >= item.FolioInicial));
        //                // if (RangosInsertados)
        //                //     throw new ValidationException($"El rango de folios en el certificado {TipoCertificado.DictNombreCertificado[item.IdTipoCertificado]} ya está registrado en los ingresos.");
        //                var RangosInsertados = _context.vVentaCertificados.Any(x => (item.FolioInicial <= x.FolioFinal && item.FolioFinal >= x.FolioInicial) || (x.FolioInicial <= item.FolioFinal && x.FolioFinal >= item.FolioInicial));
        //                if (RangosInsertados)
        //                    throw new ValidationException($"El rango de folios en el certificado {TipoCertificado.DictNombreCertificado[item.IdTipoCertificado]} ya está registrado en las ventas.");

        //                //Registramos venta de certificados 
        //                var ventaCertificadosDobleCero = new VentaCertificado
        //                {
        //                    Id = 0,
        //                    IdVentaFV = ventaFV.Id,
        //                    Cantidad = item.Cantidad,
        //                    ImporteTotal = (ciclo.ImporteFv * item.Cantidad),
        //                    Folios = $"{item.FolioInicial}-{item.FolioFinal}",
        //                    IdCatTipoCertificado = item.IdTipoCertificado
        //                };

        //                _context.VentaCertificados.Add(ventaCertificadosDobleCero);
        //                await _context.SaveChangesAsync();

        //                if (ventaCertificadosDobleCero.Id == 0)
        //                    throw new ValidationException($"No se pudo registrar la venta de certificados");

        //                //Registramos un movimiento de salida con el id de venta ligado.
        //                var movimientoDobleCero = new MovimientosInventario
        //                {
        //                    IdVentaCertificado = ventaCertificadosDobleCero.Id,
        //                    IdInventario = validado.recordId
        //                };

        //                _context.MovimientosInventarios.Add(movimientoDobleCero);
        //                await _context.SaveChangesAsync();
        //            }
        //            transaction.Complete();
        //        }
        //        return new ResponseGeneric<long>(1);
        //    }
        //    catch (ValidationException e)
        //    {
        //        return new ResponseGeneric<long>(0) { mensaje = e.Message };
        //    }
        //    catch (Exception ex)
        //    {

        //        return new ResponseGeneric<long>(ex);
        //    }
        //}

        public async Task<ResponseGeneric<long>> Registro(VentaFVRequest venta)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var registroVenta = new VentaFV();
                    var ingresoSolicitud = _context.IngresoFVs.FirstOrDefault(x => x.Id == venta.IdIngresoFV) ?? throw new ValidationException($"No se encontró un ingreso.");
                    registroVenta = new VentaFV
                    {
                        Id = ingresoSolicitud.Id,
                        NumeroReferencia = venta.NumeroReferencia,
                        IdUserRegistro = _userResolver.GetUser().IdUser,
                        FechaRegistro = DateTime.Now,
                        FechaVenta = venta.FechaVenta,
                        NumeroCompra = venta.NumeroCompra
                    };
                    _context.VentaFVs.Add(registroVenta);
                    var result = await _context.SaveChangesAsync() > 0;
                    var i = 0;
                    foreach (var f in venta.Files)
                    {
                        var url = await _blobStorage.UploadFileAsync(Array.Empty<byte>(), $"VentaCVV/{f.Tipo}" + registroVenta.Id + "/" + f.Nombre, f.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            switch (i)
                            {
                                case 0:
                                    registroVenta.UrlDoc1 = url; break;
                                case 1:
                                    registroVenta.UrlDoc2 = url; break;
                                case 2:
                                    registroVenta.UrlDoc3 = url; break;
                            }
                        }
                        i++;
                    }

                    var ingresoCertificado = await _context.IngresoCertificados.Include(x => x.MovimientosInventarios).Where(x => x.IdIngresoFV == ingresoSolicitud.Id).ToListAsync();
                    for (i = 0; i < ingresoCertificado.Count; i++)
                    {

                        var ventaCertificado = new VentaCertificado
                        {
                            IdVentaFV = registroVenta.Id,
                            Folios = $"{ingresoCertificado[i].FolioInicial} - {ingresoCertificado[i].FolioFinal}",
                            Cantidad = ingresoCertificado[i].CantidadRecibida,
                            IdCatTipoCertificado = ingresoCertificado[i].IdCatTipoCertificado,
                            ImporteTotal = ingresoCertificado[i].CantidadRecibida * venta.certificadoVentas.FirstOrDefault(x => x.IdIngresoCertificado == ingresoCertificado[i].Id)?.PrecioUnitario ?? throw new ValidationException($"No se estableció el precio de venta para la serie de folios {ingresoCertificado[i].FolioInicial} - {ingresoCertificado[i].FolioFinal}."),
                        };
                        _context.VentaCertificados.Add(ventaCertificado);
                        await _context.SaveChangesAsync();
                        var movimientos = ingresoCertificado[i].MovimientosInventarios.Select(x => new MovimientosInventario
                        {
                            IdVentaCertificado = ventaCertificado.Id,
                            IdInventario = x.IdInventario,
                        });
                        _context.MovimientosInventarios.AddRange(movimientos);
                    }
                    var solicitud = await _context.SolicitudFVs.FirstOrDefaultAsync(x => x.Id == ingresoSolicitud.IdSolicitudFV) ?? throw new ValidationException("No hay registro de la solicitud.");
                    solicitud.IdCatEstatusSolicitud = EstatusSolicitud.VentaFinalizada;
                    await _context.SaveChangesAsync();
                    transaction.Complete();
                }
                return new ResponseGeneric<long>(1);
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<long>() { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>() { mensaje = "Ocurrió un error al intentar guardar la información de la venta." };
            }
        }

        public async Task<ResponseGeneric<List<RefrendoAutocompleteResponse>>> AutocompleteSolicitud(VentaCVVAutocompletRequest request)
        {
            try
            {
                var ingresos = _context.IngresoFVs.Where(x => x.IdSolicitudFV.ToString().ToLower().Contains(request.Term.ToLower()) && x.IdAlmacen == request.IdAlmacen).AsQueryable();
                ingresos = ingresos.Skip(request.Start).Take(request.End).AsQueryable();
                var result = await ingresos.Select(x => new RefrendoAutocompleteResponse
                {
                    Id = x.IdSolicitudFV,
                    Text = x.IdSolicitudFV.ToString(),
                }).ToListAsync();
                return new ResponseGeneric<List<RefrendoAutocompleteResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<RefrendoAutocompleteResponse>>(ex);
            }
        }
    }

    public interface IVentaCVVNegocio
    {
        public Task<ResponseGeneric<ResponseGrid<SeguimientoSolicitudResponse>>> Consulta(SolicitudFormaValoradaListRequest request);

        public Task<ResponseGeneric<List<VerificentroResponse>>> ConsultaAutocomplete(string request);

        public Task<ResponseGeneric<InventarioVentaCVVResponse>> DetalleInventarioVentaCVV(long idIngreso);

        public Task<ResponseGeneric<long>> Registro(VentaFVRequest venta);

        public Task<ResponseGeneric<VentaCertificadoResponse>> DetalleVentaCertificado(long idAlmacen, long idVenta);
        public Task<ResponseGeneric<List<RefrendoAutocompleteResponse>>> AutocompleteSolicitud(VentaCVVAutocompletRequest request);
    }
}