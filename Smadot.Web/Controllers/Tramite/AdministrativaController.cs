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
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Helper.Operaciones.Tramite;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using static Smadot.Models.Entities.Administrativa.Request.AdministrativaRequestData;
using static Smadot.Models.Entities.Administrativa.Response.AdministrativaResponseData;
using static Smadot.Models.Entities.Reposicion.Request.ReposicionRequestData;
using static Smadot.Models.Entities.Reposicion.Response.ReposicionResponseData;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Models.Dicts;
using Smadot.Utilities.ServicioMultas;
using Smadot.Utilities.BlobStorage;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.Entities.Calibracion.Response;

namespace Smadot.Web.Controllers.Tramite
{
    [Authorize]
    public class AdministrativaController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IConfiguration configuration;
        private readonly IUserResolver _userResolver;
        // StorageManager _storageManager = new StorageManager();
        private readonly IPdfBuider _pdfBuider;
        private readonly IConsultaVehiculoServicio consultaVehiculoServicio;
        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";
        private readonly BlobStorage _blobStorage;
        public AdministrativaController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConsultaVehiculoServicio consultaVehiculoServicio, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            this.consultaVehiculoServicio = consultaVehiculoServicio;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            this.configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(MenuAdministrativas.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/Tramite/Administrativa/Index.cshtml");
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
            var listaResult = await AdministrativaHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<AdministrativaResponse> ?? new List<AdministrativaResponse>();

            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<AdministrativaResponseGrid>();

            foreach (var row in data)
            {
                List.Add(new AdministrativaResponseGrid
                {
                    IdAdministrativa = row.IdAdministrativa,
                    IdCatMotivoTramite = row.IdCatMotivoTramite,
                    MotivoTramite = row.MotivoTramite,
                    Placa = row.Placa,
                    Serie = row.Serie,
                    NumeroReferencia = row.NumeroReferencia,
                    UrlDoc1 = row.UrlDoc1,
                    UrlDoc2 = row.UrlDoc2,
                    UrlDoc3 = row.UrlDoc3,
                    FolioAsignado = row.FolioAsignado,
                    IdUserRegistro = row.IdUserRegistro,
                    UsuarioRegistro = row.UsuarioRegistro,
                    FechaRegistro = row.FechaRegistro,
                    ClaveTramite = row.ClaveTramite,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Tramite/Administrativa/_Acciones.cshtml", row)
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
        public async Task<JsonResult> ConsultaAutocomplete(string prefix)
        {
            var listaResult = await AdministrativaHelper.AutocompleteFolio(_proxyWebAPI, prefix);
            var folio = listaResult.Result as List<PruebaAutocompletePlacaApi> ?? new List<PruebaAutocompletePlacaApi>();
            var result = folio.Select(x => new { Id = x.Id, Placa = x.Placa, Serie = x.Serie, PlacaSerie = string.Format("{0}/{1}", x.Placa, x.Serie) });
            return Json(result);
        }

        public async Task<IActionResult> Registro()
        {
            var accionesHelper = await MotivoTramiteHelper.SelectList(_proxyWebAPI, new RequestList { Activo = true });
            ViewBag.Estados = EstadosMexicoDict.DictEstados.Select(x => new SelectListItem { Text = x.Nombre, Value = x.Clave, Selected = x.Nombre == "PUEBLA" }).ToList();
            ViewBag.Certificados = TipoCertificado.DictNombreCertificado.Where(x => x.Key != TipoCertificado.Exentos && x.Key != TipoCertificado.Testificacion && x.Key != TipoCertificado.DobleCero && x.Key != TipoCertificado.ConstanciasNoAprobado)
            .Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList();
            ViewBag.Combustible = Combustible.DictCombustible.Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList();
            ViewBag.Catalogo = accionesHelper.ToList();
            return View("~/Views/Tramite/Administrativa/Registro.cshtml", new AdministrativaApiRequest() { FechaEmisionReferencia = DateTime.Now, FechaPago = DateTime.Now });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdministrativa(AdministrativaApiRequest registro)
        {
            var result = new JsonResultDto();
            try
            {
                // var apiRequest = new AdministrativaApiRequest();
                registro.Placa = registro.Placa?.ToUpper().Replace("-", "");
                registro.Serie = registro.Serie.ToUpper();
                var placaService = registro.Placa?.ToUpper().Replace("-", "");
                // var responseServicice = await consultaVehiculoServicio.Consulta(placaService, registro.Serie);
                // if (!responseServicice.BResultado)
                // {
                //     result.Error = true;
                //     result.ErrorDescription = responseServicice.VchMensajeCiudadano;
                //     return Json(result);
                // }
                registro.IdCatMotivoTramite = registro.IdCatMotivoTramite;
                registro.NumeroReferencia = registro.NumeroReferencia;
                registro.FolioAsignado = registro.FolioAsignado;

                var usuarioActual = _userResolver.GetUser();

                if (usuarioActual == null)
                {
                    result.Error = true;
                    result.ErrorDescription = _mensajeErrorCatch;
                    return Json(result);
                }
                registro.IdUserRegistro = usuarioActual.IdUser;
                registro.Files = JsonConvert.DeserializeObject<List<AdministrativaFiles>>(registro.FilesString ?? "");

                //var files = JsonConvert.DeserializeObject<List<AdministrativaFiles>>(Request.Form["Files"].FirstOrDefault() ?? "");
                //registro.Files = files ?? new List<AdministrativaFiles>();

                var helper = await AdministrativaHelper.Guardar(_proxyWebAPI, registro);

                if (!helper.IsSuccessFully)
                {
                    result.Error = true;
                    result.ErrorDescription = helper.Message ?? "Ocurrió un error al guardar la información";
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


        [HttpPost]
        public async Task<ActionResult> GetPDF(long id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                string stringHtml = string.Empty;
                var response = new CertificadoAdministrativaResponse();
                var helper = await AdministrativaHelper.GetById(_proxyWebAPI, id);
                var folio = helper.Result as AdministrativaResponse ?? new AdministrativaResponse();

                response.Placa = folio.Placa;
                response.Serie = folio.Serie;
                response.FolioAsignado = folio.FolioAsignado.ToString();
                response.NumeroReferencia = folio.NumeroReferencia;
                response.MotivoTramite = folio.MotivoTramite;
                response.FechaRegistro = folio.FechaRegistro;

                var dataReport = new DataReport();
                var getdoc = await _pdfBuider.GetDocumentoCertificadoAdministrativa(response);

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


        public async Task<IActionResult> Detalle(long id)
        {
            SetMenuLinkActive();
            var accionesHelper = await MotivoTramiteHelper.SelectList(_proxyWebAPI, new RequestList { Activo = true });
            ViewBag.Estados = EstadosMexicoDict.DictEstados.Select(x => new SelectListItem { Text = x.Nombre, Value = x.Clave }).ToList();
            ViewBag.Certificados = TipoCertificado.DictNombreCertificado.Where(x => x.Key != TipoCertificado.Exentos && x.Key != TipoCertificado.Testificacion)
            .Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList();
            ViewBag.Combustible = Combustible.DictCombustible.Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList();
            ViewBag.Catalogo = accionesHelper.ToList();
            var vm = new AdministrativaResponse();
            try
            {

                var response= await AdministrativaHelper.GetById(_proxyWebAPI, id);
                vm = response.Result as AdministrativaResponse ?? vm;
               
            }
            catch (Exception e)
            {

            }

            return View("~/Views/Tramite/Administrativa/Detalle.cshtml", vm);
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
                    var save = await AdministrativaHelper.Eliminar(_proxyWebAPI, id);
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
