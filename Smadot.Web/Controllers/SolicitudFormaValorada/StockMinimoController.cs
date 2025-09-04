using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.StockMinimo.Request;
using Smadot.Models.Entities.StockMinimo.Response;
using Smadot.Web.Utilities;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Seguridad;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Utilities.ProxyWebAPI.Implementacion;
using Smadot.Utilities.GestionTokens;
using Smadot.Web.Models.ConsultaStockDVRFModel;
using Smadot.Web.Models.StockMinimoModel;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Models.Dicts;

namespace Smadot.Web.Controllers.SolicitudFormaValorada
{
	[Authorize]
	public class StockMinimoController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;

        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public StockMinimoController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(PermisosMenu.DeterminaciondeStockMinimo))
                return new UnauthorizedResult();
            var vm = new StockMinimoViewModel.Registro();
            var almacenes = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest() { Activo = true });
            //vm.Almacenes = (almacenes.Result as List<AlmacenResponse> ?? new List<AlmacenResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
            ViewBag.Almacenes = (almacenes.Result as List<AlmacenResponse> ?? new List<AlmacenResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();

            return View("~/Views/StockMinimo/Index.cshtml", vm);
        }


        [HttpGet]
        public async Task<JsonResult> Consulta(long? id)
        {
            var result = new ResponseViewModel(true);
            var vm = new List<StockMinimoResponse>();
            try
            {
                var request = new StockMinimoRequest();
                request.IdAlmacen = id;

                var listaResult = await SolicitudFormaValoradaHelper.GetStockMinimo(_proxyWebAPI, request);
                var data = listaResult.Result as List<StockMinimoResponse> ?? new List<StockMinimoResponse>();
                //return Json(data);
                data = data.OrderBy(x => x.ClaveCertificado).ToList();
                if(request.IdAlmacen != 1)
                {
                    data = data.Where(x => x.IdCatTipoCertificado != TipoCertificado.Testificacion && x.IdCatTipoCertificado != TipoCertificado.Exentos).ToList();
                }
                result.Result = await this.RenderViewToStringAsync("~/Views/StockMinimo/_Registro.cshtml",data);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);

        }

        [HttpPost]
        public async Task<IActionResult> Registro(List<StockMinimoResponse> registro)
        {
            var result = new ResponseViewModel(true);
            try
            {

                foreach( var item in registro )
                {
                    item.Nombre = "";
                    item.ClaveCertificado = "";
                    item.NombreAlmacen = "";
                }
                var save = await SolicitudFormaValoradaHelper.RegistroStockMinimo(_proxyWebAPI, registro);
                result = save;
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
