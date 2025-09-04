using Smadot.Models.DataBase;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Smadot.Models.Entities.Equipo.Request;
using Smadot.Models.Entities.Equipo.Response;
using Smadot.Models.Entities.Linea.Request;
using Smadot.Models.Entities.Linea.Response;
using Microsoft.Extensions.Configuration;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using System.Runtime.Intrinsics.Arm;
using Smadot.Models.Dicts;
using Newtonsoft.Json;
using System.Globalization;
using Smadot.Models.Entities.Exento.Response;
using System.ComponentModel.DataAnnotations;
using Smadot.Utilities.Modelos.Interfaz;
using MathNet.Numerics;
using Smadot.Models.Entities.Alertas.Response;
using Org.BouncyCastle.Crypto;
using Smadot.Models.Entities.Generic.Response;
using System.Net.NetworkInformation;
using Microsoft.Identity.Client;
using static Microsoft.IO.RecyclableMemoryStreamManager;
using Namespace;
using Smadot.Models.GenericProcess;

namespace Smadot.AdministracionStock.Negocio.Negocio
{
    public class EquipoNegocio : IEquipoNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        private readonly SmadsotGenericInserts _genericInserts;

        public EquipoNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration, SmadsotGenericInserts genericInserts)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _genericInserts = genericInserts;
        }

        public async Task<ResponseGeneric<List<vEquipoResponse>>> Consulta(EquipoRequest request)
        {
            try
            {
                var equipos = _context.vEquipos.Where(x => x.IdVerificentro == _userResolver.GetUser().IdVerificentro).AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    var date = new DateTime();
                    var dateDef = new DateTime();
                    DateTime.TryParseExact(request.Busqueda, "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out date);
                    equipos = equipos.Where(x => x.Id.ToString().Contains(request.Busqueda.ToLower()) || (date != dateDef && x.FechaRegistro == date) || x.NombreLinea.ToLower().Contains(request.Busqueda.ToLower()) || x.NombreEquipo.ToLower().Contains(request.Busqueda.ToLower()) || x.NumeroSerie.ToLower().Contains(request.Busqueda.ToLower()) || x.NombreUsuarioRegistro.ToLower().Contains(request.Busqueda.ToLower()));
                }
                if (request.IdLinea.HasValue)
                {
                    equipos = equipos.Where(x => x.IdLinea == request.IdLinea);
                }
                var tot = equipos.Count();
                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    equipos = equipos.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    equipos = equipos.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }
                DateTime now = DateTime.Now;

                var result = await equipos.Select(x => new vEquipoResponse
                {
                    Id = (long)x.Id,
                    NumeroSerie = x.NumeroSerie,
                    IdCatEstatusEquipo = (int)x.IdCatEstatusEquipo,
                    NombreEquipo = x.NombreEquipo,
                    IdUserRegistro = (long)x.IdUserRegistro,
                    Nombre = x.NombreUsuarioRegistro,
                    FechaRegistro = (DateTime)x.FechaRegistro,
                    NombreLinea = (x.NombreLinea) == null ? "" : (x.NombreLinea),
                    IdLinea = x.IdLinea,
                    Estatus = x.Estatus,
                    FechaProximaCalibracion = x.FechaProximaCalibracion,
                    FechaModificacion = x.FechaModificacion,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<vEquipoResponse>>(result);
            }
            catch (Exception ex)
            {
                await _genericInserts.SaveLog(ex, DictTipoLog.ExcepcionConsultaEquipo);
                return new ResponseGeneric<List<vEquipoResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<vEquipoResponse>> Detalle(long id)
        {
            try
            {
                var equipo = await _context.vEquipos.FirstOrDefaultAsync(x => x.Id == id);

                if (equipo is null)
                    return new ResponseGeneric<vEquipoResponse>();

                var result = new vEquipoResponse
                {
                    Id = (long)equipo.Id,
                    NombreEquipo = equipo.NombreEquipo,
                    NumeroSerie = equipo.NumeroSerie,
                    IdCatEstatusEquipo = (int)equipo.IdCatEstatusEquipo,
                    NombreEstatusEquipo = equipo.NombreEstatusEquipo,
                    IdUserRegistro = (long)equipo.IdUserRegistro,
                    Nombre = equipo.NombreUsuarioRegistro,
                    FechaRegistro = (DateTime)equipo.FechaRegistro,
                    FechaModificacion = equipo.FechaModificacion,
                    IdUserReviso = equipo.IdUserReviso,
                    Comentarios = equipo.Comentarios,
                    IdLinea = equipo.IdLinea,
                    IdCatTipoEquipo = equipo.IdCatTipoEquipo,
                    NombreLinea = equipo.NombreLinea,
                    NombreVerificentro = equipo.NombreVerificentro,
                    IdVerificentro = (long)equipo.IdVerificentro,
                    UrlEspecificacionTecnica = equipo.UrlEspecificacionTecnica,
                    UrlFactura = equipo.UrlFactura,
                    UrlManualUsuario = equipo.UrlManualUsuario,
                    UrlRecomendacionServicio = equipo.UrlRecomendacionServicio

                };

                return new ResponseGeneric<vEquipoResponse>(result);
            }
            catch (Exception ex)
            {
                await _genericInserts.SaveLog(ex, DictTipoLog.ExcepcionDetalleEquipo);
                return new ResponseGeneric<vEquipoResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> Registro(EquipoResponse request)
        {
            try
            {
                var equipo = new Equipo();
                var linea = new Linea();
                var tipoEquipo = await _context.CatTipoEquipos.FirstOrDefaultAsync(x => x.Id == request.IdCatTipoEquipo) ?? throw new ValidationException("No sé encontro el tipo de equipo que se intenta registrar.");
                var existeEquipoNombre = await _context.vEquipoOrdenServicios.AnyAsync(x => x.IdCatTipoEquipo == request.IdCatTipoEquipo && x.IdVerificentro == _userResolver.GetUser().IdVerificentro && x.IdLinea == request.IdLinea && x.Id != request.Id);
                if (existeEquipoNombre)
                    throw new ValidationException("Ya se encuentra un equipo registrado del mismo tipo para la línea seleccionada.");

                var existeEquipoNumSerie = await _context.vEquipoOrdenServicios.AnyAsync(x => x.NumeroSerie == request.NumeroSerie && x.IdVerificentro == _userResolver.GetUser().IdUser);
                if (existeEquipoNumSerie)
                    throw new ValidationException("Ya se encuentra registrado el número de serie para otro equipo en el Verificentro.");

                request.Nombre = tipoEquipo.Nombre;
                if (request.Id > 0)
                {
                    equipo = _context.Equipos.FirstOrDefault(x => x.Id == request.Id);
                    if (equipo != null)
                    {
                        equipo.Nombre = request.Nombre;
                        equipo.NumeroSerie = request.NumeroSerie;
                        equipo.IdLinea = request.IdLinea;
                        equipo.IdCatTipoEquipo = request.IdCatTipoEquipo;
                        equipo.FechaModificacion = DateTime.Now;
                        if (request.Files is null)
                            request.Files = new List<EquipoFiles>();
                        _context.Update(equipo);
                        _context.SaveChanges();

                        var x = 0;
                        foreach (var file in request.Files)
                        {
                            var url = await _blobStorage.UploadFileAsync(new byte[0], "Equipo/" + equipo.Id + "/" + file.Nombre, file.Base64);
                            if (!string.IsNullOrEmpty(url))
                            {
                                switch (x)
                                {
                                    case 0:
                                        equipo.UrlFactura = url; break;
                                    case 1:
                                        equipo.UrlManualUsuario = url; break;
                                    case 2:
                                        equipo.UrlEspecificacionTecnica = url; break;
                                    case 3:
                                        equipo.UrlRecomendacionServicio = url; break;
                                }
                            }
                            x++;
                        }
                        _context.SaveChanges();

                        return new ResponseGeneric<long>();
                    }
                    return new ResponseGeneric<long>();

                }
                else
                {
                    request.NumeroSerie = request.NumeroSerie.Trim();

                    equipo = new Equipo
                    {
                        Nombre = request.Nombre,
                        NumeroSerie = request.NumeroSerie,
                        IdCatEstatusEquipo = EstatusEquipo.Inactivo,
                        IdUserRegistro = _userResolver.GetUser().IdUser,
                        FechaRegistro = DateTime.Now,
                        Comentarios = request.Comentarios,
                        IdLinea = request.IdLinea,
                        IdCatTipoEquipo = request.IdCatTipoEquipo
                    };

                    if (equipo.IdLinea == null || equipo.IdLinea == 0)
                    {
                        equipo.IdVerificentro = (long)_userResolver.GetUser().IdVerificentro;
                        equipo.IdLinea = null;
                    }
                    else
                    {
                        linea = _context.Lineas.FirstOrDefault(x => x.Id == equipo.IdLinea);

                        equipo.IdVerificentro = linea.IdVerificentro;
                    }
                    if (request.Files is null)
                        request.Files = new List<EquipoFiles>();
                    _context.Equipos.Add(equipo);
                }

                var result = await _context.SaveChangesAsync() > 0;

                var i = 0;
                foreach (var file in request.Files)
                {
                    var url = await _blobStorage.UploadFileAsync(new byte[0], "Equipo/" + equipo.Id + "/" + file.Nombre, file.Base64);
                    if (!string.IsNullOrEmpty(url))
                    {
                        switch (i)
                        {
                            case 0:
                                equipo.UrlFactura = url; break;
                            case 1:
                                equipo.UrlManualUsuario = url; break;
                            case 2:
                                equipo.UrlEspecificacionTecnica = url; break;
                            case 3:
                                equipo.UrlRecomendacionServicio = url; break;
                        }
                    }
                    i++;
                }
                result = await _context.SaveChangesAsync() > 0;

                return result ? new ResponseGeneric<long>(equipo.Id) : new ResponseGeneric<long>();
            }
            catch (ValidationException ex)
            {

                return new ResponseGeneric<long>(ex) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                await _genericInserts.SaveLog(JsonConvert.SerializeObject(ex), DictTipoLog.ExcepcionRegistroEquipo);
                return new ResponseGeneric<long>(ex) { mensaje = "Ocurrio un error al querer registrar el equipo" };
            }
        }

        public async Task<ResponseGeneric<bool>> Validar(AuxValidarResponse request)
        {
            try
            {
                if (request.Id > 0)
                {
                    var equipo = await _context.Equipos.FirstOrDefaultAsync(x => x.Id == request.Id);
                    if (equipo != null && (equipo.IdCatEstatusEquipo == EstatusEquipo.Inactivo || equipo.IdCatEstatusEquipo == 2))
                    {
                        equipo.IdCatEstatusEquipo = EstatusEquipo.Inactivo;
                        _context.SaveChanges();

                        return new ResponseGeneric<bool>(true);
                    }
                    else
                        throw new Exception("Registro validado");
                }
                else
                {
                    return new ResponseGeneric<bool>(false);
                }

            }
            catch (Exception ex)
            {
                await _genericInserts.SaveLog(ex, DictTipoLog.ExcepcionValidarEquipo);
                return new ResponseGeneric<bool>("Ocurrió un error al validar el registro");
            }
        }

        public async Task<ResponseGeneric<List<LineaResponse>>> ConsultaLinea(LineaRequest request)
        {
            try
            {
                var lineas = _context.Lineas.Where(x => x.IdVerificentro == _userResolver.GetUser().IdVerificentro).AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    var date = new DateTime();
                    var dateDef = new DateTime();
                    DateTime.TryParseExact(request.Busqueda, "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out date);
                    lineas = lineas.Where(x => x.Id.ToString().Contains(request.Busqueda.ToLower()) || (date != dateDef && x.FechaRegistro == date) || x.Nombre.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var tot = lineas.Count();
                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    lineas = lineas.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    lineas = lineas.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }
                DateTime now = DateTime.Now;

                var result = await lineas.Select(x => new LineaResponse
                {
                    Id = x.Id,
                    IdVerificentro = x.IdVerificentro,
                    Nombre = x.Nombre,
                    IdCatEstatusLinea = x.IdCatEstatusLinea,
                    IdUserRegistro = x.IdUserRegistro,
                    FechaRegistro = x.FechaRegistro,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<LineaResponse>>(result);
            }
            catch (Exception ex)
            {
                await _genericInserts.SaveLog(ex, DictTipoLog.ExcepcionConsultaLineaEquipo);
                return new ResponseGeneric<List<LineaResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> ActualizaEstatus(AuxEstatusResponse request)
        {
            try
            {
                if (request.Id > 0)
                {
                    var equipo = await _context.Equipos.FirstOrDefaultAsync(x => x.Id == request.Id);

                    if (equipo != null)
                    {
                        if (request.Comentarios == null || request.Comentarios == "")
                        {
                            equipo.IdCatEstatusEquipo = 2;
                        }
                        else
                        {
                            equipo.IdCatEstatusEquipo = 4;
                            equipo.Comentarios = request.Comentarios;
                            equipo.IdUserReviso = _userResolver.GetUser().IdUser;
                        }
                        //_context.Update();
                        _context.SaveChanges();
                        return new ResponseGeneric<bool>(true);
                    }
                    else
                        throw new Exception("No se encontró el registro");
                }
                else
                {
                    return new ResponseGeneric<bool>(false);
                }
            }
            catch (Exception ex)
            {
                await _genericInserts.SaveLog(ex, DictTipoLog.ExcepcionEstatusEquipo);
                return new ResponseGeneric<bool>(ex);
            }
        }
    }
    public interface IEquipoNegocio
    {
        public Task<ResponseGeneric<List<vEquipoResponse>>> Consulta(EquipoRequest request);
        public Task<ResponseGeneric<vEquipoResponse>> Detalle(long Id);
        public Task<ResponseGeneric<long>> Registro(EquipoResponse request);
        public Task<ResponseGeneric<bool>> Validar(AuxValidarResponse request);
        public Task<ResponseGeneric<List<LineaResponse>>> ConsultaLinea(LineaRequest request);
        public Task<ResponseGeneric<bool>> ActualizaEstatus(AuxEstatusResponse request);
    }
}
