using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Polly.Caching;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.ConsultaSock.Request;
using Smadot.Models.Entities.ConsultaSock.Response;
using Smadot.Models.Entities.FoliosRegresadosSPF.Request;
using Smadot.Models.Entities.FoliosRegresadosSPF.Response;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Request;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Response;
using Smadot.Models.Entities.FoliosVendidosCentrosVerificacion.Request;
using Smadot.Models.Entities.FoliosVendidosCentrosVerificacion.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Models.Entities.StockCertificado.Response;
using Smadot.Models.Entities.StockCertificado.Request;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Net.NetworkInformation;
using Smadot.Models.Entities.StockMinimo.Response;
using Smadot.Utilities.Modelos.Interfaz;
using Smadot.Models.Entities.StockMinimo.Request;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.Entities.EstadisticasUsoFormaValorada.Request;
using Smadot.Models.Entities.Generic.Response;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Smadot.SolicitudFormaValorada.model.Negocio
{
    public class SolicitudFormaValoradaNegocio : ISolicitudFormaValoradaNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        public SolicitudFormaValoradaNegocio(SmadotDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
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
                    IdIngresoFV = x.IdIngresoFV,
                    FechaVentaVFV = x.FechaVentaVFV,
                }).AsQueryable();

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
                var result = new ResponseGrid<SeguimientoSolicitudResponse>{
                    RecordsFiltered=tot,
                    RecordsTotal=tot,
                    Data= await solicitudes.ToListAsync()
                };
                return new ResponseGeneric<ResponseGrid<SeguimientoSolicitudResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<SeguimientoSolicitudResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<SolicitudFormaValoradaResponse>>> GetById(long Id)
        {
            try
            {
                var result = new List<SolicitudFormaValoradaResponse>();
                if (Id > 0)
                {
                    var solicitud = _context.vSolicitudFormaValorada.Where(x => x.IdSolicitudFV == Id);
                    if (solicitud != null)
                    {
                        string r = JsonConvert.SerializeObject(solicitud);
                        result = JsonConvert.DeserializeObject<List<SolicitudFormaValoradaResponse>>(r);
                        result = result.OrderBy(x => x.ClaveCertificadoSC).ToList();
                    }
                }
                else
                {
                    var folios = _context.Inventarios.GroupBy(x => x.IdCatTipoCertificado).Select(x => new
                    {
                        IdCatTipoCertificadoSC = x.Key,
                        Final = x.Max(y => y.FolioFinal + 1)
                    }).ToList(); //TODO probar si devuelve una lista vacia al borrar la tabla para primer registro
                    var catTipoCert = _context.CatTipoCertificados.Where(x => x.Activo).OrderBy(x => x.ClaveCertificado).ToList();
                    foreach (var c in catTipoCert)
                    {
                        var folioInicial = folios.FirstOrDefault(x => x.IdCatTipoCertificadoSC == c.Id)?.Final ?? 1;
                        result.Add(new SolicitudFormaValoradaResponse
                        {
                            FechaSolicitudFV = DateTime.Now,
                            IdCatTipoCertificadoSC = c.Id,
                            TipoCertificadoSC = c.Nombre,
                            ClaveCertificadoSC = c.ClaveCertificado,
                            FolioInicialSC = folioInicial < 1 ? 1 : folioInicial
                        });
                    }
                }
                if (result != null && result[0] != null)
                {
                    result[0].Almacenes = await _context.Almacens.Where(x => x.Activo && (x.IdVerificentro == _userResolver.GetUser().IdVerificentro || x.IdVerificentro == null)).Select(x => new AlmacenResponse
                    {
                        Id = x.Id,
                        Activo = x.Activo,
                        IdVerificentro = x.IdVerificentro,
                        Nombre = x.Nombre
                    }).ToListAsync();
                }
                return new ResponseGeneric<List<SolicitudFormaValoradaResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<SolicitudFormaValoradaResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> Registro(SolicitudFormaValoradaRequest request)
        {
            try
            {
                var solicitud = new SolicitudFV();
                if (request.IdAlmacenFV != 1)
                {
                    request.SolicitudesCertificado = request.SolicitudesCertificado.Where(x => x.IdCatTipoCertificado != TipoCertificado.Testificacion && x.IdCatTipoCertificado != TipoCertificado.Exentos).ToList();
                }
                if (request.IdSolicitudFV > 0)
                {
                    solicitud = _context.SolicitudFVs.Include(x => x.SolicitudCertificados).FirstOrDefault(x => x.Id == request.IdSolicitudFV);
                    if (solicitud == null || solicitud.SolicitudCertificados.Count() == 0)
                        throw new ValidationException("No sé encontró una solicitud.");
                    solicitud.FechaSolicitud = request.FechaSolicitudFV;
                    solicitud.IdAlmacen = request.IdAlmacenFV;
                    var eliminarCerts = solicitud.SolicitudCertificados.Where(x => !request.SolicitudesCertificado.Any(c => c.IdCatTipoCertificado == x.IdCatTipoCertificado));
                    _context.SolicitudCertificados.RemoveRange(eliminarCerts);
                    foreach (var c in request.SolicitudesCertificado)
                    {
                        var solicitudCertificado = solicitud.SolicitudCertificados.FirstOrDefault(x => x.IdCatTipoCertificado == c.IdCatTipoCertificado);
                        if (solicitudCertificado == null)
                        {
                            solicitud.SolicitudCertificados.Add(new SolicitudCertificado { IdCatTipoCertificado = c.IdCatTipoCertificado, Cantidad = c.Cantidad });
                        }
                        else
                        {
                            solicitudCertificado.Cantidad = c.Cantidad;
                        }
                    }
                }
                else
                {
                    var user = _userResolver.GetUser();
                    solicitud = new SolicitudFV
                    {
                        FechaSolicitud = request.FechaSolicitudFV,
                        IdUserSolicita = user.IdUser,
                        IdCatEstatusSolicitud = EstatusSolicitud.Solicitado,
                        Activo = true,
                        FechaRegistro = DateTime.Now,
                        IdAlmacen = request.IdAlmacenFV,
                    };
                    if (!request.SolicitudesCertificado.Any())
                    {
                        throw new ValidationException("No hay cantidades de certificados para registrar.");
                    }
                    foreach (var c in request.SolicitudesCertificado)
                    {
                        solicitud.SolicitudCertificados.Add(new SolicitudCertificado
                        {
                            IdCatTipoCertificado = c.IdCatTipoCertificado,
                            Cantidad = c.Cantidad,
                        });
                    }
                    _context.SolicitudFVs.Add(solicitud);
                }
                var result = await _context.SaveChangesAsync() > 0;
                return result ? new ResponseGeneric<long>(solicitud.Id) : new ResponseGeneric<long> { CurrentException = "No se encontraron cambios." };
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<long>(ex) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex) { mensaje = "La información del solicitud no pudo ser guardada." };
            }
        }

        public async Task<ResponseGeneric<List<FoliosUsadosEnVentanillaResponse>>> ConsultaFoliosUsadosVentanilla(FoliosUsadosEnVentanillaRequest request)
        {
            try
            {
                var folios = _context.vFoliosUsadosVentanillas.AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    folios = folios.Where(x => x.DatosVehiculo.ToLower().Contains(request.Busqueda.ToLower())
                                            || x.Razon.ToLower().Contains(request.Busqueda.ToLower())
                                            || x.FolioTramite.ToString().ToLower().Contains(request.Busqueda.ToLower())
                                            || x.NombreUsuario.ToLower().Contains(request.Busqueda.ToLower())
                                            || x.CorreoUsuario.ToLower().Contains(request.Busqueda.ToLower())
                                            //|| x.TelefonoUsuario.ToLower().Contains(request.Busqueda.ToLower())
                                            || x.ReferenciaBancaria.ToLower().Contains(request.Busqueda.ToLower())
                                            || x.Fecha.ToString().Contains(request.Busqueda.ToLower())
                                            || x.FolioCertificado.ToLower().Contains(request.Busqueda.ToLower())
                                            || x.PersonaGeneroTramite.ToLower().Contains(request.Busqueda.ToLower())
                                            || x.TipoTramite.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var total = folios.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    folios = folios.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    folios = folios.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await folios.Select(x => new FoliosUsadosEnVentanillaResponse
                {
                    IdFormaValorada = x.IdFormaValorada,
                    NombreUsuario = x.NombreUsuario,
                    CorreoUsuario = x.CorreoUsuario,
                    TelefonoUsuario = "",
                    DatosVehiculo = x.DatosVehiculo,
                    Fecha = x.Fecha,
                    FolioCertificado = x.FolioCertificado,
                    FolioTramite = x.FolioTramite.ToString("000000000"),
                    MontoTramite = x.MontoTramite,
                    PersonaGeneroTramite = x.PersonaGeneroTramite,
                    Razon = x.Razon,
                    ReferenciaBancaria = x.ReferenciaBancaria,
                    ClaveTramite = x.ClaveTramite,
                    Total = total
                }).ToListAsync();
                return new ResponseGeneric<List<FoliosUsadosEnVentanillaResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<FoliosUsadosEnVentanillaResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<FoliosRegresadosSPFResponse>>> ConsultaFoliosRegresadosSPF(FoliosRegresadosSPFRequest request)
        {
            try
            {
                var folios = _context.vFoliosRegresadosSPFs.AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    folios = folios.Where(x => x.Fecha.ToString().Contains(request.Busqueda.ToLower()) ||
                                                x.ClaveDevolucion.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.ClaveCertificado.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.ClaveSolicitud.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.ClaveVenta.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.FolioFinal.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.FolioInicial.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.PersonaValido.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.ResponsableEntrega.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.TipoCertificado.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var total = folios.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    folios = folios.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    folios = folios.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                if (!request.Pagination)
                {
                    if (request.Pagina > 0 && request.Registros > 0)
                    {
                        folios = folios.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                    }
                }

                var result = await folios.Select(x => new FoliosRegresadosSPFResponse
                {
                    IdFormaValorada = x.IdFormaValorada,
                    ClaveVenta = x.ClaveVenta,
                    ClaveDevolucion = x.ClaveDevolucion,
                    ClaveCertificado = x.ClaveCertificado,
                    ClaveSolicitud = x.ClaveSolicitud,
                    Fecha = x.Fecha,
                    FolioFinal = x.FolioFinal,
                    FolioInicial = x.FolioInicial,
                    PersonaValido = x.PersonaValido,
                    ResponsableEntrega = x.ResponsableEntrega,
                    TipoCertificado = x.TipoCertificado,
                    Total = total
                }).ToListAsync();
                return new ResponseGeneric<List<FoliosRegresadosSPFResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<FoliosRegresadosSPFResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<FoliosVendidosCentrosVerificacionResponse>>> ConsultaFoliosVendidosCentrosVerificacion(FoliosVendidosCentrosVerificacionRequest request)
        {
            try
            {
                var folios = _context.vFoliosVendidosCentrosVerificacions.AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    folios = folios.Where(x => x.Fecha.ToString().Contains(request.Busqueda.ToLower()) ||
                                                x.Cvv.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.PersonaRecibeCertificados.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.ClaveVenta.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.ReferenciaBancaria.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var total = folios.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    folios = folios.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    folios = folios.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                if (!request.Pagination)
                {
                    if (request.Pagina > 0 && request.Registros > 0)
                    {
                        folios = folios.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                    }
                }

                var result = await folios.Select(x => new FoliosVendidosCentrosVerificacionResponse
                {
                    IdFormaValorada = x.IdFormaValorada,
                    ClaveVenta = x.ClaveVenta,
                    CVV = x.Cvv,
                    Fecha = x.Fecha,
                    FolioFV = x.FoliosFV,
                    FoliosStock = x.FoliosStock,
                    FolioVenta = x.FolioVenta,
                    MontoCadaVenta = x.Monto,
                    PersonaRecibeCertificado = x.PersonaRecibeCertificados,
                    ReferenciaBancaria = x.ReferenciaBancaria,
                    Total = x.Total,
                    TotalRegistros = total
                }).ToListAsync();
                return new ResponseGeneric<List<FoliosVendidosCentrosVerificacionResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<FoliosVendidosCentrosVerificacionResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<ConsultaStockDVRFResponse>>> InformeConsultaStockDVRF(ConsultaStockDVRFRequest request)
        {
            try
            {
                var stock = _context.vConsultaStockDVRves.AsQueryable();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    stock = stock.Where(x => x.NombreTipoCertificado.ToLower().Contains(request.Busqueda.ToLower())
                    || x.CantidadStock.ToString().Contains(request.Busqueda.ToLower())
                    || x.NumeroCaja.ToString().Contains(request.Busqueda.ToLower())
                    || x.FolioInicial.ToString().Contains(request.Busqueda.ToLower())
                    || x.FolioFinal.ToString().Contains(request.Busqueda.ToLower())
                    || x.ClaveCertificado.ToLower().Contains(request.Busqueda.ToLower()));
                }

                if (request.IdAlmacen.HasValue)
                    stock = stock.Where(x => x.IdAlmacen == request.IdAlmacen);

                var total = stock.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    stock = stock.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    stock = stock.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await stock.Select(x => new ConsultaStockDVRFResponse
                {
                    Almacen = x.Almacen,
                    NombreTipoCertificado = x.NombreTipoCertificado,
                    CantidadStock = x.CantidadStock,
                    NumeroCaja = x.NumeroCaja,
                    FolioInicial = x.FolioInicial.Value,
                    FolioFinal = x.FolioFinal.Value,
                    ClaveCertificado = x.ClaveCertificado,
                    IdAlmacen = x.IdAlmacen,
                    CantidadMedia = x.CantidadMedia,
                    CantidadMinima = x.CantidadMinima,
                    NumeroSolucitud = x.NumeroSolucitud.Value,
                    Total = total
                }).ToListAsync();

                return new ResponseGeneric<List<ConsultaStockDVRFResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<ConsultaStockDVRFResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<FoliosRegresadosSPFResponse>> DetalleFolioRegresadoSPF(long id)
        {
            try
            {
                var folio = await _context.vFoliosRegresadosSPFs.FirstOrDefaultAsync(x => x.IdFormaValorada == id);
                if (folio is null)
                    return new ResponseGeneric<FoliosRegresadosSPFResponse>();

                var result = new FoliosRegresadosSPFResponse
                {
                    IdFormaValorada = folio.IdFormaValorada,
                    ClaveVenta = folio.ClaveVenta,
                    ClaveDevolucion = folio.ClaveDevolucion,
                    ClaveCertificado = folio.ClaveCertificado,
                    ClaveSolicitud = folio.ClaveSolicitud,
                    Fecha = folio.Fecha,
                    FolioFinal = folio.FolioFinal,
                    FolioInicial = folio.FolioInicial,
                    PersonaValido = folio.PersonaValido,
                    ResponsableEntrega = folio.ResponsableEntrega,
                    TipoCertificado = folio.TipoCertificado
                };

                return new ResponseGeneric<FoliosRegresadosSPFResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<FoliosRegresadosSPFResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<FoliosVendidosCentrosVerificacionResponse>> DetalleFoliosVendidosCentrosVerificacion(long id)
        {
            try
            {
                var folio = await _context.vFoliosVendidosCentrosVerificacions.FirstOrDefaultAsync(x => x.IdFormaValorada == id);
                if (folio is null)
                    return new ResponseGeneric<FoliosVendidosCentrosVerificacionResponse>();

                var result = new FoliosVendidosCentrosVerificacionResponse
                {
                    IdFormaValorada = folio.IdFormaValorada,
                    ClaveVenta = folio.ClaveVenta,
                    CVV = folio.Cvv,
                    Fecha = folio.Fecha,
                    FolioFV = folio.FoliosFV,
                    FoliosStock = folio.FoliosStock,
                    FolioVenta = folio.FolioVenta,
                    FoliosUsados = folio.FoliosUsados,
                    MontoCadaVenta = folio.Monto,
                    PersonaRecibeCertificado = folio.PersonaRecibeCertificados,
                    ReferenciaBancaria = folio.ReferenciaBancaria,
                    Total = folio.Total
                };

                return new ResponseGeneric<FoliosVendidosCentrosVerificacionResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<FoliosVendidosCentrosVerificacionResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<FoliosUsadosEnVentanillaResponse>> DetalleFoliosVentanilla(long id)
        {
            try
            {
                var folio = await _context.vFoliosUsadosVentanillas.FirstOrDefaultAsync(x => x.IdFormaValorada == id);
                if (folio is null)
                    return new ResponseGeneric<FoliosUsadosEnVentanillaResponse>();

                var result = new FoliosUsadosEnVentanillaResponse
                {
                    IdFormaValorada = folio.IdFormaValorada,
                    NombreUsuario = folio.NombreUsuario,
                    CorreoUsuario = folio.CorreoUsuario,
                    TelefonoUsuario = "",
                    DatosVehiculo = folio.DatosVehiculo,
                    Fecha = folio.Fecha,
                    FolioCertificado = folio.FolioCertificado,
                    FolioTramite = folio.FolioTramite.ToString("000000000"),
                    MontoTramite = folio.MontoTramite,
                    PersonaGeneroTramite = folio.PersonaGeneroTramite,
                    Razon = folio.Razon,
                    ReferenciaBancaria = folio.ReferenciaBancaria,
                    TipoTramite = folio.TipoTramite
                };

                return new ResponseGeneric<FoliosUsadosEnVentanillaResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<FoliosUsadosEnVentanillaResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<ConsultaStockDVRFResponse>> GetByIdDVRF(long IdAlmacen)
        {
            try
            {
                var result = new ConsultaStockDVRFResponse();
                var catalogo = _context.vConsultaStockDVRves.FirstOrDefault(x => x.IdAlmacen == IdAlmacen);
                if (catalogo != null)
                {
                    result.IdAlmacen = catalogo.IdAlmacen;
                    result.NombreTipoCertificado = catalogo.NombreTipoCertificado;
                    result.CantidadStock = catalogo.CantidadStock;
                    result.NumeroCaja = catalogo.NumeroCaja;
                    result.FolioInicial = catalogo.FolioInicial;
                    result.FolioFinal = catalogo.FolioFinal;
                    result.ClaveCertificado = catalogo.ClaveCertificado;
                    result.CantidadMedia = catalogo.CantidadMedia;
                    result.CantidadMinima = catalogo.CantidadMinima;
                }
                return new ResponseGeneric<ConsultaStockDVRFResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ConsultaStockDVRFResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<List<StockMinimoResponse>>> ConsultaStockMinimo(StockMinimoRequest request)
        {
            try
            {
                var stock = _context.vStockMinimos.AsQueryable();

                if (request.IdAlmacen.HasValue)
                    stock = stock.Where(x => x.IdAlmacen == request.IdAlmacen);

                var total = stock.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    stock = stock.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    stock = stock.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await stock.Select(x => new StockMinimoResponse
                {
                    IdStockMinimo = x.IdStockMinimo,
                    IdAlmacen = x.IdAlmacen,
                    NombreAlmacen = x.NombreAlmacen,
                    CantidadMinima = x.CantidadMinima,
                    CantidadMedia = x.CantidadMedia,
                    IdCatTipoCertificado = x.IdCatTipoCertificado,
                    Nombre = x.Nombre,
                    ClaveCertificado = x.ClaveCertificado,
                    Total = total
                }).ToListAsync();
                if (!result.Any())
                {
                    var TiposCertificado = _context.CatTipoCertificados;
                    var almacen = _context.Almacens.FirstOrDefault(x => x.Id == request.IdAlmacen);
                    foreach (var certificado in TiposCertificado)
                    {
                        result.Add(new StockMinimoResponse
                        {
                            IdStockMinimo = null,
                            IdAlmacen = almacen.Id,
                            NombreAlmacen = almacen.Nombre,
                            IdCatTipoCertificado = certificado.Id,
                            Nombre = certificado.Nombre,
                            ClaveCertificado = certificado.ClaveCertificado,
                            Total = TiposCertificado.Count()
                        });
                    }
                }
                return new ResponseGeneric<List<StockMinimoResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<StockMinimoResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> RegistroStockMinimo(List<StockMinimoResponse> request)
        {
            try
            {
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    request = request.Where(x => x.CantidadMedia != null && x.CantidadMedia != null).ToList();
                    StockMinimo? stockMinimo = await _context.StockMinimos.Include(x => x.StockCertificados).FirstOrDefaultAsync(x => x.IdAlmacen == (long)request[0].IdAlmacen);

                    var result = false;

                    if (stockMinimo == null)
                    {
                        stockMinimo = new StockMinimo
                        {
                            // Id = (long)request[0].IdStockMinimo,
                            FechaRegistro = DateTime.Now,
                            IdAlmacen = (long)request[0].IdAlmacen
                        };
                        _context.StockMinimos.Add(stockMinimo);
                        await _context.SaveChangesAsync();
                        foreach (var c in request)
                        {
                            stockMinimo.StockCertificados.Add(new StockCertificado
                            {
                                IdCatTipoCertificado = c.IdCatTipoCertificado.Value,
                                CantidadMinima = c.CantidadMinima.Value,
                                CantidadMedia = c.CantidadMedia.Value,
                                IdStockMinimo = stockMinimo.Id
                            });
                        }
                    }
                    else
                    {
                        foreach (var sc in stockMinimo.StockCertificados)
                        {
                            var stock = request.FirstOrDefault(x => x.IdCatTipoCertificado == sc.IdCatTipoCertificado);
                            sc.CantidadMedia = (int)stock.CantidadMedia;
                            sc.CantidadMinima = (int)stock.CantidadMinima;
                        }
                    }
                    result = await _context.SaveChangesAsync() > 0;

                    ts.Complete();
                    return result ? new ResponseGeneric<long>((long)stockMinimo.Id) { mensaje = "La información se guardó de manera exitosa." } : new ResponseGeneric<long>() { mensaje = "No se detectaron cambios para realizar el registro." };
                }
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex) { mensaje = "Ocurrió un error al intentar guardar la información del stock mínimo." };
            }
        }



    }
    public interface ISolicitudFormaValoradaNegocio
    {
        Task<ResponseGeneric<ResponseGrid<SeguimientoSolicitudResponse>>> Consulta(SolicitudFormaValoradaListRequest request);
        Task<ResponseGeneric<List<SolicitudFormaValoradaResponse>>> GetById(long Id);
        Task<ResponseGeneric<long>> Registro(SolicitudFormaValoradaRequest request);
        Task<ResponseGeneric<List<FoliosUsadosEnVentanillaResponse>>> ConsultaFoliosUsadosVentanilla(FoliosUsadosEnVentanillaRequest request);
        Task<ResponseGeneric<List<FoliosRegresadosSPFResponse>>> ConsultaFoliosRegresadosSPF(FoliosRegresadosSPFRequest request);
        Task<ResponseGeneric<List<FoliosVendidosCentrosVerificacionResponse>>> ConsultaFoliosVendidosCentrosVerificacion(FoliosVendidosCentrosVerificacionRequest request);
        Task<ResponseGeneric<List<ConsultaStockDVRFResponse>>> InformeConsultaStockDVRF(ConsultaStockDVRFRequest request);
        Task<ResponseGeneric<FoliosRegresadosSPFResponse>> DetalleFolioRegresadoSPF(long id);
        Task<ResponseGeneric<FoliosVendidosCentrosVerificacionResponse>> DetalleFoliosVendidosCentrosVerificacion(long id);
        Task<ResponseGeneric<FoliosUsadosEnVentanillaResponse>> DetalleFoliosVentanilla(long id);
        Task<ResponseGeneric<ConsultaStockDVRFResponse>> GetByIdDVRF(long IdAlmacen);
        Task<ResponseGeneric<List<StockMinimoResponse>>> ConsultaStockMinimo(StockMinimoRequest request);
        Task<ResponseGeneric<long>> RegistroStockMinimo(List<StockMinimoResponse> request);

    }
}
