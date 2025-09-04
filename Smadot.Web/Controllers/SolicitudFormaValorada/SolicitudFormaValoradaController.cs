using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Win32;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Utilities.Seguridad;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Utilities;
using Smadot.Models.Dicts;

namespace Smadot.Web.Controllers.SolicitudFormaValorada
{
    [Authorize]
    public class SolicitudFormaValoradaController : Controller
    {

        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuider;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public SolicitudFormaValoradaController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
        }

        public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(PermisosMenu.SolicitudesdeFormasValoradas))
                return new UnauthorizedResult();
            var catalogo = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest());
            var list = (catalogo.Result as List<AlmacenResponse> ?? new List<AlmacenResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
            if (catalogo.AccesoTotalVerificentros)
                list.Insert(0, new SelectListItem { Text = "TODOS" });
            ViewBag.Almacenes = list;
            return View("~/Views/SolicitudFormaValorada/Index.cshtml");
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
            var listaResult = await SolicitudFormaValoradaHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as ResponseGrid<SeguimientoSolicitudResponse> ?? new ResponseGrid<SeguimientoSolicitudResponse>();

            foreach (var row in data.Data ?? new List<SeguimientoSolicitudResponse>())
            {
                row.Acciones = await this.RenderViewToStringAsync("~/Views/SolicitudFormaValorada/_Acciones.cshtml", row);
            }
            data.Draw = draw;

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            var result = new ResponseViewModel(true);
            var vm = new List<SolicitudFormaValoradaResponse>
            {
                new SolicitudFormaValoradaResponse
                {
                    IdSolicitudFV = 0
                }
            };
            try
            {
                var response = await SolicitudFormaValoradaHelper.Consulta(_proxyWebAPI, id ?? 0);
                vm = response.Result as List<SolicitudFormaValoradaResponse> ?? vm;
                var almacenes = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest() { Activo = true });
                var almaceneslist = almacenes.Result as List<AlmacenResponse> ?? new List<AlmacenResponse>();
                ViewBag.almacenes = almaceneslist.Select(obj => new SelectListItem { Value = obj.Id.ToString(), Text = obj.Nombre }).ToList();

                if (almaceneslist.FirstOrDefault()?.IdVerificentro != null)
                {

                    vm = vm.Where(x => x.IdCatTipoCertificadoSC <= TipoCertificado.ConstanciasNoAprobado).ToList();
                }
                result.Result = await this.RenderViewToStringAsync("~/Views/SolicitudFormaValorada/_Registro.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }
        [HttpGet]
        public async Task<IActionResult> TableCertificado(long? id, int idAlmacen)
        {
            var result = new ResponseViewModel(true);
            var vm = new List<SolicitudFormaValoradaResponse>
            {
                new SolicitudFormaValoradaResponse
                {
                    IdSolicitudFV = 0
                }
            };
            try
            {
                var response = await SolicitudFormaValoradaHelper.Consulta(_proxyWebAPI, id ?? 0);
                vm = response.Result as List<SolicitudFormaValoradaResponse> ?? vm;
                var almacenes = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest() { Activo = true });
                var almaceneslist = almacenes.Result as List<AlmacenResponse> ?? new List<AlmacenResponse>();
                var almacen = almaceneslist.FirstOrDefault(x => x.Id == idAlmacen);
                if (almacen == null)
                {
                    result.IsSuccessFully = false;
                    result.Message = "Hubo un error al cargar la información.";
                    return new JsonResult(result);
                }
                if (almacen.IdVerificentro != null)
                {

                    vm = vm.Where(x => x.IdCatTipoCertificadoSC <= TipoCertificado.ConstanciasNoAprobado).ToList();
                }
                result.Result = await this.RenderViewToStringAsync("~/Views/SolicitudFormaValorada/_tableCertificados.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SolicitudFormaValoradaRequest registro)
        {
            var result = new ResponseViewModel(true);
            try
            {

                var save = await SolicitudFormaValoradaHelper.Edit(_proxyWebAPI, registro);
                if (save.IsSuccessFully)
                    result.Message = "La información se guardó correctamente.";

            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Result = ex.Message;
                result.Message = "Ocurrió un error insesperado al guardar la información";
            }

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> GetPDF(List<SolicitudFormaValoradaDocument> registro)
        {
            var result = new ResponseViewModel(true);
            try
            {
                string stringHtml = string.Empty;
                var dataReport = new DataReport();
                var getdoc = await _pdfBuider.GetDocumentoSolicitudFormaValorada(registro);

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

        private void SetMenuLinkActive()
        {
            ViewBag.SolicitudFormaValorada = "here";
            ViewBag.Administracion = "";
            ViewBag.Tramites = "";
        }
    }
}
