using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.AsignacionStock.Request;
using Smadot.Models.Entities.AsignacionStock.Response;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.AdministracionStock;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Utilities;
using System.ComponentModel.DataAnnotations;
namespace Smadot.Web.Controllers.AsignacionStockVentanilla
{
    [Authorize]
    public class AsignacionStockVentanillaController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public AsignacionStockVentanillaController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(PermisosMenu.AsignacionStockdeVentanilla))
                return new UnauthorizedResult();
            return View("~/Views/AsignacionStockVentanilla/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(long id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                result = await AdministracionStockHelper.Consulta(_proxyWebAPI, id);
                if (!result.IsSuccessFully)
                    throw new ValidationException("No se encontró el registro.");
                var almacenes = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest() { Activo = true });
                if (almacenes.IsSuccessFully)
                {
                    ViewBag.almacenes = ((List<AlmacenResponse>)almacenes.Result).Select(obj => new SelectListItem { Value = obj.Id.ToString(), Text = obj.Nombre }).ToList();
                }
                result.Result = await this.RenderViewToStringAsync("~/Views/AsignacionStockVentanilla/_ModalRegistro.cshtml", (RegistroStockventanillaResponse)result.Result);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var vmTablaFija = new TablaFijaViewModel.TablaFijaMain();
                RegistroStockventanillaResponse vm = new RegistroStockventanillaResponse();
                //if (id.HasValue)
                //{
                //    var devolucion = ConsultaBD().FirstOrDefault(x => x.Id == id);
                //    vm.TablaFijaViewModel = vmTablaFija.Consultar(true);
                //    vm.NumeroSolicitud = devolucion.NumeroSolicitud;
                //    vm.NumeroDevolucion = devolucion.NumeroDevolucion;
                //    vm.FechaEntrega = DateTime.Parse(devolucion.FechaEntrega);
                //    vm.ResponsableEntrega = devolucion.ResponsableEntrega;
                //    vm.RecibioEnSPF = devolucion.RecibioEnSPF;
                //}
                //else
                //{
                //}
                var almacenes = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest() { Activo = true, VerificentrosNulos = true });
                if (almacenes.IsSuccessFully)
                {
                    ViewBag.almacenes = ((List<AlmacenResponse>)almacenes.Result).Where(x => x.IdVerificentro == null).Select(obj => new SelectListItem { Value = obj.Id.ToString(), Text = obj.Nombre }).ToList();
                }
                vm.FechaEntrega = DateTime.Now;
                vm.TablaFijaViewModel = new TablaFijaViewModel.TablaFijaMain().Consultar();

                ViewBag.Enabled = true;
                result.Result = await this.RenderViewToStringAsync("~/Views/AsignacionStockVentanilla/_ModalRegistro.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");

            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int page = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            //Invoca al método que se encarga de realizar la petición Api
            var request = new RequestList();
            request.Pagina = page;
            request.Registros = pageSize;
            request.Busqueda = searchValue;
            request.ColumnaOrdenamiento = sortColumn;
            request.Ordenamiento = sortColumnDirection;

            var response = await AdministracionStockHelper.Consulta(_proxyWebAPI, request);
            var jsonReponse = new ResponseGrid<ListAsignacionStockResponse>();

            if (response.IsSuccessFully)
            {
                jsonReponse = (ResponseGrid<ListAsignacionStockResponse>)response.Result;
                jsonReponse.Data.ForEach(async x =>
                {
                    x.Actions = await this.RenderViewToStringAsync("~/Views/AsignacionStockVentanilla/_Acciones.cshtml", x);
                });
                jsonReponse.Draw = draw;
                return Json(jsonReponse);
            }

            return Json(jsonReponse);
        }
        [HttpPost]
        public async Task<IActionResult> Registro(RegistroStockventanillaRequest request)
        {
            var result = new ResponseViewModel(true);
            try
            {
                result = await AdministracionStockHelper.Guardar(_proxyWebAPI, request);
                if (!string.IsNullOrEmpty((string)result.Result))
                {
                    result.IsSuccessFully = false;
                    result.Message = ((string)result.Result);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }
    }

}