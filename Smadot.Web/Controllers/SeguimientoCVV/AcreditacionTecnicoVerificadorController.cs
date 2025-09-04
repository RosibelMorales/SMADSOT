using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.AcreditacionTecnicoVerificador.Request;
using Smadot.Models.Entities.AcreditacionTecnicoVerificador.Response;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Refrendo.Request;
using Smadot.Models.Entities.Refrendo.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Helper.Operaciones.DirectorioCentrosVerificacion;
using Smadot.Web.Utilities;
using Smadot.Web.Handler.Autorizacion;

namespace Smadot.Web.Controllers.SeguimientoCVV
{
    [Authorize]
    public class AcreditacionTecnicoVerificadorController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuider;
        private readonly BlobStorage _blobStorage;

        public AcreditacionTecnicoVerificadorController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(MenuAcreditaciones.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");

            var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            //Invoca al método que se encarga de realizar la petición Api
            var request = new SolicitudFormaValoradaListRequest();
            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

            var listaResult = await AcreditacionTecnicoVerificadorHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as ResponseGrid<AcreditacionTecnicoVerificadorGridResponse> ?? new ResponseGrid<AcreditacionTecnicoVerificadorGridResponse>();

            foreach (var row in data.Data ?? new List<AcreditacionTecnicoVerificadorGridResponse>())
            {
                row.Acciones = await this.RenderViewToStringAsync("~/Views/AcreditacionTecnicoVerificador/_Acciones.cshtml", row);
                row.UrlAprobacionBase64 = "";
                if (!string.IsNullOrEmpty(row.UrlAprobacion))
                {
                    var arrDoc = row.UrlAprobacion.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(row.UrlAprobacion, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageDoc3 = string.Format("{0}{1}", ContentType1, archivo.Result);
                        row.UrlAprobacionBase64 = imageDoc3;
                    }
                }
            }
            data.Draw = draw;

            return Json(data);
        }

