using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Utilities.Reporting.Interfaces;
using QRCoder;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Request;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.PortalCitas.Request;
using Smadot.Models.Entities.PortalCitas.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Utilities.ServicioMultas;
using Smadot.Web.Helper.Operaciones.DirectorioCentrosVerificacion;
using Smadot.Web.Helper.Operaciones.SeguimientoCVV;
using Smadot.Web.Utilities;
using System.Runtime.InteropServices;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Dicts;
using Newtonsoft.Json;
using Smadot.Web.Helper.Operaciones.GoogleCaptcha;
using Smadot.Web.Handler.Autorizacion;

namespace Smadot.Web.Controllers.SeguimientoCVV
{
    public class PortalCitasController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IConsultaVehiculoServicio _consultaVehiculoServicio;
        private readonly BlobStorage _blobStorage;
        private readonly IPdfBuider _pdfBuilder;
        private readonly VerifyTokenReCaptchaHelper _verifyTokenHelper;
        private readonly string _urlSite;

        public PortalCitasController(IProxyWebAPI proxyWebAPI, IConsultaVehiculoServicio consultaVehiculoServicio, IConfiguration configuration, IPdfBuider pdfBuilder, VerifyTokenReCaptchaHelper verifyTokenHelper)
        {
            _proxyWebAPI = proxyWebAPI;
            _consultaVehiculoServicio = consultaVehiculoServicio;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _pdfBuilder = pdfBuilder;
            _urlSite = configuration["SiteUrl"];
            _verifyTokenHelper = verifyTokenHelper;
        }
        [Route("/")]
        public IActionResult Index()
        {
            var min = DateTime.Now.ToString("yyyy/MM/dd");
            var max = DateTime.Now.AddDays(14).ToString("yyyy/MM/dd");
            ViewBag.Min = min;
            ViewBag.Max = max;
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GeneralStep()
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
                var data = (listaResult.Result as List<DirectorioCentrosVerificacionResponse> ?? new List<DirectorioCentrosVerificacionResponse>()).OrderBy(x => x.Clave).Where(x => x.Clave.Contains("CVV")).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();


                ViewBag.CentroAtencion = data;
                ViewBag.Estados = EstadosMexicoDict.DictEstados.Select(x => new SelectListItem { Text = x.Nombre, Value = x.Clave }).Where(x => !x.Text.Contains("PUEBLA")).ToList();
                var listCombustible = Smadot.Models.Dicts.ProveedorDicts.Combustible.DictCombustible
                .Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList();
                ViewBag.TiposCombustible = listCombustible;
                result.Result = await this.RenderViewToStringAsync("~/Views/PortalCitas/_GeneralStep.cshtml");
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult> CentrosAtencion()
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
                var data = listaResult.Result as List<DirectorioCentrosVerificacionResponse> ?? new List<DirectorioCentrosVerificacionResponse>();
                data = data.OrderBy(x => x.Clave).ToList();

                result.Result = await this.RenderViewToStringAsync("~/Views/PortalCitas/_CentrosAtencion.cshtml", data);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetCalendarDates(long id)
        {
            var result = new ResponseViewModel(true);

            try
            {
                var response = new PortalCitasResponse();
                var helper = await PortalCitaHelper.GetPortalCitasCalendarioByIdCvv(_proxyWebAPI, id);
                if (helper.IsSuccessFully)
                {
                    var resulthelper = helper.Result as PortalCitasResponse ?? new PortalCitasResponse();

                    resulthelper.ViewPartial = await this.RenderViewToStringAsync("~/Views/PortalCitas/_Step2DateHour.cshtml");
                    response = resulthelper;
                }
                else
                {
                    return new JsonResult(helper);
                }

                //var request = new DirectorioCentrosVerificacionListRequest();

                //var listaResult = await DirectorioCentrosVerificacionHelper.Consulta(_proxyWebAPI, request);
                //var data = listaResult.Result as List<DirectorioCentrosVerificacionResponse> ?? new List<DirectorioCentrosVerificacionResponse>();


                result.Result = response;
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                //result.Message = ex.Message;
                result.Message = "Ocurrió un error al procesar la información.";
            }

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetMarca()
        {
            var result = new ResponseViewModel(true);

            try
            {

                var term = Request.Query["q"].FirstOrDefault();
                int page = Convert.ToInt32(Request.Query["page"].FirstOrDefault() ?? "0");
                int records = Convert.ToInt32(Request.Query["records"].FirstOrDefault() ?? "0");

                var helper = await MarcaVehiculoHelper.Consulta(_proxyWebAPI, new MarcaVehiculoRequest { NombreMarca = term, Pagina = page, Registros = records });
                if (helper.IsSuccessFully)
                {

                    var resulthelper = helper.Result;
                    if (resulthelper != null)
                    {
                        // if (!resulthelper.Data.Any())
                        // {
                        //     result.IsSuccessFully = false;
                        //     result.Message = "Ocurrió un error al obtener la información";
                        //     return new JsonResult(result);
                        // }
                        result.Result = resulthelper;

                    }
                    else
                    {
                        result.IsSuccessFully = false;
                        result.Message = "Ocurrió un error al obtener la información";
                        return new JsonResult(result);
                    }
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
        public async Task<ActionResult> GetSubMarcas(string marca, bool submarca)
        {
            var result = new ResponseViewModel(true);

            try
            {
                var term = Request.Query["q"].FirstOrDefault();
                int page = Convert.ToInt32(Request.Query["page"].FirstOrDefault() ?? "0");
                int records = Convert.ToInt32(Request.Query["records"].FirstOrDefault() ?? "0");
                var helper = await SubMarcaVehiculoHelper.Consulta(_proxyWebAPI, new SubMarcaVehiculoRequest { Marca = marca, Pagina = page, Registros = records, Busqueda = term, SubmarcaClave = submarca });
                if (helper.IsSuccessFully)
                {
                    var resulthelper = JsonConvert.SerializeObject(helper.Result);
                    var data = JsonConvert.DeserializeObject<ResponseGrid<SelectListItem>>(resulthelper);
                    var dataResult = data?.Data != null ? data.Data : new List<SelectListItem>();
                    result.Result = data;
                    return new JsonResult(result);
                }
                result.IsSuccessFully = false;
                result.Message = "Ocurrió un error al obtener la información";
                return new JsonResult(result);

            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        //[HttpGet]
        //public async Task<ActionResult> Consulta(string numserie, string placa, bool poblano)
        //{
        //    var result = new ResponseViewModel(true);

        //    try
        //    {
        //        if (!poblano)
        //        {
        //            result.IsSuccessFully = true;
        //            result.Message = "";
        //            return new JsonResult(result);
        //        }
        //        //var response = new PortalCitasHorasResponse();
        //        //var consulta = await _consultaVehiculoServicio.Consulta(numserie, placa);

        //        //if (consulta.codigo == 0)
        //        //{
        //        //    result.IsSuccessFully = false;
        //        //    result.Message = consulta.desc;
        //        //    return new JsonResult(result);
        //        //}

        //        //if (consulta.codigo == 1 && consulta.vehs[0].cuentaInfr > 0)
        //        //{

        //        //    result.IsSuccessFully = false;
        //        //    //result.Message = consulta.VchMensajeFotomulta;
        //        //    result.Message = $"El auto tiene {consulta.vehs[0].cuentaInfr} multa(s) pendiente(s) de pago derivadas del “Monitoreo Remoto” y/o de “Contaminación Ostensible”, no es posible solicitar una cita. Para mayor información visite: https://infraccionespuebla.monitorambiental.mx ";
        //        //    return new JsonResult(result);
        //        //}

        //        //if(consulta.codigo == 2 && consulta.vehs[0] == null )
        //        //{
        //        //    consulta.vehs[0].placa = placa;
        //        //    consulta.vehs[0].serie = numserie;
        //        //}


        //        var helper2 = await MarcaVehiculoHelper.Consulta(_proxyWebAPI, new MarcaVehiculoRequest { NombreMarca = null, Pagina = 1, Registros = 10 });
        //        SelectListItem marca = new SelectListItem();
        //        if (helper2.IsSuccessFully)
        //        {
        //            var resulthelper = JsonConvert.SerializeObject(helper2.Result);
        //            var data = JsonConvert.DeserializeObject<ResponseGrid<SelectListItem>>(resulthelper);
        //            marca = data?.Data != null ? data.Data.FirstOrDefault() : new SelectListItem();
        //        }
        //        var helper = await SubMarcaVehiculoHelper.Consulta(_proxyWebAPI, new SubMarcaVehiculoRequest { Marca = null, Pagina = 1, Registros = 10, Busqueda = null, Anio = 0 });
        //        SelectListItem submarca = new SelectListItem();
        //        if (helper.IsSuccessFully)
        //        {
        //            var resulthelper = JsonConvert.SerializeObject(helper.Result);
        //            var data = JsonConvert.DeserializeObject<ResponseGrid<SelectListItem>>(resulthelper);
        //            submarca = data?.Data != null ? data.Data.FirstOrDefault() : new SelectListItem();
        //        }
        //        result.Result = new
        //        {
        //            Placa = numserie,
        //            NumSerie = placa,
        //            //Marca = consulta.VchMarca,
        //            Linea = "",
        //            Anio = 0,
        //            Submarca = submarca,
        //            Marca = marca
        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccessFully = false;
        //        result.Message = ex.Message;
        //    }

        //    return new JsonResult(result);
        //}

        [HttpPost]
        public async Task<JsonResult> Registro(PortalCitaRequest request)
        {

            var result = new ResponseViewModel();
            string? remoteip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            //var validCaptchaToken = await _verifyTokenHelper.VerificarToken(request.Token, remoteip);
            //if (!validCaptchaToken.IsSuccessFully && !(HttpContext?.User?.Identity?.IsAuthenticated ?? false))
            //{
            //    return new JsonResult(validCaptchaToken);
            //}
            try
            {
                if (string.IsNullOrEmpty(request.FechaString) || string.IsNullOrEmpty(request.HoraString))
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al obtener la información";
                    return new JsonResult(result);
                }
                request.Serie = request.Serie.ToUpper().Trim();
                request.Poblano = !request.Poblano;
                request.Placa = request.Placa.ToUpper().Replace("-", "").Trim();
                //var consulta = await _consultaVehiculoServicio.Consulta(request.Serie);

                // if (!consulta.BResultado)
                // {
                //     result.IsSuccessFully = false;
                //     result.Message = consulta.VchMensajeCiudadano;
                //     return new JsonResult(result);
                // }
                // if (consulta.TiEstatusFotomulta > 0)
                // {
                //     result.IsSuccessFully = false;
                //     //result.Message = consulta.VchMensajeFotomulta;
                //     result.Message = "El auto tiene multas pendientes, no es posible solicitar una cita. Para aclaraciones, visite a su oficina recaudadora más cercana.";
                //     return new JsonResult(result);
                // }
                // if (consulta.TiEstatusAdeudo > 0)
                // {
                //     result.IsSuccessFully = false;
                //     //result.Message = consulta.VchMensajeFotomulta;
                //     result.Message = "El auto tiene adeudos pendientes, no es posible solicitar una cita. Para aclaraciones, visite a su oficina recaudadora más cercana.";
                //     return new JsonResult(result);
                // }
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
                var splitchar = "";
                var path = string.Empty;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    splitchar = "\\";
                    var dir = Directory.GetParent(Directory.GetCurrentDirectory());
                    path = Path.Combine(dir.FullName, "Smadot.Web", "wwwroot", "assets", "media", "logos", "smadot_logo_simple2.png");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    splitchar = "/";
                    //path = "/app/wwwroot/images";
                    path = "/app/wwwroot/assets/media/logos/smadot_logo_simple2.png";
                }

                if (System.IO.File.Exists(path))
                {
                    byte[] imageArray = System.IO.File.ReadAllBytes(path);
                    imageLogo = Convert.ToBase64String(imageArray);
                }
                request.Logo = imageLogo;

                var helperReq = await PortalCitaHelper.Registro(_proxyWebAPI, request);

                var helper = helperReq.Result as PortalCitasRegistroResponse ?? new PortalCitasRegistroResponse();

                if (!string.IsNullOrEmpty((string)helper.ErrorMessage))
                {
                    result.IsSuccessFully = false;
                    result.Message = ((string)helper.ErrorMessage);
                    return new JsonResult(result);
                }
                if (string.IsNullOrEmpty((string)helper.FolioResult) && !helperReq.IsSuccessFully)
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
        public async Task<ActionResult> ConsultaCitaGet([FromQuery] string folio)
        {
            var result = new ResponseViewModel(true);

            try
            {
                if (string.IsNullOrEmpty(folio))
                {
                    result.IsSuccessFully = false;
                    result.Message = "Debe ingresar un folio para realizar la busqueda";
                    return new JsonResult(result);
                }
                folio = folio.Trim();
                var response = "";
                var helper = await PortalCitaHelper.GetCitaData(_proxyWebAPI, new() { Folio = folio });
                var resulthelper = helper.Result as PortalCitasComprobanteResponse ?? new PortalCitasComprobanteResponse();

                if (!helper.IsSuccessFully)
                {

                    result.IsSuccessFully = false;
                    result.Message = "No se encontró información de este Folio";
                    return new JsonResult(result);
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

        public async Task<IActionResult> ConsultaCita(string folio)
        {
            if (string.IsNullOrEmpty(folio))
                return RedirectToAction("Index");

            var vm = new PortalCitasComprobanteResponse();
            try
            {
                var helper = await PortalCitaHelper.GetCitaData(_proxyWebAPI, new() { Folio = folio });
                if (helper.IsSuccessFully)
                {
                    var resulthelper = helper.Result as PortalCitasComprobanteResponse ?? new PortalCitasComprobanteResponse();

                    //if (resulthelper.Cancelada == true)
                    //{
                    //    return RedirectToAction("Index");
                    //}

                    vm = resulthelper;
                    if (vm.Id != 0)
                    {
                        QRCodeData qrCodeData;
                        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                        {
                            qrCodeData = qrGenerator.CreateQrCode(string.Format("{0}PortalCitas/ConsultaCita?folio={1}", _urlSite, vm.Folio), QRCodeGenerator.ECCLevel.Q);
                        }
                        // Image Format
                        var imgType = Base64QRCode.ImageType.Png;

                        var qrCode = new Base64QRCode(qrCodeData);
                        //Base64 Format
                        string qrCodeImageAsBase64 = qrCode.GetGraphic(20, SixLabors.ImageSharp.Color.Black, SixLabors.ImageSharp.Color.White, true, imgType);
                        //Sending Image format and base64 format to View
                        ViewBag.qrCodeImageAsBase64 = qrCodeImageAsBase64;
                        ViewBag.imgType = imgType.ToString().ToLower();
                        if (vm.Poblano)
                        {
                            var consulta = await _consultaVehiculoServicio.Consulta(vm.Serie, vm.Placa);
                            vm.Modelo = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

            return View("~/Views/PortalCitas/Consulta.cshtml", vm);
        }
        [Authorize]
        public async Task<IActionResult> ConsultaIdCita(long id)
        {
            if (id == 0)
                return RedirectToAction("Index");

            var vm = new PortalCitasComprobanteResponse();
            try
            {
                var helper = await PortalCitaHelper.GetCitaData(_proxyWebAPI, new() { Id = id });
                if (helper.IsSuccessFully)
                {
                    var resulthelper = helper.Result as PortalCitasComprobanteResponse ?? new PortalCitasComprobanteResponse();

                    //if (resulthelper.Cancelada == true)
                    //{
                    //    return RedirectToAction("Index");
                    //}

                    vm = resulthelper;
                    if (vm.Id != 0)
                    {
                        QRCodeData qrCodeData;
                        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                        {
                            qrCodeData = qrGenerator.CreateQrCode(string.Format("{0}PortalCitas/ConsultaCita?folio={1}", _urlSite, vm.Folio), QRCodeGenerator.ECCLevel.Q);
                        }
                        // Image Format
                        var imgType = Base64QRCode.ImageType.Png;

                        var qrCode = new Base64QRCode(qrCodeData);
                        //Base64 Format
                        string qrCodeImageAsBase64 = qrCode.GetGraphic(20, SixLabors.ImageSharp.Color.Black, SixLabors.ImageSharp.Color.White, true, imgType);
                        //Sending Image format and base64 format to View
                        ViewBag.qrCodeImageAsBase64 = qrCodeImageAsBase64;
                        ViewBag.imgType = imgType.ToString().ToLower();
                        if (vm.Poblano)
                        {
                            var consulta = await _consultaVehiculoServicio.Consulta(vm.Serie, vm.Placa);
                            vm.Modelo = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

            return View("~/Views/PortalCitas/Consulta.cshtml", vm);
        }

        [HttpPost]
        public async Task<JsonResult> CancelarCita(PortalCitaCancelarRequest request)
        {
            var result = new ResponseViewModel();
            try
            {
                var helper = await PortalCitaHelper.CancelarCita(_proxyWebAPI, request);

                if (!helper.IsSuccessFully)
                {
                    //result.Message = "Ocurrió un error al registrar el usuario.";
                    result.Message = helper.Message ?? "Ocurrió un error al realizar la acción.";
                    return new JsonResult(result);
                }
                result.IsSuccessFully = helper.IsSuccessFully;
                result.Result = helper.Result;
            }
            catch (Exception ex)
            {
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
                url = _blobStorage._url + "/PortalCita/" + url;
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


        [HttpPost]
        public async Task<ActionResult> ComprobanteCitaTest([FromBody] object registro)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var dataReport = new DataReport();
                var data = new ComprobanteCitaResponse
                {
                    DireccionCentroVerificacion = "DIRECCION DE PRUEBA",
                    Fecha = DateTime.Now,
                    Folio = "ABCDE01234",
                    NombreCentroVerificacion = "CENTRO 01 INVENTADO",
                    NombrePersona = "INDIVIDUO 3"
                };

                var getdoc = await _pdfBuilder.GetComprobanteCita(data);

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

        //Se comenta método que ayuda a confirmar la cita de verificación, para volver a funcionar, únicamente ajustar el método de abajo y el negocio
        //[HttpGet]
        //public async Task<IActionResult> ConfirmarCita(string folio)
        //{
        //    var result = new ResponseViewModel();
        //    var resulthelper = new PortalCitasComprobanteResponse();
        //    try
        //    {
        //        var helper = await PortalCitaHelper.ConfirmarCita(_proxyWebAPI, new ConfirmarCita { Folio = folio });
        //        ViewBag.MensajeCitaConfirmada = helper.Message;


        //        var helperCita = await PortalCitaHelper.GetCitaData(_proxyWebAPI, new() { Folio = folio });
        //        resulthelper = helperCita.Result as PortalCitasComprobanteResponse ?? new PortalCitasComprobanteResponse();

        //        if (!helper.IsSuccessFully)
        //        {
        //            result.Message = helper.Message ?? "Ocurrió un error al realizar la acción.";
        //            ViewBag.Error = true;
        //            return View("~/Views/PortalCitas/Consulta.cshtml", resulthelper);
        //        }


        //        if (resulthelper.Id != 0)
        //        {
        //            QRCodeData qrCodeData;
        //            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        //            {
        //                qrCodeData = qrGenerator.CreateQrCode(string.Format("{0}PortalCitas/ConsultaCita?folio={1}", _urlSite, resulthelper.Folio), QRCodeGenerator.ECCLevel.Q);
        //            }
        //            // Image Format
        //            var imgType = Base64QRCode.ImageType.Png;

        //            var qrCode = new Base64QRCode(qrCodeData);
        //            //Base64 Format
        //            string qrCodeImageAsBase64 = qrCode.GetGraphic(20, SixLabors.ImageSharp.Color.Black, SixLabors.ImageSharp.Color.White, true, imgType);
        //            //Sending Image format and base64 format to View
        //            ViewBag.qrCodeImageAsBase64 = qrCodeImageAsBase64;
        //            ViewBag.imgType = imgType.ToString().ToLower();
        //        }
        //        else
        //        {
        //            ViewBag.Expirada = false;
        //        }


        //        result.IsSuccessFully = helper.IsSuccessFully;
        //        result.Result = helper.Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.Message;
        //    }
        //    return View("~/Views/PortalCitas/Consulta.cshtml", resulthelper);
        //}

        [HttpGet]
        public async Task<IActionResult> AvisoCitaCreadaPorConfirmar(string folio)
        {
            var result = new ResponseViewModel(true);
            var resulthelper = new PortalCitasComprobanteResponse();
            try
            {

                folio = folio.Trim();
                var response = "";
                var helper = await PortalCitaHelper.GetCitaData(_proxyWebAPI, new() { Folio = folio });
                resulthelper = helper.Result as PortalCitasComprobanteResponse ?? new PortalCitasComprobanteResponse();

                if (!helper.IsSuccessFully)
                {

                    result.Message = helper.Message ?? "Ocurrió un error al realizar la acción.";
                    ViewBag.Error = true;
                    return View("~/Views/PortalCitas/Consulta.cshtml", resulthelper);
                }

                if (resulthelper.Id != 0)
                {
                    QRCodeData qrCodeData;
                    using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                    {
                        qrCodeData = qrGenerator.CreateQrCode(string.Format("{0}PortalCitas/ConsultaCita?folio={1}", _urlSite, resulthelper.Folio), QRCodeGenerator.ECCLevel.Q);
                    }
                    // Image Format
                    var imgType = Base64QRCode.ImageType.Png;

                    var qrCode = new Base64QRCode(qrCodeData);
                    //Base64 Format
                    string qrCodeImageAsBase64 = qrCode.GetGraphic(20, SixLabors.ImageSharp.Color.Black, SixLabors.ImageSharp.Color.White, true, imgType);
                    //Sending Image format and base64 format to View
                    ViewBag.qrCodeImageAsBase64 = qrCodeImageAsBase64;
                    ViewBag.imgType = imgType.ToString().ToLower();
                }
                else
                {
                    ViewBag.Expirada = false;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return View("~/Views/PortalCitas/Consulta.cshtml", resulthelper);
        }

        
    }
}
