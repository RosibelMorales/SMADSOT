using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.IngresoFormaValorada.Response;
using Smadot.Models.Entities.Refrendo.Request;
using Smadot.Models.Entities.SeguimientoCVV.Request;
using Smadot.Models.Entities.SeguimientoCVV.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.IngresoFormaValorada;
using Smadot.Web.Helper.Operaciones.SeguimientoCVV;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Utilities;
using Smadot.Utilities.BlobStorage;
using System.Web.WebPages;
using System.IO;
using Smadot.Models.Entities.Motivos.Response;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Record;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Models.ConsultaStockDVRFModel;
using Smadot.Web.Handler.Autorizacion;
using static iTextSharp.text.pdf.AcroFields;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.FoliosFormaValorada.Request;
using Smadot.Models.Entities.FoliosFormaValorada.Response;
using Smadot.Models.Entities.Linea.Response;
using Smadot.Web.Helper.Operaciones.FoliosFormaValorada;
using Smadot.Web.Views.Shared;

namespace Smadot.Web.Controllers.SeguimientoCVV
{
    [Authorize]
    public class CierreAperturaLineaController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuider;
        private readonly BlobStorage _blobStorage;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public CierreAperturaLineaController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuLineas.PermisoMenuPadre))
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
            var request = new SeguimientoCVVListRequest();
            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();
            //long i;
            //if (long.TryParse(Request.Form["idAlmacen"].FirstOrDefault(), out i)) request.IdAlmacen = i;
            //else request.IdAlmacen = null;

            var listaResult = await CierreAperturaLineaHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<SeguimientoCVVResponse> ?? new List<SeguimientoCVVResponse>();

            ////Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total.Value;

            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? totalRecord : data.Count();

            var List = new List<SeguimientoCVVResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new SeguimientoCVVResponseGrid
                {
                    Id = row.Id,
                    IdVerificentro = row.IdVerificentro,
                    NombreVerificentro = row.NombreVerificentro,
                    Clave = row.Clave,
                    IdUserRegistro = row.IdUserRegistro,
                    NombreUsuario = row.NombreUsuario,
                    NombreLinea = row.NombreLinea,
                    FechaRegistroLinea = row.FechaRegistroLinea,
                    FechaRegistroLineaMotivo = row.FechaRegistroLineaMotivo,
                    Estatus = row.Estatus,
                    Motivo = row.Motivo,
                    IdLineaMotivo = row.IdLineaMotivo,
                    Acciones = await this.RenderViewToStringAsync("~/Views/CierreAperturaLinea/_Acciones.cshtml", row)
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
        public async Task<JsonResult> ConsultaDetalle(long id)
        {
            JsonResult result = new JsonResult("");

            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int page = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            //Invoca al método que se encarga de realizar la petición Api
            var request = new RequestListLinea();
            request.Pagina = page / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = searchValue;
            request.ColumnaOrdenamiento = sortColumn;
            request.Ordenamiento = sortColumnDirection;
            request.IdLinea = id;

            var response = await CierreAperturaLineaHelper.DetalleLinea(_proxyWebAPI, request);

            if (response.IsSuccessFully)
            {
                var jsonReponse = (ResponseGrid<LineaDetalle>)response.Result;
                jsonReponse.Data.ForEach(async x =>
                {

                    x.Documento = x.DocumentoGrid == null ? string.Empty : await this.RenderViewToStringAsync("~/Views/Shared/_documento.cshtml", x.DocumentoGrid);
                });
                jsonReponse.Draw = draw;
                return Json(jsonReponse);
            }

            return Json("");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == 0 || id == null)
            {
                var result = new ResponseViewModel(true);
                var vm = new SeguimientoCVVResponse();

                try
                {
                    var response = await CierreAperturaLineaHelper.Consulta(_proxyWebAPI, id ?? 0);
                    vm = response.Result as SeguimientoCVVResponse ?? vm;
                    
                    ViewBag.Motivos = (vm.Motivos ?? new List<MotivosReponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
                    result.Result = await this.RenderViewToStringAsync("~/Views/CierreAperturaLinea/_Registro.cshtml", vm);
                }
                catch (Exception ex)
                {
                    result.IsSuccessFully = false;
                    result.Message = ex.Message;
                }

                return new JsonResult(result);
            }
            else
            {
                var result = new ResponseViewModel(true);
                var detalle = new RequestListLinea();
                var vm = new SeguimientoCVVResponse();

                try
                {
                    var response = await CierreAperturaLineaHelper.GetByIdLineaCA(_proxyWebAPI, id ?? 0);
                    vm = response.Result as SeguimientoCVVResponse ?? vm;
                    vm.UrlDocumentoBase64 = "";
                    var VBUrlDoc1 = "";
                    if (!string.IsNullOrEmpty(vm.UrlDocumento))
                    {
                        var arrDoc = vm.UrlDocumento.Split("/");
                        var NameDoc1 = arrDoc[arrDoc.Length - 1];
                        var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                        var archivo = await _blobStorage.DownloadFileAsync(vm.UrlDocumento, true);
                        if (archivo.IsSuccessFully)
                        {
                            var imageUrlDocumento = string.Format("{0}{1}", ContentType1, archivo.Result);
                            vm.UrlDocumentoBase64 = imageUrlDocumento;
                        }
                    }
                    result.Result = await this.RenderViewToStringAsync("~/Views/CierreAperturaLinea/_Detalles.cshtml", vm);
                }
                catch (Exception ex)
                {
                    result.IsSuccessFully = false;
                    result.Message = ex.Message;
                }

                return new JsonResult(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(List<SeguimientoCVVResponse> registro)
        {
            var result = new ResponseViewModel(true);
            try
            {
                foreach (var item in registro)
                {
                    item.FechaRegistroLinea = DateTime.Now;
                    item.FechaRegistroLineaMotivo = DateTime.Now;
                    item.IdVerificentro = _userResolver.GetUser().IdVerificentro;
                    item.IdUserRegistro = _userResolver.GetUser().IdUser;

                    item.Files = JsonConvert.DeserializeObject<List<SeguimientoCVVFiles>>(Request.Form["Files"].FirstOrDefault() ?? "");

                    if (item.Files == null)
                    {
                        result.Message = "No se encontro ningun archivo cargado, intenta de nuevo";
                        result.IsSuccessFully = false;
                        return new JsonResult(result);
                    }
                    if (item.IdMotivo == null || item.IdMotivo <= 0)
                    {
                        result.Message = "Selecciona un motivo para continuar";
                        result.IsSuccessFully = false;
                        return new JsonResult(result);
                    }
                }

                var save = await CierreAperturaLineaHelper.Edit(_proxyWebAPI, registro);
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

        [HttpGet]
        public async Task<IActionResult> Update(long? id)
        {

            var result = new ResponseViewModel(true);
            var vm = new SeguimientoCVVResponse();
            try
            {
                var response = await CierreAperturaLineaHelper.Consulta(_proxyWebAPI, id ?? 0);
                vm = response.Result as SeguimientoCVVResponse ?? vm;
                vm.Motivos ??= new List<MotivosReponse>();
                ViewBag.Motivos = vm.Motivos.Select(x => new SelectListItem { Text = x.Nombre, Value = x.Id.ToString() }).ToList();

                result.Result = await this.RenderViewToStringAsync("~/Views/CierreAperturaLinea/_Cierre.cshtml", vm);

                //ViewBag.Almacenes = (vm[0].Almacenes ?? new List<AlmacenResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();

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
                //var archivo = new StorageManager().ObtenerArchivo(url);
                //if (string.IsNullOrEmpty(archivo))
                //{
                //    result.IsSuccessFully = false;
                //    throw new Exception("No sé encontró el documento.");
                //}
                url = _blobStorage._url + "/AperturaLinea/" + url;
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
        public async Task<IActionResult> DescargarDocumentoCierre(string url)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var urlSplit = url.Split("/");
                var nombreArchivo = urlSplit[urlSplit.Length - 1];
                //var archivo = new StorageManager().ObtenerArchivo(url);
                //if (string.IsNullOrEmpty(archivo))
                //{
                //    result.IsSuccessFully = false;
                //    throw new Exception("No sé encontró el documento.");
                //}
                url = _blobStorage._url + "/CierreLinea/" + url;
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


    }
}
