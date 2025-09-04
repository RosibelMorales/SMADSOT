using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Newtonsoft.Json;
using Polly;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.IngresoFormaValorada.Request;
using Smadot.Models.Entities.IngresoFormaValorada.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Smadot.IngresoFormaValorada.Model.Negocio;

public class IngresoFormaValoradaNegocio : IIngresoFormaValoradaNegocio
{
    private readonly SmadotDbContext _context;
    private readonly IUserResolver _userResolver;
    private readonly BlobStorage _blobStorage;
    private readonly string _connectionString;
    public IngresoFormaValoradaNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
    {
        _context = context;
        _userResolver = userResolver;
        _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<ResponseGeneric<List<SolicitudFormaValoradaResponse>>> Consulta(SolicitudFormaValoradaListRequest request)
    {
        try
        {
            var solicitudes = _context.vSeguimientoSolicituds.AsQueryable();
            var user = _userResolver.GetUser();
            var rol = user.RoleNames.FirstOrDefault();
            var acceso = _context.Rols.ToList().Where(o => o.Nombre.Equals(rol, StringComparison.Ordinal)).FirstOrDefault();
            // if (!acceso.AccesoTotalVerificentros)
            // solicitudes = solicitudes.Where(x => x.IdVerificentro == user.IdVerificentro).AsQueryable();

            if (request.SiguienteFolio.HasValue && request.SiguienteFolio.Value == true)
                solicitudes = solicitudes.Where(x => x.IdCatEstatusSolicitudFV == TipoCertificado.DobleCero).Take(1);
            if (request.IdAlmacen.HasValue)
                solicitudes = solicitudes.Where(x => x.IdAlmacenFV == request.IdAlmacen);
            if (!string.IsNullOrEmpty(request.Busqueda))
            {
                solicitudes = solicitudes.Where(x => x.IdSolicitudFV.ToString().Contains(request.Busqueda.ToLower()) || x.UserSolicitaFV.ToLower().Contains(request.Busqueda.ToLower()) || x.EstatusFV.ToLower().Contains(request.Busqueda.ToLower()) || x.NombreRecibioIFV.ToLower().Contains(request.Busqueda.ToLower()));
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
            var result = await solicitudes.Select(x => new SolicitudFormaValoradaResponse
            {
                IdSolicitudFV = x.IdSolicitudFV,
                IdIngresoFV = x.IdIngresoFV,
                FechaSolicitudFV = x.FechaSolicitudFV,
                IdUserSolicitaFV = x.IdUserSolicitaFV,
                UserSolicitaFV = x.UserSolicitaFV,
                IdCatEstatusSolicitudFV = x.IdCatEstatusSolicitudFV,
                EstatusFV = x.EstatusFV,
                ActivoFV = x.ActivoFV,
                FechaRegistroFV = x.FechaRegistroFV,
                IdAlmacenFV = x.IdAlmacenFV,
                AlmacenFV = x.AlmacenFV,
                IdSC = x.IdCatEstatusSolicitudFV,
                FechaEntregaIFV = x.FechaEntregaIFV,
                NombreRecibioIFV = x.NombreRecibioIFV,
                Total = tot
            }).ToListAsync();
            return new ResponseGeneric<List<SolicitudFormaValoradaResponse>>(result);
        }
        catch (Exception ex)
        {
            return new ResponseGeneric<List<SolicitudFormaValoradaResponse>>(ex);
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

                    var fecha = result[0].FechaSolicitudFV;
                    var date = fecha.ToString("dd/MM/yyyy");
                    result[0].FechaSolicitud = date;


                    int sumaCantidadRecibida = result.Sum(x => x.CantidadSC);
                    result.Add(new SolicitudFormaValoradaResponse
                    {
                        CantidadTotal = sumaCantidadRecibida
                    });
                }
            }
            else
            {
                var catTipoCert = _context.CatTipoCertificados.Where(x => x.Activo).OrderBy(x => Id).ToList();
                foreach (var c in catTipoCert)
                {
                    result.Add(new SolicitudFormaValoradaResponse
                    {

                        FechaSolicitudFV = DateTime.Now,
                        IdCatTipoCertificadoSC = c.Id,
                        TipoCertificadoSC = c.Nombre,
                        ClaveCertificadoSC = c.ClaveCertificado,
                        FolioInicialSC = 0

                    });
                }

            }
            if (result[0] != null)
            {
                result[0].Almacenes = await _context.Almacens.Where(x => x.Activo && x.IdVerificentro == result[0].IdVerificentro).Select(x => new AlmacenResponse
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

    public async Task<ResponseGeneric<long>> Registro(IngresoFormaValoradaRequest request)
    {
        try
        {
            // var movimientoInventario = new MovimientosInventario();
            // var inventario = new Inventario();
            // var ingresoCertificado = new IngresoCertificado();
            var ingresoSolicitud = new IngresoFV();
            // var solicitud = new SolicitudFV();
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var user = _userResolver.GetUser();
            var cantidades = await _context.vSolicitudFormaValorada.Where(x => x.IdSolicitudFV == request.IdSolicitudFV).ToListAsync();
            var certificadosInvalidos = cantidades.Where(x => x.CantidadSC > 0 && x.IdCatTipoCertificadoSC > 0);
            // Verificamos que los certificados traigan cantidades
            if (!certificadosInvalidos.Any())
            {
                throw new ValidationException($"No se encontraron registros de cantidades solicitadas.");
            }
            certificadosInvalidos = cantidades.Where(x => x.CantidadSC < request.certificados.Where(c => c.IdCatTipoCertificado == x.IdCatTipoCertificadoSC).Sum(c => c.CantidadRecibida));
            // Validamos que no haya caertificados que superen las cantidades solicitadas.
            if (certificadosInvalidos.Any())
            {
                var mensaje = "";
                foreach (var cert in certificadosInvalidos)
                {
                    mensaje += cert.TipoCertificadoSC + ", ";
                }
                mensaje = mensaje[..^2];
                throw new ValidationException($"La cantidad de los certificados {mensaje} supera la cantidad solicitada.");

            }

            // Validamos que la solicitud exista y tenga cantidades solciitadas.
            var solicitudDatos = cantidades.FirstOrDefault() ?? throw new ValidationException($"No se encontraron registros de cantidades solicitadas.");
            // Validamos que no hay finalizado la venta de la solicitud.
            if (solicitudDatos.IdCatEstatusSolicitudFV == EstatusSolicitud.VentaFinalizada)
                throw new ValidationException($"Ya se completó la venta de las Formas Valoradas.");
            var certificadosNoEnSolicitud = request.certificados.Where(x => !cantidades.Any(c => c.IdCatTipoCertificadoSC == x.IdCatTipoCertificado));
            // Verificamos que los certificados que se registran estén en la solicitud original
            if (certificadosNoEnSolicitud.Any())
            {
                var mensaje = "";
                foreach (var cert in certificadosNoEnSolicitud)
                {
                    mensaje += cert.NombreCertificado + ", ";
                }
                mensaje = mensaje[..^2];
                throw new ValidationException($"Los certificados {mensaje} no están en el registro de la solicitud.");
            }
            // Verificamos que no haya solapamiento con las series de folios ya registrados.
            var foliosRangosExistentes = ValidarSolapamiento(request.certificados);
            if (foliosRangosExistentes.Any())
            {

                var mensaje = "";
                foreach (var rango in foliosRangosExistentes)
                {
                    mensaje += $"{rango.Inicial} - {rango.Final} , ";
                }
                mensaje = mensaje[..^2];
                throw new ValidationException($"Los certificados {mensaje} interfieren con registros previos de ingreso de formas valoradas.");
            }
            // Verificamos sí ya se le hizo un registro previo de ingreso a la solicitud]
            if (solicitudDatos.IdIngresoFV > 0)
            {
                ingresoSolicitud = await _context.IngresoFVs.Include(x => x.IngresoCertificados).FirstOrDefaultAsync(x => x.Id == solicitudDatos.IdIngresoFV);
                if (ingresoSolicitud == null)
                {
                    throw new ValidationException($"No se encontró registro del Ingreso de Formas Valoradas para actualizarlo.");
                }
                // Validamos que vengan las series originales en el registro
                var eliminarIngresos = ingresoSolicitud.IngresoCertificados.Where(x => !request.certificados.Any(c => c.IdIngresoCertificado == x.Id)).ToList();
                if (eliminarIngresos.Any())
                {
                    var certificadoNombres = eliminarIngresos.GroupBy(x => x.IdCatTipoCertificado);
                    var mensaje = "";
                    foreach (var item in certificadoNombres)
                    {
                        mensaje += TipoCertificado.DictNombreCertificado[item.Key] + ", ";
                    }
                    mensaje = mensaje[..^2];
                    throw new ValidationException($"Se eliminaron series de folios de certificados: {mensaje}. Una vez registradas las series, solo se pueden editar.");


                }

                // Editamos los campos del ingreso
                ingresoSolicitud.FechaEntrega = ingresoSolicitud.FechaEntrega;
                ingresoSolicitud.NombreRecibio = ingresoSolicitud.NombreRecibio;

                if (request.Documento1 != null)
                {
                    var file = request.Documento1;
                    if (file != null)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "IngresoFormaValorada/" + ingresoSolicitud.Id + "/Documento1/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            ingresoSolicitud.UrlDoc1 = url;
                        }
                    }
                }
                if (request.Documento2 != null)
                {
                    var file = request.Documento2;
                    if (file != null)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "RecepcionDocumentos/" + ingresoSolicitud.Id + "/Documento2/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            ingresoSolicitud.UrlDoc2 = url;
                        }
                    }
                }
                if (request.Documento3 != null)
                {
                    var file = request.Documento3;
                    if (file != null)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "RecepcionDocumentos/" + ingresoSolicitud.Id + "/Documento3/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            ingresoSolicitud.UrlDoc3 = url;
                        }
                    }
                }
                // Obtenemos la lista de ingresos a editar
                var ingresosEditar = request.certificados.Where(x => x.IdIngresoCertificado > 0);
                foreach (var item in ingresosEditar)
                {
                    var ingresoCertificado = ingresoSolicitud.IngresoCertificados.FirstOrDefault(x => x.Id == item.IdIngresoCertificado) ?? throw new ValidationException($"No existe la serie que se intenta editar.");
                    var movimiento = await _context.MovimientosInventarios.FirstOrDefaultAsync(x => x.IdIngresoCertificado == item.IdIngresoCertificado) ?? throw new ValidationException($"No existe el inventario de la serie que se intenta editar.");
                    ingresoCertificado.CantidadRecibida = item.CantidadRecibida;
                    ingresoCertificado.FolioInicial = item.FolioInicial;
                    ingresoCertificado.FolioFinal = item.FolioFinal;
                    _context.Update(ingresoCertificado);
                    var inventario = await _context.Inventarios.FirstOrDefaultAsync(x => x.Id == movimiento.IdInventario) ?? throw new ValidationException($"No existe el inventario de la serie que se intenta editar. Folios {item.FolioInicial} - {item.FolioFinal}");
                    inventario.FolioInicial = item.FolioInicial;
                    inventario.FolioFinal = item.FolioFinal;
                    inventario.CantidadStock += item.CantidadRecibida - inventario.CantidadStock;
                    if (inventario.CantidadStock < 0)
                    {
                        throw new ValidationException($"El inventario no puede quedar en números negativos.");
                    }
                }
                await _context.SaveChangesAsync();
                // Creamos la lista te nuevos ingresos
                var ingresosNuevos = request.certificados.Where(x => x.IdIngresoCertificado == 0).Select(x => new IngresoCertificado
                {
                    CantidadRecibida = x.CantidadRecibida,
                    IdIngresoFV = ingresoSolicitud.Id,
                    FolioInicial = x.FolioInicial,
                    FolioFinal = x.FolioFinal,
                    IdCatTipoCertificado = x.IdCatTipoCertificado,
                });     

                foreach (var ingresoCertificado in ingresosNuevos)          
                {
                    await _context.IngresoCertificados.AddAsync(ingresoCertificado);
                    await _context.SaveChangesAsync();
                    var inventario = new Inventario
                    {
                        IdAlmacen = solicitudDatos.IdAlmacenFV,
                        CantidadStock = ingresoCertificado.CantidadRecibida,
                        FolioInicial = ingresoCertificado.FolioInicial,
                        FolioFinal = ingresoCertificado.FolioFinal,
                        IdCatTipoCertificado = ingresoCertificado.IdCatTipoCertificado,
                    };
                    await _context.Inventarios.AddAsync(inventario);
                    await _context.SaveChangesAsync();
                    await _context.MovimientosInventarios.AddAsync(new MovimientosInventario
                    {
                        IdIngresoCertificado = ingresoCertificado.Id,
                        IdInventario = inventario.Id,
                    });
                }
            }
            else
            {
                ingresoSolicitud = new IngresoFV
                {
                    IdSolicitudFV = request.IdSolicitudFV,
                    FechaEntrega = request.FechaEntrega,
                    NombreRecibio = request.NombreRecibio,
                    IdUserRegistro = user.IdUser,
                    FechaRegistro = DateTime.Now,
                    IdAlmacen = solicitudDatos.IdAlmacenFV,
                    UrlDoc1 = string.Empty,
                    UrlDoc2 = string.Empty,
                    UrlDoc3 = string.Empty
                };
                await _context.IngresoFVs.AddAsync(ingresoSolicitud);
                await _context.SaveChangesAsync();

                if (request.Documento1 != null)
                {
                    var file = request.Documento1;
                    if (file != null)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "IngresoFormaValorada/" + ingresoSolicitud.Id + "/Documento1/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            ingresoSolicitud.UrlDoc1 = url;
                        }
                    }
                }
                if (request.Documento2 != null)
                {
                    var file = request.Documento2;
                    if (file != null)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "RecepcionDocumentos/" + ingresoSolicitud.Id + "/Documento2/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            ingresoSolicitud.UrlDoc2 = url;
                        }
                    }
                }

                if (request.Documento3 != null)
                {
                    var file = request.Documento3;
                    if (file != null)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "RecepcionDocumentos/" + ingresoSolicitud.Id + "/Documento3/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            ingresoSolicitud.UrlDoc3 = url;
                        }
                    }
                }
                // Creamos la lista de registros que se van a registrar
                var ingresos = request.certificados.Select(x => new IngresoCertificado
                {
                    CantidadRecibida = x.CantidadRecibida,
                    IdIngresoFV = ingresoSolicitud.Id,
                    FolioInicial = x.FolioInicial,
                    FolioFinal = x.FolioFinal,
                    IdCatTipoCertificado = x.IdCatTipoCertificado,
                });
                // Vamos iterando uno a uno para ir creando el movimiento de inventario y el inventario
                foreach (var ingresoCertificado in ingresos)
                {
                    await _context.IngresoCertificados.AddAsync(ingresoCertificado);
                    await _context.SaveChangesAsync();
                    var inventario = new Inventario
                    {
                        IdAlmacen = solicitudDatos.IdAlmacenFV,
                        CantidadStock = ingresoCertificado.CantidadRecibida,
                        FolioInicial = ingresoCertificado.FolioInicial,
                        FolioFinal = ingresoCertificado.FolioFinal,
                        IdCatTipoCertificado = ingresoCertificado.IdCatTipoCertificado,
                    };
                    await _context.Inventarios.AddAsync(inventario);
                    await _context.SaveChangesAsync();
                    await _context.MovimientosInventarios.AddAsync(new MovimientosInventario
                    {
                        IdIngresoCertificado = ingresoCertificado.Id,
                        IdInventario = inventario.Id,
                    });
                }
                // Actualizamos el estatus de la solicitud
                var solicitudDb = await _context.SolicitudFVs.FirstOrDefaultAsync(x => x.Id == request.IdSolicitudFV);
                solicitudDb.IdCatEstatusSolicitud = EstatusSolicitud.Entregado;
                await _context.SaveChangesAsync();
            }

            scope.Complete();
            return new ResponseGeneric<long>(ingresoSolicitud.Id);
        }
        catch (ValidationException ex)
        {
            return new ResponseGeneric<long>(ex) { mensaje = ex.Message };
        }
        catch (Exception ex)
        {
            return new ResponseGeneric<long>(ex) { mensaje = "Ocurrió un error al guardar la información del ingreso de Formas Valoradas." };
        }
    }

    public async Task<ResponseGeneric<List<IngresoCertificado>>> GetIngresoCertificadoById(long Id)
    {
        try
        {
            var result = new List<IngresoCertificado>();
            var ingresoCert = _context.IngresoCertificados.Include(x => x.IdIngresoFVNavigation).Where(x => x.IdIngresoFV == Id);
            if (ingresoCert != null && ingresoCert.Count() > 0)
            {
                //string r = JsonConvert.SerializeObject(ingresoCert);
                //result = JsonConvert.DeserializeObject<List<IngresoCertificado>>(r);
                //result = ingresoCert.ToList();

                result = await ingresoCert.Select(x => new IngresoCertificado
                {
                    Id = x.Id,
                    IdIngresoFV = x.IdIngresoFV,
                    IdCatTipoCertificado = x.IdCatTipoCertificado,
                    CantidadRecibida = x.CantidadRecibida,
                    FolioInicial = x.FolioInicial,
                    FolioFinal = x.FolioFinal
                }).ToListAsync();

            }

            return new ResponseGeneric<List<IngresoCertificado>>(result);
        }
        catch (Exception ex)
        {
            return new ResponseGeneric<List<IngresoCertificado>>(ex);
        }
    }

    #region private methods
    private List<FoliosResponse> ValidarSolapamiento(List<CertificadoData> nuevosCertificados)
    {
        string query = "SELECT COUNT(*) FROM IngresoCertificado " +
            "WHERE IdCatTipoCertificado = @IdCatTipoCertificado " +
            "AND Id <> @IdIngresoCertificado AND ((@FolioInicial BETWEEN FolioInicial AND FolioFinal) OR (@FolioFinal BETWEEN FolioInicial AND FolioFinal))";
        var listFolios = new List<FoliosResponse>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            foreach (var nuevoCertificado in nuevosCertificados)
            {
                // Ejecuta la consulta para verificar la no interferencia
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdCatTipoCertificado", nuevoCertificado.IdCatTipoCertificado);
                    command.Parameters.AddWithValue("@FolioInicial", nuevoCertificado.FolioInicial);
                    command.Parameters.AddWithValue("@FolioFinal", nuevoCertificado.FolioFinal);
                    command.Parameters.AddWithValue("@IdIngresoCertificado", nuevoCertificado.IdIngresoCertificado);

                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        listFolios.Add(new FoliosResponse { Inicial = nuevoCertificado.FolioInicial, Final = nuevoCertificado.FolioFinal });
                    }

                }
            }
        }
        return listFolios;
    }
    #endregion
}
public class FoliosResponse
{
    public long Inicial { get; set; }
    public long Final { get; set; }
}
public interface IIngresoFormaValoradaNegocio
{
    public Task<ResponseGeneric<List<SolicitudFormaValoradaResponse>>> Consulta(SolicitudFormaValoradaListRequest request);
    public Task<ResponseGeneric<List<SolicitudFormaValoradaResponse>>> GetById(long Id);
    Task<ResponseGeneric<long>> Registro(IngresoFormaValoradaRequest request);

    public Task<ResponseGeneric<List<IngresoCertificado>>> GetIngresoCertificadoById(long Id);
}

