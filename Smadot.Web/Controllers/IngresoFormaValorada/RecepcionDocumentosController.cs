using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.IngresoFormaValorada;
using Smadot.Web.Utilities;
using static Smadot.Models.Entities.RecepcionDocumentos.Response.RecepcionDocumentosResponseData;
using Smadot.Web.Handler.Autorizacion;
using static Smadot.Models.Entities.RecepcionDocumentos.Request.RecepcionDocumentosRequestData;
using Newtonsoft.Json;
using Smadot.Models.Entities.Personal.Extensions;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Request;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Response;
using Smadot.Web.Helper.Operaciones.DirectorioCentrosVerificacion;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.InteropServices;
using Smadot.Models.Entities.PortalCitas.Response;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Dicts;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.ServicioMultas;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Linea.Response;
using Smadot.Models.Entities.RecepcionDocumentos.Request;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Models;
using static Smadot.Models.Entities.FoliosCancelados.Request.FoliosCanceladosRequestData;
using Smadot.Models.Dicts.ProveedorDicts;

namespace Smadot.Web.Controllers.IngresoFormaValorada
{
    [Authorize]
    public class RecepcionDocumentosController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly BlobStorage _blobStorage;
        private readonly IUserResolver _userResolver;
        private readonly IConsultaVehiculoServicio _consultaVehiculoServicio;

        public RecepcionDocumentosController(IProxyWebAPI proxyWebAPI, IConfiguration configuration, IUserResolver userResolver, IConsultaVehiculoServicio consultaVehiculoServicio)
        {
            _proxyWebAPI = proxyWebAPI;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _userResolver = userResolver;
            _consultaVehiculoServicio = consultaVehiculoServicio;
        }

        public async Task<IActionResult> Index()
        {
            return View("~/Views/RecepcionDocumentos/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");

            var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            //Invoca al método que se encarga de realizar la petición Api
            var request = new RequestList();
            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

            var listaResult = await RecepcionDocumentosHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as ResponseGrid<RecepcionDocumentosGridResponse> ?? new ResponseGrid<RecepcionDocumentosGridResponse>();

            foreach (var row in data.Data ?? new List<RecepcionDocumentosGridResponse>())
            {
                row.Acciones = await this.RenderViewToStringAsync("~/Views/RecepcionDocumentos/_Acciones.cshtml", row);
            }
            data.Draw = draw;

            return Json(data);
        }


        public async Task<IActionResult> Documentos(long id)
        {
            try
            {
                var response = await RecepcionDocumentosHelper.GetById(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                    return RedirectToAction("Index");

                var vm = response.Result as RecepcionDocumentosGridResponse ?? new RecepcionDocumentosGridResponse();

                if (vm.IdCita == 0)
                    return RedirectToAction("Index");
                if (vm.IdCVV != _userResolver.GetUser().IdVerificentro)
                {
                    return RedirectToAction("Index", "RecepcionDocumentos");
                }
                var min = DateTime.Now.ToString("yyyy/MM/dd");
                var max = DateTime.Now.AddDays(14).ToString("yyyy/MM/dd");
                ViewBag.Min = min;
                ViewBag.Max = max;
                var motivos = await MotivoVerificacionHelper.Consulta(_proxyWebAPI, new MotivoVerificacionRequest { Activo = true });
                ViewBag.MotivosVer = (motivos.Result as List<MotivoVerificacionResponse> ?? new List<MotivoVerificacionResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre.ToString() }).ToList();

                var data = TipoServicio.DictTipoServicio.Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value.ToString() }).ToList();

                ViewBag.tServicio = data;

                var consulta = await _consultaVehiculoServicio.Consulta(vm.Serie, vm.Placa);
                // if (!consulta.BResultado)
                //     return RedirectToAction("Index");

                vm.VchLinea = ""; //consulta.VchLinea; revisar omar

