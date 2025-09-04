using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.DevolucionSPF.Request;
using Smadot.Models.Entities.DevolucionSPF.Response;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Request;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.AdministracionStock;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using System.ComponentModel.DataAnnotations;
using TablaFijaViewModel = Smadot.Models.Entities.Generic.Response.TablaFijaViewModel;

namespace Smadot.Web.Controllers.AdministracionStock
{

    namespace Smadot.Web.Controllers.AdministracionStock
    {
        [Authorize]
        public class DevolucionSPFController : Controller
        {
            private readonly IProxyWebAPI _proxyWebAPI;
            private readonly IUserResolver _userResolver;
            private readonly BlobStorage _blobStorage;
            private readonly IPdfBuider _pdfBuilder;
            private readonly IExcelBuilder _excelBuilder;


            public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";
            public DevolucionSPFController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuilder, IExcelBuilder excelBuilder, IConfiguration configuration)
            {
                _proxyWebAPI = proxyWebAPI;
                _pdfBuilder = pdfBuilder;
                _excelBuilder = excelBuilder;
                _userResolver = userResolver;
                _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            }

            public IActionResult Index()
            {
                if (!_userResolver.HasPermission(PermisosMenu.DevolucionaSPF))
                    return new UnauthorizedResult();
                return View("~/Views/DevolucionSPF/Index.cshtml");
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

                var response = await AdministracionStockHelper.ConsultaDevlucionSPF(_proxyWebAPI, request);

                if (response.IsSuccessFully)
                {
                    var jsonReponse = (ResponseGrid<ListDevolucionSPFResponse>)response.Result;
                    jsonReponse.Data.ForEach(async x =>
                    {
                        x.Actions = await this.RenderViewToStringAsync("~/Views/DevolucionSPF/_Acciones.cshtml", x);
                    });
                    jsonReponse.Draw = draw;
                    return Json(jsonReponse);
                }

                return Json("");
            }

            [HttpGet]
            public async Task<IActionResult> Registro(long? id, bool? isReadOnly)
            {
                var result = new ResponseViewModel(true);
                try
                {
                    var vmTablaFija = new TablaFijaViewModel.TablaFijaMain();
                    DevolucionSPFResponse vm = new DevolucionSPFResponse { isReadOnly = isReadOnly ?? false };
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
                    var almacenes = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest() { Activo = true });
                    if (almacenes.IsSuccessFully)
                    {
                        ViewBag.almacenes = ((List<AlmacenResponse>)almacenes.Result).Where(x => x.IdVerificentro == null).Select(obj => new SelectListItem { Value = obj.Id.ToString(), Text = obj.Nombre }).ToList();
                    }
                    vm.FechaEntrega = DateTime.Now;
                    //vm.NumeroDevolucion = new Random().Next(1000, 2000);
                    var numDev = await AdministracionStockHelper.GetNumeroDevolucion(_proxyWebAPI);
                    var resNumDev = (int)numDev.Result;

                    vm.NumeroDevolucion = resNumDev;
                    vm.TablaFijaViewModel = new TablaFijaViewModel.TablaFijaMain().Consultar();

                    ViewBag.Enabled = true;
                    result.Result = await this.RenderViewToStringAsync("~/Views/DevolucionSPF/_Registro.cshtml", vm);
                }
                catch (Exception ex)
                {
                    result.IsSuccessFully = false;
                    result.Message = ex.Message;
                }

