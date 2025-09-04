using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Instalaciones.Request;
using Smadot.Models.Entities.OrdenServicio.Request;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.FileStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.SeguimientoCVV;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using static Smadot.Models.Entities.OrdenServicio.Response.OrdenServicioResponseData;

namespace Smadot.Web.Controllers.SeguimientoCVV
{
    [Authorize]
    public class OrdenServicioController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        // StorageManager _storageManager = new StorageManager();

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public OrdenServicioController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        // GET: Instalaciones
        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuOrdenesServicio.PermisoMenuPadre))
                return new UnauthorizedResult();
            ViewBag.UserName = _userResolver.GetUser().Nombre;
            return View("~/Views/OrdenServicio/Index.cshtml");
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

            var gridData = new ResponseGrid<OrdenServicioResponseGrid>();
            var listaResult = await OrdenServicioHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<OrdenServicioResponse> ?? new List<OrdenServicioResponse>();

            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<OrdenServicioResponseGrid>();

            foreach (var row in data)
            {
                List.Add(new OrdenServicioResponseGrid
                {
                    Id = row.Id,
                    Verificentro = row.Verificentro,
                    Equipo = row.Equipo,
                    Linea = row.Linea,
                    FechaRegistro = row.FechaRegistro,
                    FolioServicio = row.FolioServicio,
                    UserRegistro = row.UserRegistro,
                    Solución = row.Solución,
                    Acciones = await this.RenderViewToStringAsync("~/Views/OrdenServicio/_Acciones.cshtml", row)
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

        [HttpPost]
        public async Task<IActionResult> CreateOrdenServicio(OrdenServicioRequestData registro)
        {
            var result = new JsonResultDto();
            try
            {
                var apiRequest = new OrdenServicioApiRequestData
                {
                    IdEquipo = registro.IdEquipo,
                    TipoOrdenServicio = registro.TipoOrdenServicio,
                    Folio = registro.Folio,
                    Solucion = registro.Solucion,
                    NoSerieActual = registro.NoSerieActual,
                    //apiRequest.NoSerieAnterior = registro.NoSerieAnterior;
                    Motivo = registro.Motivo
                };
                var usuarioActual = _userResolver.GetUser();

                if (usuarioActual == null)
                {
                    result.Error = true;
                    result.ErrorDescription = _mensajeErrorCatch;
                    return Json(result);
                }
                apiRequest.IdUserRegistro = usuarioActual.IdUser;

                apiRequest.File = JsonConvert.DeserializeObject<List<OrdenServicioDoc>>(registro.FileString ?? "");

                var helper = await OrdenServicioHelper.GuardarOrdenServicio(_proxyWebAPI, apiRequest);

                if (!helper.IsSuccessFully)
                {
                    if(helper.Message == "InvalidFolio")
                    {
                        result.Error = true;
                        result.ErrorDescription = helper.Result.ToString();
                        return Json(result);
                    }
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

        [HttpGet]
        public async Task<IActionResult> DescargarDocumento(string url)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var urlSplit = url.Split("/");
                var nombreArchivo = urlSplit[urlSplit.Length - 1];
                url = _blobStorage._url + "/OrdenServicio/" + url;
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
        public async Task<ActionResult> Detalle(long id)
        {
            var result = new JsonResultDto();

            try
            {
                var res = new JsonResultDto();
                var listaResult = await OrdenServicioHelper.GetById(_proxyWebAPI, id);
                var folio = listaResult.Result as OrdenServicioResponse ?? new OrdenServicioResponse();
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
                result.Result = await this.RenderViewToStringAsync("~/Views/OrdenServicio/_Detalle.cshtml", folio);
            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }
            return Json(result);
        }

        public async Task<JsonResult> EquipoAutocomplete()
        {
            JsonResult result = new JsonResult("");

            var prefix = Request.Query["q"].FirstOrDefault();
            int page = Convert.ToInt32(Request.Query["page"].FirstOrDefault() ?? "0");
            int records = Convert.ToInt32(Request.Query["records"].FirstOrDefault() ?? "0");
            var start = 0;
            var end = start + records;

            var listaResult = await OrdenServicioHelper.EquipoAutocomplete(_proxyWebAPI, prefix);
            var proveedor = listaResult.Result as List<BusquedaProveedorRequest> ?? new List<BusquedaProveedorRequest>();
            return Json(new { items = proveedor, total_count = proveedor.Count });
        }

        [HttpGet]
        public async Task<ActionResult> Registro()
        {
            var result = new JsonResultDto();

            try
            {
                result.Result = await this.RenderViewToStringAsync("~/Views/OrdenServicio/_Registro.cshtml");
            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> CatTipoOrdenServicio()
        {
            var listaResult = await OrdenServicioHelper.CatTipoOrdenServicio(_proxyWebAPI);
            var proveedor = listaResult.Result as List<BusquedaProveedorRequest> ?? new List<BusquedaProveedorRequest>();
            return Json(proveedor);
        }

        private void SetMenuLinkActive()
        {
            ViewBag.SolicitudFormaValorada = "here";
            ViewBag.Administracion = "";
        }
    }
}
