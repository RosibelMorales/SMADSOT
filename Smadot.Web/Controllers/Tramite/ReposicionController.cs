using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.FileStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Helper.Operaciones.Tramite;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using static Smadot.Models.Entities.Reposicion.Request.ReposicionRequestData;
using static Smadot.Models.Entities.Reposicion.Response.ReposicionResponseData;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Models.Dicts;
using Smadot.Utilities.BlobStorage;
using Smadot.Models.Entities.Reposicion.Response;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Smadot.Web.Controllers.Tramite
{
    [Authorize]
    public class ReposicionController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuider;
        private readonly BlobStorage _blobStorage;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public ReposicionController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }
        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuReposiciones.PermisoMenuPadre))
                return new UnauthorizedResult();
            SetMenuLinkActive();
            return View("~/Views/Reposicion/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");
            //Invoca al método que se encarga de realizar la petición Api
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

            var gridData = new ResponseGrid<ReposicionResponseGrid>();
            var listaResult = await ReposicionHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<ReposicionResponse> ?? new List<ReposicionResponse>();

            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<ReposicionResponseGrid>();

            foreach (var row in data)
            {
                List.Add(new ReposicionResponseGrid
                {
                    IdReposicion = row.IdReposicion,
                    IdVerificacion = row.IdVerificacion,
                    FechaRegistro = row.FechaRegistro,
                    NumeroReferencia= row.NumeroReferencia,
                    UsuarioRegistro= row.UsuarioRegistro,
                    Placa = row.Placa,
                    Serie = row.Serie,
                    ClaveTramite = row.ClaveTramite,
                    Folio = row.Folio,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Reposicion/_Acciones.cshtml", row)
                });
            }
            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            //var List = GetList();
            //var List = new List<FoliosCanceladosResponseGrid>();


            //if (!string.IsNullOrEmpty(request.Busqueda))
            //{
            //    List = List.Where(
            //        x => 
            //        x.Folio.ToLower().Contains(request.Busqueda.ToLower()) ||
            //        x.Motivo.ToLower().Contains(request.Busqueda.ToLower()) ||
            //        x.UsuarioAprobo.ToLower().Contains(request.Busqueda.ToLower()) || 
            //        x.TipoTramite.ToLower().Contains(request.Busqueda.ToLower())
            //        ).ToList();
            //}
            //if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento)) {
            //    var quer = List.AsQueryable();
            //    List = quer.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento).ToList();

            //}

            //List.Add(new EstadisiticaUsoFormaValoradaResponse() { }, new EstadisiticaUsoFormaValoradaResponse() { });
            //gridData.Data = List;
            //gridData.RecordsTotal = totalRecord;
            //gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
            //filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
            //gridData.RecordsFiltered = filterRecord;
            //gridData.Draw = draw;

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
                result.Result = await this.RenderViewToStringAsync("~/Views/Reposicion/_Registro.cshtml");
            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> ConsultaPlacaSerieAutocomplete(string prefix)
        {
            //var folio = GetList().Where(x => x.Folio.ToLower().Contains(prefix)).Take(10).ToList();
            //var folio = GetList().Take(10).ToList();
            var listaResult = await ReposicionHelper.AutocompletePlaca(_proxyWebAPI, prefix);
            var folio = listaResult.Result as List<vBusquedaPlacaSerie> ?? new List<vBusquedaPlacaSerie>();
            var result = folio.Select(x => new { Id = x.IdFv, Nombre = string.Format("{0}/{1}", x.Placa, x.Serie), x.Exento, x.Administrativa });
            return Json(result);
        }

        // [HttpGet]
        // public async Task<JsonResult> ConsultaListDataVerificacion(string id)
        // {
        //     var result = new JsonResultDto();

        //     try
        //     {
        //         var res = new JsonResultDto();
        //         var listaResult = await ReposicionHelper.ListDataVerificacion(_proxyWebAPI, id);
        //         var folio = listaResult.Result as List<vVerificacion> ?? new List<vVerificacion>();
        //         result.Result = await this.RenderViewToStringAsync("~/Views/Reposicion/_ListDataVerificacion.cshtml",folio);
        //     }
        //     catch (Exception e)
        //     {
        //         result.Error = true;
        //         result.ErrorDescription = _mensajeErrorCatch;
        //     }

        //     return Json(result);

        // }

        [HttpGet]
        public async Task<JsonResult> ConsultaDataVerificacion(long id)
        {
            var result = new JsonResultDto();

            try
            {
                var res = new JsonResultDto();
                var listaResult = await ReposicionHelper.DataVerificacion(_proxyWebAPI, id);
                var folio = listaResult.Result as vDataTramiteVentanillaResponse ?? new vDataTramiteVentanillaResponse();
                ViewBag.Estados = EstadosMexicoDict.DictEstados.Select(x => new SelectListItem { Text = x.Nombre, Value = x.Clave, Selected = x.Nombre == "PUEBLA"}).ToList();
                result.Result = await this.RenderViewToStringAsync("~/Views/Reposicion/_DataVerificacion.cshtml", folio);
            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }

            return Json(result);

        }

        [HttpPost]
        public async Task<IActionResult> CreateReposicion(ReposicionApiRequest apiRequest)
        {
            var result = new JsonResultDto();
            try
            {
               
                var usuarioActual = _userResolver.GetUser();

                if (usuarioActual == null)
                {
                    result.Error = true;
                    result.ErrorDescription = _mensajeErrorCatch;
                    return Json(result);
                }
                apiRequest.IdUserRegistro = usuarioActual.IdUser;

                apiRequest.Files = JsonConvert.DeserializeObject<List<ReposicionFiles>>(apiRequest.FilesString ?? "");

                var helper = await ReposicionHelper.GuardarReposicion(_proxyWebAPI, apiRequest);

                if (!helper.IsSuccessFully)
                {
                    result.Error = true;
                    result.ErrorDescription = string.IsNullOrEmpty(helper.Message) ? "Ocurrió un error al guardar la información" : helper.Message;
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
        public async Task<ActionResult> Detalle(long id)
        {
            var result = new JsonResultDto();

            try
            {
                var res = new JsonResultDto();
                var listaResult = await ReposicionHelper.GetById(_proxyWebAPI, id);
                var folio = listaResult.Result as vReposicionVerificacion ?? new vReposicionVerificacion();
                var VBUrlDoc1 = "";
                var VBUrlDoc2 = "";
                var VBUrlDoc3 = "";
                if (!string.IsNullOrEmpty(folio.UrlDoc1))
                {
                    var arrDoc = folio.UrlDoc1.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(folio.UrlDoc1, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageUrlFactura = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc1 = imageUrlFactura;
                    }
                }
                if (!string.IsNullOrEmpty(folio.UrlDoc2))
                {
                    var arrDoc = folio.UrlDoc2.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(folio.UrlDoc2, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageUrlFactura = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc2 = imageUrlFactura;
                    }
                }
                if (!string.IsNullOrEmpty(folio.UrlDoc3))
                {
                    var arrDoc = folio.UrlDoc3.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(folio.UrlDoc3, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageUrlFactura = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc3 = imageUrlFactura;
                    }
                }
                result.Result = await this.RenderViewToStringAsync("~/Views/Reposicion/_Detalle.cshtml",folio);
            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }
            //try
            //{
            //    var res = new JsonResultDto();
            //    var listaResult = await ReposicionHelper.DataVerificacion(_proxyWebAPI, id);
            //    var folio = listaResult.Result as vVerificacionReposicion ?? new vVerificacionReposicion();
            //    result.Result = await this.RenderViewToStringAsync("~/Views/Reposicion/_DataVerificacion.cshtml", folio);
            //}
            //catch (Exception e)
            //{
            //    result.Error = true;
            //    result.ErrorDescription = _mensajeErrorCatch;
            //}

            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> GetPDF(long id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                string stringHtml = string.Empty;
                var response = new CertificadoReposicionResponse();
                var helper = await ReposicionHelper.GetById(_proxyWebAPI, id);
                var folio = helper.Result as vReposicionVerificacion ?? new vReposicionVerificacion();
                response.TarjetaCirculacion = folio.TarjetaCirculacion;
                response.IdVerificacion = folio.IdVerificacion ?? 0;
                response.Marca = folio.Marca;
                response.Anio = folio.Anio ?? 0;
                response.Fecha = folio.Fecha;
                response.TarjetaCirculacion = folio.TarjetaCirculacion;
                response.Placa = folio.Placa;
                response.Combustible = folio.Combustible;
                response.FolioCertificado = folio.FolioCertificado;
                response.IdReposicion = folio.IdReposicion;
                response.Modelo = folio.Modelo;
                response.NumeroReferencia = folio.NumeroReferencia;
                response.Semestre = folio.Semestre;
                response.Serie = folio.Serie;
                response.TipoCertificado = folio.TipoCertificado;
                response.TipoCertificadoFV = folio.TipoCertificadoFV;
                response.Vigencia = folio.Vigencia ?? DateTime.Now;

                var dataReport = new DataReport();
                var getdoc = await _pdfBuider.GetDocumentoCertificadoReposicion(response);

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
                if (id > 0)
                {
                    var save = await ReposicionHelper.Eliminar(_proxyWebAPI, id);
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

        private void SetMenuLinkActive()
        {
            ViewBag.Tramites = "here";
            ViewBag.SolicitudFormaValorada = "";
            ViewBag.Administracion = "";
        }
    }
}
