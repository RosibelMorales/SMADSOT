using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Refrendo.Request;
using Smadot.Models.Entities.Refrendo.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Utilities.FileStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using Smadot.Models.DataBase;
using Smadot.Utilities.BlobStorage;
using Microsoft.IdentityModel.Tokens;
using Smadot.Models.Entities.FoliosFormaValorada.Response;
using NPOI.POIFS.Crypt.Dsig;
using Smadot.Web.Helper.Operaciones.FoliosFormaValorada;
using Smadot.Models.Dicts;
using Smadot.Utilities.Seguridad;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.Entities.FoliosFormaValorada.Request;

namespace Smadot.Web.Controllers.SolicitudFormaValorada
{
    [Authorize]
    public class RefrendoController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuider;
        private readonly BlobStorage _blobStorage;
        private readonly string siteUrl;
        private readonly string _llaveCifrado;

        public RefrendoController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            siteUrl = configuration["SiteUrl"];
            _llaveCifrado = configuration["Encrypter:ClaveSecreta"];
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public async Task<IActionResult> Index()
        {
            //var catalogo = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest());
            //ViewBag.Almacenes = (catalogo.Result as List<AlmacenResponse> ?? new List<AlmacenResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();

            SetMenuLinkActive();
            //Se modifica los refrendos para pertenecer a un Exento (reunion 13/01/2023)
            return RedirectToAction("Index", "Exento");
        }


