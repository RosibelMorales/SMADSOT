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
    public class TareasVerificacionNegocio : ITareasVerificacionNegocio
    {
        private SmadotDbContext _context;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;

        public TareasVerificacionNegocio(SmadotDbContext context, SmadsotGenericInserts smadsotGenericInserts)
        {
            _context = context;
            _smadsotGenericInserts = smadsotGenericInserts;
        }

        public async Task LimpiarVerificaciones()
        {
            try
            {
                var userAdmin = await _context.Users.FirstOrDefaultAsync(x => x.UserName != null);

                var verificacionesSinFolio = _context.vVerificacionSinFolios;
                var cantidadLote = 5000;
                var total = await verificacionesSinFolio.CountAsync();
                var indice = 0;
                while (indice < total)
                {
                    var restante = total - indice;
                    var miniminoRestante = Math.Min(restante, cantidadLote);
                    var verificacionesEliminar = verificacionesSinFolio.Skip(indice).Take(miniminoRestante);
                    var transaction = await _context.Database.BeginTransactionAsync();
                    foreach (var item in verificacionesEliminar)
                    {
                        try
                        {
                            var verificacion = await _context.Verificacions.Include(x => x.ParametrosTablaMaestraVerificacion)
                            .Include(x => x.ResultadosVerificacion).FirstOrDefaultAsync(x => x.Id == item.IdVerificacion);
                            if (verificacion == null)
                                continue;

                            if (verificacion.ResultadosVerificacion != null)
                                _context.Remove(verificacion.ResultadosVerificacion);
                            if (verificacion.ParametrosTablaMaestraVerificacion != null)
                                _context.Remove(verificacion.ParametrosTablaMaestraVerificacion);

                            _context.Remove(verificacion);

                            var cita = await _context.CitaVerificacions.Include(x => x.DocumentosCitum).FirstOrDefaultAsync(x => x.Id == item.IdCitaVerificacion);
                            if (cita?.DocumentosCitum != null)
                            {
                                _context.RemoveRange(cita.DocumentosCitum);
                            }
                            if (cita != null)
                                _context.Remove(cita);

                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionBorrarVerificacion);
                            continue;
                        }

                    }
                    indice += miniminoRestante;
                    transaction.Commit();
                    await Task.Delay(5000);
                }


            }
            catch (Exception ex)
            {
                await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionTareasVerificacion);
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

    public interface ITareasVerificacionNegocio
    {
        public Task LimpiarVerificaciones();
        public Task BorrarCertificados();
    }
}