using Microsoft.EntityFrameworkCore;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Alertas.Request;
using Smadot.Models.Entities.Alertas.Response;
using Smadot.Models.Entities.UsoSuministro.Request;
using Smadot.Models.Entities.UsoSuministro.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.Globalization;
using System.Linq.Dynamic.Core;

namespace Smadot.SeguimientoCVV.Model.Negocio
{
    public class AlertaNegocio : IAlertaNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private long _idVerificentro;

        public AlertaNegocio(SmadotDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
            _idVerificentro = userResolver.GetUser().IdVerificentro ?? 0;
        }

        public async Task<ResponseGeneric<List<vAlertaResponse>>> Consulta(AlertaGridRequest request)
        {
            try
            {
                var alertas = _context.vAlerta.Where(x => x.TableName != DictAlertas.ActualizacionPrueba).AsQueryable();
                var rol = _userResolver.GetUser().RoleNames.FirstOrDefault();
                var acceso = _context.Rols.ToList().Where(o => o.Nombre.Equals(rol, StringComparison.Ordinal)).FirstOrDefault()?.AccesoTotalVerificentros ?? false;

                var listPermisosAlertas = MenuAlertas.DiccionarioAlertasPermiso;
                foreach (var item in listPermisosAlertas)
                {
                    if (!_userResolver.HasPermission(item.Key))
                    {
                        alertas = alertas.Where(x => !x.TableName.Equals(item.Value));
                    }

                }
                if (!acceso)
                {
                    alertas = alertas.Where(x => x.IdVerificentro == _idVerificentro).AsQueryable();
                }
                if (request.Leido != null)
                {
                    alertas = alertas.Where(x => x.Leido == request.Leido).AsQueryable();
                }

                if (request.SinAtender)
                {
                    alertas = alertas.Where(x => x.FechaModificacion == null);
                }

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    alertas = alertas.Where(x => x.Id.ToString().Contains(request.Busqueda.ToLower()) || x.MovimientoInicial.ToLower().Contains(request.Busqueda.ToLower()) || x.MovimientoFinal.ToLower().Contains(request.Busqueda.ToLower()) || x.Nombre.ToLower().Contains(request.Busqueda.ToLower()) || x.TableName.ToLower().Contains(request.Busqueda.ToLower()));
                }
                alertas = alertas.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento).ThenByDescending(x => x.Fecha);

                var tot = alertas.Count();
                //if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                //{
                //    alertas = alertas.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                //}

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    alertas = alertas.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }
                if (request.Registros != null)
                {
                    alertas = alertas.Take(request.Registros.Value);
                }
                var result = await alertas.Select(x => new vAlertaResponse
                {
                    Id = x.Id,
                    TableId = x.TableId,
                    MovimientoInicial = x.MovimientoInicial,
                    MovimientoFinal = x.MovimientoFinal,
                    Fecha = x.Fecha,
                    FechaModificacion = x.FechaModificacion,
                    TableName = x.TableName,
                    Data = x.Data,
                    Leido = x.Leido,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<vAlertaResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<vAlertaResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<vAlertaResponse>> Detalle(long id)
        {
            try
            {
                var alerta = _context.vAlerta.FirstOrDefault(x => x.Id == id);

                var result = new vAlertaResponse
                {
                    Id = alerta.Id,
                    TableId = alerta.TableId,
                    MovimientoInicial = alerta.MovimientoInicial,
                    MovimientoFinal = alerta.MovimientoFinal,
                    Fecha = alerta.Fecha,
                    FechaModificacion = alerta.FechaModificacion,
                    TableName = alerta.TableName,
                    Nombre = alerta.Nombre,
                    Data = alerta.Data
                };
                var alertaDb = _context.Alerta.FirstOrDefault(x => x.Id == id);

                if (alertaDb != null)
                {
                    alertaDb.Leido = true;
                    await _context.SaveChangesAsync();
                }

                return new ResponseGeneric<vAlertaResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<vAlertaResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Leer(long id)
        {
            try
            {
                var result = new ResponseGeneric<bool>();

                var alerta = _context.Alerta.FirstOrDefault(x => x.Id == (long)Convert.ToDouble(id));

                if (alerta != null)
                {
                    alerta.Leido = true;
                    await _context.SaveChangesAsync();
                }

                result.Response = true;
                return result;
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }
        public async Task<ResponseGeneric<bool>> AutorizacionIngresoManual(AutorizacionIngresoManualRequest request)
        {
            try
            {
                var result = new ResponseGeneric<bool>();

                var alerta = _context.Alerta.FirstOrDefault(x => x.TableId == request.Id && x.TableName == DictAlertas.Verificacion);

                if (alerta != null)
                {
                    var estatusFinal = request.Estatus ? VerificacionIngresoManualEstatus.Autorizado : VerificacionIngresoManualEstatus.Rechazado;
                    var verificacion = _context.vVerificacionCita.FirstOrDefault(x => x.Id == alerta.TableId);
                    alerta.MovimientoFinal = string.Format(MovimientosDicts.DictMovimientoVerificacion[estatusFinal], verificacion.Linea, verificacion.Placa, verificacion.Serie);
                    alerta.IdEstatusFinal = estatusFinal;
                    alerta.FechaModificacion = DateTime.Now;
                    if (request.Estatus)
                    {
                        var verificacionDB = _context.Verificacions.FirstOrDefault(x => x.Id == alerta.TableId);
                        verificacionDB.IngresoManual = true;
                    }
                    await _context.SaveChangesAsync();
                }

                result.Response = true;
                return result;
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }
    }

    public interface IAlertaNegocio
    {
        public Task<ResponseGeneric<List<vAlertaResponse>>> Consulta(AlertaGridRequest request);
        public Task<ResponseGeneric<vAlertaResponse>> Detalle(long id);
        public Task<ResponseGeneric<bool>> AutorizacionIngresoManual(AutorizacionIngresoManualRequest request);
        public Task<ResponseGeneric<bool>> Leer(long id);
    }
}
