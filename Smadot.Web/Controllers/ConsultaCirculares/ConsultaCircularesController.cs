using Microsoft.AspNetCore.Mvc;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Utilities.Seguridad.Modelo;
using Smadot.Utilities.Seguridad;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using static Smadot.Models.Entities.ConsultaCircular.Request.ConsultaCircularRequestData;
using static Smadot.Models.Entities.ConsultaCircular.Response.ConsultaCircularResponseData;
using System.Runtime.InteropServices;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Models.Dicts;

namespace Smadot.Web.Controllers.ConsultaCirculares
{
    [Authorize]
    public class ConsultaCircularesController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
		private readonly IUserResolver _userResolver;
        private readonly IConfiguration _configuration;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public ConsultaCircularesController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(PermisosMenu.ConsultadeCirculares))
                return new UnauthorizedResult();
            return View("~/Views/ConsultaCirculares/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");
			var request = new RequestList();

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
			request.Pagina = skip / pageSize + 1;
			request.Registros = pageSize;
			request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
			request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
			request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

			var listaResult = await ConsultaCircularHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<ConsultaCircularResponse> ?? new List<ConsultaCircularResponse>();

			totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

			filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<ConsultaCircularResponseGrid>();

            foreach (var row in data)
            {
                List.Add(new ConsultaCircularResponseGrid
                {
                    Id = row.Id,
                    Fecha= row.Fecha,
					Leidos = row.Leidos,
                    NumeroCircular = row.NumeroCircular,
                    NoLeidos = row.NoLeidos,
                    Acciones = await this.RenderViewToStringAsync("~/Views/ConsultaCirculares/_Acciones.cshtml", row)
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

		[HttpPost]
		public async Task<IActionResult> CreateBodyCircular(ConsultaCircularRequest registro)
		{
			var result = new JsonResultDto();
			try
			{
				var usuarioActual = _userResolver.GetUser();
                if (usuarioActual == null)
                {
                    result.Error = true;
                    result.ErrorDescription = _mensajeErrorCatch;
				}
                registro.IdUserRegistro = usuarioActual.IdUser;


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
                registro.Logo = imageLogo;
				//var currentD = Directory.GetCurrentDirectory();
				//var DirectorP = Directory.GetParent(currentD);
				
				//if (DirectorP != null)
				//{
				//	var directories = Directory.GetDirectories(DirectorP.FullName);
				//	var directW = directories.FirstOrDefault(x => x.ToLower().Contains("web"));
				//	path = Path.Combine(directW, "wwwroot", "assets", "media", "logos", "smadot_logo_simple2.png");
				//	if (File.Exists(path))
				//	{
				//		byte[] imageArray = System.IO.File.ReadAllBytes(path);
				//		imageLogo = Convert.ToBase64String(imageArray);
				//	}
				//}

				var helper = await ConsultaCircularHelper.EnvioEmail(_proxyWebAPI, registro);
				//var helper = await SeguimientoHelper.Registro(_proxyWebAPI, registro);
				if (!helper.IsSuccessFully)
				{
					result.Error = true;
					result.ErrorDescription = "Ocurrió un error al enviar la circular";
					return Json(result);
				}
				result.Error = false;
				result.ErrorDescription = "Se envió la circular correctamente";

			}
			catch (Exception ex)
			{
				result.Error = true;
				result.ErrorDescription = _mensajeErrorCatch;
			}
			return Json(result);
		}

		[HttpGet]
		public async Task<ActionResult> Registro(int? id)
		{
			var result = new JsonResultDto();

			try
			{
				result.Result = await this.RenderViewToStringAsync("~/Views/ConsultaCirculares/_Registro.cshtml");
			}
			catch (Exception e)
			{
				result.Error = true;
				result.ErrorDescription = _mensajeErrorCatch;
			}

			return Json(result);
		}

        [HttpGet]
        public async Task<ActionResult> Detalle(long id)
        {
            var result = new JsonResultDto();

            try
            {
                var vm = new ConsultaCircularResponse();

                var helper = await ConsultaCircularHelper.GetById(_proxyWebAPI, id);
                vm = helper.Result as ConsultaCircularResponse ?? new ConsultaCircularResponse();

                result.Result = await this.RenderViewToStringAsync("~/Views/ConsultaCirculares/_Detalle.cshtml",vm);
            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }

            return Json(result);
        }

        public async Task<ActionResult> Confirmar(string idV, string idC)
        {
            try
            {
                var cifV = SeguridadBase64.DesEncriptar(idV);
                var cifC = SeguridadBase64.DesEncriptar(idC);
                long verif = 0;
                long circ = 0;

                var tryV = long.TryParse(cifV, out verif);
                var tryC = long.TryParse(cifC, out circ);
                //var tryV = long.TryParse(idV, out verif);
                //var tryC = long.TryParse(idC, out circ);

                var request = new ConfirmarCircularRequest();
                request.IdV = tryV ? verif : 0;
                request.IdC = tryC ? circ : 0;

                var helper = await ConsultaCircularHelper.ConfirmarCircular(_proxyWebAPI, request);

                
                    
                return RedirectToAction("Index", "Autenticacion");
                
                

            }
            catch (Exception e)
            {

            }
            return null;
        }

    }

   
}