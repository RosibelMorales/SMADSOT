using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Exento.Request;
using Smadot.Models.Entities.Exento.Response;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Utilities.Modelos;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Web.Utilities;
using Smadot.Web.Models;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.CicloVerificacion.Response;
using Smadot.Web.Models.Exento;
using System;
using Newtonsoft.Json;
using Smadot.Utilities.BlobStorage;
using Smadot.Models.Dicts;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Utilities.GestionTokens;
using Smadot.Models.Entities.Refrendo.Response;


namespace Smadot.Web.Controllers.SolicitudFormaValorada
{
    [Authorize]
    public class ExentoController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly BlobStorage _blobStorage;
        private readonly IUserResolver _userResolver;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public ExentoController(IProxyWebAPI proxyWebAPI, IConfiguration configuration, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _userResolver = userResolver;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(MenuExentos.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/Exento/Index.cshtml");
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
            var request = new ExentoRequest();
            request.Pagina = page;
            request.Registros = pageSize;
            request.Busqueda = searchValue;
            request.ColumnaOrdenamiento = sortColumn;
            request.Ordenamiento = sortColumnDirection;

            var listResult = await ExentoHelper.Consulta(_proxyWebAPI, request);
            var data = listResult.Result as List<vExentoResponse> ?? new List<vExentoResponse>();

            ////Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<vExentoResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new vExentoResponseGrid
                {
                    Id = row.Id,
                    Placa = row.Placa,
                    Serie = row.Serie,
                    Marca = row.Marca,
                    Submarca = row.Submarca,
                    Modelo = row.Modelo,
                    Fecha = row.Fecha,
                    IdCatTipoCertificado = row.IdCatTipoCertificado,
                    Nombre = row.Nombre,
                    Vigencia = row.Vigencia,
                    Total = totalRecord,
                    IdFoliosFormaValoradaVerificentro = row.IdFoliosFormaValoradaVerificentro,
                    Folio = row.Folio,
                    ClaveTramite = row.ClaveTramite,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Exento/_Acciones.cshtml", row)
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
        public async Task<JsonResult> Detalle(long id)
        {
            var result = new ResponseViewModel(true);
            var vm = new ExentoResponse();

            try
            {
                var response = await ExentoHelper.Detalle(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                vm = response.Result as ExentoResponse ?? vm;
                if (!string.IsNullOrEmpty(vm.UrlDoc1))
                {
                    var arrDoc = vm.UrlDoc1.Split("/");
                    ViewBag.NameDoc1 = arrDoc[arrDoc.Length - 1];
                    ViewBag.ContentType1 = ViewBag.NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm.UrlDoc1, true);
                    if (archivo.IsSuccessFully)
                        ViewBag.Doc1 = archivo.Result;
                }
                if (!string.IsNullOrEmpty(vm.UrlDoc2))
                {
                    var arrDoc = vm.UrlDoc2.Split("/");
                    ViewBag.NameDoc2 = arrDoc[arrDoc.Length - 1];
                    ViewBag.ContentType2 = ViewBag.NameDoc2.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";

                    var archivo = await _blobStorage.DownloadFileAsync(vm.UrlDoc2, true);
                    if (archivo.IsSuccessFully)
                        ViewBag.Doc2 = archivo.Result;
                }
                if (!string.IsNullOrEmpty(vm.UrlDoc3))
                {

                    var arrDoc = vm.UrlDoc3.Split("/");
                    ViewBag.NameDoc3 = arrDoc[arrDoc.Length - 1];
                    ViewBag.ContentType3 = ViewBag.NameDoc3.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";

                    var archivo = await _blobStorage.DownloadFileAsync(vm.UrlDoc3, true);
                    if (archivo.IsSuccessFully)
                        ViewBag.Doc3 = archivo.Result;
                }
                if (!string.IsNullOrEmpty(vm.UrlDoc4))
                {
                    var arrDoc = vm.UrlDoc4.Split("/");
                    ViewBag.NameDoc4 = arrDoc[arrDoc.Length - 1];
                    ViewBag.ContentType4 = ViewBag.NameDoc4.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";

                    var archivo = await _blobStorage.DownloadFileAsync(vm.UrlDoc4, true);
                    if (archivo.IsSuccessFully)
                        ViewBag.Doc4 = archivo.Result;
                }
                var listaResult = await RefrendoHelper.Consulta(_proxyWebAPI, id);
                var dataResponse = listaResult.Result as ResponseGrid<RefrendoGridResponse> ?? new ResponseGrid<RefrendoGridResponse>();
                ViewBag.ListaRefrendos = dataResponse.Data ?? new List<RefrendoGridResponse>();
                result.Result = await this.RenderViewToStringAsync("~/Views/Exento/_ModalDetalle.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }
        [HttpGet]
        public async Task<JsonResult> ListRefrendos(long id)
        {
            var result = new ResponseViewModel(true);
            var vm = new List<RefrendoGridResponse>();

            try
            {
            
                var listaResult = await RefrendoHelper.Consulta(_proxyWebAPI, id);
                var dataResponse = listaResult.Result as ResponseGrid<RefrendoGridResponse> ?? new ResponseGrid<RefrendoGridResponse>();
                vm = dataResponse.Data ?? new List<RefrendoGridResponse>();
                result.IsSuccessFully=listaResult.IsSuccessFully;
                result.Message=listaResult.Message;
                result.Result = await this.RenderViewToStringAsync("~/Views/Refrendo/_refrendos.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }
        [HttpGet]
        public async Task<IActionResult> Registro(long? id)
        {
            var result = new ResponseViewModel(true);
            var vm = new ExentoViewModel();
            try
            {
                var catalogo = await TipoCertificadoHelper.Consulta(_proxyWebAPI, new TipoCertificadoRequest { Activo = true });
                vm.Certificados = (catalogo.Result as List<TipoCertificadoResponse> ?? new List<TipoCertificadoResponse>()).Where(x => x.Id == TipoCertificado.Exentos).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre, Selected = x.Id == TipoCertificado.Exentos }).ToList();
                ViewBag.Estados = EstadosMexicoDict.DictEstados.Select(x => new SelectListItem { Text = x.Nombre, Value = x.Clave, Selected = x.Nombre.Equals("PUEBLA") }).ToList();
                ViewBag.Tcombustible = TipoCombustible.DictTipoCombustible.ToList().Select(x => new SelectListItem { Value = x.Value.ToString(), Text = x.Value.ToString() }).ToList();
                return View("~/Views/Exento/Registro.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);

        }

        [HttpPost]
        public async Task<IActionResult> Registro(ExentoViewModel registro)
        {
            var result = new JsonResultDto();
            try
            {
                if (registro.Id == 0)
                {
                    registro.Placa = registro.Placa.Replace("-", "").ToUpper().Trim();
                    registro.Serie = registro.Serie.ToUpper().Trim();
                    registro.Files = JsonConvert.DeserializeObject<List<ExentosFiles>>(registro.FilesString ?? "");
                    var save = await ExentoHelper.Registro(_proxyWebAPI, registro);
                    result.Error = !save.IsSuccessFully;
                    result.ErrorDescription = save.Message;
                }
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

        [HttpDelete]
        public async Task<IActionResult> EliminarRegistro(long id)
        {
            var result = new JsonResultDto();
            try
            {
                if(id > 0)
                {
                    var save = await ExentoHelper.Eliminar(_proxyWebAPI, id);
                    result.Error = !save.IsSuccessFully;
                    result.ErrorDescription = save.Message;
                }
            }
            catch(Exception ex)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }
            return Json(result);
        }
    }
}
