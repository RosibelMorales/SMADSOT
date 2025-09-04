using Microsoft.AspNetCore.Mvc;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Entities.FoliosRegresadosSPF.Request;
using Smadot.Models.Entities.FoliosRegresadosSPF.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Utilities.GestionTokens;
using Smadot.Models.Dicts;

namespace Smadot.Web.Controllers.EstatusDeUso
{
    [Authorize]
    public class FoliosRegresadosSPFController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IPdfBuider _pdfBuilder;
        private readonly IExcelBuilder _excelBuilder;
        private readonly IUserResolver _userResolver;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public FoliosRegresadosSPFController(IProxyWebAPI proxyWebAPI, IPdfBuider pdfBuider, IExcelBuilder excelBuilder, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _pdfBuilder = pdfBuider;
            _excelBuilder = excelBuilder;
            _userResolver = userResolver;
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(PermisosMenu.FoliosRegresadosaSPF))
                return new UnauthorizedResult();
            return View("~/Views/EstatusDeUso/FoliosRegresadosSPF/Index.cshtml");
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

            var request = new FoliosRegresadosSPFRequest();
            request.Pagina = page;
            request.Registros = pageSize;
            request.Busqueda = searchValue;
            request.ColumnaOrdenamiento = sortColumn;
            request.Ordenamiento = sortColumnDirection;

            var listaResult = await SolicitudFormaValoradaHelper.ConsultaFoliosRegresadosSPF(_proxyWebAPI, request);
            var data = listaResult.Result as List<FoliosRegresadosSPFResponse> ?? new List<FoliosRegresadosSPFResponse>();

            //Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            //Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<FoliosRegresadosSPFResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new FoliosRegresadosSPFResponseGrid()
                {
                    ClaveDevolucion = row.ClaveDevolucion,
                    ClaveSolicitud = row.ClaveSolicitud,
                    Fecha = row.Fecha,
                    TipoCertificado = row.TipoCertificado,
                    ClaveCertificado = row.ClaveCertificado,
                    FolioInicial = row.FolioInicial,
                    FolioFinal = row.FolioFinal,
                    ResponsableEntrega = row.ResponsableEntrega,
                    PersonaValido = row.PersonaValido,
                    Acciones = await this.RenderViewToStringAsync("~/Views/EstatusDeUso/FoliosRegresadosSPF/_Acciones.cshtml", row)
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
        public async Task<JsonResult> DetalleFolioRegresado(long id)
        {
            var result = new ResponseViewModel(true);
            var vm = new FoliosRegresadosSPFResponse();
            try
            {
                var response = await SolicitudFormaValoradaHelper.DetalleFolioRegresadoSPF(_proxyWebAPI, id);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                vm = response.Result as FoliosRegresadosSPFResponse ?? vm;
                result.Result = await this.RenderViewToStringAsync("~/Views/EstatusDeUso/FoliosRegresadosSPF/_ModalDetalle.cshtml", vm);
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
                var request = new FoliosRegresadosSPFRequest
                {
                    Busqueda = search,
                    ColumnaOrdenamiento = sortColumn,
                    Ordenamiento = sortColumnDirection,
                    Pagination = true
                };

                var listaResult = await SolicitudFormaValoradaHelper.ConsultaFoliosRegresadosSPF(_proxyWebAPI, request);
                var dataResult = listaResult.Result as List<FoliosRegresadosSPFResponse> ?? new List<FoliosRegresadosSPFResponse>();

                var data = dataResult.Select(x => new FoliosRegresadosSPFDocument()
                {
                    ClaveCertificado = x.ClaveCertificado,
                    FolioFinal = x.FolioFinal,
                    FolioInicial = x.FolioInicial,
                    PersonaValido = x.PersonaValido,
                    ResponsableEntrega = x.ResponsableEntrega,
                    TipoCertificado = x.TipoCertificado,
                    Fecha = x.Fecha
                }).ToList();

                byte[] bytes;
                if (esReportePdf)
                {
                    var reportePDF = await _pdfBuilder.GetDocumentoFoliosRegresadosSPF(data);
                    bytes = reportePDF.Response.DocumentoPDF;
                    return File(bytes, "application/pdf", reportePDF.Response.NombreDocumento);
                }
                else
                {
                    var reporteExcel = await _excelBuilder.GetDocumentoFoliosRegresadosSPF(data);
                    bytes = reporteExcel.Response.DocumentoExcel;
                    HttpContext.Session.SetString("FileName", reporteExcel.Response.NombreDocumento);
                    HttpContext.Session.Set("FileXlsx", bytes);
                }
                result.Result = new
                {
                    FileName = "Folios regresados a SPF",
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