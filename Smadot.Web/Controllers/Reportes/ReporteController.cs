using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Reportes.Request;
using Smadot.Models.Entities.Reportes.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.FileStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.ReporteTercerTanto;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using System.Globalization;
using System.Linq.Dynamic.Core;
using static Smadot.Models.Entities.Reportes.Response.ReporteTercerTantoResponseData;

namespace Smadot.Web.Controllers.Reportes
{
    [Authorize]
    public class ReporteController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        // StorageManager _storageManager = new StorageManager();

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public ReporteController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuReporteTercerTanto.PermisoMenuPadre))
                return new UnauthorizedResult();
            ViewBag.UserName = _userResolver.GetUser().Nombre;
            return View("~/Views/Reporte/Index.cshtml");
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

            var gridData = new ResponseGrid<ReporteTercerTantoResponseGrid>();
            var listaResult = await ReporteTercerTantoHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<ReporteTercerTantoResponse> ?? new List<ReporteTercerTantoResponse>();

            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<ReporteTercerTantoResponseGrid>();

            foreach (var row in data)
            {
                List.Add(new ReporteTercerTantoResponseGrid
                {
                    NumeroEntrega = row.NumeroEntrega,
                    FechaInicial = row.FechaInicial,
                    FechaFinal = row.FechaFinal,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Reporte/_Acciones.cshtml", row)
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
            var result = new JsonResultDto();

            try
            {
                ViewBag.Verificentro = _userResolver.GetUser().NombreVerificentro;
                result.Result = await this.RenderViewToStringAsync("~/Views/Reporte/_Registro.cshtml");
            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] ReporteTercerTantoRequestData registro)
        {
            var result = new JsonResultDto();
            ReporteCertificadoRequest reporteCertificadoRequest = new();
            try
            {
                if(registro == null)
                {
                    result.Error = true;
                    result.ErrorDescription = "Todos los parametros * son requeridos";
                    return new JsonResult(result);
                }
                List<ReporteCertificado> reporteCertificado = new();
                bool isValidCertf = false;
                foreach (var values in registro.DatosRegistro)
                {
                    var certificado = new ReporteCertificado();
                    if (values.Count() > 1) { 
                        if (values[1] != null)
                        {
                            certificado.IdCatTipoCertificado = int.Parse(values[1].ToString());
                            certificado.Cantidad = int.Parse(values[2].ToString());
                            certificado.SerieInicial = int.Parse(values[3].ToString());
                            certificado.SerieFinal = int.Parse(values[4].ToString());

                            if (certificado.SerieInicial <= certificado.SerieFinal && (certificado.SerieInicial + certificado.Cantidad - 1 == certificado.SerieFinal))
                            {
                                reporteCertificado.Add(certificado);
                                isValidCertf = true;
                            }
                            else if (certificado.Cantidad != 0)
                            {
                                result.Error = true;
                                result.ErrorDescription = "La serie inicial debe ser menor que la final o la cantidad total es erronea";
                                return new JsonResult(result);
                            }
                        }
                    }
                }
                if (!isValidCertf)
                {
                    result.Error = true;
                    result.ErrorDescription = "Debe registrarse al menos un tipo de Certificado";
                    return new JsonResult(result);
                }

                var usuarioActual = _userResolver.GetUser();

                if (usuarioActual == null)
                {
                    result.Error = true;
                    result.ErrorDescription = _mensajeErrorCatch;
                    return Json(result);
                }

                reporteCertificadoRequest.NumeroCuenta = registro.NumeroEntrega;
                reporteCertificadoRequest.FechaInicial = DateTime.ParseExact(registro.FechaInicial, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                reporteCertificadoRequest.FechaFinal = DateTime.ParseExact(registro.FechaFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                reporteCertificadoRequest.IdUserRegistro = usuarioActual.IdUser;
                reporteCertificadoRequest.ReporteCertificados = reporteCertificado;
                reporteCertificadoRequest.IdVerificentro = _userResolver.GetUser().IdVerificentro;
                reporteCertificadoRequest.File = JsonConvert.DeserializeObject<List<ReporteCertificadoDoc>>(registro.FileString ?? "");

                if (reporteCertificadoRequest.FechaInicial >= reporteCertificadoRequest.FechaFinal)
                {
                    result.Error = true;
                    result.ErrorDescription = "La Fecha Inicial debe ser menor a la Fecha Final";
                    return new JsonResult(result);
                }

                var helper = await ReporteTercerTantoHelper.GuardarReporteTercerTanto(_proxyWebAPI, reporteCertificadoRequest);

                if (!helper.IsSuccessFully)
                {
                    if (helper.Message == "InvalidNumber")
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
                result.ErrorDescription = "Ocurrió un error al guardar la información: " + ex.Message;
                return Json(result);
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<ActionResult> Detalle(long id)
        {
            var result = new JsonResultDto();

            try
            {
                var res = new JsonResultDto();
                var listaResult = await ReporteTercerTantoHelper.GetById(_proxyWebAPI, id);
                var folio = listaResult.Result as ReporteTercerTantoResponse ?? new ReporteTercerTantoResponse();
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
                ViewBag.UrlDocumento = VBUrlDoc1;
                result.Result = await this.RenderViewToStringAsync("~/Views/Reporte/_Detalle.cshtml", folio);
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
                url = _blobStorage._url + "/ReporteTercerTanto/" + url;
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
            ViewBag.SolicitudFormaValorada = "here";
            ViewBag.Administracion = "";
        }
    }
}
