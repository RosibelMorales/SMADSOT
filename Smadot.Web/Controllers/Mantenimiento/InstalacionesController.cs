using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Instalaciones.Request;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.FileStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.Mantenimiento;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using static Smadot.Models.Entities.Instalaciones.Response.InstalacionResponseData;

namespace Smadot.Web.Controllers.Mantenimiento
{
    [Authorize]
    public class InstalacionesController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        // StorageManager _storageManager = new StorageManager();

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public InstalacionesController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        // GET: Instalaciones
        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuInstalaciones.PermisoMenuPadre))
                return new UnauthorizedResult();
            ViewBag.UserName = _userResolver.GetUser().Nombre;
            return View("~/Views/Instalaciones/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");
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

            var gridData = new ResponseGrid<InstalacionResponseGrid>();
            var listaResult = await InstalacionHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<InstalacionResponse> ?? new List<InstalacionResponse>();

            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<InstalacionResponseGrid>();

            foreach (var row in data)
            {
                List.Add(new InstalacionResponseGrid
                {
                    IdInstalacion = row.IdInstalacion,
                    UserRegistro = row.UserRegistro,
                    FechaRegistro = row.FechaRegistro,
                    Proveedor = row.Proveedor,
                    UrlDocumento = row.UrlDocumento,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Instalaciones/_Acciones.cshtml", row)
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
        public async Task<ActionResult> Detalle(long id)
        {
            var result = new JsonResultDto();

            try
            {
                var res = new JsonResultDto();
                var listaResult = await InstalacionHelper.GetById(_proxyWebAPI, id);
                var folio = listaResult.Result as InstalacionResponse ?? new InstalacionResponse();
                // var doc1 = "";
                // if (!string.IsNullOrEmpty(folio.UrlDocumento))
                // {
                //     doc1 = _storageManager.ObtenerArchivo(folio.UrlDocumento);
                // }
                ViewBag.Document = folio.UrlDocumento;
                var VBUrlDoc1 = "";
                if (!string.IsNullOrEmpty(folio.UrlDocumento))
                {
                    var arrDoc = folio.UrlDocumento.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(folio.UrlDocumento, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageUrlDocumento = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc1 = imageUrlDocumento;
                    }
                }
                ViewBag.UrlResultado = VBUrlDoc1;
                result.Result = await this.RenderViewToStringAsync("~/Views/Instalaciones/_Detalle.cshtml", folio);
            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> DescargarDocumento(string url)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var urlSplit = url.Split("/");
                var nombreArchivo = urlSplit[urlSplit.Length - 1];
                url = _blobStorage._url + "/Instalacion/" + url;
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

        [HttpGet]
        public async Task<ActionResult> Registro()
        {
            var result = new JsonResultDto();

            try
            {
                ViewBag.Verificentro = _userResolver.GetUser().NombreVerificentro;
                result.Result = await this.RenderViewToStringAsync("~/Views/Instalaciones/_Registro.cshtml");
            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }

            return Json(result);
        }

        public async Task<JsonResult> ProveedorAutocomplete()
        {
            JsonResult result = new JsonResult("");

            var prefix = Request.Query["q"].FirstOrDefault();
            int page = Convert.ToInt32(Request.Query["page"].FirstOrDefault() ?? "0");
            int records = Convert.ToInt32(Request.Query["records"].FirstOrDefault() ?? "0");
            var start = 0;
            var end = start + records;

            var listaResult = await InstalacionHelper.ProveedorAutocomplete(_proxyWebAPI, prefix);
            var proveedor = listaResult.Result as List<BusquedaProveedorRequest> ?? new List<BusquedaProveedorRequest>();
            //var result = proveedor.Select(x => new { Id = x.id.ToString(), Nombre = x.NombreProveedor + " /" + x.Email});
            return Json(new { items = proveedor, total_count = proveedor.Count });
        }

        [HttpPost]
        public async Task<IActionResult> CreateInstalacion(InstalacionRequestData registro)
        {
            var result = new JsonResultDto();
            try
            {
                var apiRequest = new InstalacionApiRequestData();
                apiRequest.IdProveedor = registro.IdProveedor;

                var usuarioActual = _userResolver.GetUser();

                if (usuarioActual == null)
                {
                    result.Error = true;
                    result.ErrorDescription = _mensajeErrorCatch;
                    return Json(result);
                }
                apiRequest.IdUserRegistro = usuarioActual.IdUser;
                apiRequest.IdVerificentro = _userResolver.GetUser().IdVerificentro;

                apiRequest.File = JsonConvert.DeserializeObject<List<InstalacionDoc>>(registro.FileString ?? "");

                var helper = await InstalacionHelper.GuardarInstalacion(_proxyWebAPI, apiRequest);

                if (!helper.IsSuccessFully)
                {
                    result.Error = true;
                    result.ErrorDescription = "Ocurrió un error al guardar la información";
                    return Json(result);
                }
                result.Error = false;
                result.ErrorDescription = "Se guardo la información correctamente";

            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }
            return Json(result);
        }

        private void SetMenuLinkActive()
        {
            ViewBag.SolicitudFormaValorada = "here";
            ViewBag.Administracion = "";
        }
    }
}
