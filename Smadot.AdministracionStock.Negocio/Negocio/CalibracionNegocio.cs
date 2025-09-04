using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Calibracion.Extensions;
using Smadot.Models.Entities.Calibracion.Request;
using Smadot.Models.Entities.Calibracion.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Dynamic.Core;

namespace Smadot.AdministracionStock.Negocio.Negocio
{
    public class CalibracionNegocio : ICalibracionNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;

        public CalibracionNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public async Task<ResponseGeneric<List<vCalibracionResponse>>> Consulta(CalibracionListRequest request)
        {
            try
            {
                var calibraciones = _context.vCalibracions.Where(x => x.IdVerificentro == _userResolver.GetUser().IdVerificentro).AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    var date = new DateTime();
                    var dateDef = new DateTime();
                    DateTime.TryParseExact(request.Busqueda, "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out date);
                    calibraciones = calibraciones.Where(x => x.Id.ToString().Contains(request.Busqueda.ToLower()) || (date != dateDef && x.FechaCalibracion == date) || x.Nota.ToLower().Contains(request.Busqueda.ToLower()) || x.NombreUserRegistro.ToLower().Contains(request.Busqueda.ToLower()) || x.NombreUserValido.ToLower().Contains(request.Busqueda.ToLower()) || (date != dateDef && x.FechaProgramada == date) || (date != dateDef && x.FechaProximaCalibracion == date));
                }

                var tot = calibraciones.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    calibraciones = calibraciones.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    calibraciones = calibraciones.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }
                DateTime now = DateTime.Now;

                var result = await calibraciones.Select(x => new vCalibracionResponse
                {
                    Id = x.Id,
                    FechaCalibracion = x.FechaCalibracion,
                    Consecutivo = x.Consecutivo,
                    FechaProgramada = x.FechaProgramada,
                    FechaProximaCalibracion = x.FechaProximaCalibracion,
                    // Laboratorio = x.Laboratorio,
                    NombreEstatusCalibracion = x.NombreEstatusCalibracion,
                    IdUserRegistro = x.IdUserRegistro,
                    NombreUserRegistro = x.NombreUserRegistro,
                    IdUserValido = x.IdUserValido,
                    NombreUserValido = x.NombreUserValido,
                    Nota = x.Nota,
                    NombreLinea = x.NombreLinea,
                    NombreEquipo = x.NombreEquipo,
                    IdCatEstatusEquipo = x.IdCatEstatusEquipo,
                    FechaRegistro = x.FechaRegistro,
                    IdCatEstatusCalibracion = x.IdCatEstatusCalibracion,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<vCalibracionResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<vCalibracionResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<vCalibracionResponse>> Detalle(long Id)
        {
            try
            {
                var calibracion = _context.vCalibracions.FirstOrDefault(x => x.Id == Id);

                var result = new vCalibracionResponse
                {
                    Id = calibracion.Id,
                    IdEquipoTipoCalibracion = calibracion.IdEquipoTipoCalibracion,
                    IdEquipo = calibracion.IdEquipo,
                    NombreEquipo = calibracion.NombreEquipo,
                    Consecutivo = calibracion.Consecutivo,
                    FechaCalibracion = calibracion.FechaCalibracion,
                    FechaProgramada = calibracion.FechaProgramada,
                    FechaProximaCalibracion = calibracion.FechaProximaCalibracion,
                    IdUserRegistro = calibracion.IdUserRegistro,
                    NombreUserRegistro = calibracion.NombreUserRegistro,
                    FechaRegistro = calibracion.FechaRegistro,
                    IdUserValido = calibracion.IdUserValido,
                    NombreUserValido = calibracion.NombreUserValido,
                    Nota = calibracion.Nota,
                    IdCatEstatusCalibracion = calibracion.IdCatEstatusCalibracion,
                    NombreEstatusCalibracion = calibracion.NombreEstatusCalibracion,
                    // IdTecnicoLaboratorio = calibracion.IdTecnicoLaboratorio,
                    // NombreTecnicoLaboratorio = calibracion.NombreTecnicoLaboratorio,
                    NumeroCertificado = calibracion.NumeroCertificado,
                    FechaEmisionCertificad = calibracion.FechaEmisionCertificad,
                    UrlResultado = calibracion.UrlResultado,
                    // Laboratorio = calibracion.Laboratorio,
                    NombreLinea = calibracion.NombreLinea,
                    IdCatEstatusEquipo = calibracion.IdCatEstatusEquipo,
                    IdLinea = calibracion.IdLinea
                };

                return new ResponseGeneric<vCalibracionResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<vCalibracionResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Registro(CalibracionRequest request)
        {
            try
            {
                var equipoTipoCalibracion = _context.EquipoTipoCalibracions.Include(eq => eq.IdEquipoNavigation)
                                                                           .Where(x => x.IdEquipo == request.IdEquipo)
                                                                           .Where(x => x.IdUserValido.HasValue)
                                                                           .OrderBy(x => x.Id)
                                                                           .LastOrDefault();

                if (equipoTipoCalibracion == null)
                    return new ResponseGeneric<bool>() { mensaje = "No se ha programado el tipo de programación para este equipo.", Response = false };

                var calibracionPrevia = _context.Calibracions.Where(x => x.IdEquipoTipoCalibracion == equipoTipoCalibracion.Id)
                                                             .OrderBy(x => x.Id)
                                                             .FirstOrDefault();

                DateTime fechaProgramada = calibracionPrevia?.Id > 0 ? calibracionPrevia.FechaProximaCalibracion : equipoTipoCalibracion.PrimeraFechaCalibracion;
                DateTime fechaProximaCalibracion = request.FechaHora.ObtenerProximaFechaCalibracion(equipoTipoCalibracion.IdCatTipoCalibracion);

                int consecutivo = (int)(_context.Calibracions.OrderBy(x => x.Id).LastOrDefault()?.Consecutivo ?? 0) + 1;
                var calibracion = new Calibracion()
                {
                    IdEquipoTipoCalibracion = equipoTipoCalibracion.Id,
                    Consecutivo = consecutivo,
                    FechaCalibracion = request.FechaHora,
                    FechaProgramada = fechaProgramada,
                    FechaProximaCalibracion = fechaProximaCalibracion,
                    IdUserRegistro = _userResolver.GetUser().IdUser,
                    FechaRegistro = DateTime.Now,
                    IdUserValido = equipoTipoCalibracion.IdUserValido,
                    Nota = request.Nota.ToUpper(),
                    //IdCatEstatusCalibracion = CalibracionEstatus.SolicitaModificar,
                    IdCatEstatusCalibracion = CalibracionEstatus.DocumentacionSinRevisar,
                    // IdTecnicoLaboratorio = request.IdTecnico,
                    NumeroCertificado = request.NumeroFolio,
                    FechaEmisionCertificad = request.FechaEmisionCertificado
                };
                _context.Calibracions.Add(calibracion);
                await _context.SaveChangesAsync();
                var equipo = _context.Equipos.FirstOrDefault(x => x.Id == request.IdEquipo);
                if (equipo != null && equipo?.IdCatEstatusEquipo != EstatusEquipo.Activo)
                    equipo.IdCatEstatusEquipo = EstatusEquipo.Activo;
                foreach (var file in request.Files)
                {
                    var url = await _blobStorage.UploadFileAsync(new byte[0], "Calibracion/" + calibracion.Id + "/" + file.Nombre, file.Base64);
                    if (!string.IsNullOrEmpty(url))
                    {
                        calibracion.UrlResultado = url;
                        break;
                    }
                }

                Alertum alerta = new()
                {
                    TableName = DictAlertas.Calibracion,
                    TableId = calibracion.Id,
                    IdVerificentro = _userResolver.GetUser().IdVerificentro ?? 1,
                    Data = JsonConvert.SerializeObject(calibracion, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                        NullValueHandling = NullValueHandling.Ignore
                    }),
                    IdUser = _userResolver.GetUser().IdUser,
                    MovimientoInicial = string.Format(MovimientosDicts.DictMovimientoCalibracion[CalibracionEstatus.DocumentacionSinRevisar], equipoTipoCalibracion.IdEquipoNavigation.Nombre, calibracion.Consecutivo, calibracion.FechaCalibracion),
                    Fecha = DateTime.Now,
                    //IdEstatusInicial = CalibracionEstatus.SolicitaModificar,
                    IdEstatusInicial = CalibracionEstatus.DocumentacionSinRevisar,
                    MovimientoFinal = null,
                    FechaModificacion = null,
                    Leido = false,
                    Procesada = false
                };

                _context.Alerta.Add(alerta);

                await _context.SaveChangesAsync();
                return new ResponseGeneric<bool>(true);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> SolicitarModificacion(long Id)
        {
            try
            {
                var calibracion = await _context.Calibracions.Include(x => x.IdEquipoTipoCalibracionNavigation).ThenInclude(x => x.IdEquipoNavigation).FirstOrDefaultAsync(x => x.Id == Id);
                calibracion.IdCatEstatusCalibracion = CalibracionEstatus.SolicitaModificar;
                Alertum alerta = new()
                {
                    TableName = DictAlertas.Calibracion,
                    TableId = calibracion.Id,
                    IdVerificentro = _userResolver.GetUser().IdVerificentro ?? 1,
                    Data = JsonConvert.SerializeObject(calibracion, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                        NullValueHandling = NullValueHandling.Ignore
                    }),
                    IdUser = _userResolver.GetUser().IdUser,
                    MovimientoInicial = string.Format(MovimientosDicts.DictMovimientoCalibracion[CalibracionEstatus.SolicitaModificar], calibracion.IdEquipoTipoCalibracionNavigation.IdEquipoNavigation.Nombre, calibracion.Consecutivo, calibracion.FechaCalibracion),
                    Fecha = DateTime.Now,
                    IdEstatusInicial = CalibracionEstatus.SolicitaModificar,
                    MovimientoFinal = null,
                    FechaModificacion = null,
                    Leido = false,
                    Procesada = false
                };

                _context.Alerta.Add(alerta);
                int response = await _context.SaveChangesAsync();
                return new ResponseGeneric<bool>(response > 0);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Validar(long Id)
        {
            try
            {
                var calibracion = await _context.Calibracions.Include(x => x.IdEquipoTipoCalibracionNavigation).ThenInclude(x => x.IdEquipoNavigation).FirstOrDefaultAsync(x => x.Id == Id);
                calibracion.IdCatEstatusCalibracion = CalibracionEstatus.DocumentacionValidada;

                var alerta = await _context.Alerta.Where(d => d.TableId == Id)
                                                  .Where(d => d.TableName == "Calibracion")
                                                  .FirstOrDefaultAsync();
                if (alerta != null)
                {

                    alerta.IdEstatusFinal = CalibracionEstatus.DocumentacionValidada;
                    alerta.MovimientoFinal = string.Format(MovimientosDicts.DictMovimientoCalibracion[CalibracionEstatus.DocumentacionValidada], calibracion.IdEquipoTipoCalibracionNavigation.IdEquipoNavigation.Nombre);
                    alerta.Data = JsonConvert.SerializeObject(calibracion, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }

                int response = await _context.SaveChangesAsync();
                return new ResponseGeneric<bool>(response > 0);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> RechazarDocumentacion(long Id)
        {
            try
            {
                var calibracion = await _context.Calibracions.Include(x => x.IdEquipoTipoCalibracionNavigation).ThenInclude(x => x.IdEquipoNavigation).FirstOrDefaultAsync(x => x.Id == Id);
                calibracion.IdCatEstatusCalibracion = CalibracionEstatus.DocumentacionRechazada;

                var alerta = await _context.Alerta.Where(d => d.TableId == Id)
                                                  .Where(d => d.TableName == "Calibracion")
                                                  .FirstOrDefaultAsync();
                if (alerta != null)
                {

                    alerta.IdEstatusFinal = CalibracionEstatus.DocumentacionRechazada;
                    alerta.MovimientoFinal = string.Format(MovimientosDicts.DictMovimientoCalibracion[CalibracionEstatus.DocumentacionRechazada], calibracion.IdEquipoTipoCalibracionNavigation.IdEquipoNavigation.Nombre);
                    alerta.Data = JsonConvert.SerializeObject(calibracion, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }

                int response = await _context.SaveChangesAsync();
                return new ResponseGeneric<bool>(response > 0);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Autorizar(long Id)
        {
            try
            {
                var calibracion = await _context.Calibracions.Include(x => x.IdEquipoTipoCalibracionNavigation).ThenInclude(x => x.IdEquipoNavigation).FirstOrDefaultAsync(x => x.Id == Id);
                calibracion.IdCatEstatusCalibracion = CalibracionEstatus.PermiteModificar;

                var alerta = await _context.Alerta.Where(d => d.TableId == Id)
                                                  .Where(d => d.TableName == "Calibracion")
                                                  .FirstOrDefaultAsync();
                if (alerta != null)
                {

                    alerta.IdEstatusFinal = CalibracionEstatus.PermiteModificar;
                    alerta.MovimientoFinal = string.Format(MovimientosDicts.DictMovimientoCalibracion[CalibracionEstatus.PermiteModificar], calibracion.IdEquipoTipoCalibracionNavigation.IdEquipoNavigation.Nombre, calibracion.Consecutivo, calibracion.FechaCalibracion);
                    alerta.Data = JsonConvert.SerializeObject(calibracion, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }

                int response = await _context.SaveChangesAsync();
                return new ResponseGeneric<bool>(response > 0);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> RechazarSolicitar(long Id)
        {
            try
            {
                var calibracion = await _context.Calibracions.Include(x => x.IdEquipoTipoCalibracionNavigation).ThenInclude(x => x.IdEquipoNavigation).FirstOrDefaultAsync(x => x.Id == Id);
                calibracion.IdCatEstatusCalibracion = CalibracionEstatus.RechazaModificar;

                var alerta = await _context.Alerta.Where(d => d.TableId == Id)
                                                  .Where(d => d.TableName == "Calibracion")
                                                  .FirstOrDefaultAsync();
                if (alerta != null)
                {

                    alerta.IdEstatusFinal = CalibracionEstatus.RechazaModificar;
                    alerta.MovimientoFinal = string.Format(MovimientosDicts.DictMovimientoCalibracion[CalibracionEstatus.RechazaModificar], calibracion.IdEquipoTipoCalibracionNavigation.IdEquipoNavigation.Nombre, calibracion.Consecutivo, calibracion.FechaCalibracion);
                    alerta.Data = JsonConvert.SerializeObject(calibracion, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }

                int response = await _context.SaveChangesAsync();
                return new ResponseGeneric<bool>(response > 0);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Editar(CalibracionRequest request)
        {
            try
            {
                var equipoTipoCalibracion = _context.EquipoTipoCalibracions.Where(x => x.IdEquipo == request.IdEquipo)
                                                                           .Where(x => x.IdUserValido.HasValue)
                                                                           .OrderBy(x => x.Id)
                                                                           .LastOrDefault();

                if (equipoTipoCalibracion == null)
                    return new ResponseGeneric<bool>("No existe el tipo de calibración de este equipo.");

                var calibracionPrevia = _context.Calibracions.Where(x => x.IdEquipoTipoCalibracion == equipoTipoCalibracion.Id)
                                                             .OrderBy(x => x.Id)
                                                             .FirstOrDefault();

                DateTime fechaProgramada = calibracionPrevia?.Id > 0 ? calibracionPrevia.FechaProximaCalibracion : equipoTipoCalibracion.PrimeraFechaCalibracion;
                DateTime fechaProximaCalibracion = request.FechaHora.ObtenerProximaFechaCalibracion(equipoTipoCalibracion.IdCatTipoCalibracion);

                var calibracion = _context.Calibracions.FirstOrDefault(x => x.Id.Equals(request.Id));
                if (calibracion is null)
                    return new ResponseGeneric<bool>("No existe la calibración de este equipo.");
                calibracion.IdEquipoTipoCalibracion = equipoTipoCalibracion.Id;
                calibracion.FechaCalibracion = request.FechaHora;
                calibracion.FechaProgramada = fechaProgramada;
                calibracion.FechaProximaCalibracion = fechaProximaCalibracion;
                calibracion.IdUserRegistro = _userResolver.GetUser().IdUser;
                calibracion.FechaRegistro = DateTime.Now;
                calibracion.IdUserValido = equipoTipoCalibracion.IdUserValido;
                calibracion.Nota = request.Nota.ToUpper();
                calibracion.IdCatEstatusCalibracion = CalibracionEstatus.DocumentacionSinRevisar;
                // calibracion.IdTecnicoLaboratorio = request.IdTecnico;
                calibracion.NumeroCertificado = request.NumeroFolio;
                calibracion.FechaEmisionCertificad = request.FechaEmisionCertificado;

                foreach (var file in request.Files)
                {
                    var url = await _blobStorage.UploadFileAsync(new byte[0], "Calibracion/" + calibracion.Id + "/" + file.Nombre, file.Base64);
                    if (!string.IsNullOrEmpty(url))
                    {
                        calibracion.UrlResultado = url;
                        break;
                    }
                }

                var alerta = await _context.Alerta.Where(d => d.TableId == calibracion.Id)
                                                  .Where(d => d.TableName == "Calibracion")
                                                  .FirstOrDefaultAsync();

                alerta.IdEstatusFinal = CalibracionEstatus.SolicitaModificar;
                alerta.Data = JsonConvert.SerializeObject(calibracion, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    NullValueHandling = NullValueHandling.Ignore
                });

                int response = await _context.SaveChangesAsync();

                return new ResponseGeneric<bool>(response > 0);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }
    }

    public interface ICalibracionNegocio
    {
        public Task<ResponseGeneric<List<vCalibracionResponse>>> Consulta(CalibracionListRequest request);

        public Task<ResponseGeneric<vCalibracionResponse>> Detalle(long Id);

        public Task<ResponseGeneric<bool>> Registro(CalibracionRequest request);

        public Task<ResponseGeneric<bool>> Editar(CalibracionRequest request);

        public Task<ResponseGeneric<bool>> SolicitarModificacion(long Id);

        public Task<ResponseGeneric<bool>> Validar(long Id);

        public Task<ResponseGeneric<bool>> RechazarDocumentacion(long Id);

        public Task<ResponseGeneric<bool>> Autorizar(long Id);

        public Task<ResponseGeneric<bool>> RechazarSolicitar(long Id);
    }
}
