using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Request;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using Smadot.Utilities.GestionTokens;
using Smadot.Models.Dicts;
using Smadot.Web.Handler.Autorizacion;

namespace Smadot.Web.Controllers.EstatusDeUso
{
    [Authorize]
    public class FoliosUsadosEnVentanillaController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IPdfBuider _pdfBuilder;
        private readonly IExcelBuilder _excelBuilder;
        private readonly IUserResolver _userResolver;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public FoliosUsadosEnVentanillaController(IProxyWebAPI proxyWebAPI, IPdfBuider pdfBuilder, IExcelBuilder excelBuilder, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _pdfBuilder = pdfBuilder;
            _excelBuilder = excelBuilder;
            _userResolver = userResolver;
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(PermisosMenu.FoliosUsadosenVentanilla))
                return new UnauthorizedResult();
            return View("~/Views/EstatusDeUso/FoliosUsadosEnVentanilla/Index.cshtml");
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
            var request = new FoliosUsadosEnVentanillaRequest();
            request.Pagina = page;
            request.Registros = pageSize;
            request.Busqueda = searchValue;
            request.ColumnaOrdenamiento = sortColumn;
            request.Ordenamiento = sortColumnDirection;

            var listaResult = await SolicitudFormaValoradaHelper.ConsultaFoliosUsadosVentanilla(_proxyWebAPI, request);
            var data = listaResult.Result as List<FoliosUsadosEnVentanillaResponse> ?? new List<FoliosUsadosEnVentanillaResponse>();

            //Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            //Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<FoliosUsadosEnVentanillaResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new FoliosUsadosEnVentanillaResponseGrid()
                {
                    Fecha = row.Fecha,
                    FolioTramite = row.FolioTramite,
                    ClaveTramite = row.ClaveTramite,
                    CorreoUsuario = row.CorreoUsuario,
                    NombreUsuario = row.NombreUsuario,
                    DatosVehiculo = row.DatosVehiculo,
                    FolioCertificado = row.FolioCertificado,
                    Razon = row.Razon,
                    PersonaGeneroTramite = row.PersonaGeneroTramite,
                    MontoTramite = row.MontoTramite,
                    ReferenciaBancaria = row.ReferenciaBancaria,
                    Acciones = await this.RenderViewToStringAsync("~/Views/EstatusDeUso/FoliosUsadosEnVentanilla/_Acciones.cshtml", row)
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
        public async Task<JsonResult> DetalleFolioVentanilla(long id)
        {
            var result = new ResponseViewModel(true);
            var vm = new FoliosUsadosEnVentanillaResponse();
            try
            {
                var response = await SolicitudFormaValoradaHelper.DetalleFoliosVentanilla(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                vm = response.Result as FoliosUsadosEnVentanillaResponse ?? vm;
                result.Result = await this.RenderViewToStringAsync("~/Views/EstatusDeUso/FoliosUsadosEnVentanilla/_ModalDetalle.cshtml", vm);
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

            try
            {
                var request = new FoliosUsadosEnVentanillaRequest
                {
                    Busqueda = search,
                    ColumnaOrdenamiento = sortColumn,
                    Ordenamiento = sortColumnDirection,
                    Pagination = true
                };

                var listaResult = await SolicitudFormaValoradaHelper.ConsultaFoliosUsadosVentanilla(_proxyWebAPI, request);
                var dataResult = listaResult.Result as List<FoliosUsadosEnVentanillaResponse> ?? new List<FoliosUsadosEnVentanillaResponse>();

                var data = dataResult.Select(x => new FoliosUsadosEnVentanillaDocument()
                {
                    TipoTramite = x.TipoTramite,
                    FolioTramite = x.FolioTramite,
                    DatosVehiculo = x.DatosVehiculo,
                    FolioCertificado = x.FolioCertificado,
                    Razon = x.Razon,
                    ReferenciaBancaria = x.ReferenciaBancaria,
                    Fecha = x.Fecha
                }).ToList();

                byte[] bytes;
                if (esReportePdf)
                {
                    var reportePDF = await _pdfBuilder.GetDocumentoFoliosUsadosEnVentanilla(data);
                    bytes = reportePDF.Response.DocumentoPDF;
                    return File(bytes, "application/pdf", reportePDF.Response.NombreDocumento);
                }
                else
                {
                    var reporteExcel = await _excelBuilder.GetDocumentoFoliosUsadosEnVentanilla(data);
                    bytes = reporteExcel.Response.DocumentoExcel;
                    HttpContext.Session.SetString("FileName", reporteExcel.Response.NombreDocumento);
                    HttpContext.Session.Set("FileXlsx", bytes);
                }
                result.Result = new
                {
                    FileName = "Folios usados en ventanilla",
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

        #endregion Reportes
    }
}