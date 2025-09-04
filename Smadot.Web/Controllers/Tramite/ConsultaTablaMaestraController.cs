using Microsoft.AspNetCore.Mvc;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.Tramite;
using static Smadot.Models.Entities.ConsultaTablaMaestra.Request.ConsultaTablaMaestraRequestData;
using static Smadot.Models.Entities.ConsultaTablaMaestra.Response.ConsultaTablaMaestraResponseData;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Models.Dicts;
using Smadot.Utilities.Modelos.Documentos;
using Newtonsoft.Json;
using System.Data.OleDb;
using Smadot.Models.DataBase;
using System.Globalization;
using Smadot.Models.Entities.ConsultaTablaMaestra.Request;
using Smadot.Web.Utilities;
using Smadot.Models.Entities.ConsultaTablaMaestra.Response;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Web.Helper.Operaciones.Catalogo;

namespace Smadot.Web.Controllers.Tramite
{
    [Authorize]
    public class ConsultaTablaMaestraController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IExcelBuilder _excelBuilder;
        private readonly IPdfBuider _pdfBuider;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public ConsultaTablaMaestraController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IExcelBuilder excelBuilder)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            _excelBuilder = excelBuilder;
        }

        public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(MenuConsultaTablaMaestra.PermisoMenuPadre))
                return new UnauthorizedResult();
            var accionesHelper = await ConsultaTablaMaestraHelper.SelectList(_proxyWebAPI, new RequestList { Activo = true });
            ViewBag.select = accionesHelper.ToList();
            return View("~/Views/Tramite/ConsultaTablaMaestra/Index.cshtml");
        }
        [HttpGet]
        public async Task<ActionResult> GetSubMarcasBusqueda()
        {
            var result = new ResponseViewModel(true);

            try
            {
                var term = Request.Query["q"].FirstOrDefault();
                int page = Convert.ToInt32(Request.Query["page"].FirstOrDefault() ?? "0");
                int records = Convert.ToInt32(Request.Query["records"].FirstOrDefault() ?? "0");
                var helper = await SubMarcaVehiculoHelper.ConsultaSecundaria(_proxyWebAPI, new SubMarcaVehiculoRequest { Marca = string.Empty, Pagina = page, Registros = records, Busqueda = term, SubmarcaClave = false });
                if (helper.IsSuccessFully)
                {
                    var resulthelper = JsonConvert.SerializeObject(helper.Result);
                    var data = JsonConvert.DeserializeObject<ResponseGrid<SelectListItem>>(resulthelper);
                    var dataResult = data?.Data != null ? data.Data : new List<SelectListItem>();
                    result.Result = data;
                    return new JsonResult(result);
                }
                result.IsSuccessFully = false;
                result.Message = "Ocurrió un error al obtener la información";
                return new JsonResult(result);

            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<JsonResult> Registro(long? id)
        {
            var result = new ResponseViewModel(true);
            try
            {

                if (!_userResolver.HasPermission(MenuConsultaTablaMaestra.PermisoMenuPadre))
                {
                    throw new ValidationException("No tiene permisos para realizar esta acción.");
                }
                var requestService = await ConsultaTablaMaestraHelper.Registro(_proxyWebAPI, id ?? 0);
                if (!requestService.IsSuccessFully)
                {
                    throw new ValidationException(requestService.Message);
                }
                var vm = (requestService.Result ?? new RegistroTablaMaestraResponse()) as RegistroTablaMaestraResponse;
                if (id != null && vm?.TablaMaestra.Id == 0)
                {
                    throw new ValidationException("No se encontró el registro que se intenta editar.");

                }
                var list = vm.Protocolos.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
                list.Insert(0, new SelectListItem { Text = "Seleccionar...", Value = "" });
                ViewBag.protocolos = list;
                list = vm.Marcas.Select(x => new SelectListItem { Value = x.Nombre, Text = x.Nombre }).ToList();
                list.Insert(0, new SelectListItem { Text = "Seleccionar...", Value = "" });

                ViewBag.marcas = list;

                result.Result = await this.RenderViewToStringAsync("~/Views/Tramite/ConsultaTablaMaestra/_Registro.cshtml", vm.TablaMaestra);
            }
            catch (ValidationException ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = "Ocurrió un error al obtener la información.";
            }

            return new JsonResult(result);
        }
        [HttpPost]
        public async Task<JsonResult> Registro(TablaMaestraRequest request)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var requestService = await ConsultaTablaMaestraHelper.Registro(_proxyWebAPI, request);
                if (!requestService.IsSuccessFully)
                {
                    throw new ValidationException(requestService.Message);
                }
                requestService.Message = "La información fue guardada correctamente.";
                return new JsonResult(requestService);
            }
            catch (ValidationException ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = "Ocurrió un error al obtener la información.";
            }

            return new JsonResult(result);
        }


        [HttpPost]
        public async Task<JsonResult> Consulta(long? IdCicloVerificacion = null, string? Marca = null, string? Submarca = null, string? Linea = null, string? Modelo = null, long? IdSubmarca = null, long? IdModelo = null, int? IdCombustible = null, bool validarCombustible = true)
        {
            //Invoca al método que se encarga de realizar la petición Api
            var request = new ConsultaTablaMaestraRequestList();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

            request.IdCicloVerificacion = IdCicloVerificacion;
            request.IdSubMarca = IdSubmarca;
            request.IdMarca = IdModelo;
            request.Marca = Marca;
            request.SubMarca = Submarca;
            request.Modelo = Modelo;
            request.IdTipoCombustible = IdCombustible;
            request.ValidarCombustible = validarCombustible;

            var listaResult = await ConsultaTablaMaestraHelper.Consulta(_proxyWebAPI, request);

            var response = listaResult.Result as ResponseGrid<ConsultaTablaMaestraResponseGrid> ?? new ResponseGrid<ConsultaTablaMaestraResponseGrid>() { RecordsFiltered = 0, RecordsTotal = 0, Draw = Request.Form["draw"].FirstOrDefault() };
            foreach (var item in response.Data)
            {
                item.Acciones = await this.RenderViewToStringAsync("~/Views/Tramite/ConsultaTablaMaestra/_Acciones.cshtml", item);
            }
            response.Draw = Request.Form["draw"].FirstOrDefault();
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> MarcaAutocomplete(string prefix)
        {
            var listaResult = await ConsultaTablaMaestraHelper.AutocompleteMarca(_proxyWebAPI, prefix);
            var folio = listaResult.Result as List<TablaMaestraAutocompleteResponse> ?? new List<TablaMaestraAutocompleteResponse>();
            var result = folio.Select(x => new { Id = x.Id, Nombre = x.Text });
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> SubMarcaAutocomplete(string prefix)
        {
            var listaResult = await ConsultaTablaMaestraHelper.AutocompleteSubmarca(_proxyWebAPI, prefix);
            var folio = listaResult.Result as List<TablaMaestraAutocompleteResponse> ?? new List<TablaMaestraAutocompleteResponse>();
            var result = folio.Select(x => new { Id = x.Id, Nombre = x.Text });
            return Json(result);
        }
        [HttpGet]
        public async Task<ActionResult> GetSubMarcas(string marca, bool submarca)
        {
            var result = new ResponseViewModel(true);

            try
            {
                var term = Request.Query["q"].FirstOrDefault();
                int page = Convert.ToInt32(Request.Query["page"].FirstOrDefault() ?? "0");
                int records = Convert.ToInt32(Request.Query["records"].FirstOrDefault() ?? "0");
                var helper = await ConsultaTablaMaestraHelper.Consulta(_proxyWebAPI, new SubMarcaVehiculoRequest { Marca = marca, Pagina = page, Registros = records, Busqueda = term, SubmarcaClave = submarca });
                if (!helper.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al obtener la información";
                }
                var data = (helper.Result as ResponseGrid<Catalogo>) ?? new ResponseGrid<Catalogo>();
                var list = data.Data.Select(x => new SelectListItem { Text = x.Nombre, Value = x.Id.ToString() }).ToList();
                result.Result = list;
                return new JsonResult(result);

            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }
        [HttpPost]
        public async Task<JsonResult> LineaAutocomplete(string prefix)
        {
            var listaResult = await ConsultaTablaMaestraHelper.AutocompleteLinea(_proxyWebAPI, prefix);
            var folio = listaResult.Result as List<TablaMaestraAutocompleteResponse> ?? new List<TablaMaestraAutocompleteResponse>();
            var result = folio.Select(x => new { Id = x.Id, Nombre = x.Text });
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> ModeloAutocomplete(string prefix)
        {
            var listaResult = await ConsultaTablaMaestraHelper.AutocompleteModelo(_proxyWebAPI, prefix);
            var folio = listaResult.Result as List<TablaMaestraAutocompleteResponse> ?? new List<TablaMaestraAutocompleteResponse>();
            var result = folio.Select(x => new { Id = x.Id, Nombre = x.Text });
            return Json(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetPDF(long IdCicloVerificacion, bool PDF, string search, string sortColumnDirection, string sortColumn, string? Marca = null, string? Submarca = null, string? Modelo = null)
        {
            var result = new ResponseViewModel(true);
            try
            {
                string stringHtml = string.Empty;
                var dataReport = new DataReport();
                var vm = new ConsultaTablaMaestraPDFUtilitiesResponse
                {
                    DirecciónVerificentro = "",
                    TelefonoVerificentro = ""
                };

                var request = new ConsultaTablaMaestraRequestList();
                request.Pagina = 0;
                request.Registros = 0;
                request.Busqueda = search;
                request.ColumnaOrdenamiento = sortColumn;
                request.Ordenamiento = sortColumnDirection;
                request.IdCicloVerificacion = IdCicloVerificacion;
                request.Marca = Marca;
                request.SubMarca = Submarca;
                request.Modelo = Modelo;

                var listaResult = await ConsultaTablaMaestraHelper.Consulta(_proxyWebAPI, request);
                vm.Rows = JsonConvert.DeserializeObject<List<ConsultaTablaMaestraUtilitiesResponse>>(JsonConvert.SerializeObject(listaResult.Result));

                if (PDF)
                {
                    var getdoc = await _pdfBuider.GetDocumentoTablaMaestra(vm);
                    HttpContext.Session.SetString("FileName", getdoc.Response.NombreDocumento);
                    HttpContext.Session.Set("FileXlsx", getdoc.Response.DocumentoPDF);
                }
                else
                {
                    var getdoc = await _excelBuilder.GetDocumentoTablaMaestra(vm);
                    HttpContext.Session.SetString("FileName", getdoc.Response.NombreDocumento);
                    HttpContext.Session.Set("FileXlsx", getdoc.Response.DocumentoExcel);
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
        public async Task<JsonResult> UploadFile(IFormFile file, int tipo)
        {
            var result = new ResponseViewModel(true);
            if (file != null && file.Length > 0)
            {
                string tempFilePath = Path.GetTempFileName();

                try
                {
                    // Guarda el archivo subido en un archivo temporal en el servidor
                    using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    var records = new TablaMaestraDbfRequest
                    {
                        TablaMaestra = new List<TablaMaestra>(),
                        CatTipoDiesel = new List<CatTipoDiesel>(),
                        CatSubdieselVehiculo = new List<CatSubdieselVehiculo>(),
                        CatMarcaVehiculo = new List<CatMarcaVehiculo>(),
                        CatSubMarcaVehiculo = new List<TablaMaestraDbfCatSubMarcaVehiculoRequest>()
                    };

                    // Lee el archivo DBF utilizando DbfDataReader
                    using (var dbfDataReader = new DbfDataReader.DbfDataReader(tempFilePath))
                    {
                        Type? tablaMaestraType = null;
                        if (tipo == TipoDbfDict.TablaMaestra)
                            tablaMaestraType = typeof(TablaMaestra);
                        else if (tipo == TipoDbfDict.Diesel)
                            tablaMaestraType = typeof(CatTipoDiesel);
                        else if (tipo == TipoDbfDict.SubDiesel)
                            tablaMaestraType = typeof(CatSubdieselVehiculo);
                        else if (tipo == TipoDbfDict.Marca)
                            tablaMaestraType = typeof(CatMarcaVehiculo);
                        else if (tipo == TipoDbfDict.Submarca)
                            tablaMaestraType = typeof(TablaMaestraDbfCatSubMarcaVehiculoRequest);
                        else
                            throw new Exception("");
                        var tablaMaestraProperties = tablaMaestraType.GetProperties();

                        while (dbfDataReader.Read())
                        {
                            object? record = null;
                            if (tipo == TipoDbfDict.TablaMaestra)
                                record = new TablaMaestra();
                            else if (tipo == TipoDbfDict.Diesel)
                                record = new CatTipoDiesel();
                            else if (tipo == TipoDbfDict.SubDiesel)
                                record = new CatSubdieselVehiculo();
                            else if (tipo == TipoDbfDict.Marca)
                                record = new CatMarcaVehiculo();
                            else if (tipo == TipoDbfDict.Submarca)
                                record = new TablaMaestraDbfCatSubMarcaVehiculoRequest();
                            foreach (var property in tablaMaestraProperties)
                            {
                                var propertyName = property.Name;
                                var propertyType = property.PropertyType;

                                try
                                {
                                    object? value = null;
                                    if (propertyName.Equals("Verificacions"))
                                        continue;
                                    else if (propertyName.Equals("Id") || propertyName.Equals("IdCatMarcaVehiculo") || propertyName.Equals("IdCatSubmarcaVehiculo"))
                                    {
                                        var id = "";
                                        switch (propertyName)
                                        {
                                            case "Id":
                                                if (tipo == TipoDbfDict.TablaMaestra)
                                                    id = "ID";
                                                else if (tipo == TipoDbfDict.Diesel)
                                                    id = "BASEDATOS";
                                                else if (tipo == TipoDbfDict.SubDiesel || tipo == TipoDbfDict.Submarca)
                                                    break;
                                                else if (tipo == TipoDbfDict.Marca)
                                                    id = "BASEDATOS";
                                                value = dbfDataReader.GetValue(dbfDataReader.GetOrdinal(id));
                                                break;
                                            case "IdCatMarcaVehiculo":
                                                value = dbfDataReader.GetValue(dbfDataReader.GetOrdinal("MARCA"));
                                                break;
                                            case "IdCatSubmarcaVehiculo":
                                                value = dbfDataReader.GetValue(dbfDataReader.GetOrdinal("IDSUBMARCA"));
                                                break;
                                        }
                                    }
                                    else if (propertyName.Equals("Nombre") && (tipo == TipoDbfDict.Diesel || tipo == TipoDbfDict.SubDiesel || tipo == TipoDbfDict.Marca || tipo == TipoDbfDict.Submarca))
                                        value = dbfDataReader.GetValue(dbfDataReader.GetOrdinal("PANTALLA"));
                                    else if (propertyName.Equals("Clave") && (tipo == TipoDbfDict.SubDiesel))
                                        value = dbfDataReader.GetValue(dbfDataReader.GetOrdinal("BASEDATOS"));
                                    else if (propertyName.Equals("Clave") && (tipo == TipoDbfDict.Submarca))
                                        value = dbfDataReader.GetValue(dbfDataReader.GetOrdinal("IDSUBMARCA"));
                                    else if (propertyName.Equals("Alias") && (tipo == TipoDbfDict.Submarca))
                                        value = dbfDataReader.GetValue(dbfDataReader.GetOrdinal("BASEDATOS"));
                                    else
                                    {

                                        value = dbfDataReader.GetValue(dbfDataReader.GetOrdinal(propertyName.ToUpper(CultureInfo.InvariantCulture)));
                                    }

                                    if (value != null)
                                    {
                                        property.SetValue(record, value);
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            if (tipo == TipoDbfDict.TablaMaestra)
                                records.TablaMaestra.Add((TablaMaestra)record);
                            else if (tipo == TipoDbfDict.Diesel)
                                records.CatTipoDiesel.Add((CatTipoDiesel)record);
                            else if (tipo == TipoDbfDict.SubDiesel)
                                records.CatSubdieselVehiculo.Add((CatSubdieselVehiculo)record);
                            else if (tipo == TipoDbfDict.Marca)
                                records.CatMarcaVehiculo.Add((CatMarcaVehiculo)record);
                            else if (tipo == TipoDbfDict.Submarca)
                                records.CatSubMarcaVehiculo.Add((TablaMaestraDbfCatSubMarcaVehiculoRequest)record);
                        }
                    }
                    var resultRequest = await ConsultaTablaMaestraHelper.Guardar(_proxyWebAPI, records);
                    return Json(resultRequest);

                }
                catch (Exception ex)
                {
                    // Elimina el archivo temporal en caso de error
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        System.IO.File.Delete(tempFilePath);
                    }

                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al procesar el archivo.";
                }
                finally
                {
                    // Elimina el archivo temporal una vez finalizado el procesamiento
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        System.IO.File.Delete(tempFilePath);
                    }
                }
            }
            else
            {
                result.IsSuccessFully = false;
                result.Message = "No se ha cargado ningún archivo para actualizar la tabla maestra.";
            }
            return Json(result);
        }
        private void SetMenuLinkActive()
        {
            ViewBag.Tramites = "here";
            ViewBag.SolicitudFormaValorada = "";
            ViewBag.Administracion = "";
        }
    }
}
