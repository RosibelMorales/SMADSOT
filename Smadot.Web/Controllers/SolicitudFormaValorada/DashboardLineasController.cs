using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.DashboardLineas.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Linea.Request;
using Smadot.Models.Entities.Refrendo.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.SeguimientoCVV;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Models;
using Smadot.Web.Utilities;

namespace Smadot.Web.Controllers.SolicitudFormaValorada
{
    [Authorize]
    public class DashboardLineasController : Controller
    {
        private readonly IUserResolver _userResolver;
        private readonly IProxyWebAPI _proxyWebAPI;
        public DashboardLineasController(IUserResolver userResolver, IProxyWebAPI proxyWebAPI)
        {
            _userResolver = userResolver;
            _proxyWebAPI = proxyWebAPI;
        }

        // GET: DashboardLineasController
        public async Task<ActionResult> Index()
        {
            if (!_userResolver.HasPermission(PermisosMenu.DashboardLineas))
                return new UnauthorizedResult();
            var lineas = new List<DashboardLineaResponseData>();
            var request = await VerificacionHelper.ObetenerInformacionLineas(_proxyWebAPI);
            if (request.IsSuccessFully)
            {
                lineas = (List<DashboardLineaResponseData>)request.Result;
            }
            ViewBag.Lineas = lineas;
            ViewBag.UserName = _userResolver.GetUser().Nombre;
            ViewBag.Verificentro = _userResolver.GetUser().NombreVerificentro;
            return View("~/Views/DashboardLineas/Index.cshtml");
        }

        // Open Camera Modal
        // [HttpGet]
        // public async Task<ActionResult> CameraModal()
        // {
        //     var result = new JsonResultDto();

        //     if (_userResolver.GetUser().IdUser > 0)
        //     {
        //         result.Result = await this.RenderViewToStringAsync("~/Views/DashboardLineas/_CamaraModal.cshtml", " ");
        //     }
        //     else
        //     {
        //         return RedirectToAction("Index", "Autenticacion");
        //     }
        //     return Json(result);
        // }

        // private List<DashboardLineaResponseData> GetSampleModel()
        // {
        //     var persona = new List<DashboardLineaResponseData>()
        //     {
        //         new DashboardLineaResponseData() {Linea = "A 1", AutosEspera = 2, Turno = "456", Duenio = "Juan Hernandez", Modelo = "Vento", Placas = "NGF-92-58", Camara = true, Line = true },
        //         new DashboardLineaResponseData() {Linea = "A 2", AutosEspera = 1, Turno = "326", Duenio = "Edgar Rodriguez", Modelo = "Audi", Placas = "PJK-31-02", Camara = false, Line = true },
        //         new DashboardLineaResponseData() {Linea = "B 1", AutosEspera = 4, Turno = "245", Duenio = "Mario Cruz", Modelo = "Bmw", Placas = "QEE-22-01", Camara = true, Line = false },
        //         new DashboardLineaResponseData() {Linea = "K 5", AutosEspera = 3, Turno = "512", Duenio = "Itzel Garcia", Modelo = "Jetta", Placas = "NAD-32-67", Camara = true, Line = true },
        //         new DashboardLineaResponseData() {Linea = "C 6", AutosEspera = 5, Turno = "654", Duenio = "Felipe Juarez", Modelo = "VW", Placas = "MNV-96-00", Camara = true, Line = true },
        //         new DashboardLineaResponseData() {Linea = "M 3", AutosEspera = 1, Turno = "342", Duenio = "Hector Sanchez", Modelo = "Mazda", Placas = "HJK-43-21", Camara = false, Line = true },
        //         new DashboardLineaResponseData() {Linea = "J 1", AutosEspera = 6, Turno = "498", Duenio = "Emilio Estrada", Modelo = "Chevrolet", Placas = "FGH-32-99", Camara = true, Line = false },
        //         new DashboardLineaResponseData() {Linea = "H 2", AutosEspera = 4, Turno = "510", Duenio = "Carlos Lopez", Modelo = "Jaguar", Placas = "YLN-18-11", Camara = true, Line = true },
        //     };
        //     return persona;
        // }

        [HttpGet]
        public async Task<IActionResult> UpdateLineas()
        {
            var result = new ResponseViewModel(true);
            try
            {
                if (!_userResolver.HasPermission(PermisosMenu.DashboardLineas))
                    throw new Exception("No tiene permisos.");
                var lineas = new List<DashboardLineaResponseData>();
                var request = await VerificacionHelper.ObetenerInformacionLineas(_proxyWebAPI);
                if (request.IsSuccessFully)
                {
                    lineas = (List<DashboardLineaResponseData>)request.Result;
                }
                foreach (var linea in lineas)
                {
                    result.Result += "<div class='col-xl-4 mb-xl-4'>" + await this.RenderViewToStringAsync("~/Views/DashboardLineas/Dash_Lineas_Partial.cshtml", linea) + "</div>";
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
        public async Task<JsonResult> AbrirLinea(DashboardLineaRequest request)
        {
            var result = new ResponseViewModel();
            try
            {
                request.Nota = "Se bloquea por irregularidad";
                request.IdMotivo = MotivoLinea.Irregularidad;
                request.Bloquear = false;

                var helper = await CierreAperturaLineaHelper.DashboardLineaAperturaCierre(_proxyWebAPI, request);

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

        [HttpPost]
        public async Task<JsonResult> CerrarLinea(DashboardLineaRequest request)
        {
            var result = new ResponseViewModel();
            try
            {
                request.Nota = "Se desbloquea por irregularidad";
                request.IdMotivo = MotivoLinea.Irregularidad;
                request.Bloquear = true;

                var helper = await CierreAperturaLineaHelper.DashboardLineaAperturaCierre(_proxyWebAPI, request);

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

    }
}
