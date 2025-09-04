using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Calibracion.Request;
using Smadot.Models.Entities.Calibracion.Response;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.ReposicionCredencial.Request;
using Smadot.Models.Entities.ReposicionCredencial.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.AdministracionStock;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Helper.Operaciones.SeguimientoCVV;
using Smadot.Web.Utilities;
using System.Linq.Dynamic.Core;

namespace Smadot.Web.Controllers.SeguimientoCVV
{
    [Authorize]
    public class ReposicionCredencialController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly BlobStorage _blobStorage;
        private readonly IUserResolver _userResolver;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public ReposicionCredencialController(IProxyWebAPI proxyWebAPI, IConfiguration configuration, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _userResolver = userResolver;
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuReposicionCredenciales.PermisoMenuPadre))
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
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var page = skip / pageSize + 1;

            //Invoca al método que se encarga de realizar la petición Api
            var request = new ReposicionCredencialListRequest();
            request.Pagina = page;
            request.Registros = pageSize;
            request.Busqueda = searchValue;
            request.ColumnaOrdenamiento = sortColumn;
            request.Ordenamiento = sortColumnDirection;

            var listResult = await ReposicionCredencialHelper.Consulta(_proxyWebAPI, request);
            var data = listResult.Result as List<vReposicionCredencialResponse> ?? new List<vReposicionCredencialResponse>();

            //Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            //Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? totalRecord : data.Count();

            var List = new List<ReposicionCredencialGridResponse>();
            foreach (var row in data)
            {
                List.Add(new ReposicionCredencialGridResponse
                {
                    Id = row.Id,
                    IdCatEstatusReporteCredencial = row.IdCatEstatusReporteCredencial,
                    Denuncia = row.Denuncia,
                    FechaRegistro = row.FechaRegistro,
                    IdCatMotivoReporteCredencial = row.IdCatEstatusReporteCredencial,
                    IdUserPuestoVerificentro = row.IdUserPuestoVerificentro,
                    Nombre = row.Nombre,
                    NumeroTrabajador = row.NumeroTrabajador,
                    UrlCredencial = row.UrlCredencial,
                    EstatusReporteCredencial = row.EstatusReporteCredencial,
                    MotivoReporteCredencial = row.MotivoReporteCredencial,
                    Acciones = await this.RenderViewToStringAsync("~/Views/ReposicionCredencial/_Acciones.cshtml", row)
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
        public async Task<IActionResult> Registro()
        {
            var result = new ResponseViewModel(true);
            var motivos = await MotivoReporteCredencialHelper.Consulta(_proxyWebAPI, new MotivoReporteCredencialRequest() { Activo = true });
            if (motivos.IsSuccessFully)
            {
                ViewBag.Motivos = ((List<MotivoReporteCredencialResponse>)motivos.Result).Select(obj => new SelectListItem { Value = obj.Id.ToString(), Text = obj.Nombre }).ToList();
            }

            result.Result = await this.RenderViewToStringAsync("~/Views/ReposicionCredencial/_ModalRegistro.cshtml");
            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> Registro(ReposicionCredencialRequest request)
        
        {
            var result = new ResponseViewModel(true);
            try
            {
                var files = JsonConvert.DeserializeObject<List<CredencialFile>>(Request.Form["Files"].FirstOrDefault() ?? "");
                request.Files = files ?? new List<CredencialFile>();
                var response = await ReposicionCredencialHelper.Registro(_proxyWebAPI, request);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                result.Result = response.Result;
            }
            catch (Exception e)
            {
                result.IsSuccessFully = false;
                result.Message = e.Message;
            }

            return new JsonResult(result);
        }
        [HttpGet]
        public async Task<JsonResult> Detalle(long id)
        {
            var result = new ResponseViewModel(true);
            var vm = new vReposicionCredencialResponse();
            try
            {
                var response = await ReposicionCredencialHelper.Detalle(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                vm = response.Result as vReposicionCredencialResponse ?? vm;
				var VBUrlDoc1 = "";
				if (!string.IsNullOrEmpty(vm.UrlCredencial))
				{
					var arrDoc = vm.UrlCredencial.Split("/");
					var NameDoc1 = arrDoc[arrDoc.Length - 1];
					var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
					var archivo = await _blobStorage.DownloadFileAsync(vm.UrlCredencial, true);
					if (archivo.IsSuccessFully)
					{
						var imageUrlCredencial = string.Format("{0}{1}", ContentType1, archivo.Result);
						VBUrlDoc1 = imageUrlCredencial;
					}
				}
				ViewBag.UrlCredencial = VBUrlDoc1;
				result.Result = await this.RenderViewToStringAsync("~/Views/ReposicionCredencial/_ModalDetalle.cshtml", vm);
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
            var listaResult = await ReposicionCredencialHelper.Autocomplete(_proxyWebAPI, new UserPuestoAutoCompleteRequest { Term = term, Start = start, End = end });
            var data = listaResult.Result as List<UserPuestoAutoCompleteResponse> ?? new List<UserPuestoAutoCompleteResponse>();

            return Json(new { items = data, total_count = data.Count });
        }

        [HttpPut]
        public async Task<JsonResult> Aprobar(long id)
        {
            var result = new ResponseViewModel(true);
            var vm = new vReposicionCredencialResponse();
            try
            {
                var response = await ReposicionCredencialHelper.Aprobar(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al aprobar esta solicitud";
                    return new JsonResult(result);
                }
                result.Message = "Se ha aprobado la solicitud correctamente";
                return new JsonResult(result);

            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPut]
        public async Task<JsonResult> Rechazar(long id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var response = await ReposicionCredencialHelper.Rechazar(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al rechazar esta solicitud";
                    return new JsonResult(result);
                }
                result.Message = "Se ha rechazado la solicitud correctamente";
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> DescargarDocumento(string url)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var urlSplit = url.Split("/");
                var nombreArchivo = urlSplit[urlSplit.Length - 1];
                var archivo = await _blobStorage.DownloadFileAsync(url, true);
                if (!archivo.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    throw new Exception("No sé encontró el documento.");
                }
                result.Result = new
                {
                    FileName = nombreArchivo,
                    Base64 = archivo.Result,
                    Ext = nombreArchivo.Split('.')[1]
                };
            }
            catch (Exception ex)
            {
                if (result.IsSuccessFully)
                    result.Message = "Error al descargar el documento.";
                else
                    result.Message = ex.Message;
                result.IsSuccessFully = false;
            }
            return new JsonResult(result);
        }
        private void SetMenuLinkActive()
        {
            ViewBag.Tramites = "";
            ViewBag.SolicitudFormaValorada = "";
            ViewBag.Administracion = "";
            ViewBag.SeguimientoCVV = "";
            ViewBag.linkAdminCVV = "here";
        }
    }
}
