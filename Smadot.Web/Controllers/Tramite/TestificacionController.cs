using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Models.Dicts;
using Smadot.Utilities.ServicioMultas;
using Smadot.Utilities.BlobStorage;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Models.Entities.ReposicionCredencial.Request;
using Smadot.Models.Entities.Testificacion.Response;
using Smadot.Models.Entities.Testificacion.Request;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Models.DataBase;
using Smadot.Utilities.Seguridad;
using NPOI.POIFS.Crypt.Dsig;


namespace Smadot.Web.Controllers.Tramite
{
    [Authorize]
    public class TestificacionController : Controller
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
        private readonly string _llaveCifrado;
        private readonly string siteUrl;
        public TestificacionController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConsultaVehiculoServicio consultaVehiculoServicio, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            this.consultaVehiculoServicio = consultaVehiculoServicio;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            this.configuration = configuration;
            _llaveCifrado = configuration["Encrypter:ClaveSecreta"];
        }

        public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(MenuTestificacion.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/Tramite/Testificacion/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");
            //Invoca al método que se encarga de realizar la petición Api
            var request = new ReposicionCredencialListRequest();

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

            var listaResult = await TestificacionHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as ResponseGrid<TestificacionResponseGrid> ?? new ResponseGrid<TestificacionResponseGrid>() { Data = new() };
            
            data.Data.ForEach(async x =>
            {
                x.Acciones = await this.RenderViewToStringAsync("~/Views/Tramite/Testificacion/_Acciones.cshtml", x);
            });
            data.Draw = draw;
            return Json(data);
        }

        [HttpPost]
        public async Task<JsonResult> ConsultaAutocomplete(string prefix)
        {
            var listaResult = await TestificacionHelper.AutocompleteFolio(_proxyWebAPI, prefix);
            var folio = listaResult.Result as List<PruebaAutocompletePlacaApi> ?? new List<PruebaAutocompletePlacaApi>();
            var result = folio.Select(x => new { Id = x.Id, Placa = x.Placa, Serie = x.Serie, PlacaSerie = string.Format("{0}/{1}", x.Placa, x.Serie) });
            return Json(result);
        }

        public async Task<IActionResult> Registro()
        {
            var accionesHelper = await MotivoTramiteHelper.SelectList(_proxyWebAPI, new RequestList { Activo = true });
            ViewBag.Estados = EstadosMexicoDict.DictEstados.Select(x => new SelectListItem { Text = x.Nombre, Value = x.Clave }).ToList();
            ViewBag.Certificados = TipoCertificado.DictNombreCertificado.Where(x => x.Key != TipoCertificado.DobleCero&& x.Key != TipoCertificado.Cero && x.Key != TipoCertificado.ConstanciasNoAprobado && x.Key != TipoCertificado.Exentos && x.Key != TipoCertificado.Uno && x.Key != TipoCertificado.Dos )
            .Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList();
            ViewBag.Combustible = Combustible.DictCombustible.Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList();
            ViewBag.Catalogo = accionesHelper.ToList();
            return View("~/Views/Tramite/Testificacion/Registro.cshtml", new TestificacionApiRequest() { FechaEmisionReferencia = DateTime.Now, FechaPago = DateTime.Now, VigenciaOrigen = DateTime.Now });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTestificacion(TestificacionApiRequest registro)
        {
            var result = new JsonResultDto();
            try
            {
                registro.Placa = registro.Placa?.ToUpper().Replace("-", "");
                registro.Serie = registro.Serie?.ToUpper();
                var files = JsonConvert.DeserializeObject<List<TestificacionFiles>>(registro.FilesString ?? "");
                registro.Files = files ?? new List<TestificacionFiles>();

                var helper = await TestificacionHelper.Guardar(_proxyWebAPI, registro);

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

        [HttpDelete]
        public async Task<IActionResult> EliminarRegistro(long id)
        {
            var result = new JsonResultDto();
            try
            {
                if (id > 0)
                {
                    var save = await TestificacionHelper.Eliminar(_proxyWebAPI, id);
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


        //[HttpGet]
        //public async Task<ActionResult> GetPDF(long id, bool testificacion)
        //{
        //    var result = new ResponseViewModel(true);
        //    try
        //    {
        //        string stringHtml = string.Empty;
        //        var dataReport = new DataReport();
        //        var vm = new TestificacionResponseGrid();
        //        var dataImpresion = new ImpresionPDFResponse();

        //        var response = await TestificacionHelper.Consulta(_proxyWebAPI, id, false);
        //        if (!response.IsSuccessFully)
        //            throw new Exception("Error al obtener los datos de la testificación.");

        //        var encrypter = new AesEncryption(_llaveCifrado);

        //        var data = result.Result as TestificacionResponseGrid ?? new TestificacionResponseGrid();
        //        dataImpresion = new ImpresionPDFResponse
        //        {
        //            Folio = data.IdFoliosFormaValoradaVerificentro,
        //            Marca = data.Marca,
        //            Anio = data.Anio.ToString(),
        //            Placa = data.Placa,
        //            Holograma = data.TipoCertificado,
        //            Submarca = data.Modelo,
        //            Vigencia = data.Vigencia.ToString(),
        //            Centro = data.NombreVerificentro,
        //            Linea = data.ClaveTramite,
        //            FolioAnterior = data.FolioCertificadoAnterior ?? 0,
        //            Fecha = data.FechaRegistro.Value.ToString("d"),
        //            Nombre = data.Propietario,
        //            Combustible = data.Combustible,
        //            Version = "1.0",
        //            TecnicoPruebaNombre = data.NombreTecnico,
        //            TecnicoPruebaNumero = data?.IdUserTecnico.ToString() ?? "-",
        //            UrlExpediente = data.IdCatMotivoTramite == TipoTramiteDict.CVV || data.IdVerificacion != null ? $"{siteUrl}Citas/ExpedientePublico?clave={encrypter.Encrypt(data.IdVerificacion.ToString())}" : null,
        //            Emisiones = new List<ImpresionPDFEmisionResponse>(),
        //            NumSerie = data.Serie,
        //            IdCatTipoTramite = data.IdCatTipoTramite ?? 0
        //        };

        //        //if(data.IdVerificacion != null && data.Modelo != null)
        //        //{
        //        //    var limites = LimitesPermisiblesCertificadoDict.ObtenerLimites(data.)
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccessFully = false;
        //        result.Message = ex.Message;
        //    }
        //    return new JsonResult(result);
        //}


        public async Task<IActionResult> Detalle(long id)
        {
            // var accionesHelper = await MotivoTramiteHelper.SelectList(_proxyWebAPI, new RequestList { Activo = true });
            ViewBag.Estados = EstadosMexicoDict.DictEstados.Select(x => new SelectListItem { Text = x.Nombre, Value = x.Clave }).ToList();
            ViewBag.Certificados = DictTipoCertificadoTestificacion.DictNombreCertificado
            .Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList();
            ViewBag.Combustible = Combustible.DictCombustible.Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList();
            // ViewBag.Catalogo = accionesHelper.ToList();
            var vm = new TestificacionResponse();
            try
            {

                var response = await TestificacionHelper.GetById(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                    return BadRequest();
                vm = response.Result as TestificacionResponse ?? new TestificacionResponse();              

            }
            catch (Exception e)
            {

            }

            return View("~/Views/Tramite/Testificacion/Detalle.cshtml", vm);
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

        private void SetMenuLinkActive()
        {
            ViewBag.Tramites = "here";
            ViewBag.SolicitudFormaValorada = "";
            ViewBag.Administracion = "";
        }

    }
}