                return new JsonResult(result);
            }
            [HttpPost]
            public async Task<IActionResult> Registro(DevolucionSPFRequest devolucionSPF)
            {
                var result = new ResponseViewModel(true);
                try
                {
                    var files = JsonConvert.DeserializeObject<List<DevolucionSPFFiles>>(Request.Form["Files"].FirstOrDefault() ?? "");
                    devolucionSPF.Files = files;

                    result = await AdministracionStockHelper.GuardarDevolucionSPF(_proxyWebAPI, devolucionSPF);
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
            public async Task<IActionResult> Detalle(long id)
            {
                var result = new ResponseViewModel(true);
                try
                {
                    //var vmTablaFija = new TablaFijaViewModel.TablaFijaMain().Consultar();
                    //var vm = new DevolucionSPFResponse
                    //{
                    //    FechaEntrega = DateTime.Now,
                    //    NumeroDevolucion = new Random().Next(1000, 2000),
                    //    TablaFijaViewModel = vmTablaFija
                    //};
                    result = await AdministracionStockHelper.ConsultaByIdDevolucion(_proxyWebAPI, id);
                    if (!result.IsSuccessFully)
                        throw new ValidationException("No se encontró el registro.");
                    var almacenes = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest() { Activo = true });
                    if (almacenes.IsSuccessFully)
                    {
                        ViewBag.almacenes = ((List<AlmacenResponse>)almacenes.Result).Select(obj => new SelectListItem { Value = obj.Id.ToString(), Text = obj.Nombre }).ToList();
                    }

                    var vm = result.Result as DevolucionSPFResponse ?? new DevolucionSPFResponse();
                    var VBUrlDoc1 = "";
                    var VBUrlDoc2 = "";
                    var VBUrlDoc3 = "";
                    if (!string.IsNullOrEmpty(vm.UrlDoc1))
                    {
                        var arrDoc = vm.UrlDoc1.Split("/");
                        var NameDoc1 = arrDoc[arrDoc.Length - 1];
                        var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                        var archivo = await _blobStorage.DownloadFileAsync(vm.UrlDoc1, true);
                        if (archivo.IsSuccessFully)
                        {
                            var imageURLFotoTecnico = string.Format("{0}{1}", ContentType1, archivo.Result);
                            VBUrlDoc1 = imageURLFotoTecnico;
                        }
                    }
                    if (!string.IsNullOrEmpty(vm.UrlDoc2))
                    {
                        var arrDoc = vm.UrlDoc2.Split("/");
                        var NameDoc1 = arrDoc[arrDoc.Length - 1];
                        var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                        var archivo = await _blobStorage.DownloadFileAsync(vm.UrlDoc2, true);
                        if (archivo.IsSuccessFully)
                        {
                            var imageURLFotoTecnico = string.Format("{0}{1}", ContentType1, archivo.Result);
                            VBUrlDoc2 = imageURLFotoTecnico;
                        }
                    }
                    if (!string.IsNullOrEmpty(vm.UrlDoc3))
                    {
                        var arrDoc = vm.UrlDoc3.Split("/");
                        var NameDoc1 = arrDoc[arrDoc.Length - 1];
                        var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                        var archivo = await _blobStorage.DownloadFileAsync(vm.UrlDoc3, true);
                        if (archivo.IsSuccessFully)
                        {
                            var imageURLFotoTecnico = string.Format("{0}{1}", ContentType1, archivo.Result);
                            VBUrlDoc3 = imageURLFotoTecnico;
                        }
                    }
                    ViewBag.UrlDoc1 = VBUrlDoc1;
                    ViewBag.UrlDoc2 = VBUrlDoc2;
                    ViewBag.UrlDoc3 = VBUrlDoc3;
                    //result.Result = await this.RenderViewToStringAsync("~/Views/DevolucionSPF/_Registro.cshtml", vm);
                    result.Result = await this.RenderViewToStringAsync("~/Views/DevolucionSPF/_Registro.cshtml", (DevolucionSPFResponse)result.Result);
                }
                catch (Exception ex)
                {
                    result.IsSuccessFully = false;
                    result.Message = ex.Message;
                }

                return new JsonResult(result);
            }

            [HttpPost]
            public async Task<JsonResult> NumeroSolicitudAutocomplete(string prefix)
            {
                //    var result = new List<string>
                //{
                //    "1092",
                //    "1083",
                //    "1074",
                //    "1065",
                //    "1056",
                //    "1047",
                //    "1038",
                //    "1029",
                //    "1010"
                //};
                //    return Json(result);
                var listaResult = await AdministracionStockHelper.AutocompleteSolicitudFV(_proxyWebAPI, prefix);
                var folio = listaResult.Result as List<SolicitudFV> ?? new List<SolicitudFV>();
                var result = folio.Select(x => new { Id = x.Id, Nombre = x.Id });
                return Json(result);
            }

            [HttpPost]
            public async Task<JsonResult> ResponsableEntregaAutocomplete(string prefix)
            {
                //    var result = new List<string>
                //{
                //    "Oscar Hernandez",
                //    "Manuel Hernandez",
                //    "Ada Hernandez",
                //    "Jaime Perez",
                //    "Luis Ramirez",
                //    "Adrian Marin",
                //    "Pepe Lopez",
                //    "Raul Fernandez",
                //    "Rene Sosa"
                //};
                //    return Json(result);
                var listaResult = await AdministracionStockHelper.AutocompleteResponsable(_proxyWebAPI, prefix);
                var folio = listaResult.Result as List<User> ?? new List<User>();
                var result = folio.Select(x => new { Id = x.Id, Nombre = x.Nombre });
                return Json(result);
            }

            [HttpPost]
            public async Task<IActionResult> ActualizarTablaFija(long almacen)
            {
                var result = new ResponseViewModel(true);
                try
                {
                    ViewBag.Enabled = true;

                    var helper = await AdministracionStockHelper.GetInventarioByAlmacen(_proxyWebAPI, almacen);
                    var vm = helper.Result as List<TablaFijaViewModel.TablaFijaRow> ?? new TablaFijaViewModel.TablaFijaMain().Consultar(); ;

                    //var vm = new TablaFijaViewModel.TablaFijaMain().Consultar();


                    result.IsSuccessFully = true;
                    result.Result = await this.RenderViewToStringAsync("~/Views/Shared/_TablaFija2.cshtml", vm);
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
                    url = _blobStorage._url + "/DevolucionSPF/" + url;
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


            #region Reportes

            [HttpGet]
            public async Task<IActionResult> CrearReporte(string search, string sortColumnDirection, string sortColumn, bool esReportePdf)
            {
                JsonResultDto result = new JsonResultDto();

                try
                {
                    var request = new RequestList
                    {
                        Busqueda = search,
                        ColumnaOrdenamiento = sortColumn,
                        Ordenamiento = sortColumnDirection,
                        Pagination = true
                    };

                    var listaResult = await AdministracionStockHelper.GetDevolucionSPFDocumento(_proxyWebAPI, request);
                    var dataResult = listaResult.Result as List<DevolucionSPFResponse> ?? new List<DevolucionSPFResponse>();

                    var data = dataResult.Select(x => new DevolucionSPFDocument()
                    {
                        NumeroDevolucion = x.NumeroDevolucion,
                        FechaRegistro = (DateTime)x.FechaRegistro,
                        FechaEntrega = (DateTime)x.FechaEntrega,
                        NumeroSolicitud = (long)x.NumeroSolicitud,
                        UsuarioAprobo = x.UsuarioAprobo,
                        RecibioSPF = x.RecibioSPF,
                        ResponsableEntrega = x.ResponsableEntrega,
                        Direccion = x.Direccion,
                        NombreC = x.NombreEncargado,
                        NombreEncargado = x.NombreEncargado,
                        FolioCertificado = x.Folio,
                        Telefono = x.Telefono,
                        UrlRoot = DocumentDic.UrlRootPDF,
                        Cantidad = x.Cantidad,

                    }).ToList();

                    byte[] bytes;
                    if (esReportePdf)
                    {
                        var reportePDF = await _pdfBuilder.GetDevolucionSPF(data);
                        bytes = reportePDF.Response.DocumentoPDF;
                        return File(bytes, "application/pdf", reportePDF.Response.NombreDocumento);
                    }
                    else
                    {
                        var reporteExcel = await _excelBuilder.GetDocumentoDevolucionSPF(data);
                        bytes = reporteExcel.Response.DocumentoExcel;
                        HttpContext.Session.SetString("FileName", reporteExcel.Response.NombreDocumento);
                        HttpContext.Session.Set("FileXlsx", bytes);
                    }
                    result.Result = new
                    {
                        FileName = "Devolución SPF",
                        Bytes = bytes
                    };
                }
                catch (Exception ex)
                {
                    result.Error = true;
                    result.ErrorDescription = $"Error al generar el reporte | {ex.Message} --- {ex.StackTrace}";
                }
                return Json(result);
            }

            #endregion Reportes
        }
    }
}