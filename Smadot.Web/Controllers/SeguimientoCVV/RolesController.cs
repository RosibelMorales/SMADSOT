using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Motivos.Response;
using Smadot.Models.Entities.ReporteMensual.Request;
using Smadot.Models.Entities.ReporteMensual.Response;
using Smadot.Models.Entities.Roles.Request;
using Smadot.Models.Entities.Roles.Response;
using Smadot.Models.Entities.SeguimientoCVV.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.ReporteMensual;
using Smadot.Web.Helper.Operaciones.Roles;
using Smadot.Web.Helper.Operaciones.SeguimientoCVV;
using Smadot.Web.Utilities;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Models.Dicts;

namespace Smadot.Web.Controllers.SeguimientoCVV
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuider;
        private readonly BlobStorage _blobStorage;

        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public RolesController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }
        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuRoles.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");

            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            //Invoca al método que se encarga de realizar la petición Api
            var request = new RolesListRequest();
            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();
            //long i;
            //if (long.TryParse(Request.Form["idAlmacen"].FirstOrDefault(), out i)) request.IdAlmacen = i;
            //else request.IdAlmacen = null;

            var listaResult = await RolesHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<RolesResponse> ?? new List<RolesResponse>();

            ////Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total.Value;

            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? totalRecord : data.Count();

            var List = new List<RolesResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new RolesResponseGrid
                {
                    Id = row.Id,
                    Nombre = row.Nombre,
                    Alias = row.Alias,
                    AccesoTotalVerificentros = row.AccesoTotalVerificentros,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Roles/_Acciones.cshtml", row)
                });
            }

            var returnObj = new
            {
                draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = List
            };

            return Json(returnObj);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {

                var result = new ResponseViewModel(true);
                var vm = new List<RolesResponse>
            {
                new RolesResponse
                {
                    Id = 0
                }
            };
                try
                {
                    var response = await RolesHelper.Consulta(_proxyWebAPI, id ?? 0);
                    vm = response.Result as List<RolesResponse> ?? vm;
                    result.Result = await this.RenderViewToStringAsync("~/Views/Roles/_Registro.cshtml", vm);
                }
                catch (Exception ex)
                {
                    result.IsSuccessFully = false;
                    result.Message = ex.Message;
                }

                return new JsonResult(result);
            

           
        }

        [HttpPost]
        public async Task<IActionResult> Edit(List<RolesResponse> registro)
        {
            var result = new ResponseViewModel(true);
            try
            {         
                var save = await RolesHelper.Edit(_proxyWebAPI, registro);
                if (!save.IsSuccessFully)
                    throw new Exception(save.Message);

                result.Message = "La información se guardó correctamente.";

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
