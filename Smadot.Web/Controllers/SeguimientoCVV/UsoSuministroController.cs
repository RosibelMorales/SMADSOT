using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.UsoSuministro.Response;
using Smadot.Models.Entities.UsoSuministro.Request;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System;
using Newtonsoft.Json;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Utilities;
using System.Linq.Dynamic.Core;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.FileStorage;
using Smadot.Web.Helper.Operaciones.SeguimientoCVV;
using NPOI.HSSF.Record;
using Smadot.Web.Models.EquipoTipoCalibracion;
using Smadot.Web.Models;
using Smadot.Web.Helper.Operaciones.AdministracionStock;

namespace Smadot.Web.Controllers.SeguimientoCVV
{
    [Authorize]
    public class UsoSuministroController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public UsoSuministroController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuUsoSuministros.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/UsoSuministro/Index.cshtml");
        }
        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");
            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            //Invoca al método que se encarga de realizar la petición Api
            var request = new UsoSuministroRequest();
            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

            var listaResult = await UsoSuministroHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<vUsoSuministroResponse> ?? new List<vUsoSuministroResponse>();

            ////Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? totalRecord : data.Count();

            var List = new List<vUsoSuministroResponseGrid>();
            foreach( var row in data)
            {
                List.Add(new vUsoSuministroResponseGrid
                {
                    Id = row.Id,
                    NombreUsoSuministro = row.NombreUsoSuministro,
                    Cantidad = row.Cantidad,
                    Fecha = row.Fecha,
                    IdUserRegistro = row.IdUserRegistro,
                    NombreUser = row.NombreUser,
                    UrlFactura = row.UrlFactura,
                    Acciones = await this.RenderViewToStringAsync("~/Views/UsoSuministro/_Acciones.cshtml", row)
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
        public async Task<IActionResult> Registro()
        {
            var result = new ResponseViewModel(true);
            result.Result = await this.RenderViewToStringAsync("~/Views/UsoSuministro/_ModalRegistro.cshtml");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Registro([FromBody] UsoSuministroResponse registro)
        {
            var result = new JsonResultDto();
            try
            {
                if(registro.Id == 0)
                {
                    //registro.Files = JsonConvert.DeserializeObject<List<UsoSuministroFile>>(Request.Form["Files"].FirstOrDefault() ?? "");
                    registro.Files = JsonConvert.DeserializeObject<List<UsoSuministroFile>>(registro.FilesString ?? "");

                    var save = await UsoSuministroHelper.Registro(_proxyWebAPI, registro);
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
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> Detalle(long id)
        {
            var result = new ResponseViewModel(true);
            var vm = new vUsoSuministroResponse();
            try
            {
                var response = await UsoSuministroHelper.Detalle(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                vm = response.Result as vUsoSuministroResponse ?? vm;
                var VBUrlDoc1 = "";
                if (!string.IsNullOrEmpty(vm.UrlFactura))
                {
                    var arrDoc = vm.UrlFactura.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm.UrlFactura, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageUrlFactura = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc1 = imageUrlFactura;
                    }
                }
                ViewBag.UrlFactura = VBUrlDoc1;
                result.Result = await this.RenderViewToStringAsync("~/Views/UsoSuministro/_ModalDetalle.cshtml", vm);
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

        //private void SetMenuLinkActive()
        //{
        //    ViewBag.Tramites = "";
        //    ViewBag.SolicitudFormaValorada = "";
        //    ViewBag.Administracion = "";
        //    ViewBag.SeguimientoCVV = "here";
        //    ViewBag.linkAdminCVV = "";
        //}
    }
}
