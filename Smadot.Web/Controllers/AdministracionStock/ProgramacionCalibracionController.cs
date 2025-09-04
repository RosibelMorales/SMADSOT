using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.ProgramacionCalibracion.Request;
using Smadot.Models.Entities.ProgramacionCalibracion.Response;
using Smadot.Models.Entities.EquipoTipoCalibracion.Request;
using Smadot.Models.Entities.EquipoTipoCalibracion.Response;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.AdministracionStock;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Utilities.Modelos;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Web.Utilities;
using Smadot.Web.Models;
using System;
using Newtonsoft.Json;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Utilities.GestionTokens;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Web.Models.EquipoTipoCalibracion;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.SeguimientoCVV.Response;
using Smadot.Models.Entities.Equipo.Response;
using System.Web.Mvc.Async;
using Smadot.Web.Models.Exento;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Dicts;

namespace Smadot.Web.Controllers.AdministracionStock
{
    [Authorize]
    public class ProgramacionCalibracionController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuider;
        private readonly BlobStorage _blobStorage;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public ProgramacionCalibracionController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }
        public IActionResult Index(long id)
        {
            ViewBag.idEquipo = id;
            if (!_userResolver.HasPermission(MenuInventarioEquipos.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/ProgramacionCalibracion/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Consulta(long id)
        {
            JsonResult result = new JsonResult("");

            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            var request = new ProgramacionCalibracionRequest();
            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();
            request.IdEquipo = id;

            var listaResult = await ProgramacionCalibracionHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<vProgramacionCalibracionResponse> ?? new List<vProgramacionCalibracionResponse>();

            ////Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? totalRecord : data.Count();

            var List = new List<vProgramacionCalibracionResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new vProgramacionCalibracionResponseGrid
                {
                    Id = row.Id,
                    IdCatTipoCalibracion = row.IdCatTipoCalibracion,
                    NombreTipoCalibracion = row.NombreTipoCalibracion,
                    PrimeraFechaCalibracion = row.PrimeraFechaCalibracion,
                    IdUserRegistro = row.IdUserRegistro,
                    NombreUser = row.NombreUser,
                    FechaRegistro = row.FechaRegistro,
                    IdUserValido= row.IdUserValido,
                    NombreValido= row.NombreValido,
                    Estatus = row.Estatus,
                    Nota = row.Nota,
                    Acciones = await this.RenderViewToStringAsync("~/Views/ProgramacionCalibracion/_Acciones.cshtml", row)
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
            var vm = new vProgramacionCalibracionResponse();

            try
            {
                var response = await ProgramacionCalibracionHelper.Detalle(_proxyWebAPI, id);
                if(!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                vm = response.Result as vProgramacionCalibracionResponse ?? vm;
                var VBUrlDoc1 = "";
                if (!string.IsNullOrEmpty(vm.UrlDocumento1))
                {
                    var arrDoc = vm.UrlDocumento1.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm.UrlDocumento1, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageUrlDocumento1 = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc1 = imageUrlDocumento1;
                    }
                }
                ViewBag.UrlDoc1 = VBUrlDoc1;
                result.Result = await this.RenderViewToStringAsync("~/Views/ProgramacionCalibracion/_ModalDetalle.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Detalle([FromBody] AuxiliarEstatusResponse request)
        {
            var result = new JsonResultDto();
            try
            {
                if(request.Id > 0)
                {
                    var save = await ProgramacionCalibracionHelper.CambioEstatus(_proxyWebAPI, request);
                    if (!save.IsSuccessFully)
                    {
                        result.Error = true;
                        result.ErrorDescription = _mensajeErrorCatch;
                    }
                    else
                    {
                        result.Error = false;
                        result.ErrorDescription = save.Message ?? "La información se actualizó correctamente";
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> DetalleRechazar([FromBody] AuxiliarEstatusResponse request)
        {
            var result = new JsonResultDto();
            try
            {
                if (request.Id > 0)
                {
                    var save = await ProgramacionCalibracionHelper.EstatusRechazar(_proxyWebAPI, request);
                    if (!save.IsSuccessFully)
                    {
                        result.Error = true;
                        result.ErrorDescription = _mensajeErrorCatch;
                    }
                    else
                    {
                        result.Error = false;
                        result.ErrorDescription = save.Message ?? "La información se actualizó correctamente";
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Registro(long? id)
        {
            var result = new ResponseViewModel(true);
            var vm = new EquipoTipoCalibracionViewModel();
            try
            {
                var calibracion = await TipoCalibracionHelper.Consulta(_proxyWebAPI, new TipoCalibracionRequest() { Activo = true });
                vm.Calibraciones = (calibracion.Result as List<TipoCalibracionResponse> ?? new List<TipoCalibracionResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();

                result.Result = await this.RenderViewToStringAsync("~/Views/ProgramacionCalibracion/_Registro.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Registro([FromBody] EquipoTipoCalibracionViewModel registro)
        {
            var result = new JsonResultDto();
            try
            {
                if(registro.Id == 0)
                {
                    registro.Files = JsonConvert.DeserializeObject<List<EquipoTipoCalibracionDoc>>(registro.FilesString ?? "");

                    var save = await ProgramacionCalibracionHelper.Registro(_proxyWebAPI, registro);
                    if (!save.IsSuccessFully)
                    {
                        result.Error = true;
                        result.ErrorDescription = _mensajeErrorCatch;
                    }
                    else
                    {
                        result.Error = false;
                        result.ErrorDescription = save.Message ?? "La información se guardo correctamente";
                    }
                }
                return Json(result);
            }
            catch(Exception ex)
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

    }
}
