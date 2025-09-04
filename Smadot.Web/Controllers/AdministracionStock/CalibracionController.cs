using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Calibracion.Request;
using Smadot.Models.Entities.Calibracion.Response;
using Smadot.Models.Entities.Equipo.Request;
using Smadot.Models.Entities.Equipo.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Linea.Request;
using Smadot.Models.Entities.Linea.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.AdministracionStock;
using Smadot.Web.Helper.Operaciones.Venta;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using static Smadot.Models.Entities.ProveedorFolioServicio.Request.ProveedorFolioServicioRequestData;
using static Smadot.Models.Entities.ProveedorFolioServicio.Response.ProveedorFolioServicioResponseData;

namespace Smadot.Web.Controllers.AdministracionStock
{
    [Authorize]
    public class CalibracionController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly BlobStorage _blobStorage;
        private readonly IUserResolver _userResolver;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public CalibracionController(IProxyWebAPI proxyWebAPI, IConfiguration configuration, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _userResolver = userResolver;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(MenuCalibraciones.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/Calibracion/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Consulta(long? id)
        {
            JsonResult result = new JsonResult("");

            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var page = skip / pageSize + 1;

            //Invoca al método que se encarga de realizar la petición Api
            var request = new CalibracionListRequest();
            request.Pagina = page;
            request.Registros = pageSize;
            request.Busqueda = searchValue;
            request.ColumnaOrdenamiento = sortColumn;
            request.Ordenamiento = sortColumnDirection;
            request.IdVerificentro = id;

            var listResult = await CalibracionHelper.Consulta(_proxyWebAPI, request);
            var data = listResult.Result as List<vCalibracionResponse> ?? new List<vCalibracionResponse>();

            //Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            //Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? totalRecord : data.Count();

            var List = new List<vCalibracionResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new vCalibracionResponseGrid
                {
                    Id = row.Id,
                    FechaCalibracion = row.FechaCalibracion,
                    Consecutivo = row.Consecutivo,
                    FechaProgramada = row.FechaProgramada,
                    FechaProximaCalibracion = row.FechaProximaCalibracion,
                    Laboratorio = row.Laboratorio,
                    IdUserRegistro = row.IdUserRegistro,
                    NombreUserRegistro = row.NombreUserRegistro,
                    IdUserValido = row.IdUserValido,
                    NombreUserValido = row.NombreUserValido,
                    Nota = row.Nota,
                    NombreEquipo = row.NombreEquipo,
                    NombreEstatusCalibracion = row.NombreEstatusCalibracion,
                    NombreLinea = row.NombreLinea,
                    FechaRegistro = row.FechaRegistro,
                    IdCatEstatusCalibracion = row.IdCatEstatusCalibracion,
                    IdCatEstatusEquipo = row.IdCatEstatusEquipo,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Calibracion/_Acciones.cshtml", row)
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
        public async Task<JsonResult> Detalle(long id)
        {
            var result = new ResponseViewModel(true);
            var vm = new vCalibracionResponse();
            try
            {
                var response = await CalibracionHelper.Detalle(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                vm = response.Result as vCalibracionResponse ?? vm;

                ViewBag.IsDSinRevisar = vm.IdCatEstatusCalibracion > 0 ? (vm.IdCatEstatusCalibracion == CalibracionEstatus.DocumentacionSinRevisar) ? 1 : 0 : 0;
                ViewBag.IsSolicitarM = vm.IdCatEstatusCalibracion > 0 ? vm.IdCatEstatusCalibracion == CalibracionEstatus.SolicitaModificar ? 1 : 0 : 0;
                var VBUrlDoc1 = "";
                if (!string.IsNullOrEmpty(vm.UrlResultado))
                {
                    var arrDoc = vm.UrlResultado.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm.UrlResultado, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageUrlResultado = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc1 = imageUrlResultado;
                    }
                }
                ViewBag.UrlResultado = VBUrlDoc1;
                result.Result = await this.RenderViewToStringAsync("~/Views/Calibracion/_ModalDetalle.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> Registro(long id)
        {
            var result = new ResponseViewModel(true);
            var lineas = await EquipoHelper.ConsultaLinea(_proxyWebAPI, new LineaRequest() { Activo = true });
            if (lineas.IsSuccessFully)
            {
                ViewBag.Lineas = ((List<LineaResponse>)lineas.Result).Select(obj => new SelectListItem { Value = obj.Id.ToString(), Text = obj.Nombre }).ToList();
            }

            var vm = new vCalibracionResponse() { FechaCalibracion = DateTime.Now };
            if (id > 0)
            {
                var response = await CalibracionHelper.Detalle(_proxyWebAPI, id);
                vm = response.Result as vCalibracionResponse ?? vm;
            }

            result.Result = await this.RenderViewToStringAsync("~/Views/Calibracion/_ModalRegistro.cshtml", vm);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetLineas()
        {
            var lineas = await EquipoHelper.ConsultaLinea(_proxyWebAPI, new LineaRequest() { Activo = true });
            if (lineas.IsSuccessFully)
            {
                ViewBag.Lineas = ((List<LineaResponse>)lineas.Result).Select(obj => new SelectListItem { Value = obj.Id.ToString(), Text = obj.Nombre }).ToList();
            }
            return Json(ViewBag.Lineas);
        }

        [HttpGet]
        public async Task<IActionResult> GetEquipos(long? idLinea)
        {
            var jr = new JsonResultDto();
            try
            {
                var listaResult = await EquipoHelper.Consulta(_proxyWebAPI, new EquipoRequest { Activo = true, IdLinea = idLinea });

                var equipos = ((List<vEquipoResponse>)listaResult.Result).Select(x => new { Id = x.Id, Nombre = x.NombreEquipo })
                                                                         .ToList();

                jr.Result = equipos;
            }
            catch (Exception e)
            {
                jr.Error = true;
                jr.ErrorDescription = e.Message;
            }

            return Json(jr);
        }

        [HttpPost]
        public async Task<ActionResult> Registro(CalibracionRequest request)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var files = JsonConvert.DeserializeObject<List<CalibracionFile>>(Request.Form["Files"].FirstOrDefault() ?? "");
                request.Files = files ?? new List<CalibracionFile>();
                var response = new ResponseViewModel(true);

                if (request.Id > 0)
                    response = await CalibracionHelper.Editar(_proxyWebAPI, request);
                else
                    response = await CalibracionHelper.Registro(_proxyWebAPI, request);

                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    result.Message = response.Message;
                    return new JsonResult(result);
                }

                result.Result = response.Result;
            }
            catch (Exception e)
            {
                result.IsSuccessFully = false;
                result.Message = e.Message;
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
            var listaResult = await ProveedorHelper.Autocomplete(_proxyWebAPI, new ProveedorFolioServicioAutocompletRequest { Term = term, Start = start, End = end, EsLaboratorio = true });
            var data = listaResult.Result as List<ProveedorFolioServicioAutocompleteResponse> ?? new List<ProveedorFolioServicioAutocompleteResponse>();

            return Json(new { items = data, total_count = data.Count });
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

        [HttpPut]
        public async Task<JsonResult> SolicitarModificacion(long id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var response = await CalibracionHelper.SolicitarModificacion(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al solicitar la modificación";
                    return new JsonResult(result);
                }
                result.Message = "Se ha solicitado la modificación correctamente";
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPut]
        public async Task<JsonResult> Validar(long id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var response = await CalibracionHelper.Validar(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al validar esta solicitud";
                    return new JsonResult(result);
                }
                result.Message = "Se ha validado la solicitud correctamente";
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPut]
        public async Task<JsonResult> RechazarDocumentacion(long id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var response = await CalibracionHelper.RechazarDocumentacion(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al rechazar esta solicitud";
                    return new JsonResult(result);
                }
                result.Message = "Se ha rechazado la solicitud correctamente";
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPut]
        public async Task<JsonResult> Autorizar(long id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var response = await CalibracionHelper.Autorizar(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al autorizar esta solicitud";
                    return new JsonResult(result);
                }
                result.Message = "Se ha autorizado la solicitud correctamente";
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPut]
        public async Task<JsonResult> RechazarSolicitar(long id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var response = await CalibracionHelper.RechazarSolicitar(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al rechazar esta solicitud";
                    return new JsonResult(result);
                }
                result.Message = "Se ha rechazado la solicitud correctamente";
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

    }
}
