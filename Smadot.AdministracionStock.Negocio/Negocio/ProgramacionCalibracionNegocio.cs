using Smadot.Models.DataBase;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Smadot.Models.Entities.ProgramacionCalibracion.Request;
using Smadot.Models.Entities.ProgramacionCalibracion.Response;
using Smadot.Models.Entities.EquipoTipoCalibracion.Request;
using Smadot.Models.Entities.EquipoTipoCalibracion.Response;
using Microsoft.Extensions.Configuration;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using System.Runtime.Intrinsics.Arm;
using Smadot.Models.Dicts;
using Newtonsoft.Json;
using System.Globalization;
using Smadot.Models.Entities.Equipo.Response;
using iTextSharp.text.pdf;

namespace Smadot.AdministracionStock.Negocio.Negocio
{
    public class ProgramacionCalibracionNegocio : IProgramacionCalibracionNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;

        public ProgramacionCalibracionNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public async Task<ResponseGeneric<List<vProgramacionCalibracionResponse>>> Consulta(ProgramacionCalibracionRequest request)
        {
            try
            {
                var calibracion = _context.vProgramacionCalibracions.Where(x => x.IdEquipo == request.IdEquipo).AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    var date = new DateTime();
                    var dateDef = new DateTime();
                    DateTime.TryParseExact(request.Busqueda, "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out date);
                    calibracion = calibracion.Where(x => x.Id.ToString().Contains(request.Busqueda.ToLower()) || (date != dateDef && x.FechaRegistro == date) || x.NombreTipoCalibracion.ToLower().Contains(request.Busqueda.ToLower()) || x.NombreUser.ToLower().Contains(request.Busqueda.ToLower()) || (date != dateDef && x.PrimeraFechaCalibracion == date));
                }

                var tot = calibracion.Count();
                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    calibracion = calibracion.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    calibracion = calibracion.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                DateTime now = DateTime.Now;

                var result = await calibracion.Select(x => new vProgramacionCalibracionResponse
                {
                    Id = (long)x.Id,
                    IdCatTipoCalibracion = (int)x.IdCatTipoCalibracion,
                    NombreTipoCalibracion = x.NombreTipoCalibracion,
                    PrimeraFechaCalibracion = (DateTime)x.PrimeraFechaCalibracion,
                    IdUserRegistro = (long)x.IdUserRegistro,
                    NombreUser = x.NombreUser,
                    FechaRegistro = (DateTime)x.FechaRegistro,
                    IdUserValido = x.IdUserValido,
                    NombreValido = x.NombreValido,
                    Nota = x.Nota,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<vProgramacionCalibracionResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<vProgramacionCalibracionResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<vProgramacionCalibracionResponse>> Detalle(long id)
        {
            try
            {
                var calibracion = await _context.vProgramacionCalibracions.FirstOrDefaultAsync(x => x.Id == id);
                if (calibracion is null)
                    return new ResponseGeneric<vProgramacionCalibracionResponse>();

                var result = new vProgramacionCalibracionResponse
                {
                    Id = (long)calibracion.Id,
                    IdEquipo = (long)calibracion.IdEquipo,
                    NombreEquipo = calibracion.NombreEquipo,
                    IdCatTipoCalibracion = (int)calibracion.IdCatTipoCalibracion,
                    NombreTipoCalibracion = calibracion.NombreTipoCalibracion,
                    PrimeraFechaCalibracion = (DateTime)calibracion.PrimeraFechaCalibracion,
                    Nota = calibracion.Nota,
                    IdUserRegistro = (long)calibracion.IdUserRegistro,
                    NombreUser = calibracion.NombreUser,
                    FechaRegistro = (DateTime)calibracion.FechaRegistro,
                    IdUserValido = calibracion.IdUserValido,
                    NombreValido = calibracion.NombreValido,
                    UrlDocumento1 = calibracion.UrlDocumento1
                };
                return new ResponseGeneric<vProgramacionCalibracionResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<vProgramacionCalibracionResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Registro(EquipoTipoCalibracionResponse request)
        {
            try
            {
                var equipo = new EquipoTipoCalibracion();

                if (request.Id > 0)
                {
                    equipo = _context.EquipoTipoCalibracions.FirstOrDefault(x => x.Id == request.Id);
                    if (equipo == null)
                        throw new Exception("No se encontró el registro");
                }
                else
                {

                    equipo = new EquipoTipoCalibracion
                    {
                        IdEquipo = request.IdEquipo,
                        IdCatTipoCalibracion = request.IdCatTipoCalibracion,
                        PrimeraFechaCalibracion = request.PrimeraFechaCalibracion,
                        Nota = request.Nota,
                        IdUserRegistro = _userResolver.GetUser().IdUser,
                        FechaRegistro = request.FechaRegistro
                        //IdUserValido = _userResolver.GetUser().IdUser
                    };
                    _context.EquipoTipoCalibracions.Add(equipo);
                    await _context.SaveChangesAsync();

                    //Registrar UrlDocument
                    foreach (var file in request.Files)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "ProgramacionCalibracion/" + equipo.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            equipo.UrlDocumento1 = url; break;
                        }
                    }
                    await _context.SaveChangesAsync();

                    int estatusProgramacionCalibracion = ObtenerEstatus(equipo.Nota, equipo.IdUserValido);
                    var tipoCalibracion = equipo?.IdCatTipoCalibracion ?? 1;
                    // obtener el nombre del equipo para la alerta
                    var datosEquipo = _context.Equipos.FirstOrDefault(x => x.Id == request.IdEquipo);
                    Alertum alerta = new()
                    {
                        TableName = DictAlertas.EquipoTipoCalibracion,
                        TableId = equipo.Id,
                        IdVerificentro = _userResolver.GetUser().IdVerificentro ?? 1,
                        Data = JsonConvert.SerializeObject(equipo, new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            PreserveReferencesHandling = PreserveReferencesHandling.None,
                            NullValueHandling = NullValueHandling.Ignore
                        }),
                        IdUser = _userResolver.GetUser().IdUser,
                        IdEstatusInicial = ProgramacionCalibracionEstatus.Pendiente,
                        MovimientoInicial = string.Format(MovimientosDicts.DictMovimientoProgramacionCalibracion[ProgramacionCalibracionEstatus.Pendiente], datosEquipo?.Nombre, CatTipoCalibracionDic.Nombres[tipoCalibracion], equipo.FechaRegistro.ToString("g")),
                        //MovimientoInicial = string.Format(MovimientosDicts.DictMovimiento[MovimientosDicts.DictProgramacionCalibracionEstatus[estatusProgramacionCalibracion]], string.Format("Nombre del equipo: {0}, Tipo de Calibración : {1}, Fecha de registro: {2}", equipo?.IdEquipoNavigation?.Nombre, equipo?.IdCatTipoCalibracion, equipo?.FechaRegistro.ToString("dd/MM/yyyy")).ToUpper()),
                        MovimientoFinal = null,
                        FechaModificacion = null,
                        Fecha = DateTime.Now,
                        Leido = false,
                        Procesada = false
                    };

                    _context.Alerta.Add(alerta);
                    await _context.SaveChangesAsync();
                }


                return new ResponseGeneric<bool>(true);
                //return result ? new ResponseGeneric<bool>(equipo.Id) : new ResponseGeneric<long>();
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> CambioEstatus(AuxiliarEstatusResponse request)
        {
            try
            {
                if (request.Id > 0)
                {
                    var programacion = await _context.EquipoTipoCalibracions.Include(x => x.IdEquipoNavigation).FirstOrDefaultAsync(x => x.Id == request.Id);

                    if (programacion != null)
                    {

                        programacion.IdUserValido = _userResolver.GetUser().IdUser;

                        var alerta = await _context.Alerta.Where(d => d.TableId == programacion.Id)
                                                          .Where(d => d.TableName == "EquipoTipoCalibracion")
                                                          .FirstOrDefaultAsync();
                        if (alerta != null)
                        {
                            int estatusProgramacionCalibracion = ObtenerEstatus(programacion.Nota, programacion.IdUserValido);
                            alerta.MovimientoFinal = string.Format(MovimientosDicts.DictMovimientoProgramacionCalibracion[estatusProgramacionCalibracion], CatTipoCalibracionDic.Nombres[programacion.IdCatTipoCalibracion], programacion?.IdEquipoNavigation?.Nombre, programacion.FechaRegistro.ToString("g"));
                            alerta.FechaModificacion = DateTime.Now;
                            alerta.IdEstatusFinal = estatusProgramacionCalibracion;
                        }
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
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> EstatusRechazar(AuxiliarEstatusResponse request)
        {
            try
            {
                if (request.Id > 0)
                {
                    var calibracion = await _context.EquipoTipoCalibracions.Include(x => x.IdEquipoNavigation).FirstOrDefaultAsync(x => x.Id == request.Id);

                    if (calibracion != null)
                    {
                        calibracion.Nota = request.Nota;

                        var alerta = await _context.Alerta.Where(d => d.TableId == calibracion.Id)
                                                          .Where(d => d.TableName == "EquipoTipoCalibracion")
                                                          .FirstOrDefaultAsync();
                        if (alerta != null)
                        {
                            int estatusProgramacionCalibracion = ObtenerEstatus(calibracion.Nota, calibracion.IdUserValido);
                            alerta.MovimientoFinal = string.Format(MovimientosDicts.DictMovimientoProgramacionCalibracion[estatusProgramacionCalibracion], CatTipoCalibracionDic.Nombres[calibracion.IdCatTipoCalibracion], calibracion?.IdEquipoNavigation?.Nombre, calibracion.FechaRegistro.ToString("g"));
                            alerta.FechaModificacion = DateTime.Now;
                            alerta.IdEstatusFinal = estatusProgramacionCalibracion;
                        }
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
                return new ResponseGeneric<bool>(ex);
            }
        }

        private int ObtenerEstatus(string Nota, long? IdUserValido)
        {
            if (IdUserValido == null && !string.IsNullOrEmpty(Nota))
                return ProgramacionCalibracionEstatus.Pendiente;
            if (IdUserValido.HasValue && !string.IsNullOrEmpty(Nota))
                return ProgramacionCalibracionEstatus.Rechazado;
            if (IdUserValido.HasValue && string.IsNullOrEmpty(Nota))
                return ProgramacionCalibracionEstatus.Autorizado;

            return 3;
        }
    }

    public interface IProgramacionCalibracionNegocio
    {
        public Task<ResponseGeneric<List<vProgramacionCalibracionResponse>>> Consulta(ProgramacionCalibracionRequest request);

        public Task<ResponseGeneric<vProgramacionCalibracionResponse>> Detalle(long Id);

        public Task<ResponseGeneric<bool>> Registro(EquipoTipoCalibracionResponse request);

        public Task<ResponseGeneric<bool>> CambioEstatus(AuxiliarEstatusResponse request);

        public Task<ResponseGeneric<bool>> EstatusRechazar(AuxiliarEstatusResponse request);
    }
}
