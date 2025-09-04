using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.IngresoFormaValorada.Response;
using Smadot.Models.Entities.Motivos.Response;
using Smadot.Models.Entities.ReporteMensual.Request;
using Smadot.Models.Entities.ReporteMensual.Response;
using Smadot.Models.Entities.SeguimientoCVV.Request;
using Smadot.Models.Entities.SeguimientoCVV.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.IngresoFormaValorada;
using Smadot.Web.Helper.Operaciones.ReporteMensual;
using Smadot.Web.Helper.Operaciones.SeguimientoCVV;
using Smadot.Web.Utilities;
using Smadot.Utilities.BlobStorage;
using Smadot.Web.Handler.Autorizacion;

namespace Smadot.Web.Controllers.ReporteMensual
{
    [Authorize]
    public class ReporteMensualController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuider;
        private readonly BlobStorage _blobStorage;
        public ReporteMensualController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }
        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuReporteMensual.PermisoMenuPadre))
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
            var request = new ReporteMensualListRequest();
            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();
            //long i;
            //if (long.TryParse(Request.Form["idAlmacen"].FirstOrDefault(), out i)) request.IdAlmacen = i;
            //else request.IdAlmacen = null;

            var listaResult = await ReporteMensualHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<ReporteMensualResponse> ?? new List<ReporteMensualResponse>();

            ////Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total.Value;

            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? totalRecord : data.Count();

            var List = new List<ReporteMensualResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new ReporteMensualResponseGrid
                {
                    IdReporte = row.IdReporte,
                    NumeroReporte = row.NumeroReporte,
                    FechaModificacionReporte = row.FechaModificacionReporte,
                    FechaRegistroReporte = row.FechaRegistroReporte,
                    IdCatEstatusReporte = row.IdCatEstatusReporte,
                    NombreCatEstatusReporte = row.NombreCatEstatusReporte,
                    ActivoCatEstatusReporte = row.ActivoCatEstatusReporte,
                    IdUsuario = row.IdUsuario,
                    NombreUsuario = row.NombreUsuario,
                    UrlEntrega = row.UrlEntrega,
                    UrlExcel = row.UrlExcel,
                    Acciones = await this.RenderViewToStringAsync("~/Views/ReporteMensual/_Acciones.cshtml", row)
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
            if (id == 0 || id == null)
            {
                var result = new ResponseViewModel(true);
                var vm = new List<ReporteMensualResponse>
                {
                    new ReporteMensualResponse
                    {
                        NumeroReporte = 0
                    }
                };
                try
                {
                    var response = await ReporteMensualHelper.Consulta(_proxyWebAPI, id ?? 0);
                    vm = response.Result as List<ReporteMensualResponse> ?? vm;
                    result.Result = await this.RenderViewToStringAsync("~/Views/ReporteMensual/_Registro.cshtml", vm);
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
                var vm = new List<ReporteMensualResponse>
                {
                    new ReporteMensualResponse
                    {
                        NumeroReporte = 0
                    }
                };
                try
                {
                    var response = await ReporteMensualHelper.Consulta(_proxyWebAPI, id ?? 0);
                    vm = response.Result as List<ReporteMensualResponse> ?? vm;
                    //ViewBag.Almacenes = (vm[0].Almacenes ?? new List<AlmacenResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
                    var VBUrlDoc1 = "";
                    var VBUrlDoc2 = "";
                    if (!string.IsNullOrEmpty(vm[0].UrlEntrega))
                    {
                        var arrDoc = vm[0].UrlEntrega.Split("/");
                        var NameDoc1 = arrDoc[arrDoc.Length - 1];
                        var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                        var archivo = await _blobStorage.DownloadFileAsync(vm[0].UrlEntrega, true);
                        if (archivo.IsSuccessFully)
                        {
                            var imageUrlEntrega = string.Format("{0}{1}", ContentType1, archivo.Result);
                            VBUrlDoc1 = imageUrlEntrega;
                        }
                    }
                    if (!string.IsNullOrEmpty(vm[0].UrlExcel))
                    {
                        var arrDoc = vm[0].UrlExcel.Split("/");
                        var NameDoc1 = arrDoc[arrDoc.Length - 1];
                        var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                        var archivo = await _blobStorage.DownloadFileAsync(vm[0].UrlExcel, true);
                        if (archivo.IsSuccessFully)
                        {
                            var imageUrlExcel = string.Format("{0}{1}", ContentType1, archivo.Result);
                            VBUrlDoc2 = imageUrlExcel;
                        }
                    }
                    ViewBag.UrlFactura = VBUrlDoc1;
                    ViewBag.UrlExcel = VBUrlDoc2;
                    result.Result = await this.RenderViewToStringAsync("~/Views/ReporteMensual/_Detalles.cshtml", vm);
                }
                catch (Exception ex)
                {
                    result.IsSuccessFully = false;
                    result.Message = ex.Message;
                }

                return new JsonResult(result);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Update(long? id)
        {

            var result = new ResponseViewModel(true);
            var vm = new List<ReporteMensualResponse>
                {
                new ReporteMensualResponse
                {
                    NumeroReporte = 0
                }
            };
            try
            {
                var response = await ReporteMensualHelper.Consulta(_proxyWebAPI, id ?? 0);
                vm = response.Result as List<ReporteMensualResponse> ?? vm;
                result.Result = await this.RenderViewToStringAsync("~/Views/ReporteMensual/_Registro.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(List<ReporteMensualResponse> registro)
        {
            var result = new ResponseViewModel(true);
            try
            {
                
                if (registro[0].IdReporte > 0 && registro[0].NumeroReporte > 0)
                {
                    var tipoCertificado = new TipoCertificado();

                    int p = 0;
                    foreach (var obj in registro)
                    {
                        obj.NombreCatTipoCertificado = (string?)tipoCertificado.NombreCertificado[p];
                        if (registro[p].FolioFinal < registro[p].FolioInicial)
                        {
                            result.Message = "Verifica que el Folio Final sea mayor o igual al Folio Inicial - Clave del certificado: " + obj.NombreCatTipoCertificado;
                            result.IsSuccessFully = false;
                            return new JsonResult(result);
                        }
                        p++;
                    }

                    int i = 1;

                    foreach (var item in registro)
                    {
                        item.IdCatTipoCertificado = i;
                        item.IdUsuario = _userResolver.GetUser().IdUser;
                        item.Files = JsonConvert.DeserializeObject<List<ReporteMensualFiles>>(Request.Form["Files"].FirstOrDefault() ?? "");
                        i++;
                    }
                    var save = await ReporteMensualHelper.Edit(_proxyWebAPI, registro);
                    if (!save.IsSuccessFully)
                        throw new Exception(save.Message);

                    result.Message = "La información se actualizo correctamente.";

                }
                else if (registro[0].IdReporte > 0)
                {
                    registro[0].FechaModificacionReporte = DateTime.Now;
                    registro[0].IdUsuario = _userResolver.GetUser().IdUser;
                    var save = await ReporteMensualHelper.Edit(_proxyWebAPI, registro);
                    if (!save.IsSuccessFully)
                        throw new Exception(save.Message);

                    result.Message = "La información se actualizo correctamente.";
                }
                else
                {
                    var tipoCertificado = new TipoCertificado();

                    int p = 0;
                    foreach (var obj in registro)
                    {
                        obj.NombreCatTipoCertificado = (string?)tipoCertificado.NombreCertificado[p];
                        if (registro[p].FolioFinal < registro[p].FolioInicial)
                        {
                            result.Message = "Verifica que el Folio Final sea mayor o igual al Folio Inicial - Clave del certificado: " + obj.NombreCatTipoCertificado;
                            result.IsSuccessFully = false;
                            return new JsonResult(result);
                        }
                        p++;
                    }

                    int i = 1;

                    foreach (var item in registro)
                    {
                        item.IdCatTipoCertificado = i;
                        item.IdUsuario = _userResolver.GetUser().IdUser;
                        item.Files = JsonConvert.DeserializeObject<List<ReporteMensualFiles>>(Request.Form["Files"].FirstOrDefault() ?? "");
                        if (item.Files == null)
                        {
                            result.Message = "No se encontro ningun archivo cargado, intenta de nuevo";
                            result.IsSuccessFully = false;
                            return new JsonResult(result);
                        }
                        i++;
                    }
                    var save = await ReporteMensualHelper.Edit(_proxyWebAPI, registro);
                    if (!save.IsSuccessFully)
                    {
                        save.Message = "No se pueden almacenar 2 reportes en el mismo mes";
                        throw new Exception(save.Message);
                    }

                    result.Message = "La información se guardó correctamente.";

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
                url = _blobStorage._url + "/ReporteMensual/" + url;
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
