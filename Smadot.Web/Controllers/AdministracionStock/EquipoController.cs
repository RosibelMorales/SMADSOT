using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Equipo.Request;
using Smadot.Models.Entities.Equipo.Response;
using Smadot.Models.Entities.Linea.Request;
using Smadot.Models.Entities.Linea.Response;
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
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Web.Models.Exento;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.FileStorage;
using Smadot.Models.Dicts;
using System.ComponentModel.DataAnnotations;
using Smadot.Models.DataBase;
using System.Net;
using Smadot.Models.Entities.Catalogos.Request;

namespace Smadot.Web.Controllers.AdministracionStock
{
    [Authorize]
    public class EquipoController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;


        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public EquipoController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuInventarioEquipos.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/Equipo/Index.cshtml");
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

            var request = new EquipoRequest();
            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

            var listaResult = await EquipoHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<vEquipoResponse> ?? new List<vEquipoResponse>();

            ////Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? totalRecord : data.Count();

            var List = new List<vEquipoResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new vEquipoResponseGrid
                {
                    Id = row.Id,
                    NombreLinea = row.NombreLinea,
                    NumeroSerie = row.NumeroSerie,
                    IdCatEstatusEquipo = row.IdCatEstatusEquipo,
                    NombreEquipo = row.NombreEquipo,
                    IdUserRegistro = row.IdUserRegistro,
                    Nombre = row.Nombre,
                    FechaRegistro = row.FechaRegistro,
                    FechaProximaCalibracion = row.FechaProximaCalibracion,
                    Estatus = row.Estatus,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Equipo/_Acciones.cshtml", row)
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
        public async Task<IActionResult> Detalle(long id)
        {
            var listaResult = await EquipoHelper.Detalle(_proxyWebAPI, id);
            if (!listaResult.IsSuccessFully)
                return BadRequest();
            var vm = listaResult.Result as vEquipoResponse ?? new vEquipoResponse();
            var VBUrlDoc1 = "";
            var VBUrlDoc2 = "";
            var VBUrlDoc3 = "";
            var VBUrlDoc4 = "";
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
            if (!string.IsNullOrEmpty(vm.UrlManualUsuario))
            {
                var arrDoc = vm.UrlManualUsuario.Split("/");
                var NameDoc1 = arrDoc[arrDoc.Length - 1];
                var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                var archivo = await _blobStorage.DownloadFileAsync(vm.UrlManualUsuario, true);
                if (archivo.IsSuccessFully)
                {
                    var imageUrlManualUsuario = string.Format("{0}{1}", ContentType1, archivo.Result);
                    VBUrlDoc2 = imageUrlManualUsuario;
                }
            }
            if (!string.IsNullOrEmpty(vm.UrlEspecificacionTecnica))
            {
                var arrDoc = vm.UrlEspecificacionTecnica.Split("/");
                var NameDoc1 = arrDoc[arrDoc.Length - 1];
                var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                var archivo = await _blobStorage.DownloadFileAsync(vm.UrlEspecificacionTecnica, true);
                if (archivo.IsSuccessFully)
                {
                    var imageUrlEspecificacionTecnica = string.Format("{0}{1}", ContentType1, archivo.Result);
                    VBUrlDoc3 = imageUrlEspecificacionTecnica;
                }
            }
            if (!string.IsNullOrEmpty(vm.UrlRecomendacionServicio))
            {
                var arrDoc = vm.UrlRecomendacionServicio.Split("/");
                var NameDoc1 = arrDoc[arrDoc.Length - 1];
                var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                var archivo = await _blobStorage.DownloadFileAsync(vm.UrlRecomendacionServicio, true);
                if (archivo.IsSuccessFully)
                {
                    var imageUrlRecomendacionServicio = string.Format("{0}{1}", ContentType1, archivo.Result);
                    VBUrlDoc4 = imageUrlRecomendacionServicio;
                }
            }
            ViewBag.UrlFactura = VBUrlDoc1;
            ViewBag.UrlManualUsuario = VBUrlDoc2;
            ViewBag.UrlEspecificacionTecnica = VBUrlDoc3;
            ViewBag.UrlRecomendacionServicio = VBUrlDoc4;
            return View("~/Views/Equipo/Detalle.cshtml", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Detalle([FromBody] AuxEstatusResponse request)
        {
            var result = new JsonResultDto();
            try
            {
                if (request.Id > 0)
                {
                    var save = await EquipoHelper.ActualizarEstatus(_proxyWebAPI, request);
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

        // [HttpGet]
        // public async Task<IActionResult> Registro(long? id)
        // {
        //     var result = new ResponseViewModel(true);
        //     try
        //     {
        //         ViewBag.TiposEquipo = EquiposDict.Equipos.Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value }).ToList();
        //         var lineas = await EquipoHelper.ConsultaLinea(_proxyWebAPI, new LineaRequest() { Activo = true });
        //         if (lineas.IsSuccessFully)
        //         {
        //             ViewBag.lineas = ((List<LineaResponse>)lineas.Result).Select(obj => new SelectListItem { Value = obj.Id.ToString(), Text = obj.Nombre }).ToList();
        //         }
        //         ViewBag.Enabled = true;
        //         return View("~/Views/Equipo/Registro.cshtml");
        //     }
        //     catch (Exception ex)
        //     {
        //         result.IsSuccessFully = false;
        //         result.Message = ex.Message;
        //     }
        //     return new JsonResult(result);

        // }
        [HttpGet]
        public async Task<JsonResult> Registro(long? id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var vm = new EquipoResponse();
                vm.Files = new List<EquipoFiles>();
                if (id.HasValue)
                {
                    var response = await EquipoHelper.Detalle(_proxyWebAPI, id.Value);
                    if (!response.IsSuccessFully)
                    {
                        result.IsSuccessFully = false;
                        result.Message = "No se encontró información del Equipo";
                        return new JsonResult(result);
                    }
                    var data = response.Result as vEquipoResponse ?? new vEquipoResponse();
                    vm.Id = data.Id ?? 0;
                    vm.IdCatTipoEquipo = data.IdCatTipoEquipo;
                    vm.Nombre = data.NombreEquipo;
                    vm.IdTipoEquipo = EquiposDict.Equipos.FirstOrDefault(x => x.Value.Contains(data.NombreEquipo ?? "")).Key;
                    vm.IdLinea = data.IdLinea;
                    vm.NumeroSerie = data.NumeroSerie;

                    if (!string.IsNullOrEmpty(data.UrlFactura))
                        vm.Files.Add(new EquipoFiles { Tipo = "Factura", Nombre = data.UrlFactura.Split("/")[data.UrlFactura.Split("/").Length - 1], Base64 = StorageManager.GetParsePath(data.UrlFactura) });
                    if (!string.IsNullOrEmpty(data.UrlEspecificacionTecnica))
                        vm.Files.Add(new EquipoFiles { Tipo = "EspecificacionTecnica", Nombre = data.UrlEspecificacionTecnica.Split("/")[data.UrlEspecificacionTecnica.Split("/").Length - 1], Base64 = StorageManager.GetParsePath(data.UrlEspecificacionTecnica) });
                    if (!string.IsNullOrEmpty(data.UrlManualUsuario))
                        vm.Files.Add(new EquipoFiles { Tipo = "ManualUsuario", Nombre = data.UrlManualUsuario.Split("/")[data.UrlManualUsuario.Split("/").Length - 1], Base64 = StorageManager.GetParsePath(data.UrlManualUsuario) });
                    if (!string.IsNullOrEmpty(data.UrlRecomendacionServicio))
                        vm.Files.Add(new EquipoFiles { Tipo = "RecomendacionesServcio", Nombre = data.UrlRecomendacionServicio.Split("/")[data.UrlRecomendacionServicio.Split("/").Length - 1], Base64 = StorageManager.GetParsePath(data.UrlRecomendacionServicio) });
                }
                //ViewBag.TiposEquipo = EquiposDict.Equipos.Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value }).ToList();
                var equipo = await TipoEquipoHelper.Consulta(_proxyWebAPI, new TipoEquipoRequest { Activo = true });
                if (equipo.IsSuccessFully)
                {
                    ViewBag.equipo = ((List<TipoEquipoResponse>)equipo.Result).Select(obj => new SelectListItem { Value = obj.Id.ToString(), Text = obj.Nombre }).ToList();
                }
                var lineas = await EquipoHelper.ConsultaLinea(_proxyWebAPI, new LineaRequest() { Activo = true });
                if (lineas.IsSuccessFully)
                {
                    ViewBag.lineas = ((List<LineaResponse>)lineas.Result).Select(obj => new SelectListItem { Value = obj.Id.ToString(), Text = obj.Nombre }).ToList();
                }
                ViewBag.Enabled = true;
                result.Result = await this.RenderViewToStringAsync("~/Views/Equipo/Registro.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Registro([FromBody] EquipoResponse registro)
        {
            var result = new JsonResultDto();
            try
            {

                //if (EquiposDict.Equipos.TryGetValue(registro.IdCatTipoEquipo, out string? nombre))
                //{
                //    throw new ValidationException("No existe el tipo de equipo que se intenta registrar.");
                //}
                registro.Nombre = EquiposDict.Equipos[registro.IdCatTipoEquipo];
                registro.Files = JsonConvert.DeserializeObject<List<EquipoFiles>>(registro.FilesString ?? "");

                var save = await EquipoHelper.Registro(_proxyWebAPI, registro);
                result.Error = !save.IsSuccessFully;
                result.ErrorDescription = save.Message;
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
        public async Task<IActionResult> Validar(long id)
        {
            var result = new JsonResultDto();
            try
            {
                var aux = new AuxValidarResponse
                {
                    Id = id
                };

                if (id > 0)
                {
                    var validar = await EquipoHelper.Validar(_proxyWebAPI, aux);

                    if (!validar.IsSuccessFully)
                    {
                        result.Error = true;
                        result.ErrorDescription = _mensajeErrorCatch;

                        return Json(result);
                    }

                    result.ErrorDescription = "La operación se realizó éxitosamente";
                }
                else
                {
                    result.Error = true;
                    result.ErrorDescription = _mensajeErrorCatch;
                }
            }
            catch (Exception ex)
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
