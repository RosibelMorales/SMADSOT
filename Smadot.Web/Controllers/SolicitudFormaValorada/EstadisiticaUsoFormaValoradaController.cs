using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Smadot.Models.Dicts;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.Entities.ConsultaTablaMaestra.Response;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Request;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Response;
using Smadot.Models.Entities.EstadisticasUsoFormaValorada.Request;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Personal.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Helper.Operaciones.DirectorioCentrosVerificacion;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Models;
using Smadot.Web.Utilities;
using System;
using System.Globalization;

namespace Smadot.Web.Controllers.SolicitudFormaValorada
{
    [Authorize]
    public class EstadisiticaUsoFormaValoradaController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuider;
        private readonly IExcelBuilder _excelBuilder;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public EstadisiticaUsoFormaValoradaController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IExcelBuilder excelBuilder)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            _excelBuilder = excelBuilder;
        }

        public async Task<IActionResult> Index()
        {

            if (!_userResolver.HasPermission(PermisosMenu.Dashboard))
                return View("~/Views/Home/Index.cshtml");
            var request = new DirectorioCentrosVerificacionListRequest();
            var listaResult = await DirectorioCentrosVerificacionHelper.Consulta(_proxyWebAPI, request);
            var data = (listaResult.Result as List<DirectorioCentrosVerificacionResponse> ?? new List<DirectorioCentrosVerificacionResponse>()).Where(x => !x.Clave.Contains("SMADSOT")).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre, Selected = x.Id == _userResolver.GetUser().IdVerificentro }).OrderBy(x => x.Text).ToList();
            ViewBag.CentroAtencion = data;
            var listItem = TipoCertificado.DictNombreCertificadoCVV.Select(x => new SelectListItem
            {
                Value = x.Key.ToString(),
                Text = x.Value
            }).ToList();

            ViewBag.TipoCertificado = listItem;

            listItem = CausaRechazo.DictClaveRechazo.Where(x => x.Key != 0).Select(x => new SelectListItem
            {
                Value = x.Key.ToString(),
                Text = x.Value
            }).ToList();

            ViewBag.CausaRechazo = listItem;
            listaResult = await CatalogosVerificacionHelper.ConsultaTipoServicio(_proxyWebAPI);
            data = (listaResult.Result as List<Catalogo> ?? new List<Catalogo>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).OrderBy(x => x.Text).ToList();
            ViewBag.TiposServicio = data;
            listaResult = await CatalogosVerificacionHelper.ConsultaMotivosVerificacion(_proxyWebAPI);
            data = (listaResult.Result as List<Catalogo> ?? new List<Catalogo>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).OrderBy(x => x.Text).ToList();
            ViewBag.MotivosVerificacion = data;

            return View("~/Views/EstadisiticaUsoFormaValorada/Index.cshtml");
        }
        [HttpPost]
        public async Task<JsonResult> Consulta(EstadisticaUsoFormaValoradaGridRequest request)
        {
            JsonResult result = new JsonResult("");
            // var request = new EstadisticaUsoFormaValoradaGridRequest();

            int totalRecord = 0;
            int filterRecord = 0;

            var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

            var gridData = new ResponseGrid<EstadisiticaUsoFormaValoradaResponseGridAcciones>();
            var listaResult = await DahsboardVerificacionesHelper.ConsultaEstadisticasDashboard(_proxyWebAPI, request);



            if (listaResult.IsSuccessFully)
            {
                var jsonReponse = (ResponseGrid<EstadisiticaUsoFormaValoradaResponseGridAcciones>)listaResult.Result;
                jsonReponse.Data.ForEach(async x =>
                {
                    x.Acciones = await this.RenderViewToStringAsync("~/Views/EstadisiticaUsoFormaValorada/_actions.cshtml", x);
                });
                jsonReponse.Draw = draw;
                return Json(jsonReponse);
            }

            return Json("");
        }


        [HttpGet]
        public async Task<JsonResult> GetConteoCertificados(EstadisticaUsoFormaValoradaRequest req)
        {
            var result = new ResponseViewModel(true);

            try
            {
                var datosBar = await DahsboardVerificacionesHelper.GetConteoCertificados(_proxyWebAPI, req);
                var vm = datosBar.Result as List<CertificadoMapeo> ?? new List<CertificadoMapeo>();
                result.Result = await this.RenderViewToStringAsync("~/Views/EstadisiticaUsoFormaValorada/_conteoCerificados.cshtml", vm);

            }
            catch (Exception ex)
            {
                result = new ResponseViewModel(false);
            }

            return new JsonResult(result);
        }
        [HttpGet]
        public async Task<ActionResult> GetGraphBar(EstadisticaUsoFormaValoradaRequest req)
        {
            _ = new List<VerificacionCVVRangoFecha>();
            List<VerificacionCVVRangoFecha>? result;
            try
            {
                //var datosBar = await SolicitudFormaValoradaHelper.ConsultaGraficaBarras(_proxyWebAPI, new EstadisticaUsoFormaValoradaRequest { FechaInicial = DateTime.Now.AddDays(-60).ToString("d"), FechaFinal = DateTime.Now.ToString("d"), IdVerificentro = _userResolver.GetUser().IdVerificentro });
                var datosBar = await DahsboardVerificacionesHelper.ConsultaGraficaBarras(_proxyWebAPI, new EstadisticaUsoFormaValoradaRequest { FechaInicial = req.FechaInicial, FechaFinal = req.FechaFinal });
                result = datosBar.Result as List<VerificacionCVVRangoFecha> ?? new List<VerificacionCVVRangoFecha>();

            }
            catch (Exception ex)
            {
                result = new List<VerificacionCVVRangoFecha>();
            }

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetGraphPie(EstadisticaUsoFormaValoradaRequest req)
        {
            var result = new ResponseViewModel(true);

            try
            {
                //var datosBar = await SolicitudFormaValoradaHelper.ConsultaGraficaPie(_proxyWebAPI, new EstadisticaUsoFormaValoradaRequest { FechaInicial = DateTime.Now.AddDays(-60).ToString("d"), FechaFinal = DateTime.Now.ToString("d"), IdVerificentro = _userResolver.GetUser().IdVerificentro });
                var datosBar = await DahsboardVerificacionesHelper.ConsultaGraficaPie(_proxyWebAPI, new EstadisticaUsoFormaValoradaRequest { FechaInicial = req.FechaInicial, FechaFinal = req.FechaFinal });
                var resuDtosBar = datosBar.Result as List<DataPieChart> ?? new List<DataPieChart>();
                result.Result = resuDtosBar;
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetGraphGauge(EstadisticaUsoFormaValoradaRequest req)
        {
            var result = new ResponseViewModel(true);

            try
            {
                //var datosBar = await SolicitudFormaValoradaHelper.ConsultaGraficaGauge(_proxyWebAPI, new EstadisticaUsoFormaValoradaRequest { FechaInicial = DateTime.Now.AddDays(-60).ToString("d"), FechaFinal = DateTime.Now.ToString("d"), IdVerificentro = _userResolver.GetUser().IdVerificentro });
                var datosBar = await DahsboardVerificacionesHelper.ConsultaGraficaGauge(_proxyWebAPI, new EstadisticaUsoFormaValoradaRequest { FechaInicial = req.FechaInicial, FechaFinal = req.FechaFinal });
                var resuDtosBar = datosBar.Result as List<DataGaugeChart> ?? new List<DataGaugeChart>();
                var maxVG = resuDtosBar.Max(x => x.Value);
                result.Result = new
                {
                    result = resuDtosBar,
                    maxValue = maxVG
                };
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }


        [HttpGet]
        public async Task<ActionResult> GetCounters(EstadisticaUsoFormaValoradaRequest req)
        {
            var result = new ResponseViewModel(true);

            try
            {
                var datos = await DahsboardVerificacionesHelper.ConsultaEstadisticaUsoFormaValorada(_proxyWebAPI, req);
                var data = datos.Result as DashboardIndicadores ?? new DashboardIndicadores(new());
                result.Result = await this.RenderViewToStringAsync("~/Views/EstadisiticaUsoFormaValorada/_indicadores.cshtml", data); ;



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
        public async Task<IActionResult> CrearReporte(EstadisticaUsoFormaValoradaGridRequest request)
        {
            JsonResultDto result = new JsonResultDto();
            var text = "";

            try
            {

                var listaResvVerf = await DahsboardVerificacionesHelper.ConsultaEstadisticasDashboard(_proxyWebAPI, request);
                var jsonReponse = (ResponseGrid<EstadisiticaUsoFormaValoradaResponseGridAcciones>)listaResvVerf.Result;
                var dataResult = (jsonReponse.Data != null && jsonReponse.Data.Count() > 0) ? jsonReponse.Data : new List<EstadisiticaUsoFormaValoradaResponseGridAcciones>();

                //var listaResult = await VentaCVVHelper.Consulta(_proxyWebAPI, request);
                //var dataResult2 = listaResult.Result as List<VentaCVVListResponse> ?? new List<VentaCVVListResponse>();

                var data = dataResult.Select(x => new EstadisticasDashboardGridDocumentResponse()
                {
                    Id = x.Id,
                    Combustible = x.Combustible,
                    Serie = x.Serie,
                    Placa = x.Placa,
                    Modelo = x.Modelo,
                    Marca = x.Marca,
                    SubMarca = x.SubMarca,
                    NombrePropietario = x.NombrePropietario,
                    TarjetaCirculacion = x.TarjetaCirculacion,
                    NombreVerificentro = x.NombreVerificentro,
                    FechaVerificacion = x.FechaVerificacion,
                    Vigencia = x.Vigencia,
                    MotivoVerificacion = x.MotivoVerificacion,
                    IdCatTipoServicio = x.IdCatTipoServicio,
                    IdMotivoVerificacion = x.IdMotivoVerificacion,
                    TipoServicio = x.TipoServicio,
                    CausaRechazo = x.CausaRechazo,
                    CambioPlacas = x.CambioPlacas,
                    FechaFacturacion = x.FechaFacturacion,
                    NoIntentos = (int)x.NoIntentos,
                    SinMulta = x.SinMulta,
                    IdVerificacion = x.IdVerificacion,
                    IdCatMarcaVehiculo = x.IdCatMarcaVehiculo,
                    IdCatSubmarcaVehiculo = x.IdCatSubmarcaVehiculo,
                    Motor_DSL = x.Motor_DSL,
                    COMB_ORIG = x.COMB_ORIG,
                    CARROCERIA = x.CARROCERIA,
                    ALIM_COMB = x.ALIM_COMB,
                    CILINDROS = x.CILINDROS,
                    CILINDRADA = x.CILINDRADA,
                    PBV = x.PBV,
                    PBV_EQUIV = x.PBV_EQUIV,
                    PBV_ASM = x.PBV_ASM,
                    CONV_CATAL = x.CONV_CATAL,
                    OBD = x.OBD,
                    C_ABS = x.C_ABS,
                    T_TRACC = x.T_TRACC,
                    C_TRACC = x.C_TRACC,
                    T_PRUEBA = x.T_PRUEBA,
                    PROTOCOLO = x.PROTOCOLO,
                    POTMAX_RPM = x.POTMAX_RPM,
                    ANO_DESDE = x.ANO_DESDE,
                    ANO_HASTA = x.ANO_HASTA,
                    O2_MAX = x.O2_MAX,
                    LAMDA_MAX = x.LAMDA_MAX,
                    POT_5024 = x.POT_5024,
                    POT_2540 = x.POT_2540,
                    DOBLECERO = x.DOBLECERO,
                    CERO_GASOL = x.CERO_GASOL,
                    CERO_GASLP = x.CERO_GASLP,
                    CERO_GASNC = x.CERO_GASNC,
                    CERO_DSL = x.CERO_DSL,
                    REF_00 = x.REF_00,
                    InicioPruebas = x.InicioPruebas,
                    FinalizacionPruebas = x.FinalizacionPruebas,
                    EstatusPrueba = x.EstatusPrueba,
                    FugasSistemaEscape = x.FugasSistemaEscape,
                    PortafiltroAire = x.PortafiltroAire,
                    TaponDispositivoAceite = x.TaponDispositivoAceite,
                    FiltroAire = x.FiltroAire,
                    TaponCombustible = x.TaponCombustible,
                    Bayoneta = x.Bayoneta,
                    FugaAceiteMotor = x.FugaAceiteMotor,
                    FugaAceiteTransmision = x.FugaAceiteTransmision,
                    FugaLiquidoRefrigerante = x.FugaLiquidoRefrigerante,
                    DibujoNeumaticos = x.DibujoNeumaticos,
                    DesperfectosNeumaticos = x.DesperfectosNeumaticos,
                    DimensionesNeumaticoIncorrectas = x.DimensionesNeumaticoIncorrectas,
                    ControlEmisionDesconectados = x.ControlEmisionDesconectados,
                    ControlEmisionAlterados = x.ControlEmisionAlterados,
                    PlacasCorrespondientes = x.PlacasCorrespondientes,
                    GobernadorBuenEstado = x.GobernadorBuenEstado,
                    NumeroEscapes = x.NumeroEscapes,
                    Etapa = x.Etapa,
                    SPS_Humo = x.SPS_Humo,
                    SPS_5024 = x.SPS_5024,
                    SPS_2540 = x.SPS_2540,
                    HC = x.HC,
                    CO = x.CO,
                    CO2 = x.CO2,
                    O2 = x.O2,
                    NO = x.NO,
                    LAMDA = x.LAMDA,
                    FCNOX = x.FCNOX,
                    FCDIL = x.FCDIL,
                    RPM = x.RPM,
                    KPH = x.KPH,
                    VEL_LIN = x.VEL_LIN,
                    VEL_ANG = x.VEL_ANG,
                    BHP = x.BHP,
                    PAR_TOR = x.PAR_TOR,
                    FUERZA = x.FUERZA,
                    POT_FRENO = x.POT_FRENO,
                    TEMP = x.TEMP,
                    PRESION = x.PRESION,
                    HUMREL = x.HUMREL,
                    OBD_TIPO_SDB = x.OBD_TIPO_SDB,
                    OBD_MIL = x.OBD_MIL,
                    OBD_CATAL = x.OBD_CATAL,
                    OBD_CILIN = x.OBD_CILIN,
                    OBD_COMBU = x.OBD_COMBU,
                    OBD_INTEG = x.OBD_INTEG,
                    OBD_OXIGE = x.OBD_OXIGE,
                    LAMDA_5024 = x.LAMDA_5024,
                    TEMP_5024 = x.TEMP_5024,
                    HR_5024 = x.HR_5024,
                    PSI_5024 = x.PSI_5024,
                    FCNOX_5024 = x.FCNOX_5024,
                    FCDIL_5024 = x.FCDIL_5024,
                    RPM_5024 = x.RPM_5024,
                    KPH_5024 = x.KPH_5024,
                    THP_5024 = x.THP_5024,
                    VOLTS_5024 = x.VOLTS_5024,
                    HC_5024 = x.HC_5024,
                    CO_5024 = x.CO_5024,
                    CO2_5024 = x.CO2_5024,
                    COCO2_5024 = x.COCO2_5024,
                    O2_5024 = x.O2_5024,
                    NO_5024 = x.NO_5024,
                    LAMDA_2540 = x.LAMDA_2540,
                    TEMP_2540 = x.TEMP_2540,
                    HR_2540 = x.HR_2540,
                    PSI_2540 = x.PSI_2540,
                    FCNOX_2540 = x.FCNOX_2540,
                    FCDIL_2540 = x.FCDIL_2540,
                    RPM_2540 = x.RPM_2540,
                    KPH_2540 = x.KPH_2540,
                    THP_2540 = x.THP_2540,
                    VOLTS_2540 = x.VOLTS_2540,
                    HC_2540 = x.HC_2540,
                    CO_2540 = x.CO_2540,
                    CO2_2540 = x.CO2_2540,
                    COCO2_2540 = x.COCO2_2540,
                    O2_2540 = x.O2_2540,
                    NO_2540 = x.NO_2540,
                    OPACIDADP = x.OPACIDADP,
                    OPACIDADK = x.OPACIDADK,
                    TEMP_MOT = x.TEMP_MOT,
                    VEL_GOB = x.VEL_GOB,
                    TEMP_GAS = x.TEMP_GAS,
                    TEMP_CAM = x.TEMP_CAM,
                    RESULTADO = x.RESULTADO,
                    C_RECHAZO = x.C_RECHAZO,
                    C_RECHAZO_OBD = x.C_RECHAZO_OBD,
                    PruebaObd = x.PruebaObd,
                    PruebaEmisiones = x.PruebaEmisiones,
                    PruebaOpacidad = x.PruebaOpacidad,
                    EntidadProcedencia = x.EntidadProcedencia,
                    IdTipoCertificado = x.IdTipoCertificado,
                    FolioCertificadoActual = x.FolioCertificadoActual,
                    FolioCertificado = x.FolioCertificadoAnterior,
                    ClaveTramite = x.ClaveTramite,
                    IdVerificentro = x.IdVerificentro
                }).ToList();

                byte[] bytes;
                var reporteExcel = await _excelBuilder.GetDocumentoDashboardGrid(data);
                text = reporteExcel.CurrentException+reporteExcel.mensaje;
                bytes = reporteExcel.Response.DocumentoExcel;
                HttpContext.Session.SetString("FileName", reporteExcel.Response.NombreDocumento);
                HttpContext.Session.Set("FileXlsx", bytes);
                result.Result = new
                {
                    FileName = "Dashboard Estadisticas",
                    Bytes = bytes
                };
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorDescription = $"Error al generar el reporte | {text} --- {ex.StackTrace}";
            }
            return Json(result);
        }

        #endregion Reportes

        private void SetMenuLinkActive()
        {
            ViewBag.EstadisiticaUsoFormaValorada = "here";
            ViewBag.Administracion = "";
        }
    }
}
