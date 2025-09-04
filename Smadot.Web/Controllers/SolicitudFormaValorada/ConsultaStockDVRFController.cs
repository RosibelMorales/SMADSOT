using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.ConsultaSock.Request;
using Smadot.Models.Entities.ConsultaSock.Response;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Web.Models.ConsultaStockDVRFModel;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Web.Utilities;
using Smadot.Web.Models;
using Smadot.Models.Dicts;
using Smadot.Utilities.GestionTokens;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.Reporting.Interfaces;

namespace Smadot.Web.Controllers.SolicitudFormaValorada
{
    [Authorize]
    public class ConsultaStockDVRFController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuilder;
        private readonly IExcelBuilder _excelBuilder;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public ConsultaStockDVRFController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuilder, IExcelBuilder excelBuilder)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuilder = pdfBuilder;
            _excelBuilder = excelBuilder;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(MenuConsultaStockDVRF.PermisoMenuPadre))
                return new UnauthorizedResult();
            var vm = new ConsultaStockDVRFViewModel.Registro();
            var almacenes = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest() { Activo = true });
            //vm.Almacenes = (almacenes.Result as List<AlmacenResponse> ?? new List<AlmacenResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
            ViewBag.Almacenes = (almacenes.Result as List<AlmacenResponse> ?? new List<AlmacenResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();

            return View("~/Views/ConsultaStockDVRF/Index.cshtml", vm);
        }


        [HttpPost]
        public async Task<JsonResult> Consulta(long? IdAlmacen)
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

            //Invoca al método que se encarga de ralizar la peticion Api
            var request = new ConsultaStockDVRFRequest();
			request.Pagina = page;
			request.Registros = pageSize;
			request.Busqueda = searchValue;
			request.ColumnaOrdenamiento = sortColumn;
			request.Ordenamiento = sortColumnDirection;
            request.IdAlmacen = IdAlmacen;


            var listaResult = await SolicitudFormaValoradaHelper.InformeConsultaStockDVRF(_proxyWebAPI, request);
            var data = listaResult.Result as List<ConsultaStockDVRFResponse> ?? new List<ConsultaStockDVRFResponse>();

            //Almacena un total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

			//Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
			filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<ConsultaStockDVRFResponse>();
            foreach (var row in data)
            {
                List.Add(new ConsultaStockDVRFResponse()
                {
                    Almacen=row.Almacen,
                    NombreTipoCertificado = row.NombreTipoCertificado,
                    CantidadStock = row.CantidadStock,
                    NumeroCaja = row.NumeroCaja,
                    FolioInicial = row.FolioInicial,
                    FolioFinal = row.FolioFinal,
                    ClaveCertificado = row.ClaveCertificado,
                    CantidadMinima = row.CantidadMinima,
                    CantidadMedia = row.CantidadMedia,
                    NumeroSolucitud = row.NumeroSolucitud,
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
        public async Task<IActionResult> BuscarRegistro(int? IdAlmacen)
        {
            var result = new JsonResultDto();
            try
            {
                var vm = new ConsultaStockDVRFViewModel.Registro();

                if (IdAlmacen.HasValue && IdAlmacen > 0)
                {
                    var catalogo = await SolicitudFormaValoradaHelper.GetCatalogo(_proxyWebAPI, IdAlmacen.Value);

                    if (catalogo.IsSuccessFully)
                    {
                        var response = catalogo.Result as ConsultaStockDVRFResponse;

                        if (response.IdAlmacen == 0)
                        {

                        }

                        vm.NombreTipoCertificado = response.NombreTipoCertificado;
                        vm.CantidadStock = response.CantidadStock;
                        vm.NumeroCaja = response.NumeroCaja;
                        vm.FolioInicial = response.FolioInicial;
                        vm.FolioFinal = response.FolioFinal;
                        vm.ClaveCertificado = response.ClaveCertificado;

                    }
                }
                result.Result = await this.RenderViewToStringAsync("~/Views/ConsultaStockDVRF/Index.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> CrearReporte(string search, int almacen, string sortColumnDirection, string sortColumn, bool esReportePdf)
        {
            JsonResultDto result = new JsonResultDto();

            try
            {
                var request = new ConsultaStockDVRFRequest
                {
                    Busqueda = search,
                    ColumnaOrdenamiento = sortColumn,
                    Ordenamiento = sortColumnDirection,
                    Pagination = true,
                    //IdAlmacen = almacen,
                };

                var listaResult = await SolicitudFormaValoradaHelper.InformeConsultaStockDVRF(_proxyWebAPI, request);
                var dataResult = listaResult.Result as List<ConsultaStockDVRFResponse> ?? new List<ConsultaStockDVRFResponse>();

                var data = dataResult.Select(x => new ConsultaStockDVRFDocument()
                {
                    TipoCertificado = x.NombreTipoCertificado,
                    Cantidad = x.CantidadStock,
                    Caja = x.NumeroCaja,
                    FolioInicial = x.FolioInicial,
                    FolioFinal = x.FolioFinal,
                    ClaveCertificado = x.ClaveCertificado,
                }).ToList();

                byte[] bytes;
                if (esReportePdf)
                {
                    var reportePDF = await _pdfBuilder.GetDocumentoConsultaStockDVRF(data);
                    bytes = reportePDF.Response.DocumentoPDF;
                    return File(bytes, "application/pdf", reportePDF.Response.NombreDocumento);
                }
                else
                {
                    var reporteExcel = await _excelBuilder.GetDocumentoConsultaStockDVRF(data);
                    bytes = reporteExcel.Response.DocumentoExcel;
                    HttpContext.Session.SetString("FileName", reporteExcel.Response.NombreDocumento);
                    HttpContext.Session.Set("FileXlsx", bytes);
                }
                result.Result = new
                {
                    FileName = "Consulta Stock DVRF",
                    Bytes = bytes
                };
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorDescription = $"Error al generar el reporte | {ex.Message} --- {ex.StackTrace}";
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<FileResult> DescargarReporte()
        {
            var fileName = HttpContext.Session.GetString("FileName");
            var bytes = HttpContext.Session.Get("FileXlsx");

            HttpContext.Session.Remove("FileName");
            HttpContext.Session.Remove("FileXlsx");

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<FileResult> DescargarReportePDF()
        {
            var fileName = HttpContext.Session.GetString("FileName");
            var bytes = HttpContext.Session.Get("FileXlsx");

            HttpContext.Session.Remove("FileName");
            HttpContext.Session.Remove("FileXlsx");

            return File(bytes, "application/pdf", fileName);
        }
    }   
}