                var VBUrlDoc1 = "";
                var VBUrlDoc2 = "";
                var VBUrlDoc3 = "";
                var VBUrlDoc4 = "";
                var VBUrlDoc5 = "";
                var VBUrlDoc6 = "";
                if (!string.IsNullOrEmpty(vm?.DataDocumentos?.URLIdentificacion))
                {
                    var arrDoc = vm?.DataDocumentos?.URLIdentificacion.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm?.DataDocumentos?.URLIdentificacion, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageURLIdentificacion = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc1 = imageURLIdentificacion;
                    }
                }
                if (!string.IsNullOrEmpty(vm?.DataDocumentos?.URLFactura))
                {
                    var arrDoc = vm?.DataDocumentos?.URLFactura.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm?.DataDocumentos?.URLFactura, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageURLFactura = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc2 = imageURLFactura;
                    }
                }
                if (!string.IsNullOrEmpty(vm?.DataDocumentos?.URLCartaFactura))
                {
                    var arrDoc = vm?.DataDocumentos?.URLCartaFactura.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm?.DataDocumentos?.URLCartaFactura, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageURLCartaFactura = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc3 = imageURLCartaFactura;
                    }
                }
                if (!string.IsNullOrEmpty(vm?.DataDocumentos?.URLValidacionCertificado))
                {
                    var arrDoc = vm?.DataDocumentos?.URLValidacionCertificado.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm?.DataDocumentos?.URLValidacionCertificado, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageURLValidacionCertificado = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc4 = imageURLValidacionCertificado;
                    }
                }
                if (!string.IsNullOrEmpty(vm?.DataDocumentos?.URLBajaPlacas))
                {
                    var arrDoc = vm?.DataDocumentos?.URLBajaPlacas.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm?.DataDocumentos?.URLBajaPlacas, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageURLBajaPlacas = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc5 = imageURLBajaPlacas;
                    }
                }
                if (!string.IsNullOrEmpty(vm?.DataDocumentos?.URLAltaPlacas))
                {
                    var arrDoc = vm?.DataDocumentos?.URLAltaPlacas.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm?.DataDocumentos?.URLAltaPlacas, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageURLAltaPlacas = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc6 = imageURLAltaPlacas;
                    }
                }
                ViewBag.Estados = EstadosMexicoDict.DictEstados.Select(x => new SelectListItem { Text = x.Nombre, Value = x.Clave }).ToList();

                ViewBag.Combustible = Combustible.DictCombustible.Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList();
                ViewBag.URLIdentificacion = VBUrlDoc1;
                ViewBag.URLFactura = VBUrlDoc2;
                ViewBag.URLCartaFactura = VBUrlDoc3;
                ViewBag.URLValidacionCertificado = VBUrlDoc4;
                ViewBag.URLBajaPlacas = VBUrlDoc5;
                ViewBag.URLAltaPlacas = VBUrlDoc6;
                ViewBag.Marcas = vm.ListaMarcas.Select(x => new SelectListItem { Value = x.Nombre, Text = x.Nombre }).ToList();
                ViewBag.Diesel = vm.ListaMarcasDisel.Select(x => new SelectListItem { Value = x.IdCatMarcaVehiculo.ToString(), Text = $"{x.Marca} - {x.Nombre} - {x.GOB_FAB}" }).ToList();
                return View("~/Views/RecepcionDocumentos/Documentos.cshtml", vm);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public async Task<JsonResult> Documentos(RecepcionDocumentosRequest request)
        {
            var result = new ResponseViewModel();
            try
            {
                var files = HttpContext.Request.Form.Files;

                request.FileIdentificacion = files.Select(x => new RecepcionDocumentoFiles { Nombre = x.FileName, Base64 = ConvertFileToBase64(x), Tipo = x.Name }).FirstOrDefault(x => x.Tipo.Contains("FileIdentificacion")) ?? null;
                request.FileAltaPlacas = files.Select(x => new RecepcionDocumentoFiles { Nombre = x.FileName, Base64 = ConvertFileToBase64(x), Tipo = x.Name }).FirstOrDefault(x => x.Tipo.Contains("FileAltaPlacas")) ?? null;
                request.FileBajaPlacas = files.Select(x => new RecepcionDocumentoFiles { Nombre = x.FileName, Base64 = ConvertFileToBase64(x), Tipo = x.Name }).FirstOrDefault(x => x.Tipo.Contains("FileBajaPlacas")) ?? null;
                request.FileFactura = files.Select(x => new RecepcionDocumentoFiles { Nombre = x.FileName, Base64 = ConvertFileToBase64(x), Tipo = x.Name }).FirstOrDefault(x => x.Tipo.Contains("FileFactura")) ?? null;
                request.FileValidacionCertificado = files.Select(x => new RecepcionDocumentoFiles { Nombre = x.FileName, Base64 = ConvertFileToBase64(x), Tipo = x.Name }).FirstOrDefault(x => x.Tipo.Contains("FileValidacionCertificado")) ?? null;
                request.FileMulta = files.Select(x => new RecepcionDocumentoFiles { Nombre = x.FileName, Base64 = ConvertFileToBase64(x), Tipo = x.Name }).FirstOrDefault(x => x.Tipo.Contains("FileMulta")) ?? null;

                //if (request.IdMotivoVerificacion != 7)
                //{
                //    var consulta = await _consultaVehiculoServicio.Consulta(request.Serie, request.Placa);

                //    if (consulta.codigo == 0)
                //    {
                //        result.IsSuccessFully = false;
                //        result.Message = consulta.desc;
                //        return new JsonResult(result);
                //    }

                //    if (consulta.codigo == 1 && consulta.vehs[0].cuentaInfr > 0)
                //    {
                //        result.IsSuccessFully = false;
                //        //result.Message = consulta.VchMensajeFotomulta;
                //        result.Message = $"El auto tiene {consulta.vehs[0].cuentaInfr} multa(s) pendiente(s) de pago derivadas del “Monitoreo Remoto” y/o de “Contaminación Ostensible”, no es posible solicitar una cita. Para mayor información visite: https://infraccionespuebla.monitorambiental.mx ";
                //        return new JsonResult(result);
                //    }
                //}


                var registro = await RecepcionDocumentosHelper.Documentos(_proxyWebAPI, request);
                if (!registro.IsSuccessFully)
                {
                    result.Message = registro?.Message.ObtenerMensajeExcepcion() ?? "Ocurrió un error al registrar la documentación.";
                    if (string.IsNullOrEmpty(result.Message))
                        result.Message = registro.Message;
                    return new JsonResult(result);
                }
                result.IsSuccessFully = registro.IsSuccessFully;
                result.Message = registro.Message;
                result.Result = registro.Result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult> Reagendar()
        {
            var result = new ResponseViewModel(true);

            try
            {
                //var response = new AcreditacionTecnicoVerificadorGridResponse();
                //var helper = await AcreditacionTecnicoVerificadorHelper.GetById(_proxyWebAPI, id);
                //if (helper.IsSuccessFully)
                //{
                //    var resulthelper = helper.Result as AcreditacionTecnicoVerificadorGridResponse ?? new AcreditacionTecnicoVerificadorGridResponse();
                //    response = resulthelper;
                //}

                var request = new DirectorioCentrosVerificacionListRequest();

                var listaResult = await DirectorioCentrosVerificacionHelper.Consulta(_proxyWebAPI, request);
                var data = (listaResult.Result as List<DirectorioCentrosVerificacionResponse> ?? new List<DirectorioCentrosVerificacionResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();

                ViewBag.CentroAtencion = data;


                result.Result = await this.RenderViewToStringAsync("~/Views/RecepcionDocumentos/_Reagendar.cshtml");
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        public async Task<IActionResult> ReagendarCita(long id)
        {
            try
            {
                var response = await RecepcionDocumentosHelper.GetById(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                    return RedirectToAction("Index");

                var vm = response.Result as RecepcionDocumentosGridResponse ?? new RecepcionDocumentosGridResponse();

                if (vm.IdCita == 0)
                    return RedirectToAction("Index");

                var min = DateTime.Now.ToString("yyyy/MM/dd");
                var max = DateTime.Now.AddDays(14).ToString("yyyy/MM/dd");
                ViewBag.Min = min;
                ViewBag.Max = max;
                ViewBag.Folio = vm.Folio;
                ViewBag.Cita = vm.IdCita;

                var request = new DirectorioCentrosVerificacionListRequest();

                var listaResult = await DirectorioCentrosVerificacionHelper.Consulta(_proxyWebAPI, request);
                var data = (listaResult.Result as List<DirectorioCentrosVerificacionResponse> ?? new List<DirectorioCentrosVerificacionResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();

                ViewBag.CentroAtencion = data;

                return View("~/Views/RecepcionDocumentos/ReagendarCita.cshtml");
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public async Task<JsonResult> ReagendarCita([FromBody] ReagendarCitaRequest request)
        {
            var result = new ResponseViewModel();
            try
            {
                if (string.IsNullOrEmpty(request.FechaString) || string.IsNullOrEmpty(request.HoraString))
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al obtener la información";
                    return new JsonResult(result);
                }

                DateTime dt;
                var fechaHour = string.Format("{0} {1}", request.FechaString, request.HoraString);
                var tryParseDate = DateTime.TryParseExact(fechaHour, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.GetCultureInfo("es-ES"), System.Globalization.DateTimeStyles.None, out dt);
                if (!tryParseDate)
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al enviar la información";
                    return new JsonResult(result);
                }
                request.Fecha = dt;



                string imageLogo = "";
                var path = string.Empty;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var dir = Directory.GetParent(Directory.GetCurrentDirectory());
                    path = Path.Combine(dir.FullName, "Smadot.Web", "wwwroot", "assets", "media", "logos", "smadot_logo_simple2.png");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    //path = "/app/wwwroot/images";
                    path = "/app/wwwroot/assets/media/logos/smadot_logo_simple2.png";
                }

                if (System.IO.File.Exists(path))
                {
                    byte[] imageArray = System.IO.File.ReadAllBytes(path);
                    imageLogo = Convert.ToBase64String(imageArray);
                }
                request.Logo = imageLogo;

                var helperReq = await RecepcionDocumentosHelper.Reagendar(_proxyWebAPI, request);

                var helper = helperReq.Result as PortalCitasRegistroResponse ?? new PortalCitasRegistroResponse();

                if (!string.IsNullOrEmpty((string)helper.ErrorMessage))
                {
                    result.IsSuccessFully = false;
                    result.Message = ((string)helper.ErrorMessage);
                    return new JsonResult(result);
                }
                if (string.IsNullOrEmpty((string)helper.FolioResult))
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al obtener la información";
                    return new JsonResult(result);
                }
                result.IsSuccessFully = helperReq.IsSuccessFully;
                result.Result = helper.FolioResult;
                //var registroUsuario = await PersonalHelper.Registro(_proxyWebAPI, request);
                //if (!registroUsuario.IsSuccessFully)
                //{
                //    result.Message = "Ocurrió un error al registrar el usuario.";
                //    return new JsonResult(result);
                //}
                //result.IsSuccessFully = registroUsuario.IsSuccessFully;
                //result.Result = registroUsuario.Result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetCilindros(int cita, long IdMarca, int IdSubmarca)
        {
            var result = new ResponseViewModel(true);

            try
            {
                var response = new List<SelectListItem>();
                var helper = await RecepcionDocumentosHelper.ConsultaTablaMaestra(_proxyWebAPI,
                    new CatalogoTablaMaestraRequest
                    {
                        IdCita = cita,
                        IdMarca = IdMarca,
                        IdSubmarca = IdSubmarca
                    }
                );
                if (helper.IsSuccessFully)
                {

                    var resulthelper = (helper.Result as List<CatalogoTablaMaestraResponse> ?? new List<CatalogoTablaMaestraResponse>()).Select(x => new SelectListItem { Value = x.CILINDROS.ToString(), Text = x.CILINDROS.ToString() }).ToList();

                    response = resulthelper;
                }

                //var request = new DirectorioCentrosVerificacionListRequest();

                //var listaResult = await DirectorioCentrosVerificacionHelper.Consulta(_proxyWebAPI, request);
                //var data = listaResult.Result as List<DirectorioCentrosVerificacionResponse> ?? new List<DirectorioCentrosVerificacionResponse>();


                result.Result = response;
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetCilindrada(int cita, long IdMarca, int IdSubmarca, int cilindros)
        {
            var result = new ResponseViewModel(true);

            try
            {
                var response = new List<SelectListItem>();
                var helper = await RecepcionDocumentosHelper.ConsultaTablaMaestra(_proxyWebAPI,
                    new CatalogoTablaMaestraRequest
                    {
                        IdCita = cita,
                        IdMarca = IdMarca,
                        IdSubmarca = IdSubmarca,
                        Cilindros = cilindros
                    });

                if (helper.IsSuccessFully)
                {

                    var resulthelper = (helper.Result as List<CatalogoTablaMaestraResponse> ?? new List<CatalogoTablaMaestraResponse>()).Select(x => new SelectListItem { Value = x.CILINDRADA.ToString(), Text = x.CILINDRADA.ToString() }).ToList();

                    response = resulthelper;
                }

                //var request = new DirectorioCentrosVerificacionListRequest();

                //var listaResult = await DirectorioCentrosVerificacionHelper.Consulta(_proxyWebAPI, request);
                //var data = listaResult.Result as List<DirectorioCentrosVerificacionResponse> ?? new List<DirectorioCentrosVerificacionResponse>();


                result.Result = response;
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
            int page = Convert.ToInt32(Request.Query["page"].FirstOrDefault() ?? "0");
            int records = Convert.ToInt32(Request.Query["records"].FirstOrDefault() ?? "0");
            var start = 0;
            var end = start + records;
            var listaResult = await RecepcionDocumentosHelper.Autocomplete(_proxyWebAPI, new GenericSelect2AutocompleRequest { Term = term, Start = start, End = end });
            var data = listaResult.Result as List<DieselAutocompleteResponse> ?? new List<DieselAutocompleteResponse>();

            return Json(new { items = data, total_count = data.Count });
        }

        [HttpGet]
        public async Task<ActionResult> GetSubDiesel(int marca)
        {
            var result = new ResponseViewModel(true);

            try
            {
                var response = new List<SelectListItem>();
                var helper = await RecepcionDocumentosHelper.ConsultaSubDiesel(_proxyWebAPI,
                    new SubDieselRequest
                    {
                        IdCatMarcaVehiculo = marca
                    });

                if (helper.IsSuccessFully)
                {

                    var resulthelper = (helper.Result as List<SubDieselResponse> ?? new List<SubDieselResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();

                    response = resulthelper;
                }

                //var request = new DirectorioCentrosVerificacionListRequest();

                //var listaResult = await DirectorioCentrosVerificacionHelper.Consulta(_proxyWebAPI, request);
                //var data = listaResult.Result as List<DirectorioCentrosVerificacionResponse> ?? new List<DirectorioCentrosVerificacionResponse>();


                result.Result = response;
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> CambioLinea(long id)
        {
            var result = new ResponseViewModel(true);
            var vm = new vLineaEquipoResponse();
            try
            {
                var linea = await RecepcionDocumentosHelper.CambioLinea(_proxyWebAPI, id);
                result.IsSuccessFully = linea.IsSuccessFully;
                if (linea.IsSuccessFully)
                {
                    ViewBag.LineaEquipos = ((List<LineaPendientes>?)linea.Result ?? new()).Select(obj => new SelectListItem { Value = obj.Linea.Id.ToString(), Text = obj.Linea.Nombre }).ToList();
                }
                result.Message = result.IsSuccessFully ? "" : "Ocurrió un error al cargar la informacioón de las líneas";
                ViewBag.IdVerificacion = id;
                //var aux = (linea.Result as List<vLineaEquipo> ?? new List<vLineaEquipo>()); 
                result.Result = await this.RenderViewToStringAsync("~/Views/RecepcionDocumentos/_CambiarLinea.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> CambiarLinea(RecepcionDocumentosCambiarLineaRequest req)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var save = await RecepcionDocumentosHelper.CambiarLinea(_proxyWebAPI, req);
                result.Result = save.Result;
                result.IsSuccessFully = save.IsSuccessFully;
                result.Message = save.Message;
            }
            catch (Exception e)
            {
                result.IsSuccessFully = false;
                result.Message = e.Message;
            }
            return new JsonResult(result);
        }
        [HttpPost]
        public async Task<JsonResult> CalcularResultados(long id)
        {
            var result = new JsonResultDto();
            try
            {


                var helper = await FoliosCanceladosHelper.CalcularResultados(_proxyWebAPI, id);

                if (!helper.IsSuccessFully)
                {
                    result.Error = true;
                    result.ErrorDescription = helper.Message;
                    return Json(result);
                }
                if ((helper.Result as long? ?? 0) == 0)
                {

                    result.Error = false;
                    result.ErrorDescription = helper.Message;
                    return Json(result);
                }
                result.Error = false;
                result.ErrorDescription = helper.Message ?? "Los resultados se generaron correctamente";

            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = "Ocurrió un error inesperado";
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> ConsultaNumSerie(string NumSerie, string Placa)
        {
            var result = new ResponseViewModel(true);
            try
            {              
                var consulta = await _consultaVehiculoServicio.Consulta(NumSerie, Placa);

                if (consulta.codigo == 0)
                {
                    result.IsSuccessFully = false;
                    result.Message = consulta.desc;
                    return new JsonResult(result);
                }

                if (consulta.vehs[0].cuentaInfr > 0)
                {
                    result.IsSuccessFully = false;
                    //result.Message = consulta.VchMensajeFotomulta;
                    result.Message = $"El auto tiene {consulta.vehs[0].cuentaInfr} multa(s) pendiente(s) de pago derivadas del “Monitoreo Remoto” y/o de “Contaminación Ostensible”, no es posible solicitar una cita. Para mayor información visite: https://infraccionespuebla.monitorambiental.mx ";
                    return new JsonResult(result);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        #region Private Methods
        private string ConvertFileToBase64(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                byte[] bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
        #endregion
    }
}
