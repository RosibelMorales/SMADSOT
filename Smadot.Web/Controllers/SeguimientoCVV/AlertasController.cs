using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Alertas.Request;
using Smadot.Models.Entities.Alertas.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.SeguimientoCVV;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using System.Linq.Dynamic.Core;

namespace Smadot.Web.Controllers.SeguimientoCVV
{
    [Authorize]
    public class AlertasController : Controller
    {

        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public AlertasController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuAutorizacionesValidacionesDVRF.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/Alertas/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var page = skip / pageSize + 1;

            //Invoca al método que se encarga de realizar la petición Api
            AlertaGridRequest request = new()
            {
                Pagina = page,
                Registros = pageSize,
                Busqueda = searchValue,
                ColumnaOrdenamiento = sortColumn,
                Ordenamiento = sortColumnDirection
            };
            var listResult = await AlertaHelper.Consulta(_proxyWebAPI, request);
            var data = listResult.Result as List<vAlertaResponse> ?? new List<vAlertaResponse>();
            //Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            //Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? totalRecord : data.Count();

            var list = new List<AlertaGridResponse>();
            data.ForEach(async row =>
                list.Add(new AlertaGridResponse
                {
                    Id = row.Id,
                    MovimientoInicial = row.MovimientoInicial,
                    MovimientoFinal = row.MovimientoFinal,
                    Fecha = row.Fecha,
                    FechaModificacion = row.FechaModificacion,
                    TableName = row.TableName,
                    Data = row.Data,
                    LeidoStatus = row.Leido ? "Sí" : "No",
                    Acciones = await this.RenderViewToStringAsync("~/Views/Alertas/_Acciones.cshtml", row)
                })
            );
            var returnObj = new
            {
                draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = list
            };

            return Json(returnObj);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(long id)
        {
            var result = new JsonResultDto();

            try
            {
                var alerta = await AlertaHelper.Detalle(_proxyWebAPI, id);
                var data = alerta.Result as vAlertaResponse ?? new vAlertaResponse();
                var idData = data.TableId;
                if (data.TableName == DictAlertas.EquipoTipoCalibracion)
                {
                    var Equipo = JsonConvert.DeserializeObject<EquipoTipoCalibracion>(data.Data);
                    idData = Equipo.IdEquipo;
                }
                result.Result = new
                {
                    html = await this.RenderViewToStringAsync("~/Views/Shared/_ModalAlertaContent.cshtml", data),
                    tipo = DictAlertas.EquivAlertaTitle[data.TableName],
                    tableName = data.TableName,
                    id = idData,
                    atendida = !string.IsNullOrEmpty(data.MovimientoFinal)
                };
            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> AutorizarIngresoManual(AutorizacionIngresoManualRequest request)
        {
            try
            {
                var res = await AlertaHelper.AutorizacionIngresoManual(_proxyWebAPI, request);
                if (!res.IsSuccessFully)
                {
                    res.Message = _mensajeErrorCatch;
                }
                return Json(res);

            }
            catch (Exception e)
            {
                return Json(new ResponseViewModel { IsSuccessFully = false, Message = "Ocurrió un error al actualizar la solicitud." });
            }
        }

        [HttpGet]
        public async Task<JsonResult> MenuNotificaciones()
        {
            var result = new ResponseViewModel(true);
            try
            {
                AlertaGridRequest request = new()
                {
                    Registros = 10,
                    SinAtender = true,
                    Leido = false,
                    Ordenamiento = "desc",
                    ColumnaOrdenamiento = "Leido",
                };

                var listResult = await AlertaHelper.Consulta(_proxyWebAPI, request);
                var data = listResult.Result as List<vAlertaResponse> ?? new List<vAlertaResponse>();
                data = data.OrderByDescending(x => x.Id).ToList();
                result.Result = await this.RenderViewToStringAsync("~/Views/Alertas/_MenuNotificaciones.cshtml", data);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        public IActionResult Redireccionar(string tablename, long? id)
        {
            try
            {
                CatTablesNameAlertasDic catTablesNameAlertasDic = new CatTablesNameAlertasDic();
                //if (tablename == DictAlertas.UserPuestoVerificentro)
                //{
                //    return RedirectToAction("Detalle", catTablesNameAlertasDic.RutasTablesName[tablename], new { id });

                //}
                //return RedirectToAction("Index", catTablesNameAlertasDic.RutasTablesName[tablename], new { id });
                if (tablename == DictAlertas.UserPuestoVerificentro || tablename == DictAlertas.EquipoTipoCalibracion)
                {
                    if (tablename == DictAlertas.UserPuestoVerificentro)
                    {
                        return RedirectToAction("Detalle", catTablesNameAlertasDic.RutasTablesName[tablename], new { id });

                    }
                    return RedirectToAction("Index", catTablesNameAlertasDic.RutasTablesName[tablename], new { id });
                }
                else
                {
                    return RedirectToAction("Index", catTablesNameAlertasDic.RutasTablesName[tablename]);
                }
                
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
    }
}
