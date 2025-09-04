using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.Venta;
using Smadot.Web.Utilities;
using static Smadot.Models.Entities.ProveedorFolioServicio.Request.ProveedorFolioServicioRequestData;
using static Smadot.Models.Entities.ProveedorFolioServicio.Response.ProveedorFolioServicioResponseData;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Models.Dicts;

namespace Smadot.Web.Controllers.Venta
{
    [Authorize]
    public class ProveedorFolioServicioController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public ProveedorFolioServicioController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuProveedoresFolio.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/Venta/ProveedorFolioServicio/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");
            //Invoca al método que se encarga de realizar la petición Api
            var request = new RequestList();

            int totalRecord = 0;
            int filterRecord = 0;

            var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

            var gridData = new ResponseGrid<ProveedorFolioServicioResponse>();
            var listaResult = await ProveedorFolioServicioHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<ProveedorFolioServicioResponse> ?? new List<ProveedorFolioServicioResponse>();

            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            //filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();
            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().TotalFilter;

            var List = new List<ProveedorFolioServicioResponseGrid>();

            foreach (var row in data)
            {
                List.Add(new ProveedorFolioServicioResponseGrid
                {
                    Id = row.Id,
                    Equipo = row.Equipo,
                    EstatusFolio= row.EstatusFolio,
                    FechaRegistro= row.FechaRegistro,
                    FolioOS = row.FolioOS,
                    Motivo = row.Motivo,
                    ProveedorEmpresa = row.ProveedorEmpresa,
                    Proveedor = row.Proveedor,
                    FolioPF = row.FolioPF,
                    EsLaboratorio = row.EsLaboratorio,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Venta/ProveedorFolioServicio/_Acciones.cshtml", row)
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
        public async Task<ActionResult> Registro()
        {
            var result = new ResponseViewModel(true);

            try
            {
                var response = new ProveedorFolioServicioRequest();
                result.Result = await this.RenderViewToStringAsync("~/Views/Venta/ProveedorFolioServicio/_Registro.cshtml", response);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Registro(ProveedorFolioServicioRequest registro)
        {
            var result = new ResponseViewModel(true);
            try
            {
                result = await ProveedorFolioServicioHelper.Guardar(_proxyWebAPI, registro);

                if (!string.IsNullOrEmpty((string)result.Message))
                {
                    result.IsSuccessFully = false;
                    result.Message = ((string)result.Message);
                }

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

        [HttpGet]
        public async Task<ActionResult> Detalle(long id)
        {
            var result = new ResponseViewModel(true);

            try
            {
                //var res = new JsonResultDto();
                var listaResult = await ProveedorFolioServicioHelper.GetById(_proxyWebAPI, id);
                var response = listaResult.Result as ProveedorFolioServicioResponse ?? new ProveedorFolioServicioResponse();

                result.Result = await this.RenderViewToStringAsync("~/Views/Venta/ProveedorFolioServicio/_Detalle.cshtml", response);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        public async Task<JsonResult> Autocomplete()
        {
            JsonResult result = new JsonResult("");

            var term = Request.Query["q"].FirstOrDefault();
            int page = Convert.ToInt32(Request.Query["page"].FirstOrDefault() ?? "0");
            int records = Convert.ToInt32(Request.Query["records"].FirstOrDefault() ?? "0");
            var start = 0;
            var end = start + records;
            var listaResult = await ProveedorHelper.Autocomplete(_proxyWebAPI, new ProveedorFolioServicioAutocompletRequest { Term = term, Start = start, End = end });
            var data = listaResult.Result as List<ProveedorFolioServicioAutocompleteResponse> ?? new List<ProveedorFolioServicioAutocompleteResponse>();

            return Json(new { items = data, total_count = data.Count });
        }

        private void SetMenuLinkActive()
        {
            ViewBag.Tramites = "";
            ViewBag.SolicitudFormaValorada = "";
            ViewBag.Administracion = "";
            ViewBag.SeguimientoCVV = "here";
        }
    }
}
