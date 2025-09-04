using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.ConsultaPruebaVerificacion.Request;
using Smadot.Models.Entities.ConsultaPruebaVerificacion.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.ConsultaPruebaVerificacion;

namespace Smadot.Web.Controllers.ConsultaPruebasVerificacion
{
    [Authorize]
    public class ConsultaPruebaVerificacionController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public ConsultaPruebaVerificacionController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
        }

        // GET: ConsultaPruebaVerificacion
        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuConsultaPruebasVerificacion.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/ConsultaPruebaVerificacion/Index.cshtml");
        }

        //Llenado de la tabla
        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");

            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var page = skip / pageSize + 1;
            var placa = Request.Form["placa"].FirstOrDefault() == "true" ? true : false;
            var placaSerie = Request.Form["placaSerie"].FirstOrDefault();

            //Invoca al método que se encarga de realizar la petición Api
            var request = new ConsultaPruebaVerificacionRequest();
            request.Pagina = page;
            request.Registros = pageSize;
            request.Busqueda = searchValue;
            request.ColumnaOrdenamiento = sortColumn;
            request.Ordenamiento = sortColumnDirection;
            request.placa = placa;
            request.placaSerie = placaSerie;

            var listaResult = await ConsultaPruebaVerificacionHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<ConsultaPruebaVerificacionResponse> ?? new List<ConsultaPruebaVerificacionResponse>();

            ////Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total.Value;

            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var returnObj = new
            {
                draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = data
            };

            return Json(returnObj);
        }

        public async Task<JsonResult> ProveedorAutocomplete()
        {
            JsonResult result = new JsonResult("");

            var prefix = Request.Query["q"].FirstOrDefault();
            var flag = Request.Query["f"].FirstOrDefault();
            int page = Convert.ToInt32(Request.Query["page"].FirstOrDefault() ?? "0");
            int records = Convert.ToInt32(Request.Query["records"].FirstOrDefault() ?? "0");
            var start = 0;
            var end = start + records;

            var listaResult = await ConsultaPruebaVerificacionHelper.BusquedaPlacaSerie(_proxyWebAPI, prefix, flag);
            var placaserie = listaResult.Result as List<BusquedaPlacaSerieRequest> ?? new List<BusquedaPlacaSerieRequest>();
            return Json(new { items = placaserie, total_count = placaserie.Count });
        }

        private void SetMenuLinkActive()
        {
            ViewBag.SolicitudFormaValorada = "here";
            ViewBag.Administracion = "";
        }
    }
}
