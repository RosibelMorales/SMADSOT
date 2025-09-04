using Microsoft.AspNetCore.Mvc;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Request;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Response;
using Smadot.Models.Entities.Cita.Response;
using Smadot.Models.Entities.Verificacion.Response;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.CargaMasiva.Request;
using Smadot.Models.Entities.CargaMasiva.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.DirectorioCentrosVerificacion;
using Smadot.Web.Utilities;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Models.Entities.ConfiguracionLayout.Request;
using Smadot.Models.Entities.ConfiguracionLayout.Response;
using Smadot.Models.Dicts;
using Smadot.Utilities.Reporting.Implementacion;
using Smadot.Web.Models;
using static Smadot.Models.Entities.Proveedor.Response.ProveedorResponseData;
using System.ComponentModel;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.FoliosFormaValorada.Response;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.IdentityModel.Tokens;
using Smadot.Web.Helper.Operaciones.Catalogo;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Smadot.Web.Controllers.Tramite
{
    [Authorize]
    public class DirectorioCentrosVerificacionController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuider;
        private readonly IExcelBuilder _excelBuilder;


        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public DirectorioCentrosVerificacionController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IExcelBuilder excelBuilder)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            _excelBuilder = excelBuilder;
        }
        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuDirectorioVerificentros.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View();
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
            var request = new DirectorioCentrosVerificacionListRequest();
            request.Pagina = page;
            request.Registros = pageSize;
            request.Busqueda = searchValue;
            request.ColumnaOrdenamiento = sortColumn;
            request.Ordenamiento = sortColumnDirection;
            long i;
            if (long.TryParse(Request.Form["idAlmacen"].FirstOrDefault(), out i)) request.IdAlmacen = i;
            else request.IdAlmacen = null;

            var listaResult = await DirectorioCentrosVerificacionHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<DirectorioCentrosVerificacionResponse> ?? new List<DirectorioCentrosVerificacionResponse>();

            ////Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total.Value;

            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<DirectorioCentrosVerificacionResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new DirectorioCentrosVerificacionResponseGrid
                {
                    Id = row.Id,
                    Nombre = row.Nombre,
                    Rfc = row.Rfc,
                    Clave = row.Clave,
                    Telefono = row.Telefono,
                    Correo = row.Correo,
                    Longitud = row.Longitud,
                    Latitud = row.Latitud,
                    RepresentanteLegal = row.RepresentanteLegal,

                    //Acciones = await this.RenderViewToStringAsync("~/Views/SolicitudFormaValorada/_Acciones.cshtml", row)
                    Acciones = await this.RenderViewToStringAsync("~/Views/DirectorioCentrosVerificacion/_Acciones.cshtml", row)
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
        public async Task<IActionResult> ConfiguradorCitas(long id, string nombre)
        {
            ViewBag.FileCitas = HttpContext.Session.TryGetValue("FileCitasXlsx", out _);
            ViewBag.IdCVV = id;
            ViewBag.Nombre = nombre;
            return View("~/Views/ConfiguradorCita/Index.cshtml");
        }
        [HttpPost]
        public async Task<JsonResult> ConsultaFechas(long id)
        {
            JsonResult result = new JsonResult("");
            //Invoca al método que se encarga de realizar la petición Api
            var request = new ConfiguradorCitaRequestList();
            request.IdCVV = id;
            int totalRecord = 0;
            int filterRecord = 0;

            //var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

            var listaResult = await DirectorioCentrosVerificacionHelper.ConsultaFechas(_proxyWebAPI, request);

            var data = listaResult.Result as ResponseGrid<ConfiguradorCitaResponse?> ?? new ResponseGrid<ConfiguradorCitaResponse?>() { Data = new() };
            data.Data.ForEach(async x =>
            {
                x.Acciones = await this.RenderViewToStringAsync("~/Views/ConfiguradorCita/_Acciones.cshtml", x);
            });
            return Json(data);
        }
        public async Task<IActionResult> DetallesVerificentro(long? id)
        {
            var result = new ResponseViewModel(true);
            var vm = new List<DirectorioCentrosVerificacionResponse>
            {
                new DirectorioCentrosVerificacionResponse
                {
                    Id = 0
                }
            };
            try
            {
                var response = await DirectorioCentrosVerificacionHelper.Consulta(_proxyWebAPI, id ?? 0);
                vm = response.Result as List<DirectorioCentrosVerificacionResponse> ?? vm;

            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> CargarHorarios(long id, string nombre)
        {
            ViewBag.IdCVV = id;
            ViewBag.Nombre = nombre;
            return View("~/Views/ConfiguradorCita/CargaHorarios.cshtml");
        }
        [HttpGet]
        public async Task<ActionResult> ConfiguradorCita(long id)
        {
            var result = new ResponseViewModel(true);

            try
            {
                //var res = new JsonResultDto();
                var response = await DirectorioCentrosVerificacionHelper.GetConfiguradorByCVV(_proxyWebAPI, id);
                var data = response.Result as ConfiguradorCitaResponse ?? new ConfiguradorCitaResponse();
                result.IsSuccessFully = response.IsSuccessFully;
                result.Message = response.Message;
                result.Result = new
                {
                    html = await this.RenderViewToStringAsync("~/Views/ConfiguradorCita/_Registro.cshtml", data),
                    data.FechaStr
                };
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<JsonResult> ConfiguradorCita(List<ConfiguradorCitaDiasRequest> request)
        {
            var result = new ResponseViewModel();
            try
            {
                var currenteDate = DateTime.Now.Date;
                var existeRegistro = request.Any(x => x.Fecha <= currenteDate && x.Habilitado);
                if (existeRegistro)
                {
                    result.Message = "Una o más fechas son anteriores a la fecha actual.";
                    return new JsonResult(result);
                }
                existeRegistro = request.Any(x => x.IntervaloMin < 5 && x.Habilitado);
                if (existeRegistro)
                {
                    result.Message = "Uno o más intervalos de tiempo no so válidos.";
                    return new JsonResult(result);
                }
                existeRegistro = request.Any(x => x.Capacidad < 1 && x.Habilitado);
                if (existeRegistro)
                {
                    result.Message = "La capacidad de citas de uno o más registros son menores a uno.";
                    return new JsonResult(result);
                }

                existeRegistro = request.Any(x => x.HoraFin < x.HoraInicio && x.Habilitado);
                if (existeRegistro)
                {
                    result.Message = "El hora de finalización de citas de uno o más registros es menor a la de Inicio.";
                    return new JsonResult(result);
                }

                var helper = await DirectorioCentrosVerificacionHelper.ConfiguradorCita(_proxyWebAPI, request);

                if (!helper.IsSuccessFully)
                {
                    result.Message = helper.Message;
                    return new JsonResult(result);
                }

                return new JsonResult(helper);
            }
            catch (Exception ex)
            {
                result.Message = "Ocurrió un error al procesar la información.";

            }
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<JsonResult> EdicionFecha(ConfiguradorCitaEditRequest request)
        {
            var result = new ResponseViewModel();
            try
            {

                var helper = await DirectorioCentrosVerificacionHelper.ConfiguradorCita(_proxyWebAPI, request);
                if (!helper.IsSuccessFully || helper.Result == null)
                {
                    result.Message = helper.Message;
                    //result.Result = helper.Result;
                    return new JsonResult(result);
                }

                var resultRequest = helper.Result as EditFechaResponse;
                bool procesoValido = !resultRequest.Error;
                string mensajError = helper.Message;

                return new JsonResult(new
                {
                    IsSuccessFully = procesoValido,
                    Message = mensajError,
                    Result = new
                    {
                        resultRequest.Modificar
                    }
                }
                );

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }


        [HttpGet]
        public async Task<IActionResult> ConfiguracionLayout()
        {
            var result = new ResponseViewModel(true);
            try
            {
                if (!_userResolver.HasPermission(MenuConfiguracion.PermisoMenuPadre))
                    throw new Exception("No tiene autorización.");
                var response = await DirectorioCentrosVerificacionHelper.ConfiguracionLayout(_proxyWebAPI);
                if (!response.IsSuccessFully)
                    throw new Exception(response.Message);
                result.Result = await this.RenderViewToStringAsync("~/Views/Shared/_ConfiguracionLayout.cshtml", response.Result);
                //result.Result = View("~/Views/Shared/ConfiguracionLayout.cshtml", response.Result);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> ConfiguracionLayout(List<ConfiguracionLayoutResponse> registro)
        {
            var result = new ResponseViewModel(true);
            try
            {
                registro = registro.Where(x => x.Selected).ToList();
                foreach (var i in registro)
                {
                    i.Verificentro = "";
                    i.CatTipoCertificado = "";
                }
                var save = await DirectorioCentrosVerificacionHelper.ConfiguracionLayout(_proxyWebAPI, registro);
                if (!save.IsSuccessFully)
                    throw new Exception(save.Message);

                result.Message = "La información se guardó correctamente.";
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message.Contains("El folio ingresado debe ser mayor.") ? "El folio ingresado debe ser mayor." : ex.Message;
            }

            return new JsonResult(result);
        }
        [HttpGet]
        public FileResult DescargarReporte()
        {
            var fileName = HttpContext.Session.GetString("FileCitaName");
            var bytes = HttpContext.Session.Get("FileCitasXlsx");

            HttpContext.Session.Remove("FileCitaName");
            HttpContext.Session.Remove("FileCitasXlsx");

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<JsonResult> RegistroCVV(long? id)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var vm = new NuevoVerificentroResponse();
                if (id.HasValue)
                {
                    var response = await DirectorioCentrosVerificacionHelper.EditCVV(_proxyWebAPI, id.Value);
                    if (!response.IsSuccessFully)
                    {
                        result.IsSuccessFully = false;
                        result.Message = "No se encontró información del Equipo";
                        return new JsonResult(result);
                    }
                    var data = response.Result as NuevoVerificentroResponse ?? new NuevoVerificentroResponse();
                    vm.Id = data.Id ?? 0;
                    vm.Nombre = data.Nombre;
                    vm.Clave = data.Clave;
                    vm.Direccion = data.Direccion;
                    vm.Rfc = data.Rfc;
                    vm.Telefono = data.Telefono;
                    vm.Correo = data.Correo;
                    vm.GerenteTecnico = data.GerenteTecnico;
                    vm.RepresentanteLegal = data.RepresentanteLegal;
                    vm.Longitud = data.Longitud;
                    vm.Latitud = data.Latitud;
                    vm.DirectorGestionCalidadAire = data.DirectorGestionCalidadAire;
                    vm.Municipio = data.Municipio;
                    vm.Activo = data.Activo;
                    vm.ApiEndPoint = data.ApiEndPoint;
                    vm.ApiKey = data.ApiKey;

                }
                result.Result = await this.RenderViewToStringAsync("~/Views/DirectorioCentrosVerificacion/_RegistroCVV.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> RegistroCVV(NuevoVerificentroResponse request)
        {
            var result = new JsonResultDto();
            try
            {
                if (request.Id == 0)
                {
                    request.Activo = true;
                    request.ApiEndPoint = "pendiente";
                    request.ApiKey = "pendiente";
                }
                var save = await DirectorioCentrosVerificacionHelper.RegistroCVV(_proxyWebAPI, request);
                result.Error = !save.IsSuccessFully;
                result.ErrorDescription = save.Message;
                return Json(result);
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }

            return Json(result);
        }

        public IActionResult DescargarPlantilla()
        {

            // Ruta del archivo Excel dentro de la carpeta "Plantillas" en la raíz del proyecto
            string rutaArchivoExcel = Path.Combine(Directory.GetCurrentDirectory(), "Plantilla", "PlantillaBD.xlsx");
            // Verificar si el archivo existe
            if
            (!System.IO.File.Exists(rutaArchivoExcel))
            {
                return
                NotFound();
                // Devolver 404 si el archivo no existe
            }
            // Devolver el archivo Excel como una respuesta HTTP
            return
            File(System.IO.File.ReadAllBytes(rutaArchivoExcel),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            ,
            "PlantillaBD.xlsx"
            );
        }

        [HttpPost]
        public async Task<IActionResult> CargaMasiva(IFormFile file)
        {
            var result = new ResponseViewModel(true);

            if (file != null && file.Length > 0)
            {

                try
                {
                    using var stream = new MemoryStream();
                    file.CopyTo(stream);
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    using var package = new ExcelPackage(stream);
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.End.Row;
                    int colCount = worksheet.Dimension.Columns;
                    var list = new List<CargaMasivaExcelResponse>();
                    var errorList = new List<string>();
                    var errorMessageBase = "Ocurrió un error en la fila {0}, columna {1}. Detalle: {2}";
                    // Agregar una columna de Estado
                    var statusColumn = colCount + 1;
                    worksheet.Cells[2, statusColumn].Value = "Estado";
                    // Agregar una columna para errores
                    var errorsColumn = colCount + 2;
                    worksheet.Cells[2, errorsColumn].Value = "Errores";
                    worksheet.Column(errorsColumn).Width = 150;


                    for (int row = 3; row <= rowCount; row++)
                    {
                        var carga = new CargaMasivaExcelResponse();
                        string errorMessage = "";

                        if (string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.Nombre].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.RazonSocial].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.Correo].Value?.ToString()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.FechaCita].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.IdCVV].Value?.ToString()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.Placa].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.IdMarca].Value?.ToString()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.IdSubMarca].Value?.ToString()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.Anio].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.VIN].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.Color].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.Estado].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.Poblano].Value?.ToString()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.IdCatTipoCertificado].Value?.ToString()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.ImporteActual].Value?.ToString()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.FechaRegistro].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.ConsecutivoTramite].Value?.ToString()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.FechaEmisionRef].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.FechaPago].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.FolioCertificado].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.Vigencia].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.Marca].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.Modelo].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.Combustible].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.TarjetaCirculacion].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.ClaveLinea].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.NumeroReferencia].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.Semestre].Value?.ToString()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.AnioVerificacion].Value?.ToString()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.FechaVerificacion].Value?.ToString()?.Trim()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.IdCatTipoCertificado].Value?.ToString()) &&
                                                            string.IsNullOrEmpty(worksheet.Cells[row, (int)ColumnImportadorExcelCargaEnum.IdMotivoVerificacion].Value?.ToString()))
                            continue;
                        ExcelRange? celda = null;
                        string? celdaValor = null;
                        var rowErrorList = new List<string>();
                        foreach (ColumnImportadorExcelCargaEnum columna in System.Enum.GetValues(typeof(ColumnImportadorExcelCargaEnum)))
                        {
                            celda = worksheet.Cells[row, (int)columna];
                            celdaValor = (celda?.Value ?? "")?.ToString()?.Trim();
                            var columnaName = CargaMasivaDic.Dictionary[columna];
                            //if(string.IsNullOrEmpty(celdaValor) && columna != )
                            #region Validaciones Celdas
                            #region Validacion Nombre
                            if (columna == ColumnImportadorExcelCargaEnum.Nombre)
                            {
                                if (!string.IsNullOrEmpty(celdaValor))
                                {
                                    if (celdaValor?.Length > 3 && EsAlfanumericoEspacios(celdaValor))
                                    {
                                        carga.Nombre = celdaValor;
                                    }
                                    else if (celdaValor?.Length > 3 && !EsAlfanumericoEspacios(celdaValor))
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "El valor contiene caracterés especiales");
                                        rowErrorList.Add(errorMessage);
                                    }
                                    else
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "No se estableció ningún valor o el valor es muy corto");
                                        rowErrorList.Add(errorMessage);
                                    }
                                }
                            }
                            #endregion
                            #region Validacion Razon Social
                            if (columna == ColumnImportadorExcelCargaEnum.RazonSocial)
                            {
                                if (!string.IsNullOrEmpty(celdaValor))
                                {

                                    if (celdaValor?.Length > 3 && EsAlfanumericoEspacios(celdaValor))
                                    {
                                        carga.RazonSocial = celdaValor;
                                    }
                                    else if (celdaValor?.Length > 3 && !EsAlfanumericoEspacios(celdaValor))
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "El valor contiene caracterés especiales");
                                        rowErrorList.Add(errorMessage);
                                    }
                                    else
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "No se estableció ningún valor o el valor es muy corto");
                                        rowErrorList.Add(errorMessage);
                                    }
                                }
                                else if (string.IsNullOrEmpty(carga.Nombre))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "Sedebe establecer una razón social o nombre del propietario.");
                                    rowErrorList.Add(errorMessage);
                                }

                            }
                            #endregion
                            #region Validacion Correo
                            if (columna == ColumnImportadorExcelCargaEnum.Correo)
                            {
                                if (celdaValor?.Length > 3 && EsCorreoEelectronico(celdaValor))
                                {
                                    carga.Correo = celdaValor;
                                }
                                else if (celdaValor?.Length > 3 && !EsCorreoEelectronico(celdaValor))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "El valor no contiene el formato de correo electrónico");
                                    rowErrorList.Add(errorMessage);
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion FechaCita
                            if (columna == ColumnImportadorExcelCargaEnum.FechaCita)
                            {
                                if (celdaValor?.Length > 3)
                                {
                                    // TODO: Corregir formato de fecha
                                    var parseDate = DateTime.TryParseExact(celdaValor, "yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateParsed);
                                    if (!parseDate)
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "La fecha no tiene un formato correcto");
                                        rowErrorList.Add(errorMessage);
                                    }
                                    carga.Fecha = dateParsed;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion IdCVV
                            if (columna == ColumnImportadorExcelCargaEnum.IdCVV)
                            {
                                if (int.TryParse(celdaValor, out int idCvv))
                                {
                                    if (System.Enum.IsDefined(typeof(VerificentroEnum), idCvv))
                                    {

                                        carga.IdCVV = idCvv;
                                    }
                                    else
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "El Verificentro no existe en el catálogo actualmente");
                                        rowErrorList.Add(errorMessage);
                                    }
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció un valor válido. Debe ser un número entero");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Placa
                            if (columna == ColumnImportadorExcelCargaEnum.Placa)
                            {
                                if (celdaValor?.Length > 3 && EsAlfanumerico(celdaValor))
                                {
                                    carga.Placa = celdaValor;
                                }

                                else if (celdaValor?.Length > 3 && !EsAlfanumerico(celdaValor))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                   "El valor contiene caracterés especiales");
                                    rowErrorList.Add(errorMessage);
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion IdMarca
                            if (columna == ColumnImportadorExcelCargaEnum.IdMarca)
                            {
                                if (int.TryParse(celdaValor, out int IdMarca))
                                {
                                    carga.IdCatMarcaVehiculo = IdMarca;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció un valor válido. Debe ser un número entero");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion IdSubmarca
                            if (columna == ColumnImportadorExcelCargaEnum.IdSubMarca)
                            {
                                if (int.TryParse(celdaValor, out int IdSubmarca))
                                {
                                    carga.IdSubMarcaVehiculo = IdSubmarca;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció un valor válido. Debe ser un número entero");
                                    rowErrorList.Add(errorMessage);
                                }

                            }
                            #endregion
                            #region Validacion Anio
                            if (columna == ColumnImportadorExcelCargaEnum.Anio)
                            {
                                if (short.TryParse(celdaValor, out short anio))
                                {
                                    carga.Anio = anio;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció un valor válido. Debe ser un número entero");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion VIN
                            if (columna == ColumnImportadorExcelCargaEnum.VIN)
                            {

                                if (celdaValor?.Length >= 6 && celdaValor?.Length <= 17 && EsAlfanumerico(celdaValor))
                                {
                                    carga.Serie = celdaValor;
                                }
                                else if (celdaValor != null && celdaValor?.Length != 17)
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                "El valor no tiene un longitud correcta");

                                    rowErrorList.Add(errorMessage);
                                }
                                else if (celdaValor?.Length > 3 && !EsAlfanumerico(celdaValor))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                   "El valor contiene caracterés especiales.");
                                    rowErrorList.Add(errorMessage);
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto.");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Color
                            if (columna == ColumnImportadorExcelCargaEnum.Color)
                            {
                                if (celdaValor?.Length > 3 && SoloLetras(celdaValor))
                                {
                                    carga.ColorVehiculo = celdaValor;
                                }
                                else if (celdaValor?.Length > 3 && !SoloLetras(celdaValor))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                   "El valor contiene caracterés especiales o números.");
                                    rowErrorList.Add(errorMessage);
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto.");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Estado
                            if (columna == ColumnImportadorExcelCargaEnum.Estado)
                            {
                                if (celdaValor?.Length > 3 && EsAlfanumericoEspacios(celdaValor) && EstadosList.Estados.Contains(celdaValor))
                                {
                                    carga.Estado = celdaValor;
                                }
                                else if (celdaValor?.Length > 3 && !EsAlfanumericoEspacios(celdaValor))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "El valor contiene caracterés no permitidos");
                                    rowErrorList.Add(errorMessage);
                                }
                                else if (celdaValor?.Length > 3 && EsAlfanumericoEspacios(celdaValor) && !EstadosList.Estados.Contains(celdaValor))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "El estado no esta dentro de los valores permitidos");
                                    rowErrorList.Add(errorMessage);
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto.");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Poblano
                            if (columna == ColumnImportadorExcelCargaEnum.Poblano)
                            {
                                if (bool.TryParse(celdaValor, out bool poblano))
                                {
                                    carga.Poblano = poblano;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció un valor válido. Debe ser un true o false");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Tipo Combustible
                            if (columna == ColumnImportadorExcelCargaEnum.IdCatTipoCombustible)
                            {
                                if (int.TryParse(celdaValor, out int idCatTipoCombustible))
                                {
                                    if (System.Enum.IsDefined(typeof(TipoCombustibleEnum), idCatTipoCombustible))
                                    {

                                        carga.IdTipoCombustible = idCatTipoCombustible;
                                    }
                                    else
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "El tipo de combustible no existe en el catálogo actualmente");
                                        rowErrorList.Add(errorMessage);
                                    }
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció un valor válido. Debe ser un número entero");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Importe Actual
                            if (columna == ColumnImportadorExcelCargaEnum.ImporteActual)
                            {
                                if (decimal.TryParse(celdaValor, out decimal importeActual))
                                {
                                    carga.ImporteActual = importeActual;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció un valor válido. Debe ser un número decimal");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Fecha Registro
                            if (columna == ColumnImportadorExcelCargaEnum.FechaRegistro)
                            {
                                if (celdaValor?.Length > 3)
                                {
                                    var parseDate = DateTime.TryParseExact(celdaValor, "yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateParsed);
                                    if (!parseDate)
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "La fecha no tiene un formato correcto");
                                        rowErrorList.Add(errorMessage);
                                    }
                                    carga.FechaRegistro = dateParsed;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion ConsecutivoTramite
                            if (columna == ColumnImportadorExcelCargaEnum.ConsecutivoTramite)
                            {
                                if (int.TryParse(celdaValor, out int consecutivoTramite))
                                {
                                    carga.ConsecutivoTramite = consecutivoTramite;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció un valor válido. Debe ser un número decimal");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Fecha Emision Ref
                            if (columna == ColumnImportadorExcelCargaEnum.FechaEmisionRef)
                            {
                                if (celdaValor?.Length > 3)
                                {
                                    var parseDate = DateTime.TryParseExact(celdaValor, "yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateParsed);
                                    if (!parseDate)
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "La fecha no tiene un formato correcto");
                                        rowErrorList.Add(errorMessage);
                                    }
                                    carga.FechaEmisionRef = dateParsed;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Fecha Pago
                            if (columna == ColumnImportadorExcelCargaEnum.FechaPago)
                            {
                                if (celdaValor?.Length > 3)
                                {
                                    var parseDate = DateTime.TryParseExact(celdaValor, "yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateParsed);
                                    if (!parseDate)
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "La fecha no tiene un formato correcto");
                                        rowErrorList.Add(errorMessage);
                                    }
                                    carga.FechaPago = dateParsed;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Folio Certificado
                            if (columna == ColumnImportadorExcelCargaEnum.FolioCertificado)
                            {
                                if (long.TryParse(celdaValor, out long folioCertificado))
                                {
                                    carga.FolioCertificado = folioCertificado;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció un valor válido. Debe ser un número entero");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Vigencia
                            if (columna == ColumnImportadorExcelCargaEnum.Vigencia)
                            {
                                if (celdaValor?.Length > 3)
                                {
                                    // TODO: Corregir formato de fecha
                                    var parseDate = DateTime.TryParseExact(celdaValor, "yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateParsed);
                                    if (!parseDate)
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "La fecha no tiene un formato correcto");
                                        rowErrorList.Add(errorMessage);
                                    }
                                    carga.Vigencia = dateParsed;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Marca
                            if (columna == ColumnImportadorExcelCargaEnum.Marca)
                            {
                                if (celdaValor?.Length >= 2 && EsAlfanumericoEspaciosEspeciales(celdaValor))
                                {
                                    carga.Marca = celdaValor;
                                }
                                else if (celdaValor?.Length >= 2 && !EsAlfanumericoEspaciosEspeciales(celdaValor))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "El valor contiene caracterés no permitidos.");
                                    rowErrorList.Add(errorMessage);
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Modelo
                            if (columna == ColumnImportadorExcelCargaEnum.Modelo)
                            {
                                if (celdaValor?.Length >= 2 && EsAlfanumericoEspaciosEspeciales(celdaValor))
                                {
                                    carga.Modelo = celdaValor;
                                }
                                else if (celdaValor?.Length >= 2 && !EsAlfanumericoEspaciosEspeciales(celdaValor))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "El valor contiene caracterés especiales");
                                    rowErrorList.Add(errorMessage);
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Combustible
                            if (columna == ColumnImportadorExcelCargaEnum.Combustible)
                            {
                                if (celdaValor?.Length >= 3 && EsLetrasEspacios(celdaValor))
                                {
                                    carga.Combustible = celdaValor;
                                }
                                else if (celdaValor?.Length >= 3 && !EsLetrasEspacios(celdaValor))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "El valor contiene caracterés especiales o números");
                                    rowErrorList.Add(errorMessage);
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion 
                            #region Validacion TarjetaCirculacion
                            if (columna == ColumnImportadorExcelCargaEnum.TarjetaCirculacion)
                            {
                                if (celdaValor?.Length > 3 && EsAlfanumerico(celdaValor))
                                {
                                    carga.TarjetaCirculacion = celdaValor;
                                }
                                else if (celdaValor?.Length > 3 && !EsAlfanumerico(celdaValor))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "El valor contiene caracterés especiales");
                                    rowErrorList.Add(errorMessage);
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion ClaveLinea
                            if (columna == ColumnImportadorExcelCargaEnum.ClaveLinea)
                            {
                                if (ClaveLineaList.ClaveLineas.Contains(celdaValor))
                                {
                                    carga.ClaveLinea = celdaValor;
                                }
                                else if (!ClaveLineaList.ClaveLineas.Contains(celdaValor))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "El valor contiene una linea incorrecta");
                                    rowErrorList.Add(errorMessage);
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion NumeroReferencia
                            if (columna == ColumnImportadorExcelCargaEnum.NumeroReferencia)
                            {
                                if (celdaValor?.Length > 3 && SoloNumeros(celdaValor))
                                {
                                    carga.NumeroReferencia = celdaValor;
                                }
                                else if (celdaValor?.Length >= 1 && !SoloNumeros(celdaValor))
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "El valor contiene caracterés especiales");
                                    rowErrorList.Add(errorMessage);
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Semestre
                            if (columna == ColumnImportadorExcelCargaEnum.Semestre)
                            {
                                if (int.TryParse(celdaValor, out int semestre))
                                {
                                    carga.Semestre = semestre;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció un valor válido. Debe ser un número entero");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion AnioVerificacion
                            if (columna == ColumnImportadorExcelCargaEnum.AnioVerificacion)
                            {
                                if (celdaValor?.Length == 4)
                                {
                                    if (int.TryParse(celdaValor, out int consecutivoTramite))
                                    {
                                        carga.AnioVerificacion = consecutivoTramite;
                                    }
                                    else
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "No se estableció un valor válido. Debe ser un número decimal");
                                        rowErrorList.Add(errorMessage);
                                    }
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion Fecha Verificacion
                            if (columna == ColumnImportadorExcelCargaEnum.FechaVerificacion)
                            {
                                if (celdaValor?.Length > 3)
                                {
                                    var parseDate = DateTime.TryParseExact(celdaValor, "yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateParsed);
                                    if (!parseDate)
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "La fecha no tiene un formato correcto");
                                        rowErrorList.Add(errorMessage);
                                    }
                                    carga.FechaVerificacion = dateParsed;
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció ningún valor o el valor es muy corto");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion TipoCertificado
                            if (columna == ColumnImportadorExcelCargaEnum.IdCatTipoCertificado)
                            {
                                if (int.TryParse(celdaValor, out int idCatTipoCertificado))
                                {
                                    if (System.Enum.IsDefined(typeof(TipoCertificadoEnum), idCatTipoCertificado))
                                    {

                                        carga.IdTipoCertificado = idCatTipoCertificado;
                                    }
                                    else
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "El Certificado no existe en el catálogo actualmente");
                                        rowErrorList.Add(errorMessage);
                                    }
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció un valor válido. Debe ser un número entero");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #region Validacion MotivoVerificacion
                            if (columna == ColumnImportadorExcelCargaEnum.IdMotivoVerificacion)
                            {
                                if (int.TryParse(celdaValor, out int idMotivoVerificacion))
                                {
                                    if (System.Enum.IsDefined(typeof(MotivoVerificacionEnum), idMotivoVerificacion))
                                    {

                                        carga.IdMotivoVerificacion = idMotivoVerificacion;
                                    }
                                    else
                                    {
                                        errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                        "El Motivo de Verificación no existe en el catálogo actualmente");
                                        rowErrorList.Add(errorMessage);
                                    }
                                }
                                else
                                {
                                    errorMessage = string.Format(errorMessageBase, row, columnaName,
                                                                    "No se estableció un valor válido. Debe ser un número entero");
                                    rowErrorList.Add(errorMessage);
                                }
                            }
                            #endregion
                            #endregion
                        }
                        if (rowErrorList.Count != 0)
                        {
                            worksheet.Cells[row, statusColumn].Value = "Hay Errores";
                            worksheet.Cells[row, statusColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, statusColumn].Style.Fill.BackgroundColor.SetColor(Color.LightCoral); // Rojo bajito para errores
                            worksheet.Cells[row, errorsColumn].Value = string.Join(",\n", rowErrorList);
                            worksheet.Cells[row, errorsColumn].Style.WrapText = true;
                            worksheet.Cells[row, errorsColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, errorsColumn].Style.Fill.BackgroundColor.SetColor(Color.LightCoral); // Rojo bajito para errores
                            errorList.AddRange(rowErrorList);
                        }
                        list.Add(carga);
                    }
                    if (errorList.Any())
                    {
                        byte[] bytes = package.GetAsByteArray();

                        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file.FileName);
                    }
                    var resultRequest = await DirectorioCentrosVerificacionHelper.CargaMasiva(_proxyWebAPI, new() { Lista = list, FileName = file.FileName, FileBase64 = Convert.ToBase64String(stream.ToArray())});
                    return Json(resultRequest);
                }
                catch (ValidationException e)
                {

                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al procesar el archivo.";
                }
                catch (Exception e)
                {

                    result.IsSuccessFully = false;
                    result.Message = "Ocurrió un error al procesar el archivo.";
                }
            }
            else
            {
                result.IsSuccessFully = false;
                result.Message = "No se ha cargado ningún archivo para realizar la carga masiva";
            }
            return Json(result);
        }


        #region Private Methods
        private static bool EsAlfanumericoEspacios(string cadena)
        {
            // Expresión regular para detectar caracteres no alfanuméricos
            Regex patron = new("^[a-zA-Z0-9 ñÑ.]+$");
            // Verificar si la cadena contiene algún carácter no alfanumérico
            return patron.IsMatch(cadena);
        }
        private static bool EsLetrasEspacios(string cadena)
        {
            // Expresión regular para detectar caracteres no alfanuméricos
            Regex patron = new("^[a-zA-Z0-9 ]+$");
            // Verificar si la cadena contiene algún carácter no alfanumérico
            return patron.IsMatch(cadena);
        }

        private static bool EsAlfanumericoEspaciosEspeciales(string cadena)
        {
            // Expresión regular para detectar caracteres no alfanuméricos
            Regex patron = new("^[a-zA-Z0-9 ñÑ &(),.+/_-]+$");
            // Verificar si la cadena contiene algún carácter no alfanumérico
            return patron.IsMatch(cadena);
        }

        private static bool EsCorreoEelectronico(string cadena)
        {
            try
            {
                new MailAddress(cadena);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool EsAlfanumerico(string cadena)
        {
            // Expresión regular para detectar caracteres no alfanuméricos
            Regex patron = new("^[a-zA-Z0-9]+$");
            // Verificar si la cadena contiene algún carácter no alfanumérico
            return patron.IsMatch(cadena);
        }

        private static bool SoloNumeros(string cadena)
        {
            // Expresión regular para detectar caracteres no alfanuméricos
            Regex patron = new("^[0-9]+$");

            // Verificar si la cadena contiene algún carácter no alfanumérico
            return patron.IsMatch(cadena);
        }

        private static bool SoloLetras(string cadena)
        {
            // Expresión regular para detectar caracteres no alfanuméricos
            Regex patron = new("^[a-zA-Z]+$");

            // Verificar si la cadena contiene algún carácter no alfanumérico
            return patron.IsMatch(cadena);
        }

        private static bool SoloFecha(string cadena)
        {
            // Expresión regular para detectar caracteres no alfanuméricos
            Regex patron = new("^([0-2][0-9]|3[0-1])(\\/|-)(0[1-9]|1[0-2])\\2(\\d{4})$");

            // Verificar si la cadena contiene algún carácter no alfanumérico
            return !patron.IsMatch(cadena);
        }

        private static bool SoloNumeroDecimales(string cadena)
        {
            Regex patron = new("^[0-9]+([,][0-9]+)?$");

            // Verificar si la cadena contiene algún carácter no alfanumérico
            return !patron.IsMatch(cadena);
        }
        #endregion
    }
}
