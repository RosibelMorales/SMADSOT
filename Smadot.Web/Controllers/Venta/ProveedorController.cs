using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.Venta;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using System.Text;
using static Smadot.Models.Entities.Proveedor.Request.ProveedorRequestData;
using static Smadot.Models.Entities.Proveedor.Response.ProveedorResponseData;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Models.Dicts;

namespace Smadot.Web.Controllers.Venta
{
    [Authorize]
    public class ProveedorController : Controller
    {

        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public ProveedorController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuProveedores.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/Venta/Proveedor/Index.cshtml");
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

            var gridData = new ResponseGrid<ProveedorResponse>();
            var listaResult = await ProveedorHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<ProveedorResponse> ?? new List<ProveedorResponse>();

            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<ProveedorResponseGrid>();

            foreach (var row in data)
            {
                List.Add(new ProveedorResponseGrid
                {
                    Id= row.Id,
                    Nombre = row.Nombre,
                    CorreoElectronico = row.CorreoElectronico,
                    Direccion = row.Direccion,
                    Telefono = row.Telefono,
                    Empresa = row.Empresa,
                    Autorizado = row.Autorizado,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Venta/Proveedor/_Acciones.cshtml", row)
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
        public async Task<ActionResult> Registro()
        {
            var result = new ResponseViewModel(true);

            try
            {
                var response = new ProveedorResponse();
                result.Result = await this.RenderViewToStringAsync("~/Views/Venta/Proveedor/_Registro.cshtml",response);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Registro(ProveedorRequest registro)
        {
            var result = new ResponseViewModel(true);
            try
            {
                //if (!string.IsNullOrEmpty(registro.Telefono))
                //{
                //    var convert = Encoding.ASCII.GetBytes(registro.Telefono.Trim());

                //    registro.NumeroTelefono = convert;
                //}

                var helper = await ProveedorHelper.Guardar(_proxyWebAPI, registro);

                if (!string.IsNullOrEmpty((string)result.Result))
                {
                    result.IsSuccessFully = false;
                    result.Message = ((string)result.Result);
                }

            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> EstatusProveedor(EstatusProveedor registro)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var helper = await ProveedorHelper.CambiarAutorizacion(_proxyWebAPI, registro);

                if (!string.IsNullOrEmpty((string)result.Result))
                {
                    result.IsSuccessFully = false;
                    result.Message = ((string)result.Result);
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
        public async Task<ActionResult> Detalle(long id)
        {
            var result = new ResponseViewModel(true);

            try
            {
                var res = new JsonResultDto();
                var listaResult = await ProveedorHelper.GetById(_proxyWebAPI, id);
                var response = listaResult.Result as ProveedorResponse ?? new ProveedorResponse();
                
                result.Result = await this.RenderViewToStringAsync("~/Views/Venta/Proveedor/_Detalle.cshtml", response);
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
            ViewBag.Tramites = "";
            ViewBag.SolicitudFormaValorada = "";
            ViewBag.Administracion = "";
        }
    }
}
