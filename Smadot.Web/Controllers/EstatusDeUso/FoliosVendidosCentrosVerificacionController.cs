using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.FoliosVendidosCentrosVerificacion.Request;
using Smadot.Models.Entities.FoliosVendidosCentrosVerificacion.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Models.Dicts;
using Smadot.Utilities.GestionTokens;

namespace Smadot.Web.Controllers.EstatusDeUso
{
    [Authorize]
    public class FoliosVendidosCentrosVerificacionController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IPdfBuider _pdfBuilder;
        private readonly IExcelBuilder _excelBuilder;
        private readonly IUserResolver _userResolver;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public FoliosVendidosCentrosVerificacionController(IProxyWebAPI proxyWebAPI, IPdfBuider pdfBuilder, IExcelBuilder excelBuilder, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _pdfBuilder = pdfBuilder;
            _excelBuilder = excelBuilder;
            _userResolver = userResolver;
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(PermisosMenu.FoliosVendidosaCentrosdeVerificacion))
                return new UnauthorizedResult();
            return View("~/Views/EstatusDeUso/FoliosVendidosCentrosVerificacion/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Consulta()
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

            //Invoca al método que se encarga de realizar la petición Api
            var request = new FoliosVendidosCentrosVerificacionRequest();
            request.Pagina = page;
            request.Registros = pageSize;
            request.Busqueda = searchValue;
            request.ColumnaOrdenamiento = sortColumn;
            request.Ordenamiento = sortColumnDirection;

            var listaResult = await SolicitudFormaValoradaHelper.ConsultaFoliosVendidosCentrosVerificacion(_proxyWebAPI, request);
            var data = listaResult.Result as List<FoliosVendidosCentrosVerificacionResponse> ?? new List<FoliosVendidosCentrosVerificacionResponse>();

            //Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().TotalRegistros;

            //Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<FoliosVendidosCentrosVerificacionsResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new FoliosVendidosCentrosVerificacionsResponseGrid()
                {
                    Fecha = row.Fecha,
                    CVV = row.CVV,
                    FolioVenta = row.FolioVenta,
                    FolioFV = row.FolioFV,
                    PersonaRecibeCertificado = row.PersonaRecibeCertificado,
                    ClaveVenta = row.ClaveVenta,
                    Total = row.Total,
                    FoliosStock = row.FoliosStock,
                    MontoCadaVenta = row.MontoCadaVenta,
                    ReferenciaBancaria = row.ReferenciaBancaria,
                    Acciones = await this.RenderViewToStringAsync("~/Views/EstatusDeUso/FoliosVendidosCentrosVerificacion/_Acciones.cshtml", row)
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
        public async Task<JsonResult> DetalleFolioVendido(long id)
        {
            var result = new ResponseViewModel(true);
            var vm = new FoliosVendidosCentrosVerificacionResponse();
            try
            {
                var response = await SolicitudFormaValoradaHelper.DetalleFoliosVendidosCentrosVerificacion(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                vm = response.Result as FoliosVendidosCentrosVerificacionResponse ?? vm;
                result.Result = await this.RenderViewToStringAsync("~/Views/EstatusDeUso/FoliosVendidosCentrosVerificacion/_ModalDetalle.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        #region Reportes

        [HttpGet]
        public async Task<IActionResult> CrearReporte(string search, string sortColumnDirection, string sortColumn, bool esReportePdf)
        {
            JsonResultDto result = new JsonResultDto();
            ResponseGeneric<DataExcelReport> reporteExcel = new();
            try
            {
                var request = new FoliosVendidosCentrosVerificacionRequest
                {
                    Busqueda = search,
                    ColumnaOrdenamiento = sortColumn,
                    Ordenamiento = sortColumnDirection,
                    Pagination = true
                };

                var listaResult = await SolicitudFormaValoradaHelper.ConsultaFoliosVendidosCentrosVerificacion(_proxyWebAPI, request);
                var dataResult = listaResult.Result as List<FoliosVendidosCentrosVerificacionResponse> ?? new List<FoliosVendidosCentrosVerificacionResponse>();

                var data = dataResult.Select(x => new FoliosVendidosCentroVerificacionDocument()
                {
                    FolioVenta = x.FolioVenta,
                    ClaveVenta = x.ClaveVenta,
                    FolioFV = x.FolioFV,
                    FoliosStock = x.FolioFV,
                    ReferenciaBancaria = x.ReferenciaBancaria,
                    MontoCadaVenta = x.MontoCadaVenta,
                    Fecha = x.Fecha
                }).ToList();

                byte[] bytes;
                if (esReportePdf)
                {
                    var reportePDF = await _pdfBuilder.GetDocumentoFoliosVendidosCentroVerificacion(data);
                    bytes = reportePDF.Response.DocumentoPDF;
                    return File(bytes, "application/pdf", reportePDF.Response.NombreDocumento);
                }
                else
                {
                    reporteExcel = await _excelBuilder.GetDocumentoFoliosVendidosCentroVerificacion(data);
                    bytes = reporteExcel.Response.DocumentoExcel;
                    HttpContext.Session.SetString("FileName", reporteExcel.Response.NombreDocumento);
                    HttpContext.Session.Set("FileXlsx", bytes);
                }
                result.Result = new
                {
                    FileName = "Folios vendidos en centros de verificación",
                    Bytes = bytes
                };
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorDescription = $"Error al generar el reporte | {ex.Message} --- {ex.StackTrace} --- { JsonConvert.SerializeObject( new { reporteExcel, result } ) }";
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

        #endregion Reportes
    }
}