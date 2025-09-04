using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.VentaCVV.Request;
using Smadot.Models.Entities.VentaCVV.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Helper.Operaciones.Venta;
using Smadot.Web.Utilities;
using Smadot.Models.Dicts;
using Smadot.Utilities.GestionTokens;
using Smadot.Models.Entities.DevolucionSPF.Response;
using Smadot.Utilities.Reporting.Implementacion;
using Smadot.Web.Helper.Operaciones.AdministracionStock;
using Smadot.Web.Models;
using System.Reflection.PortableExecutable;
using Smadot.Models.Entities.Personal.Extensions;
using static Smadot.Models.Entities.Generic.Response.TablaFijaViewModel;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Request;
using Smadot.Web.Helper.Operaciones.DirectorioCentrosVerificacion;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Response;
using Smadot.Models.Entities.Refrendo.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.IngresoFormaValorada.Response;
using Smadot.Web.Helper.Operaciones.IngresoFormaValorada;
using System.ComponentModel.DataAnnotations;

namespace Smadot.Web.Controllers.Venta
{
    [Authorize]
    public class VentaCVVController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IPdfBuider _pdfBuider;
        private readonly IUserResolver _userResolver;
        private readonly IExcelBuilder _excelBuilder;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public VentaCVVController(IProxyWebAPI proxyWebAPI, IPdfBuider pdfBuider, IExcelBuilder excelBuilder, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _pdfBuider = pdfBuider;
            _excelBuilder = excelBuilder;
            _userResolver = userResolver;
        }
        public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(PermisosMenu.VentadeFormasValoradasaCVV))
                return new UnauthorizedResult();
            var almacenes = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest() { Activo = true, VerificentrosNulos = true });
            ViewBag.Almacenes = (almacenes.Result as List<AlmacenResponse> ?? new List<AlmacenResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
            return View("~/Views/Venta/VentaCVV/Index.cshtml");
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
            if (long.TryParse(Request.Form["idAlmacen"].FirstOrDefault(), out long i)) request.IdAlmacen = i;
            // else request.IdAlmacen = null;
            var listaResult = await VentaCVVHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as ResponseGrid<SeguimientoSolicitudResponse> ?? new ResponseGrid<SeguimientoSolicitudResponse>();

            foreach (var row in data.Data ?? new List<SeguimientoSolicitudResponse>())
            {
                row.Acciones = await this.RenderViewToStringAsync("~/Views/Venta/VentaCVV/_Acciones.cshtml", row);
            }
            data.Draw = draw;

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> Registrar(long id, long idSolicitudFV)
        {
            try
            {
                if (!_userResolver.HasPermission(MenuVentaFormasValoradasCVV.Registrar))
                    return new UnauthorizedResult();
                var result = new ResponseViewModel(true);
                var response = await IngresoFormaValoradaHelper.Consulta(_proxyWebAPI, idSolicitudFV);
                var listSolicitud = response.Result as List<SolicitudFormaValoradaResponse> ?? new List<SolicitudFormaValoradaResponse>();
                listSolicitud = listSolicitud.Where(x => x.IdCatTipoCertificadoSC != 0).ToList();
                var certificadosResponse = await IngresoFormaValoradaHelper.GetIngresoCertificadoById(_proxyWebAPI, id);
                var certificadosIngreso = certificadosResponse.Result as List<IngresoCertificado> ?? new List<IngresoCertificado>();
                var vm = new IngresoFormaValoradaFormResponse()
                {
                    Almacen = listSolicitud[0].AlmacenFV,
                    IdSolicitudFV = listSolicitud[0].IdSolicitudFV,
                    IdAlmacen = listSolicitud[0].IdAlmacenFV,
                    FechaEntrega = listSolicitud[0].FechaEntregaIFV ?? DateTime.Now,
                    FechaSolicitud = listSolicitud[0].FechaSolicitudFV,
                    NombreRecibio = listSolicitud[0].NombreRecibioIFV ?? string.Empty,
                    UrlDoc1 = listSolicitud[0].UrlDoc1 ?? string.Empty,
                    UrlDoc2 = listSolicitud[0].UrlDoc2 ?? string.Empty,
                    UrlDoc3 = listSolicitud[0].UrlDoc3 ?? string.Empty,
                    IdIngresoFV = id,
                    certificados = listSolicitud.Select(x => new Smadot.Models.Entities.IngresoFormaValorada.Response.CertificadoGrupo
                    {
                        IdCatTipoCertificado = x.IdCatTipoCertificadoSC,
                        Certificados = certificadosIngreso.Where(c => c.IdCatTipoCertificado == x.IdCatTipoCertificadoSC).Select(x => new Smadot.Models.Entities.IngresoFormaValorada.Response.CertificadoData
                        {
                            IdIngresoCertificado = x.Id,
                            CantidadRecibida = x.CantidadRecibida,
                            IdCatTipoCertificado = x.IdCatTipoCertificado,
                            FolioInicial = x.FolioInicial,
                            FolioFinal = x.FolioFinal
                        }).ToList(),
                    }).OrderBy(x => x.Clave).ToList(),
                    certificadosTotalData = listSolicitud.Select(x => new Smadot.Models.Entities.IngresoFormaValorada.Response.CertificadosTotalData
                    {
                        CantidadRecibida = certificadosIngreso.Where(ci => ci.IdCatTipoCertificado == x.IdCatTipoCertificadoSC).Sum(ci => ci.CantidadRecibida),
                        CantidadSolicitada = x.CantidadSC,
                        IdCatTipoCertificado = x.IdCatTipoCertificadoSC
                    }).OrderBy(x => x.Clave).ToList(),
                };
                return View("~/Views/Venta/VentaCVV/Registro.cshtml", vm);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "VentaCVV");
            }
        }

        [HttpGet]
        public async Task<JsonResult> ConsultarAlmacenes(long Id)
        {
            var almacenes = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest() { Activo = true, VerificentrosNulos = false, IdVerificentroDestino = Id });

            var alm = (almacenes.Result as List<AlmacenResponse> ?? new List<AlmacenResponse>()).Where(x => x.IdVerificentro == Id).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();

            return Json(alm);
        }

        // [HttpGet]
        // public async Task<JsonResult> ConsultaVerificentrosAutocomplete(string prefix)
        // {
        //     var listaResult = await VentaCVVHelper.AutocompleteFolio(_proxyWebAPI, prefix);
        //     var verificentros = listaResult.Result as List<VerificentroResponse> ?? new List<VerificentroResponse>();
        //     return Json(verificentros);
        // }

        // [HttpGet]
        // public async Task<JsonResult> ObtenerInventario(long id)
        // {
        //     var result = new ResponseViewModel(true);
        //     try
        //     {
        //         var listaResult = await VentaCVVHelper.GetInventarioVentaCVV(_proxyWebAPI, id);
        //         if (!listaResult.IsSuccessFully)
        //         {
        //             result.IsSuccessFully = false;
        //             return new JsonResult(result);
        //         }
        //         var vm = listaResult.Result as InventarioVentaCVVResponse ?? new InventarioVentaCVVResponse();
        //         result.Result = await this.RenderViewToStringAsync("~/Views/Venta/VentaCVV/_Inventario.cshtml", vm);
        //     }
        //     catch (Exception ex)
        //     {
        //         result.IsSuccessFully = false;
        //         result.Message = ex.Message;
        //     }
        //     return new JsonResult(result);
        // }



        [HttpGet]
        public async Task<IActionResult> Detalle(long idAlmacen, long idVenta)
        {
            var listaResult = await VentaCVVHelper.GetDetalleVentaCertificados(_proxyWebAPI, idAlmacen, idVenta);
            if (!listaResult.IsSuccessFully)
                return BadRequest();
            var vm = listaResult.Result as VentaCertificadoResponse ?? new VentaCertificadoResponse();
            return View("~/Views/Venta/VentaCVV/Detalle.cshtml", vm);
        }

        [HttpPost]
        public async Task<JsonResult> RegistrarVenta(VentaFVRequest venta)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var files = HttpContext.Request.Form.Files;
                var docs = files.Select(x => new VentaFiles { Nombre = x.FileName, Base64 = ConvertFileToBase64(x), Tipo = x.Name }).ToList();
                venta.Files = docs.Any() ? docs : new List<VentaFiles>();
                if (venta.certificadoVentas == null || !venta.certificadoVentas.Any())
                {
                    throw new ValidationException("Debe establecer los precios para finalizar la venta");
                }
                var response = await VentaCVVHelper.Registro(_proxyWebAPI, venta);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;

                    if (!string.IsNullOrEmpty(response.Message))
                    {
                        result.Message = response?.Message ?? _mensajeErrorCatch;
                    }

                    return new JsonResult(result);
                }
                else if ((response.Result as long? ?? 0) == 0)
                {
                    result.IsSuccessFully = false;
                    result.Message = response.Message;
                }

                result.Result = response.Result;
            }
            catch (ValidationException ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = _mensajeErrorCatch;
            }
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetPDF(long idAlmacen, long idVenta)
        {
            var result = new ResponseViewModel(true);
            try
            {
                string stringHtml = string.Empty;
                var dataReport = new DataReport();

                var listaResult = await VentaCVVHelper.GetDetalleVentaCertificados(_proxyWebAPI, idAlmacen, idVenta);

                var vm = listaResult.Result as VentaCertificadoResponse ?? new VentaCertificadoResponse();

                VentaCertificadoDocument ventaCertificadoDocument = new()
                {
                    NombreAlmacen = vm.NombreAlmacen,
                    FechaVenta = vm.FechaVenta,
                    NombreVerificentro = vm.NombreVerificentro,
                    NumeroCompra = vm.NumeroCompra,
                    NumeroReferencia = vm.NumeroReferencia,
                    ReferenciaBancaria = vm.ReferenciaBancaria,
                    Clave = vm.Clave,
                    Rfc = vm.Rfc,
                    TotalCantidadHologramas = vm.Cantidad,
                    SumaImporteTotal = vm.SumaImporteTotal,
                    certificadosTotalData = new List<Smadot.Utilities.Modelos.Documentos.CertificadoDataTotal>()
                };
                foreach (var item in vm.certificadosTotalData)
                {
                    ventaCertificadoDocument.certificadosTotalData.Add(new()
                    {
                        Cantidad = item.Cantidad,
                        FolioFinal = item.FolioFinal,
                        FolioInicial = item.FolioInicial,
                        NombreHolograma = item.NombreHolograma,
                        ImporteTotal = item.ImporteTotal
                    });
                }

                var getdoc = await _pdfBuider.GetDocumentoVentaCertificados(ventaCertificadoDocument);

                var doc = getdoc.Response.DocumentoPDF;
                var nomb = getdoc.Response.NombreDocumento;

                dataReport.NombreDocumento = nomb;
                dataReport.DocumentoPDF = doc;

                var pdf = dataReport;

                // var stream = new MemoryStream(pdf.DocumentoPDF);
                // var contentType = @"application/pdf";
                // var fileName = "reporte_" + pdf.NombreDocumento;

                // var doc1 = File(stream, contentType, fileName);

                result.Result = Convert.ToBase64String(doc);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        public async Task<JsonResult> AutocompleteSolicitud()
        {
            JsonResult result = new JsonResult("");

            var term = Request.Query["q"].FirstOrDefault();
            int page = Convert.ToInt32(Request.Query["page"].FirstOrDefault() ?? "0");
            int records = Convert.ToInt32(Request.Query["records"].FirstOrDefault() ?? "0");
            int idAlmacen = Convert.ToInt32(Request.Query["idAlmacen"].FirstOrDefault() ?? "0");
            var start = 0;
            var end = start + records;
            var listaResult = await VentaCVVHelper.AutocompleteSolicitud(_proxyWebAPI, new VentaCVVAutocompletRequest { Term = term, Start = start, End = end, IdAlmacen = idAlmacen });
            var data = listaResult.Result as List<RefrendoAutocompleteResponse> ?? new List<RefrendoAutocompleteResponse>();

            return Json(new { items = data, total_count = data.Count });
        }

        #region Reportes

        // [HttpGet]
        // public async Task<IActionResult> CrearReporte(string search, string sortColumnDirection, string sortColumn, bool esReportePdf)
        // {
        //     JsonResultDto result = new JsonResultDto();

        //     try
        //     {
        //         var request = new VentaCVVListRequest
        //         {
        //             Busqueda = search,
        //             ColumnaOrdenamiento = sortColumn,
        //             Ordenamiento = sortColumnDirection,
        //             Pagination = true
        //         };

        //         var listaResult = await VentaCVVHelper.Consulta(_proxyWebAPI, request);
        //         var dataResult = listaResult.Result as List<VentaCVVListResponse> ?? new List<VentaCVVListResponse>();

        //         var data = dataResult.Select(x => new VentaCVVDocument()
        //         {
        //             NumeroVenta = x.NumeroVenta,
        //             Verificentro = x.Verificentro,
        //             UserRegistro = x.UserRegistro,
        //             CantidadHologramas = x.CantidadHologramas,
        //             FechaVenta = x.Fecha,
        //             Direccion = x.Direccion,
        //             FolioCertificado = x.FolioCertificado,
        //             Telefono = x.Telefono,
        //             UrlRoot = DocumentDic.UrlRootPDF,
        //         }).ToList();

        //         byte[] bytes;
        //         if (esReportePdf)
        //         {
        //             var reportePDF = await _pdfBuider.GetDocumentoVentaCVV(data);
        //             bytes = reportePDF.Response.DocumentoPDF;
        //             return File(bytes, "application/pdf", reportePDF.Response.NombreDocumento);
        //         }
        //         else
        //         {
        //             var reporteExcel = await _excelBuilder.GetDocumentoVentaCVV(data);
        //             bytes = reporteExcel.Response.DocumentoExcel;
        //             HttpContext.Session.SetString("FileName", reporteExcel.Response.NombreDocumento);
        //             HttpContext.Session.Set("FileXlsx", bytes);
        //         }
        //         result.Result = new
        //         {
        //             FileName = "Venta Forma Valorada a CVV",
        //             Bytes = bytes
        //         };
        //     }
        //     catch (Exception ex)
        //     {
        //         result.Error = true;
        //         result.ErrorDescription = $"Error al generar el reporte | {ex.Message} --- {ex.StackTrace}";
        //     }
        //     return Json(result);
        // }

        #endregion Reportes
        #region Private methods
        private string ConvertFileToBase64(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                byte[] bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
        #endregion
    }
}
