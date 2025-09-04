using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.DevolucionSPF.Response;
using Smadot.Models.Entities.FoliosFormaValorada.Request;
using Smadot.Models.Entities.FoliosFormaValorada.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.FoliosFormaValorada;
using Smadot.Web.Utilities;

namespace Smadot.Web.Controllers.FoliosFormaValorada
{
    [Authorize]
    public class FoliosFormaValoradaController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuilder;
        private readonly BlobStorage _blobStorage;
        public FoliosFormaValoradaController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuilder = pdfBuider;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }
        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuImpresiones.PermisoMenuPadre))
                return new UnauthorizedResult();
            var datatCert = TipoCertificado.DictNombreCertificado.ToList().Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value }).ToList();
            ViewBag.tCert = datatCert;
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");

            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int page = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            //Invoca al método que se encarga de realizar la petición Api
            var request = new FolioFormaValoradaGridRequest();
            request.Pagina = page / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = searchValue;
            request.ColumnaOrdenamiento = sortColumn;
            request.Ordenamiento = sortColumnDirection;

            long tc;
            if (long.TryParse(Request.Form["tipoCertificado"].FirstOrDefault(), out tc)) request.TipoCertificado = tc;
            else request.TipoCertificado = null;

            var response = await FoliosFormaValoradaHelper.Consulta(_proxyWebAPI, request);

            if (response.IsSuccessFully)
            {
                var jsonReponse = (ResponseGrid<FolioPendienteImprimirResponse>)response.Result;
                jsonReponse.Data.ForEach(async x =>
                {
                    x.Actions = await this.RenderViewToStringAsync("~/Views/FoliosFormaValorada/_Acciones.cshtml", x);
                });
                jsonReponse.Draw = draw;
                return Json(jsonReponse);
            }

            return Json("");
        }

        [HttpPost]
        public async Task<JsonResult> EnviarImpresion(GenerateImpresionRequest request)
        {
            try
            {
                var data = new FolioFormaValoradaImpresionResponse();

                var result = new ResponseViewModel(true);
                var citaHelper = await FoliosFormaValoradaHelper.Consulta(_proxyWebAPI, request);
                if (citaHelper.IsSuccessFully)
                {
                    if (!request.Imprimir)
                    {
                        data = citaHelper.Result as FolioFormaValoradaImpresionResponse ?? new FolioFormaValoradaImpresionResponse();
                        var vm = new ImpresionPDFResponse
                        {
                            Folio = data.FolioFoliosFormaValoradaVerificentro,
                            Marca = data.Marca,
                            Anio = data.Anio.ToString(),
                            Placa = data.Placa,
                            Holograma = data.TipoCertificado,
                            Submarca = data.Modelo,
                            Vigencia = data.VigenciaStr,
                            VigenciaFecha = data.Vigencia.Value,
                            Centro = data.NombreVerificentro,
                            Linea = data.Clave,
                            Equipo = data.NombreEquipo,
                            LeyendaCNA = data.LeyendaCNA,
                            FolioAnterior = data.FolioCertificadoAnterior,
                            Fecha = data.FechaRegistro.Value.ToString("dd/MM/yyyy HH:mm"),
                            HoraInicio = data.InicioPruebas.HasValue ? data.InicioPruebas.Value.ToString("HH:mm:ss") : "",
                            HoraFin = data.FinalizacionPruebas.HasValue ? data.FinalizacionPruebas.Value.ToString("HH:mm:ss") : "",
                            Nombre = data.Propietario,
                            Semestre = data.Semestre,
                            Combustible = data.Combustible,
                            TecnicoCapturaNumero = data?.NumeroCapturista.ToString() ?? "-",
                            TecnicoPruebaNumero = data?.NumeroTecnico.ToString() ?? "-",
                            AprobadoPor = data.AprobadoPor,
                            NumSerie = data.Serie,
                            IdCatTipoTramite = data.IdCatTipoTramite ?? 0,
                            Entidad = data.EntidadProcedencia,
                        };
                        result.Result = await this.RenderViewToStringAsync("~/Views/FoliosFormaValorada/_ModalDetalleCertificado.cshtml", vm);
                    }
                    else
                    {
                        result.Message = "El folio se imprimió correctamente.";
                    }
                }
                result.Message = citaHelper.Message;
                result.IsSuccessFully = citaHelper.IsSuccessFully;
                return Json(result);

            }
            catch (Exception e)
            {

                return Json(new ResponseViewModel { IsSuccessFully = false, Message = "No se pudo generar correctamente el documento para imprimirlo." });

            }

        }

        [HttpGet]
        public async Task<JsonResult> DetalleCertificado(GenerateImpresionRequest request)
        {
            var result = new ResponseViewModel(true);
            var responseModel = new FolioFormaValoradaImpresionResponse();
            try
            {
               
                var response = await FoliosFormaValoradaHelper.DetalleCertificado(_proxyWebAPI, request);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    result.Message=response.Message;
                    return new JsonResult(result);
                }
                responseModel = response.Result as FolioFormaValoradaImpresionResponse ?? new();
                var vm = new ImpresionPDFResponse()
                {
                    Folio = responseModel.FolioFoliosFormaValoradaVerificentro,
                    Marca = responseModel.Marca,
                    Anio = responseModel.Anio.ToString(),
                    Placa = responseModel.Placa,
                    Holograma = responseModel.TipoCertificado,
                    Submarca = responseModel.Modelo,
                    Vigencia = responseModel.VigenciaVistaPreviaStr,
                    VigenciaFecha = responseModel.Vigencia.Value,
                    Centro = responseModel.NombreVerificentro,
                    Linea = responseModel.Clave,
                    LeyendaCNA = responseModel.LeyendaCNA,
                    Equipo = responseModel.NombreEquipo,
                    FolioAnterior = responseModel.FolioCertificadoAnterior,
                    Fecha = responseModel.FechaRegistro.Value.ToString("dd/MM/yyyy HH:mm"),
                    HoraInicio = responseModel.InicioPruebas.HasValue ? responseModel.InicioPruebas.Value.ToString("HH:mm:ss") : "",
                    HoraFin = responseModel.FinalizacionPruebas.HasValue ? responseModel.FinalizacionPruebas.Value.ToString("HH:mm:ss") : "",
                    Nombre = responseModel.Propietario,
                    Semestre = responseModel.Semestre,
                    Combustible = responseModel.Combustible,
                    Version = "1.5",
                    // TecnicoCapturaNombre = responseModel.NombreCapturista,
                    // TecnicoCapturaNumero = responseModel?.NumeroCapturista ?? "-",
                    TecnicoPruebaNombre = responseModel.NombreTecnico,
                    TecnicoPruebaNumero = responseModel?.NumeroTecnico ?? "-",
                    AprobadoPor = responseModel?.AprobadoPor ?? "-",
                    NumSerie = responseModel.Serie,
                    IdCatTipoTramite = TipoTramiteDict.CVV,
                    // UrlExpediente = (responseModel.IdCatTipoCertificado != TipoCertificado.Exentos) ? $"{siteUrl}Citas/ExpedientePublico?clave={HttpUtility.UrlEncode(idBase64)}" : null,
                    // Emisiones = new List<ImpresionPDFEmisionResponse>(),
                    Entidad = responseModel.EntidadProcedencia
                };
                result.Result = await this.RenderViewToStringAsync("~/Views/FoliosFormaValorada/_ModalDetalleCertificado.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetFoliosProximos()
        {
            var result = new ResponseViewModel(true);
            var vm = new ImpresionPDFResponse();
            try
            {
                var response = await FoliosFormaValoradaHelper.ProximosFolios(_proxyWebAPI);
                if (!response.IsSuccessFully)
                {
                    result.Result = new FoliosProximosResponse();
                    return new JsonResult(result);
                }
                result.Result = response.Result;
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpPut]
        public async Task<JsonResult> RecalcularFolio(long idFolioFormaValorada, long idVerificentro)
        {
            var result = new ResponseViewModel();
            try
            {
                var validar = await FoliosFormaValoradaHelper.Recalcular(_proxyWebAPI, idFolioFormaValorada, idVerificentro);

                return new JsonResult(validar);
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
