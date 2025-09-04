using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Namespace;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Calibracion.Extensions;
using Smadot.Models.GenericProcess;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.Transactions;

namespace Smadot.SmadotTareas.Negocio.Negocio
{
    public class CalibracionesDestiempoNegocio : ICalibracionesDestiempoNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;

        public CalibracionesDestiempoNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration, SmadsotGenericInserts smadsotGenericInserts)
        {
            _context = context;
            _userResolver = userResolver;
            _smadsotGenericInserts = smadsotGenericInserts;
        }

        public async Task<ResponseGeneric<List<long>>> Generar()
        {
            try
            {
                List<long> calibracionesDestiempo = new();

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    DateTime fechaZonaHorariaCiudadMexico = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)"));

                    var ultimasCalibracionesDeEquipo = _context.vEquipoCalibracions.Where(x => x.FechaProximaCalibracion < fechaZonaHorariaCiudadMexico)
                                                                                .AsQueryable();

                    var userAdmin = await _context.Users.FirstOrDefaultAsync(x => x.UserName != null);

                    foreach (var calibracionEquipo in ultimasCalibracionesDeEquipo)
                    {
                        try
                        {
                            var tipoCalibracion = _context.EquipoTipoCalibracions.OrderByDescending(x => x.Id).FirstOrDefault(x => x.IdUserValido != null && x.IdEquipo == calibracionEquipo.IdEquipo);
                            if (tipoCalibracion == null)
                                continue;
                            var ultimaCalibracion = _context.Calibracions.OrderByDescending(x => x.Id)
                                                                        .FirstOrDefault(x => x.IdEquipoTipoCalibracion == tipoCalibracion.Id);
                            DateTime fechaProgramada = calibracionEquipo.FechaProximaCalibracion.Value;
                            DateTime fechaProximaCalibracion = calibracionEquipo.FechaCalibracion.Value.ObtenerProximaFechaCalibracion(tipoCalibracion.IdCatTipoCalibracion);

                            int consecutivo = (ultimaCalibracion?.Consecutivo ?? 0) + 1;

                            Calibracion calibracion = new()
                            {
                                IdEquipoTipoCalibracion = tipoCalibracion.Id,
                                Consecutivo = consecutivo,
                                FechaCalibracion = calibracionEquipo.FechaCalibracion.Value,
                                FechaProgramada = fechaProgramada,
                                FechaProximaCalibracion = fechaProximaCalibracion,
                                IdUserRegistro = userAdmin?.Id ?? 1,
                                FechaRegistro = DateTime.Now,
                                IdUserValido = null,
                                Nota = calibracionEquipo.Nota.ToUpper(),
                                IdCatEstatusCalibracion = CalibracionEstatus.PendienteCalibracion,
                                NumeroCertificado = null,
                                FechaEmisionCertificad = null
                            };

                            _context.Calibracions.Add(calibracion);
                            _context.SaveChanges();

                            var equipo = _context.Equipos.FirstOrDefault(x => x.Id == calibracionEquipo.IdEquipo);
                            equipo.IdCatEstatusEquipo = EstatusEquipo.SinCalibrar;

                            _context.Equipos.Update(equipo);
                            _context.SaveChanges();

                            Alertum alerta = new()
                            {
                                TableName = DictAlertas.Calibracion,
                                TableId = calibracion.Id,
                                IdVerificentro = calibracionEquipo.IdVerificentro,
                                Data = JsonConvert.SerializeObject(calibracion, new JsonSerializerSettings
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                                    NullValueHandling = NullValueHandling.Ignore
                                }),
                                IdUser = null,
                                MovimientoInicial = string.Format(MovimientosDicts.DictMovimientoCalibracion[CalibracionEstatus.PendienteCalibracion], calibracionEquipo.Nombre, calibracion.Consecutivo, calibracion.FechaProgramada.ToString("g")),
                                Fecha = DateTime.Now,
                                IdEstatusFinal = CalibracionEstatus.PendienteCalibracion,
                                MovimientoFinal = null,
                                FechaModificacion = null,
                                Leido = false,
                                Procesada = false
                            };

                            _context.Alerta.Add(alerta);
                            _context.SaveChanges();

                            calibracionesDestiempo.Add(alerta.Id);

                        }
                        catch (Exception ex)
                        {
                            await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionTareasCalibracion);
                            continue;
                        }
                    }

                    scope.Complete();
                }

                return new ResponseGeneric<List<long>>(calibracionesDestiempo);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<long>>(ex);
            }
        }
    }

    public interface ICalibracionesDestiempoNegocio
    {
        public Task<ResponseGeneric<List<long>>> Generar();
    }
}