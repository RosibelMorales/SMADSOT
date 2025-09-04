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
using Smadsot.Historico.Models.DataBase;
using System.Transactions;
using System.Web.Helpers;

namespace Smadot.SmadotTareas.Negocio.Negocio
{
    public class TareasCitasNegocio : ITareasCitasNegocio
    {
        private readonly SmadotDbContext _context;
        private readonly SmadsotHistoricoDbContext _contextHistorico;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;

        public TareasCitasNegocio(SmadotDbContext context, SmadsotGenericInserts smadsotGenericInserts, SmadsotHistoricoDbContext contextHistorico)
        {
            _context = context;
            _smadsotGenericInserts = smadsotGenericInserts;
            _contextHistorico = contextHistorico;
        }

        public async Task ProcesarHistoricoCitas()
        {
            try
            {
                DateTime fechaZonaHorariaCiudadMexico = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)"));
                DateTime fechaHistorico = fechaZonaHorariaCiudadMexico.AddDays(-15);
                List<long> calibracionesDestiempo = new();

                var citasSinVerificacion = _context.vHistoricoCita.Where(x => x.Fecha <= fechaHistorico);
                // var transactionOptions = new TransactionOptions
                // {
                //     IsolationLevel = IsolationLevel.ReadCommitted, // Puedes ajustar el nivel de aislamiento según tus necesidades.
                //     Timeout = TimeSpan.FromMinutes(8)
                // };
                // var userAdmin = await _context.Users.FirstOrDefaultAsync(x => x.UserName != null);

                var cantidadLote = 1000;
                var total = await citasSinVerificacion.CountAsync();
                var indice = 0;

                while (indice < total)
                {
                    using (var transaction = await _contextHistorico.Database.BeginTransactionAsync())
                    {
                        var restante = total - indice;
                        if (indice > 60000)
                        {
                            break;
                        }
                        var miniminoRestante = Math.Min(restante, cantidadLote);
                        var citasLote = citasSinVerificacion.Skip(indice).Take(miniminoRestante);
                        var citasEliminar = new List<CitaVerificacion>();
                        var citasAgregar = new List<CitaVerificacionHistorico>();

                        // using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
                        // {
                        foreach (var item in citasLote)
                        {
                            var jsonCita = JsonConvert.SerializeObject(item);
                            var insertHistoricoCita = JsonConvert.DeserializeObject<CitaVerificacionHistorico>(jsonCita);
                            if (insertHistoricoCita != null)
                            {
                                // var existeCita = await _contextHistorico.CitaVerificacionHistoricos.AnyAsync(x => x.Id == item.Id);
                                // if (existeCita)
                                //     continue;
                                insertHistoricoCita.Id = 0;
                                // await _contextHistorico.CitaVerificacionHistoricos.AddAsync(insertHistoricoCita);
                                citasAgregar.Add(insertHistoricoCita);
                                await _contextHistorico.SaveChangesAsync();
                                var cita = await _context.CitaVerificacions.FirstOrDefaultAsync(x => x.Id == item.Id);
                                if (cita != null)
                                    citasEliminar.Add(cita);
                            }

                        }
                        // scope.Complete();
                        await _contextHistorico.AddRangeAsync(citasAgregar);
                        _context.CitaVerificacions.RemoveRange(citasEliminar);
                        await _contextHistorico.SaveChangesAsync();
                        await _context.SaveChangesAsync();
                        indice += miniminoRestante;
                        transaction.Commit();
                        await Task.Delay(3000);
                        // }
                    }


                }
            }
            catch (Exception ex)
            {

                await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionTareasVerificacion);
                _context.SaveChanges();
            }
        }

        public async Task BorrarCertificados()
        {
            try
            {
                var folios = await _context.FoliosFormaValoradaVerificentros.Where(x => x.IdCatTipoTramite == null).ToListAsync();
                var cantidadLote = 10;
                var total = folios.Count;
                var indice = 0;
                while (indice < total)
                {
                    var restante = total - indice;
                    var miniminoRestante = Math.Min(restante, cantidadLote);
                    var verificacionesEliminar = folios.Skip(indice).Take(miniminoRestante);
                    foreach (var item in verificacionesEliminar)
                    {
                        try
                        {

                            _context.Remove(item);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionTareasFoliosSinUso);
                            continue;
                        }

                    }
                    indice += miniminoRestante;
                    await Task.Delay(2000);
                }
            }
            catch (Exception ex)
            {

                await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionTareasFoliosSinUso);

            }
        }
    }

    public interface ITareasCitasNegocio
    {
        public Task ProcesarHistoricoCitas();
    }
}