        public async Task<IActionResult> Edit(long id = 0, long idExento = 0)
        {
            //var catalogo = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest());
            //ViewBag.Almacenes = (catalogo.Result as List<AlmacenResponse> ?? new List<AlmacenResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
            ViewBag.id = id;
            ViewBag.idExento = idExento;

            SetMenuLinkActive();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(long? id, long idExento)
        {
            var result = new ResponseViewModel(true);
            var vm = new RefrendoGridResponse()
            {
                IdExento = idExento
            };
            try
            {
                if (id.HasValue)
                {

                    var response = await RefrendoHelper.Consulta(_proxyWebAPI, id.Value);
                    vm = response.Result as RefrendoGridResponse ?? vm;

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
                            var imageDoc1 = string.Format("{0}{1}", ContentType1, archivo.Result);
                            VBUrlDoc1 = imageDoc1;
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
                            var imageDoc2 = string.Format("{0}{1}", ContentType1, archivo.Result);
                            VBUrlDoc2 = imageDoc2;
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
                            var imageDoc3 = string.Format("{0}{1}", ContentType1, archivo.Result);
                            VBUrlDoc3 = imageDoc3;
                        }
                    }
                    ViewBag.UrlDoc1 = VBUrlDoc1;
                    ViewBag.UrlDoc2 = VBUrlDoc2;
                    ViewBag.UrlDoc3 = VBUrlDoc3;
                }
                ViewBag.Estados = EstadosMexicoDict.DictEstados.Select(x => new SelectListItem { Text = x.Nombre, Value = x.Clave, Selected = x.Nombre.Equals("PUEBLA") }).ToList();
                result.Result = await this.RenderViewToStringAsync("~/Views/Refrendo/_EditDatos.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(RefrendoRequest req)
        {
            var result = new ResponseViewModel(true);
            var urls = new List<string>();
            try
            {
                var files = JsonConvert.DeserializeObject<List<RefrendoFiles>>(Request.Form["Files"].FirstOrDefault() ?? "");
                req.Files = files ?? new List<RefrendoFiles>();
                var save = await RefrendoHelper.Edit(_proxyWebAPI, req);
                result.Result = save.Result;
                result.IsSuccessFully = save.IsSuccessFully;
                result.Message = save.Message;
            }
            catch (Exception e)
            {
                result.IsSuccessFully = false;
                result.Message = e.Message;
                //DeleteArchivosRefrendo(urls);
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
                url = _blobStorage._url + "/Refrendo/" + url;
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

        //private void DeleteArchivosRefrendo(List<string> urls)
        //{
        //    var fileStorage = new StorageManager();
        //    foreach (var file in urls)
        //    {
        //        fileStorage.EliminarArchivo(file);
        //    }
        //}

        [HttpGet]
        public async Task<ActionResult> GetPDFCertificado(long id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                string stringHtml = string.Empty;
                var dataReport = new DataReport();
                var vm = new RefrendoCertificadoResponse();

                var response = await RefrendoHelper.Consulta(_proxyWebAPI, id);
                vm = JsonConvert.DeserializeObject<RefrendoCertificadoResponse>(JsonConvert.SerializeObject(response.Result));
                var getdoc = await _pdfBuider.GetRefrendoCertificado(vm);

                var doc = getdoc.Response.DocumentoPDF;
                var nomb = getdoc.Response.NombreDocumento;

                dataReport.NombreDocumento = nomb;
                dataReport.DocumentoPDF = doc;

                var pdf = dataReport;

                // var stream = new MemoryStream(pdf.DocumentoPDF);
                // var contentType = @"application/pdf";
                // var fileName = "reporte_" + pdf.NombreDocumento;

                // var doc1 = File(stream, contentType, fileName);

                result.Result = Convert.ToBase64String(doc);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetPDFFormaValorada(long id, bool exento)
        {
            var result = new ResponseViewModel(true);
            try
            {
                string stringHtml = string.Empty;
                var dataReport = new DataReport();
                var dataImpresion = new ImpresionPDFResponse();
                var request = new GenerateImpresionRequest
                {
                    Id = id,
                    Imprimir = false
                };
                var citaHelper = await FoliosFormaValoradaHelper.Consulta(_proxyWebAPI, request);
                if (!citaHelper.IsSuccessFully)
                    throw new Exception("Error al obtener los datos de la forma valorada.");
                var encrypter = new AesEncryption(_llaveCifrado);
                var data = citaHelper.Result as FolioFormaValoradaImpresionResponse ?? new FolioFormaValoradaImpresionResponse();
                dataImpresion = new ImpresionPDFResponse
                {
                    Folio = data.FolioFoliosFormaValoradaVerificentro,
                    Marca = data.Marca,
                    Anio = data.Anio.ToString(),
                    Placa = data.Placa,
                    Holograma = data.TipoCertificado,
                    Submarca = data.Modelo,
                    Vigencia = data.VigenciaStr,
                    VigenciaFecha = data.Vigencia.Value,
                    Centro = data.NombreVerificentro,
                    Linea = data.Clave,
                    //Equipo
                    LeyendaCNA = data.LeyendaCNA,
                    FolioAnterior = data.FolioCertificadoAnterior,
                    Fecha = data.FechaRegistro.Value.ToString("dd/MM/yyyy HH:mm"),
                    HoraInicio = data.InicioPruebas.HasValue ? data.InicioPruebas.Value.ToString("HH:mm:ss") : "",
                    HoraFin = data.FinalizacionPruebas.HasValue ? data.FinalizacionPruebas.Value.ToString("HH:mm:ss") : "",
                    Nombre = data.Propietario,
                    Semestre = data.Semestre,
                    Equipo = data.NumeroSerieEquipo,
                    Combustible = data.Combustible,
                    Version = "1.0",
                    TecnicoCapturaNombre = data.NombreCapturista,
                    TecnicoCapturaNumero = data?.IdUserCapturista.ToString() ?? "-",
                    TecnicoPruebaNombre = data.NombreTecnico,
                    TecnicoPruebaNumero = data?.IdUserTecnico.ToString() ?? "-",
                    AprobadoPor = data.AprobadoPor,
                    UrlExpediente = null,
                    Emisiones = new List<ImpresionPDFEmisionResponse>(),
                    NumSerie = data.Serie,
                    IdCatTipoTramite = data.IdCatTipoTramite ?? 0,
                    Entidad = data.EntidadProcedencia
                };
                var getdoc = await _pdfBuider.GetImpresionCertificado(dataImpresion, data.IdCatTipoCertificado.Value);

                var doc = getdoc.Response.DocumentoPDF;
                var nomb = getdoc.Response.NombreDocumento;

                dataReport.NombreDocumento = nomb;
                dataReport.DocumentoPDF = doc;

                var pdf = dataReport;

                // var stream = new MemoryStream(pdf.DocumentoPDF);
                // var contentType = @"application/pdf";
                // var fileName = "FormaValorada_" + pdf.NombreDocumento;

                // var doc1 = File(stream, contentType, fileName);

                result.Result = Convert.ToBase64String(doc);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        private void SetMenuLinkActive()
        {
            ViewBag.Tramites = "here";
            ViewBag.SolicitudFormaValorada = "";
            ViewBag.Administracion = "";
        }
    }
}
