using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Record;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Capacitacion.Request;
using Smadot.Models.Entities.Capacitacion.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Motivos.Response;
using Smadot.Models.Entities.ReporteMensual.Request;
using Smadot.Models.Entities.ReporteMensual.Response;
using Smadot.Models.Entities.Reportes.Response;
using Smadot.Models.Entities.SeguimientoCVV.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.ReporteMensual;
using Smadot.Web.Helper.Operaciones.SeguimientoCVV;
using Smadot.Web.Helper.Operaciones.Tramite;
using Smadot.Web.Utilities;
using System.Linq.Dynamic.Core;
using static iTextSharp.text.pdf.AcroFields;

namespace Smadot.Web.Controllers.Tramite
{
	[Authorize]
    public class CapacitacionController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
		private readonly BlobStorage _blobStorage;

		//Mensaje genérico de error
		public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public CapacitacionController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
			_blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
		}

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuCapacitaciones.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/Capacitacion/Index.cshtml");
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
			var request = new CapacitacionListRequest();
			request.Pagina = skip / pageSize + 1;
			request.Registros = pageSize;
			request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
			request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
			request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();
			//long i;
			//if (long.TryParse(Request.Form["idAlmacen"].FirstOrDefault(), out i)) request.IdAlmacen = i;
			//else request.IdAlmacen = null;

			var listaResult = await CapacitacionHelper.Consulta(_proxyWebAPI, request);
			var data = listaResult.Result as List<CapacitacionResponse> ?? new List<CapacitacionResponse>();

			////Almacena el total de registros
			totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total.Value;

			////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
			filterRecord = string.IsNullOrEmpty(request.Busqueda) ? totalRecord : data.Count();

			var List = new List<CapacitacionResponseGrid>();
			foreach (var row in data)
			{
				List.Add(new CapacitacionResponseGrid
				{
					IdCapacitacion = row.IdCapacitacion,
					FechaCapacitacion = row.FechaCapacitacion,
					TemaCapacitacion = row.TemaCapacitacion,
					IdCatEstatusCapacitacion = row.IdCatEstatusCapacitacion,
					NombreCatEstatusCapacitacion = row.NombreCatEstatusCapacitacion,
					ActivoCatEstatusCapacitacion = row.ActivoCatEstatusCapacitacion,
					TotalAsistentes = row.TotalAsistentes,
					Acciones = await this.RenderViewToStringAsync("~/Views/Capacitacion/_Acciones.cshtml", row)
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
		public async Task<IActionResult> Edit(long? id)
        {
            if (!id.HasValue && !_userResolver.HasPermission(MenuCapacitaciones.Registrar))
                return new UnauthorizedResult();
			else if(id.HasValue && !_userResolver.HasPermission(MenuCapacitaciones.Detalle))
                return new UnauthorizedResult();
            var result = new ResponseViewModel(true);
			var vm = new List<CapacitacionResponse>
			{
				new CapacitacionResponse
				{
					IdCapacitacion = 0
				}
			};
			try
			{
				var response = await CapacitacionHelper.Consulta(_proxyWebAPI, id ?? 0);
				vm = response.Result as List<CapacitacionResponse> ?? vm;

                foreach (var item in vm)
                {
                    item.UrlEvaluacionBase64 = "";
                    item.UrlFotografiaBase64 = "";
                    if (!string.IsNullOrEmpty(item.UrlEvaluacion))
                    {
                        var arrDoc = item.UrlEvaluacion.Split("/");
                        var NameDoc1 = arrDoc[arrDoc.Length - 1];
                        var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                        var archivo = await _blobStorage.DownloadFileAsync(item.UrlEvaluacion, true);
                        if (archivo.IsSuccessFully)
                        {
                            var imageUrlDocumento = string.Format("{0}{1}", ContentType1, archivo.Result);
                            item.UrlEvaluacionBase64 = imageUrlDocumento;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.UrlFotografia))
                    {
                        var arrDoc = item.UrlFotografia.Split("/");
                        var NameDoc1 = arrDoc[arrDoc.Length - 1];
                        var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                        var archivo = await _blobStorage.DownloadFileAsync(item.UrlFotografia, true);
                        if (archivo.IsSuccessFully)
                        {
                            var imageUrlDocumento = string.Format("{0}{1}", ContentType1, archivo.Result);
                            item.UrlFotografiaBase64 = imageUrlDocumento;
                        }
                    }
                }
                //result.Result = await this.RenderViewToStringAsync("~/Views/Capacitacion/_Registro.cshtml", vm);
            }
			catch (Exception ex)
			{
				result.IsSuccessFully = false;
				result.Message = ex.Message;
			}

            //return new JsonResult(result);
            return View(vm);
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
				url = _blobStorage._url + "/CapacitacionEmpleadoEvaluacion/" + url;
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

        [HttpGet]
        public async Task<IActionResult> DescargarFotografia(string url)
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
                url = _blobStorage._url + "/CapacitacionEmpleadoFotografia/" + url;
                var archivo = await _blobStorage.DownloadFileAsync(url, true);
                if (!archivo.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    throw new Exception("No sé encontró la fotografía.");
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
                    result.Message = "Error al descargar la fotografía.";
                else
                    result.Message = ex.Message;
                result.IsSuccessFully = false;
            }
            return new JsonResult(result);
        }

        [HttpGet]
		public async Task<IActionResult> Update(long? id)
		{
			var result = new ResponseViewModel(true);
			var vm = new List<CapacitacionResponse>
			{
				new CapacitacionResponse
				{
					Id = 0
				}
			};
			try
			{
				var response = await CapacitacionHelper.GetByIdEmpleado(_proxyWebAPI, id ?? 0);
				vm = response.Result as List<CapacitacionResponse> ?? vm;
				result.Result = await this.RenderViewToStringAsync("~/Views/Capacitacion/_Update.cshtml", vm);
			}
			catch (Exception ex)
			{
				result.IsSuccessFully = false;
				result.Message = ex.Message;
			}

			return new JsonResult(result);
		}

		[HttpGet]
		public async Task<IActionResult> UploadPhoto(long? id)
		{
			var result = new ResponseViewModel(true);
			var vm = new List<CapacitacionResponse>
			{
				new CapacitacionResponse
				{
					Id = 0
				}
			};
			try
			{
				var response = await CapacitacionHelper.GetByIdEmpleado(_proxyWebAPI, id ?? 0);
				vm = response.Result as List<CapacitacionResponse> ?? vm;
                result.Result = await this.RenderViewToStringAsync("~/Views/Capacitacion/_UploadPhoto.cshtml", vm);
                
			}
			catch (Exception ex)
			{
				result.IsSuccessFully = false;
				result.Message = ex.Message;
			}

			return new JsonResult(result);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(List<CapacitacionResponse> registro)
		{
			var result = new ResponseViewModel(true);
			try
			{

				if (registro[0].IdCapacitacion > 0 && registro[0].AcceptOrDenied == 2)
				{
					var save = await CapacitacionHelper.Edit(_proxyWebAPI, registro);
					if (!save.IsSuccessFully)
						throw new Exception(save.Message);

					result.Message = "Se autorizo con exito la Capacitación";
				}
				else if (registro[0].IdCapacitacion > 0 && registro[0].AcceptOrDenied == 3)
				{
					var save = await CapacitacionHelper.Edit(_proxyWebAPI, registro);
					if (!save.IsSuccessFully)
						throw new Exception(save.Message);

					result.Message = "Se denego con exito la Capacitación";
				}
				//Actualizar archivo
				else if (registro[0].Id > 0 && registro[0].IdEmpleado > 0)
				{
					int i = 1;

					foreach (var item in registro)
					{
						
						item.Files = JsonConvert.DeserializeObject<List<CapacitacionFiles>>(Request.Form["Files"].FirstOrDefault() ?? "");
						if (item.Files == null)
						{
							result.Message = "No se encontro ningun archivo cargado, intenta de nuevo";
							result.IsSuccessFully = false;
							return new JsonResult(result);
						}
						i++;
					}
					var save = await CapacitacionHelper.Edit(_proxyWebAPI, registro);
					if (!save.IsSuccessFully)
						throw new Exception(save.Message);

					result.Message = "La información se actualizo correctamente.";
				}
				//Registro Capacitacion
				else
				{
					if (registro[0].TemaCapacitacion == null || registro[0].TemaCapacitacion == string.Empty)
					{
						result.Message = "Es necesario agregar un Tema de Capacitación ";
						result.IsSuccessFully = false;
						return new JsonResult(result);
					}
					else if (registro[0].IdCapEmp == 0 || registro[0].IdCapEmp == null)
					{
						result.Message = "Es necesario agregar al menos un Empleado";
						result.IsSuccessFully = false;
						return new JsonResult(result);
					}
					else
					{
						foreach (var item in registro)
						{
							item.FechaCapacitacion = registro[0].FechaCapacitacion;
							item.IdUserPuestoVerificentro = _userResolver.GetUser().IdUser;
							item.IdVerificentro = (int)_userResolver.GetUser().IdVerificentro;
						}

						var save = await CapacitacionHelper.Edit(_proxyWebAPI, registro);
						if (!save.IsSuccessFully)
							throw new Exception(save.Message);

						result.Message = "La información se actualizo correctamente.";
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

		[HttpPost]
		public async Task<IActionResult> UpdateAsistencia(long id)
		{
			var result = new ResponseViewModel(true);
			try
			{
				var List = new List<CapacitacionResponse>();


					List.Add(new CapacitacionResponse
					{
						Id = (int?)id					
					});


				var save = await CapacitacionHelper.Edit(_proxyWebAPI, List);
				if (!save.IsSuccessFully)
					throw new Exception(save.Message);

				result.Message = "La información se actualizo correctamente.";
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