        [HttpGet]
        public async Task<ActionResult> Autorizar(long id)
        {
            var result = new ResponseViewModel(true);

            try
            {
                var response = new AcreditacionTecnicoVerificadorGridResponse();
                var helper = await AcreditacionTecnicoVerificadorHelper.GetById(_proxyWebAPI, id);
                if (helper.IsSuccessFully) 
                {
                    var resulthelper = helper.Result as AcreditacionTecnicoVerificadorGridResponse ?? new AcreditacionTecnicoVerificadorGridResponse();
                    response = resulthelper;
                }

                result.Result = await this.RenderViewToStringAsync("~/Views/AcreditacionTecnicoVerificador/_Autorizar.cshtml", response);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Autorizar([FromBody] AutorizarAcreditacionRequest request)
        {
            var result = new ResponseViewModel(true);
            try
            {
                //if (!string.IsNullOrEmpty(registro.Telefono))
                //{
                //    var convert = Encoding.ASCII.GetBytes(registro.Telefono.Trim());

                //    registro.NumeroTelefono = convert;
                //}
                request.UrlAprobacion = JsonConvert.DeserializeObject<List<AcreditacionTecnicoFile>>(request.UrlAprobacionString ?? "");
                request.Estatus = true;
                var helper = await AcreditacionTecnicoVerificadorHelper.AutorizarAcreditacion(_proxyWebAPI, request);

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

        [HttpPost]
        public async Task<IActionResult> Rechazar([FromBody] AutorizarAcreditacionRequest request)
        {
            var result = new ResponseViewModel(true);
            try
            {
                //if (!string.IsNullOrEmpty(registro.Telefono))
                //{
                //    var convert = Encoding.ASCII.GetBytes(registro.Telefono.Trim());

                //    registro.NumeroTelefono = convert;
                //}
                request.UrlAprobacion = JsonConvert.DeserializeObject<List<AcreditacionTecnicoFile>>(request.UrlAprobacionString ?? "");
                request.Estatus = false;
                var helper = await AcreditacionTecnicoVerificadorHelper.AutorizarAcreditacion(_proxyWebAPI, request);

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

        public async Task<IActionResult> Registro()
        {
            if (!_userResolver.HasPermission(MenuAcreditaciones.Registrar))
                return new UnauthorizedResult();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro([FromBody] AcreditacionTecnicoSolicitudRequest request)
        {
            var result = new ResponseViewModel(true);
            try
            {
                //if (!string.IsNullOrEmpty(registro.Telefono))
                //{
                //    var convert = Encoding.ASCII.GetBytes(registro.Telefono.Trim());

                //    registro.NumeroTelefono = convert;
                //}
                
                var helper = await AcreditacionTecnicoVerificadorHelper.Registro(_proxyWebAPI, request);

                if (!string.IsNullOrEmpty((string)helper.Message))
                {
                    result.IsSuccessFully = false;
                    result.Message = ((string)helper.Message);
                    return new JsonResult(result);
                }
                result.IsSuccessFully = helper.IsSuccessFully;
                result.Result = helper.Result;

            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        public async Task<IActionResult> Evidencia(string solicitud)
        {
            if (!_userResolver.HasPermission(MenuAcreditaciones.Evidencia))
                return new UnauthorizedResult();
            var response = new AcreditacionTecnicoEvidenciaResponse();
            var vm = new AcreditacionTecnicoEvidenciaRequest();
            var helper = await AcreditacionTecnicoVerificadorHelper.GetEvidenciaBySolicitud(_proxyWebAPI, solicitud);
            if (helper.IsSuccessFully)
            {
                var resulthelper = helper.Result as AcreditacionTecnicoEvidenciaResponse ?? new AcreditacionTecnicoEvidenciaResponse();

                if (resulthelper.EstatusAcreditacionSolicitud != EstatusAcreditacion.ApruebaDVRF)
                    return RedirectToAction("Index");

                response = resulthelper;

                vm.Id = response.Id;
                vm.EvidenciaEmpleados = response.ListIdUserPuestoVerificentros;
                vm.NumeroSolicitud = response.NumeroSolicitud;
                vm.TipoTramite = response.TipoTramite;
                vm.NumeroAcreditacion = response.NumeroAcreditacion;
                vm.NumeroReferencia = response.NumeroReferencia;
                vm.FechaAcreditacion = response.FechaAcreditacion;
                vm.FechaAmpliacion = response.FechaAmpliacion;
                vm.FechaEmision = response.FechaEmision;
                vm.DocumentoUrl = response.UrlAcreditacion;

            }
            var catalogoTipoAcreditacion = await TipoAcreditacionHelper.Consulta(_proxyWebAPI, new TipoAcreditacionRequest { Activo = true });
            ViewBag.Tipo = (catalogoTipoAcreditacion.Result as List<TipoAcreditacionResponse> ?? new List<TipoAcreditacionResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();

            var catalogoNormaAcreditacion = await NormaAcreditacionHelper.Consulta(_proxyWebAPI, new NormaAcreditacionRequest {  });
            ViewBag.Norma = (catalogoNormaAcreditacion.Result as List<NormaAcreditacionResponse> ?? new List<NormaAcreditacionResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Clave }).ToList();


            var VBUrlDoc1 = "";
            if (!string.IsNullOrEmpty(vm.DocumentoUrl))
            {
                var arrDoc = vm.DocumentoUrl.Split("/");
                var NameDoc1 = arrDoc[arrDoc.Length - 1];
                var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                var archivo = await _blobStorage.DownloadFileAsync(vm.DocumentoUrl, true);
                if (archivo.IsSuccessFully)
                {
                    var imageDoc1 = string.Format("{0}{1}", ContentType1, archivo.Result);
                    VBUrlDoc1 = imageDoc1;
                }
            }
            ViewBag.UrlDoc1 = VBUrlDoc1;

            return View("~/Views/AcreditacionTecnicoVerificador/Evidencia.cshtml", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Evidencia([FromBody] AcreditacionTecnicoEvidenciaRequest request)
        {
            var result = new ResponseViewModel(true);
            try
            {
                //if (!string.IsNullOrEmpty(registro.Telefono))
                //{
                //    var convert = Encoding.ASCII.GetBytes(registro.Telefono.Trim());

                //    registro.NumeroTelefono = convert;
                //}

                if(request == null)
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error, revise su información e intente de nuevo";
                }

                request.UrlAcreditacion = JsonConvert.DeserializeObject<List<AcreditacionTecnicoFile>>(request.UrlAcreditacionString ?? "");

                var helper = await AcreditacionTecnicoVerificadorHelper.RegistroEvidencia(_proxyWebAPI, request);

                if (!string.IsNullOrEmpty((string)helper.Message))
                {
                    result.IsSuccessFully = false;
                    result.Message = ((string)helper.Message);
                }

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
            var registros = Request.Query["emp"].FirstOrDefault();
            //var reg = JsonConvert.DeserializeObject<List<EmpleadoTermRequest>>(registros ?? "");
            //var reg = JsonConvert.DeserializeObject<List<string>>(registros);
            int page = Convert.ToInt32(Request.Query["page"].FirstOrDefault() ?? "0");
            int records = Convert.ToInt32(Request.Query["records"].FirstOrDefault() ?? "0");
            var start = 0;
            var end = start + records;
            var listaResult = await AcreditacionTecnicoVerificadorHelper.Autocomplete(_proxyWebAPI, new EmpleadoAutocompletRequest { Term = term, Start = start, End = end, Emp = registros });
            var data = listaResult.Result as List<EmpleadoAutocompleteResponse> ?? new List<EmpleadoAutocompleteResponse>();

            return Json(new { items = data, total_count = data.Count });
        }

        [HttpGet]
        public async Task<IActionResult> DescargarDocumento(string url, long id)
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
                url = _blobStorage._url + "/AcreditacionTecnicoSolicitud/"+id+"/" + url;
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
        public async Task<IActionResult> DescargarDocumentoEvidencia(string url)
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
                url = _blobStorage._url + "/AcreditacionTecnicoEvidencia/" + url;
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
