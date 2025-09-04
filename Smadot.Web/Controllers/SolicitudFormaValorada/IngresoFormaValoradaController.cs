using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.POIFS.Crypt.Dsig;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Caja.Response;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.IngresoFormaValorada.Request;
using Smadot.Models.Entities.IngresoFormaValorada.Response;
using Smadot.Models.Entities.Personal.Extensions;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.Caja;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Helper.Operaciones.IngresoFormaValorada;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Web.Utilities;

namespace Smadot.Web.Controllers.SolicitudFormaValorada
{
    [Authorize]
    public class IngresoFormaValoradaController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public IngresoFormaValoradaController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        //TABLA PRINCIPAL
        public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(PermisosMenu.IngresodeFormasValoradas))
                return new UnauthorizedResult();
            var catalogo = await AlmacenHelper.Consulta(_proxyWebAPI, new AlmacenRequest());
            var list = (catalogo.Result as List<AlmacenResponse> ?? new List<AlmacenResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
            if (catalogo.AccesoTotalVerificentros)
                list.Insert(0, new SelectListItem { Text = "TODOS" });
            ViewBag.Almacenes = list;
            return View("~/Views/IngresoFormaValorada/Index.cshtml");
        }

        //Llenado de la tabla
        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");

            int totalRecord;
            int filterRecord;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var page = skip / pageSize + 1;

            //Invoca al método que se encarga de realizar la petición Api
            var request = new SolicitudFormaValoradaListRequest
            {
                Pagina = page,
                Registros = pageSize,
                Busqueda = searchValue,
                ColumnaOrdenamiento = sortColumn,
                Ordenamiento = sortColumnDirection
            };
            if (long.TryParse(Request.Form["idAlmacen"].FirstOrDefault(), out long i)) request.IdAlmacen = i;
            var listaResult = await IngresoFormaValoradaHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<SolicitudFormaValoradaResponse> ?? new List<SolicitudFormaValoradaResponse>();

            ////Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total.Value;

            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<SolicitudFormaValoradaResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new SolicitudFormaValoradaResponseGrid
                {
                    IdSolicitudFV = row.IdSolicitudFV,
                    IdIngresoFV = row.IdIngresoFV,
                    FechaSolicitudFV = row.FechaSolicitudFV,
                    ClaveCertificadoSC = row.ClaveCertificadoSC,
                    IdUserSolicitaFV = row.IdUserSolicitaFV,
                    UserSolicitaFV = row.UserSolicitaFV,
                    IdCatEstatusSolicitudFV = row.IdCatEstatusSolicitudFV,
                    EstatusFV = row.EstatusFV,
                    ActivoFV = row.ActivoFV,
                    FechaRegistroFV = row.FechaRegistroFV,
                    IdAlmacenFV = row.IdAlmacenFV,
                    AlmacenFV = row.AlmacenFV,
                    IdSC = row.IdSC,
                    IdCatTipoCertificadoSC = row.IdCatTipoCertificadoSC,
                    TipoCertificadoSC = row.TipoCertificadoSC,
                    CantidadSC = row.CantidadSC,
                    FolioInicialSC = row.FolioInicialSC,
                    FolioFinalSC = row.FolioFinalSC,
                    FechaEntregaIFV = row.FechaEntregaIFV,
                    NombreRecibioIFV = row.NombreRecibioIFV,
                    HasCaja = row.HasCaja,
                    Acciones = await this.RenderViewToStringAsync("~/Views/IngresoFormaValorada/_Acciones.cshtml", row)
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

        //formulario de Ingresos de Forma Valorada
        [HttpGet]
        public async Task<IActionResult> Ingresos(long? id)
        {
            if (!_userResolver.HasPermission(MenuIngresoFormasValoradas.RegistrarIngreso))
                return new UnauthorizedResult();
            var result = new ResponseViewModel(true);
            ViewBag.Detail = false;
            ViewBag.isDetail = "Registro";
            ViewBag.TotalRecibido = 0;
            var vm = new List<SolicitudFormaValoradaResponse>
            {
                new SolicitudFormaValoradaResponse
                {
                    IdSolicitudFV = 0
                }
            };
            try
            {
                var response = await IngresoFormaValoradaHelper.Consulta(_proxyWebAPI, id ?? 0);
                vm = response.Result as List<SolicitudFormaValoradaResponse> ?? vm;

            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return View(vm);
        }
        //formulario de Ingresos de Forma Valorada
        [HttpGet]
        public async Task<IActionResult> EditIngresos(long? id, long idSolicitudFV)
        {
            try
            {
                if (!_userResolver.HasPermission(MenuIngresoFormasValoradas.RegistrarIngreso))
                    return new UnauthorizedResult();
                var result = new ResponseViewModel(true);
                var response = await IngresoFormaValoradaHelper.Consulta(_proxyWebAPI, idSolicitudFV);
                var listSolicitud = response.Result as List<SolicitudFormaValoradaResponse> ?? new List<SolicitudFormaValoradaResponse>();
                listSolicitud = listSolicitud.Where(x => x.IdCatTipoCertificadoSC != 0).ToList();
                var certificadosResponse = await IngresoFormaValoradaHelper.GetIngresoCertificadoById(_proxyWebAPI, id ?? 0);
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
                    IdIngresoFV = id ?? 0,
                    certificados = listSolicitud.Select(x => new CertificadoGrupo
                    {
                        IdCatTipoCertificado = x.IdCatTipoCertificadoSC,
                        Certificados = certificadosIngreso.Where(c => c.IdCatTipoCertificado == x.IdCatTipoCertificadoSC).Select(x => new CertificadoData
                        {
                            IdIngresoCertificado = x.Id,
                            CantidadRecibida = x.CantidadRecibida,
                            IdCatTipoCertificado = x.IdCatTipoCertificado,
                            FolioInicial = x.FolioInicial,
                            FolioFinal = x.FolioFinal
                        }).ToList(),
                    }).OrderBy(x => x.Clave).ToList(),
                    certificadosTotalData = listSolicitud.Select(x => new CertificadosTotalData
                    {
                        CantidadRecibida = certificadosIngreso.Where(ci => ci.IdCatTipoCertificado == x.IdCatTipoCertificadoSC).Sum(ci => ci.CantidadRecibida),
                        CantidadSolicitada = x.CantidadSC,
                        IdCatTipoCertificado = x.IdCatTipoCertificadoSC
                    }).OrderBy(x => x.Clave).ToList(),
                };
                foreach (var grupocertificado in vm.certificados)
                {
                    var existeVacio = grupocertificado.Certificados.Any(x => x.CantidadRecibida == 0);
                    if (!existeVacio)
                    {
                        grupocertificado.Certificados.Add(new CertificadoData
                        {
                            CantidadRecibida = 0,
                            IdCatTipoCertificado = grupocertificado.IdCatTipoCertificado,
                            FolioInicial = 0,
                            FolioFinal = 0
                        });
                    }

                }
                return View(vm);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "IngresoFormaValorada");
            }
        }

        //Registro de Ingresos de Forma Valorada
        [HttpPost]
        public async Task<IActionResult> Ingresos(IngresoFormaValoradaRequest request)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var tipoCertificado = new TipoCertificado();
                var files = HttpContext.Request.Form.Files;
                request.certificados = request.certificados.Where(x => x.CantidadRecibida > 0).ToList();
                request.Documento1 = files.Select(x => new IngresoFormaValoradaFiles { Nombre = x.FileName, Base64 = ConvertFileToBase64(x), Tipo = x.Name }).FirstOrDefault(x => x.Tipo.Contains("documento1")) ?? null;
                request.Documento2 = files.Select(x => new IngresoFormaValoradaFiles { Nombre = x.FileName, Base64 = ConvertFileToBase64(x), Tipo = x.Name }).FirstOrDefault(x => x.Tipo.Contains("documento2")) ?? null;
                request.Documento3 = files.Select(x => new IngresoFormaValoradaFiles { Nombre = x.FileName, Base64 = ConvertFileToBase64(x), Tipo = x.Name }).FirstOrDefault(x => x.Tipo.Contains("documento3")) ?? null;
                var algunCertificado = request.certificados.Any(x => x.CantidadRecibida > 0);
                if (!algunCertificado)
                {
                    result.Message = "No se ha establecido cantidad recibida para ningún certificado";
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                var certificadosInvalidos = request.certificados.Where(x => x.FolioInicial == 0 || x.FolioFinal == 0);
                if (certificadosInvalidos.Any())
                {
                    result.Message = "El folio inicial o final de la serie no puede ser 0. Certificados: ";
                    var certificadoNombres = certificadosInvalidos.GroupBy(x => x.NombreCertificado);
                    foreach (var item in certificadoNombres)
                    {
                        result.Message += item.Key + ", ";
                    }
                    result.Message = result.Message[..^2];
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                certificadosInvalidos = request.certificados.Where(x => (x.FolioFinal - x.FolioInicial) != x.CantidadRecibida - 1).ToList();

                if (certificadosInvalidos.Any())
                {
                    result.Message = "El folio inicial o final de la serie no corresponden a la cantidad recibida. Certificados: ";
                    var certificadoNombres = certificadosInvalidos.GroupBy(x => x.NombreCertificado);
                    foreach (var item in certificadoNombres)
                    {
                        result.Message += item.Key + ", ";
                    }
                    result.Message = result.Message[..^2];
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                bool rangosNoSolapados = VerificarRangosNoSolapados(request.certificados);
                if (!rangosNoSolapados)
                {
                    result.Message = "Hay series de folios que interfieren unos con otros, revise la información.";
                    result.IsSuccessFully = false;
                    return new JsonResult(result);
                }
                var save = await IngresoFormaValoradaHelper.Edit(_proxyWebAPI, request);
                if (!save.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    //result.Message = save.Message;
                    result.Message = save.Message;
                    return new JsonResult(result);
                }

                result.Message = "La información se guardó correctamente.";

            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                //result.Message = ex.Message;
                result.Message = "Ocurrió un error al guardar la información";
            }

            return new JsonResult(result);
        }

        //Formulario de Caja
        public async Task<IActionResult> Cajas(long? id)
        {
            if (!_userResolver.HasPermission(MenuIngresoFormasValoradas.RegistrarCajas))
                return new UnauthorizedResult();
            var result = new ResponseViewModel(true);
            ViewBag.isDetail = "Registro";
            var vm = new List<IngresoFormaValoradaResponse>
            {
                new IngresoFormaValoradaResponse
                {
                    IdSolicitudFV = 0
                }
            };
            try
            {
                var response = await CajaHelper.Consulta(_proxyWebAPI, id ?? 0);
                vm = response.Result as List<IngresoFormaValoradaResponse> ?? vm;

            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            ViewBag.Detail = false;
            return View(vm);
        }

        //Registro de Caja
        [HttpPost]
        public async Task<IActionResult> Cajas(List<CajaResponse> registro)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var tipoCertificado = new TipoCertificado();
                int p = 0;
                foreach (var obj in registro)
                {
                    obj.NombreCatTipoCertificado = (string?)tipoCertificado.NombreCertificado[p];
                    if (registro[p].FolioFinal < registro[p].FolioInicial)
                    {
                        result.Message = "Verifica que el Folio Final sea mayor o igual al Folio Inicial - Clave del certificado: " + obj.NombreCatTipoCertificado;
                        result.IsSuccessFully = false;
                        return new JsonResult(result);
                    }
                    p++;
                }

                int i = 1;
                foreach (var item in registro)
                {
                    item.IdIngresoFV = registro[0].IdIngresoFV;
                    item.IdCatTipoCertificado = i;
                    i++;
                }
                var save = await CajaHelper.Edit(_proxyWebAPI, registro);
                if (!save.IsSuccessFully)
                    throw new Exception(save.Message);

                result.Message = "La información se guardó correctamente.";
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }


        [HttpGet]
        public async Task<IActionResult> DetalleIngresos(long? id, long idSolicitudFV)
        {
            try
            {
                if (!_userResolver.HasPermission(MenuIngresoFormasValoradas.RegistrarIngreso))
                    return new UnauthorizedResult();
                var result = new ResponseViewModel(true);
                var response = await IngresoFormaValoradaHelper.Consulta(_proxyWebAPI, idSolicitudFV);
                var listSolicitud = response.Result as List<SolicitudFormaValoradaResponse> ?? new List<SolicitudFormaValoradaResponse>();
                listSolicitud = listSolicitud.Where(x => x.IdCatTipoCertificadoSC != 0).ToList();
                var certificadosResponse = await IngresoFormaValoradaHelper.GetIngresoCertificadoById(_proxyWebAPI, id ?? 0);
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
                    IdIngresoFV = id ?? 0,
                    certificados = listSolicitud.Select(x => new CertificadoGrupo
                    {
                        IdCatTipoCertificado = x.IdCatTipoCertificadoSC,
                        Certificados = certificadosIngreso.Where(c => c.IdCatTipoCertificado == x.IdCatTipoCertificadoSC).Select(x => new CertificadoData
                        {
                            IdIngresoCertificado = x.Id,
                            CantidadRecibida = x.CantidadRecibida,
                            IdCatTipoCertificado = x.IdCatTipoCertificado,
                            FolioInicial = x.FolioInicial,
                            FolioFinal = x.FolioFinal
                        }).ToList(),
                    }).OrderBy(x => x.Clave).ToList(),
                    certificadosTotalData = listSolicitud.Select(x => new CertificadosTotalData
                    {
                        CantidadRecibida = certificadosIngreso.Where(ci => ci.IdCatTipoCertificado == x.IdCatTipoCertificadoSC).Sum(ci => ci.CantidadRecibida),
                        CantidadSolicitada = x.CantidadSC,
                        IdCatTipoCertificado = x.IdCatTipoCertificadoSC
                    }).OrderBy(x => x.Clave).ToList(),
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "IngresoFormaValorada");
            }
        }

        public async Task<IActionResult> DetalleCajas(long? id)
        {
            if (!_userResolver.HasPermission(MenuIngresoFormasValoradas.RegistrarCajas))
                return new UnauthorizedResult();
            var result = new ResponseViewModel(true);
            var vm = new List<IngresoFormaValoradaResponse>
            {
                new IngresoFormaValoradaResponse
                {
                    IdSolicitudFV = 0
                }
            };
            try
            {
                var response = await CajaHelper.Consulta(_proxyWebAPI, id ?? 0);
                vm = response.Result as List<IngresoFormaValoradaResponse> ?? vm;

                var cajashelper = await CajaHelper.GetCajaById(_proxyWebAPI, id ?? 0);
                var resultcajashelper = cajashelper.Result as List<Caja> ?? new List<Caja>();
                if (resultcajashelper.Count == 0)
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        resultcajashelper.Add(new Caja
                        {
                            IdIngresoFV = 0,
                            IdCatTipoCertificado = i,
                            NumeroCaja = 0,
                            FolioInicial = 0,
                            FolioFinal = 0
                        });
                    }
                }

                ViewBag.ListaCajas = resultcajashelper;

            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            ViewBag.Detail = true;
            ViewBag.isDetail = "Detalle";
            return View("~/Views/IngresoFormaValorada/Cajas.cshtml", vm);
        }

        #region Private Methods
        private string ConvertFileToBase64(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                byte[] bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
        public static bool VerificarRangosNoSolapados(List<CertificadoData> certificados)
        {
            // Ordenar la lista por FolioInicial
            certificados.Sort((x, y) => x.FolioInicial.CompareTo(y.FolioInicial));

            // Verificar si hay solapamiento en los rangos de folios
            for (int i = 0; i < certificados.Count - 1; i++)
            {
                if (certificados[i].FolioFinal >= certificados[i + 1].FolioInicial)
                {
                    // Hay solapamiento
                    return false;
                }
            }

            // No hay solapamiento
            return true;
        }
        #endregion
    }
}
