using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Refrendo.Request;
using Smadot.Models.Entities.Refrendo.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Utilities;
using Smadot.Models.Entities.ConstanciaUltimaVerificacion.Response;
using Smadot.Web.Helper.Operaciones.Tramite;
using Smadot.Models.Entities.ConstanciaUltimaVerificacion.Request;
using Smadot.Models.Dicts;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Models.Entities.Reposicion.Response;
using Smadot.Web.Models;

namespace Smadot.Web.Controllers.Tramite
{
    [Authorize]
	public class ConstanciaUltimaVerificacionController : Controller
	{
		private readonly IProxyWebAPI _proxyWebAPI;
		private readonly IUserResolver _userResolver;
		private readonly IPdfBuider _pdfBuider;
		private readonly BlobStorage _blobStorage;

		public ConstanciaUltimaVerificacionController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConfiguration configuration)
		{
			_proxyWebAPI = proxyWebAPI;
			_userResolver = userResolver;
			_pdfBuider = pdfBuider;
			_blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
		}

		public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(MenuConstanciasUltimaVerificacion.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/Tramite/ConstanciaUltimaVerificacion/Index.cshtml");
		}

		[HttpPost]
		public async Task<JsonResult> Consulta()
		{
			JsonResult result = new JsonResult("");

			var draw = Request.Form["draw"].FirstOrDefault();
			int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
			int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            //Invoca al método que se encarga de realizar la petición Api
            var request = new SolicitudFormaValoradaListRequest
            {
                Pagina = skip / pageSize + 1,
                Registros = pageSize,
                Busqueda = Request.Form["search[value]"].FirstOrDefault(),
                ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault(),
                Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault()
            };

            var listaResult = await ConstanciaUltimaVerificacionHelper.Consulta(_proxyWebAPI, request);
			var data = listaResult.Result as ResponseGrid<ConstanciaUltimaVerificacionGridResponse> ?? new ResponseGrid<ConstanciaUltimaVerificacionGridResponse>();

			foreach (var row in data.Data ?? new List<ConstanciaUltimaVerificacionGridResponse>())
			{
				row.Acciones = await this.RenderViewToStringAsync("~/Views/Tramite/ConstanciaUltimaVerificacion/_Acciones.cshtml", row);
			}
			data.Draw = draw;

			return Json(data);
		}

		public async Task<JsonResult> Autocomplete()
		{
			JsonResult result = new JsonResult("");

			var term = Request.Query["q"].FirstOrDefault();
			int page = Convert.ToInt32(Request.Query["page"].FirstOrDefault() ?? "0");
			int records = Convert.ToInt32(Request.Query["records"].FirstOrDefault() ?? "0");
			bool placa = Convert.ToBoolean(Request.Query["placa"].FirstOrDefault() ?? "false");
			var start = 0;
			var end = start + records;
			var listaResult = await ConstanciaUltimaVerificacionHelper.Autocomplete(_proxyWebAPI, new RefrendoAutocompletRequest { Term = term, Start = start, End = end, Placa = placa });
			var data = listaResult.Result as List<RefrendoAutocompleteResponse> ?? new List<RefrendoAutocompleteResponse>();

			return Json(new { items = data, total_count = data.Count });
		}

        public async Task<IActionResult> Edit()
        {
            

            SetMenuLinkActive();
            return View("~/Views/Tramite/ConstanciaUltimaVerificacion/Edit.cshtml");
        }

        public async Task<IActionResult> Detalle(long id)
		{
			var vm = new ConstanciaUltimaVerificacionDetalleResponse();
			try
			{
                var response = await ConstanciaUltimaVerificacionHelper.Detalle(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                    return BadRequest();
                vm = response.Result as ConstanciaUltimaVerificacionDetalleResponse ?? new ConstanciaUltimaVerificacionDetalleResponse();
            }
			catch(Exception e)
			{

			}

            return View("~/Views/Tramite/ConstanciaUltimaVerificacion/Detalle.cshtml", vm);
        }

        [HttpGet]
        public async Task<JsonResult> EditPartial(long id)
        {
            var result = new JsonResultDto();

            try
            {
                var res = new JsonResultDto();
                var listaResult = await ReposicionHelper.DataVerificacion(_proxyWebAPI, id);
                var folio = listaResult.Result as vDataTramiteVentanillaResponse ?? new vDataTramiteVentanillaResponse();
				ViewBag.DataVerificacion = folio;
				ViewBag.IdFv = folio.Id;
                ViewBag.Estados = EstadosMexicoDict.DictEstados.Select(x => new SelectListItem { Text = x.Nombre, Value = x.Clave, Selected = x.Nombre == "PUEBLA" }).ToList();
                result.Result = await this.RenderViewToStringAsync("~/Views/Tramite/ConstanciaUltimaVerificacion/_EditDatos.cshtml", new ConstanciaUltimaVerificacionGridResponse());
            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = "Ocurrió un error al realizar la operación";
            }

            return Json(result);

        }

        [HttpPost]
		public async Task<ActionResult> Edit(ConstanciaUltimaVerificacionRequest req)
		{
			var result = new ResponseViewModel(true);
			var urls = new List<string>();
			try
			{
				var files = JsonConvert.DeserializeObject<List<RefrendoFiles>>(Request.Form["Files"].FirstOrDefault() ?? "");
				req.Files = files == null ? new List<RefrendoFiles>() : files;
				var save = await ConstanciaUltimaVerificacionHelper.Edit(_proxyWebAPI, req);
				result.IsSuccessFully = save.IsSuccessFully;
				result.Message = result.IsSuccessFully ? "La información se guardó correctamente." : save.Message;
			}
			catch (Exception e)
			{
				result.IsSuccessFully = false;
				result.Message = e.Message;
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
				url = _blobStorage._url + "/ConstanciaUltimaVerificacion/" + url;
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
