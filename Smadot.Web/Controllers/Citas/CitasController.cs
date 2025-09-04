using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.Util;
using Smadot.FrontEnd.Models;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Cita.Request;
using Smadot.Models.Entities.FoliosFormaValorada.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.PortalCita.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Utilities.Seguridad;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones;
using Smadot.Web.Helper.Operaciones.FoliosFormaValorada;
using Smadot.Web.Models;
using Smadot.Web.Utilities;

namespace Smadot.Web.Controllers.HistorialCitas
{

    public class CitasController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuider;
        private readonly BlobStorage _blobStorage;
        private string _llaveCifrado;
        private readonly IExcelBuilder _excelBuilder;
        public CitasController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConfiguration configuration, IExcelBuilder excelBuilder)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;
            _llaveCifrado = configuration["Encrypter:ClaveSecreta"];
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _excelBuilder = excelBuilder;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View("~/Views/Citas/Index.cshtml");

        }
        [Authorize]
        [HttpPost]
        public async Task<JsonResult> Consulta(string fecha1, string fecha2, bool? atendida)
        {
            JsonResult result = new JsonResult("");

            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int page = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            //Invoca al método que se encarga de realizar la petición Api
            var request = new CitaGridRequest();
            request.Pagina = page / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = string.IsNullOrEmpty(searchValue) ? null : searchValue;
            request.ColumnaOrdenamiento = sortColumn;
            request.Ordenamiento = sortColumnDirection;
            request.Atendida = atendida;
            DateTime fecharesult;
            string formatoFecha = "dd/MM/yyyy";
            if (DateTime.TryParseExact(fecha1, formatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecharesult))
            {
                request.Fecha1 = fecharesult;
            }
            else
            {
                request.Fecha1 = null;
            }
            if (DateTime.TryParseExact(fecha2, formatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecharesult))
            {
                request.Fecha2 = fecharesult;
            }
            else
            {
                request.Fecha2 = null;
            }
            var response = await CitasHelper.Consulta(_proxyWebAPI, request);

            if (response.IsSuccessFully)
            {
                var jsonReponse = (ResponseGrid<HistorialCitasResponse>)response.Result;
                jsonReponse.Data.ForEach(async x =>
                {
                    x.Acciones = await this.RenderViewToStringAsync("~/Views/Citas/_Acciones.cshtml", x);
                });
                jsonReponse.Draw = draw;
                return Json(jsonReponse);
            }

            return Json("");
        }
        [Authorize]
        public async Task<IActionResult> Expediente(long id)
        {
            var citaHelper = await CitasHelper.Expediente(_proxyWebAPI, id);
            var vm = new vVerificacionExpediente();
            if (citaHelper.IsSuccessFully)
            {
                vm = citaHelper.Result as vVerificacionExpediente ?? new vVerificacionExpediente();
                var listPdf = new List<ListDocumentos>();
                var listImage = new List<ListDocumentos>();


                if (!string.IsNullOrEmpty(vm.URLFotoTecnico))
                {

                    listImage.Add(new ListDocumentos
                    {
                        DocName = "Foto Técnico Verificador",
                        Extension = "imagen",
                        UrlName = vm.URLFotoTecnico.Contains(_blobStorage._url) ? vm.URLFotoTecnico.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                    });
                }

                if (!string.IsNullOrEmpty(vm.URLPlacaDelantera))
                {
                    listImage.Add(new ListDocumentos
                    {
                        DocName = "Placa Delantera",
                        Extension = "imagen",
                        UrlName = vm.URLPlacaDelantera.Contains(_blobStorage._url) ? vm.URLPlacaDelantera.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                    });
                }

                if (!string.IsNullOrEmpty(vm.URLPlacaTrasera))
                {

                    listImage.Add(new ListDocumentos
                    {
                        DocName = "Placa Trasera",
                        Extension = "imagen",
                        UrlName = vm.URLPlacaTrasera.Contains(_blobStorage._url) ? vm.URLPlacaTrasera.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                    });

                }

                if (!string.IsNullOrEmpty(vm.URLPlacaCamara))
                {
                    listImage.Add(new ListDocumentos
                    {
                        DocName = "Captura Cámara Delantera",
                        Extension = "imagen",
                        UrlName = vm.URLPlacaCamara.Contains(_blobStorage._url) ? vm.URLPlacaCamara.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                    });
                }
                if (!string.IsNullOrEmpty(vm.URLPlacaCamaraTrasera))
                {
                    listImage.Add(new ListDocumentos
                    {
                        DocName = "Captura Cámara Trasera",
                        Extension = "imagen",
                        UrlName = vm.URLPlacaCamaraTrasera.Contains(_blobStorage._url) ? vm.URLPlacaCamaraTrasera.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                    });
                }
                if (!string.IsNullOrEmpty(vm.URLEvidenciaFalla))
                {
                    listImage.Add(new ListDocumentos
                    {
                        DocName = "Evidencia Falla Inspección Visual",
                        Extension = "imagen",
                        UrlName = vm.URLEvidenciaFalla.Contains(_blobStorage._url) ? vm.URLEvidenciaFalla.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                    });
                }
                if (!string.IsNullOrEmpty(vm.URLIdentificacion))
                {
                    if (vm.URLIdentificacion.Contains(".pdf"))
                    {
                        listPdf.Add(new ListDocumentos
                        {
                            DocName = "Identificación",
                            Extension = "pdf",
                            UrlName = vm.URLIdentificacion.Contains(_blobStorage._url) ? vm.URLIdentificacion.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                        });
                    }
                    else
                    {
                        listImage.Add(new ListDocumentos
                        {
                            DocName = "Identificación",
                            Extension = "imagen",
                            UrlName = vm.URLIdentificacion.Contains(_blobStorage._url) ? vm.URLIdentificacion.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                        });
                    }
                }
                if (!string.IsNullOrEmpty(vm.URLFactura))
                {
                    if (vm.URLFactura.Contains(".pdf"))
                    {
                        listPdf.Add(new ListDocumentos
                        {
                            DocName = "Factura/Carta Factura",
                            Extension = "pdf",
                            UrlName = vm.URLFactura.Contains(_blobStorage._url) ? vm.URLFactura.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                        });
                    }
                    else
                    {
                        listImage.Add(new ListDocumentos
                        {
                            DocName = "Factura/Carta Factura",
                            Extension = "imagen",
                            UrlName = vm.URLFactura.Contains(_blobStorage._url) ? vm.URLFactura.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                        });
                    }
                }
                //if (!string.IsNullOrEmpty(vm.URLCartaFactura))
                //{
                //    if (vm.URLCartaFactura.Contains(".pdf"))
                //    {
                //        listPdf.Add(new ListDocumentos
                //        {
                //            DocName = "Carta Factura",
                //            Extension = "pdf",
                //            UrlName = vm.URLCartaFactura.Contains(_blobStorage._url) ? vm.URLCartaFactura.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //        });
                //    }
                //    else
                //    {
                //        listImage.Add(new ListDocumentos
                //        {
                //            DocName = "Carta Factura",
                //            Extension = "imagen",
                //            UrlName = vm.URLCartaFactura.Contains(_blobStorage._url) ? vm.URLCartaFactura.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //        });
                //    }
                //}
                if (!string.IsNullOrEmpty(vm.URLValidacionCertificado))
                {
                    if (vm.URLValidacionCertificado.Contains(".pdf"))
                    {
                        listPdf.Add(new ListDocumentos
                        {
                            DocName = "Validación Certificado",
                            Extension = "pdf",
                            UrlName = vm.URLValidacionCertificado.Contains(_blobStorage._url) ? vm.URLValidacionCertificado.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                        });
                    }
                    else
                    {
                        listImage.Add(new ListDocumentos
                        {
                            DocName = "Validación Certificado",
                            Extension = "imagen",
                            UrlName = vm.URLValidacionCertificado.Contains(_blobStorage._url) ? vm.URLValidacionCertificado.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                        });
                    }
                }
                if (!string.IsNullOrEmpty(vm.URLBajaPlacas))
                {
                    if (vm.URLBajaPlacas.Contains(".pdf"))
                    {
                        listPdf.Add(new ListDocumentos
                        {
                            DocName = "Baja de Placas",
                            Extension = "pdf",
                            UrlName = vm.URLBajaPlacas.Contains(_blobStorage._url) ? vm.URLBajaPlacas.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                        });
                    }
                    else
                    {
                        listImage.Add(new ListDocumentos
                        {
                            DocName = "Baja de Placas",
                            Extension = "imagen",
                            UrlName = vm.URLBajaPlacas.Contains(_blobStorage._url) ? vm.URLBajaPlacas.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                        });
                    }
                }
                if (!string.IsNullOrEmpty(vm.URLAltaPlacas))
                {
                    if (vm.URLAltaPlacas.Contains(".pdf"))
                    {
                        listPdf.Add(new ListDocumentos
                        {
                            DocName = "Alta de Placas",
                            Extension = "pdf",
                            UrlName = vm.URLAltaPlacas.Contains(_blobStorage._url) ? vm.URLAltaPlacas.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                        });
                    }
                    else
                    {
                        listImage.Add(new ListDocumentos
                        {
                            DocName = "Alta de Placas",
                            Extension = "imagen",
                            UrlName = vm.URLAltaPlacas.Contains(_blobStorage._url) ? vm.URLAltaPlacas.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                        });
                    }
                }



                ViewBag.ListaPdf = listPdf;
                ViewBag.ListaImagen = listImage;

            }

            return View("~/Views/Citas/Expediente.cshtml", vm);
        }

        [HttpGet]
        public async Task<JsonResult> VolverImprimir(long Id)
        {
            var result = new JsonResultDto();
            try
            {
                var data = new FolioFormaValoradaImpresionResponse();
                var citaHelper = await CitasHelper.VolverImprimir(_proxyWebAPI, Id);

                if (!citaHelper.IsSuccessFully)
                {
                    result.Error = true;
                    return Json(result);
                }
                result.ErrorDescription = "La operación se realizó éxitosamente";

            }
            catch (Exception)
            {

                return Json(new ResponseViewModel { IsSuccessFully = false, Message = "No se pudo generar correctamente el documento para imprimirlo." });

            }
            return Json(result);
        }

        public async Task<IActionResult> ExpedientePublico(string clave)
        {
            clave = clave.TrimEnd();
            clave = clave.Replace(" ", "+");
            var encrypter = new AesEncryption(_llaveCifrado);
            var idString = encrypter.Decrypt(clave);

            long.TryParse(idString, out long id);

            if (id < 1)

            {

                return RedirectToAction("Error", "Home", new ErrorViewModel { Title = "No se encontró el expediente.", TypeError = "404 Not Found", Description = "Al parecer no hay registro de la información del expediente solicitado." });

            }
            // long.TryParse(idString, out long id);
            // if (id < 1)
            // {
            //     return RedirectToAction("Home", new ErrorViewModel { Title = "No se encontró el expediente.", TypeError = "404 Not Found", Description = "Al parecer no hay registro de la información del expediente solicitado." });
            // }
            var citaHelper = await CitasHelper.Expediente(_proxyWebAPI, id);
            var vm = new vVerificacionExpediente();
            if (citaHelper.IsSuccessFully)
            {
                vm = citaHelper.Result as vVerificacionExpediente ?? new vVerificacionExpediente();
                ViewBag.esPublico = true;
                var listPdf = new List<ListDocumentos>();
                var listImage = new List<ListDocumentos>();


                // if (!string.IsNullOrEmpty(vm.URLFotoTecnico))
                // {

                //     listImage.Add(new ListDocumentos
                //     {
                //         DocName = "Foto Técnico Verificador",
                //         Extension = "imagen",
                //         UrlName = vm.URLFotoTecnico.Contains(_blobStorage._url) ? vm.URLFotoTecnico.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //     });
                // }

                if (!string.IsNullOrEmpty(vm.URLPlacaDelantera))
                {
                    listImage.Add(new ListDocumentos
                    {
                        DocName = "Placa Delantera",
                        Extension = "imagen",
                        UrlName = vm.URLPlacaDelantera.Contains(_blobStorage._url) ? vm.URLPlacaDelantera.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                    });
                }

                if (!string.IsNullOrEmpty(vm.URLPlacaTrasera))
                {

                    listImage.Add(new ListDocumentos
                    {
                        DocName = "Placa Trasera",
                        Extension = "imagen",
                        UrlName = vm.URLPlacaTrasera.Contains(_blobStorage._url) ? vm.URLPlacaTrasera.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                    });

                }

                if (!string.IsNullOrEmpty(vm.URLPlacaCamara))
                {
                    listImage.Add(new ListDocumentos
                    {
                        DocName = "Captura Cámara de la Línea",
                        Extension = "imagen",
                        UrlName = vm.URLPlacaCamara.Contains(_blobStorage._url) ? vm.URLPlacaCamara.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                    });
                }

                if (!string.IsNullOrEmpty(vm.URLEvidenciaFalla))
                {
                    listImage.Add(new ListDocumentos
                    {
                        DocName = "Evidencia Falla Inspección Visual",
                        Extension = "imagen",
                        UrlName = vm.URLEvidenciaFalla.Contains(_blobStorage._url) ? vm.URLEvidenciaFalla.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                    });
                }
                // if (!string.IsNullOrEmpty(vm.URLIdentificacion))
                // {
                //     if (vm.URLIdentificacion.Contains(".pdf"))
                //     {
                //         listPdf.Add(new ListDocumentos
                //         {
                //             DocName = "Identificación",
                //             Extension = "pdf",
                //             UrlName = vm.URLIdentificacion.Contains(_blobStorage._url) ? vm.URLIdentificacion.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //         });
                //     }
                //     else
                //     {
                //         listImage.Add(new ListDocumentos
                //         {
                //             DocName = "Identificación",
                //             Extension = "imagen",
                //             UrlName = vm.URLIdentificacion.Contains(_blobStorage._url) ? vm.URLIdentificacion.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //         });
                //     }
                // }
                // if (!string.IsNullOrEmpty(vm.URLFactura))
                // {
                //     if (vm.URLFactura.Contains(".pdf"))
                //     {
                //         listPdf.Add(new ListDocumentos
                //         {
                //             DocName = "Factura/Carta Factura",
                //             Extension = "pdf",
                //             UrlName = vm.URLFactura.Contains(_blobStorage._url) ? vm.URLFactura.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //         });
                //     }
                //     else
                //     {
                //         listImage.Add(new ListDocumentos
                //         {
                //             DocName = "Factura/Carta Factura",
                //             Extension = "imagen",
                //             UrlName = vm.URLFactura.Contains(_blobStorage._url) ? vm.URLFactura.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //         });
                //     }
                // }
                //if (!string.IsNullOrEmpty(vm.URLCartaFactura))
                //{
                //    if (vm.URLCartaFactura.Contains(".pdf"))
                //    {
                //        listPdf.Add(new ListDocumentos
                //        {
                //            DocName = "Carta Factura",
                //            Extension = "pdf",
                //            UrlName = vm.URLCartaFactura.Contains(_blobStorage._url) ? vm.URLCartaFactura.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //        });
                //    }
                //    else
                //    {
                //        listImage.Add(new ListDocumentos
                //        {
                //            DocName = "Carta Factura",
                //            Extension = "imagen",
                //            UrlName = vm.URLCartaFactura.Contains(_blobStorage._url) ? vm.URLCartaFactura.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //        });
                //    }
                //}
                // if (!string.IsNullOrEmpty(vm.URLValidacionCertificado))
                // {
                //     if (vm.URLValidacionCertificado.Contains(".pdf"))
                //     {
                //         listPdf.Add(new ListDocumentos
                //         {
                //             DocName = "Validación Certificado",
                //             Extension = "pdf",
                //             UrlName = vm.URLValidacionCertificado.Contains(_blobStorage._url) ? vm.URLValidacionCertificado.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //         });
                //     }
                //     else
                //     {
                //         listImage.Add(new ListDocumentos
                //         {
                //             DocName = "Validación Certificado",
                //             Extension = "imagen",
                //             UrlName = vm.URLValidacionCertificado.Contains(_blobStorage._url) ? vm.URLValidacionCertificado.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //         });
                //     }
                // }
                // if (!string.IsNullOrEmpty(vm.URLBajaPlacas))
                // {
                //     if (vm.URLBajaPlacas.Contains(".pdf"))
                //     {
                //         listPdf.Add(new ListDocumentos
                //         {
                //             DocName = "Baja de Placas",
                //             Extension = "pdf",
                //             UrlName = vm.URLBajaPlacas.Contains(_blobStorage._url) ? vm.URLBajaPlacas.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //         });
                //     }
                //     else
                //     {
                //         listImage.Add(new ListDocumentos
                //         {
                //             DocName = "Baja de Placas",
                //             Extension = "imagen",
                //             UrlName = vm.URLBajaPlacas.Contains(_blobStorage._url) ? vm.URLBajaPlacas.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //         });
                //     }
                // }
                // if (!string.IsNullOrEmpty(vm.URLAltaPlacas))
                // {
                //     if (vm.URLAltaPlacas.Contains(".pdf"))
                //     {
                //         listPdf.Add(new ListDocumentos
                //         {
                //             DocName = "Alta de Placas",
                //             Extension = "pdf",
                //             UrlName = vm.URLAltaPlacas.Contains(_blobStorage._url) ? vm.URLAltaPlacas.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //         });
                //     }
                //     else
                //     {
                //         listImage.Add(new ListDocumentos
                //         {
                //             DocName = "Alta de Placas",
                //             Extension = "imagen",
                //             UrlName = vm.URLAltaPlacas.Contains(_blobStorage._url) ? vm.URLAltaPlacas.Replace(string.Format("{0}/", _blobStorage._url), "") : ""
                //         });
                //     }
                // }



                ViewBag.ListaPdf = listPdf;
                ViewBag.ListaImagen = listImage;
            }
            else
            {
                return RedirectToAction("Home", new ErrorViewModel { Title = "No se encontró el expediente.", TypeError = "404 Not Found", Description = "Al parecer no hay registro de la información del expediente solicitado." });

            }

            return View("~/Views/Citas/Expediente.cshtml", vm);
        }
        [HttpGet]
        public FileResult DescargarReporte()
        {
            var fileName = HttpContext.Session.GetString("FileName");
            var bytes = HttpContext.Session.Get("FileXlsx");

            HttpContext.Session.Remove("FileName");
            HttpContext.Session.Remove("FileXlsx");

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpGet]
        public async Task<ActionResult> GetPDF(long IdCicloVerificacion, bool PDF, string search, string sortColumnDirection, string sortColumn, string? Fecha1, string? Fecha2, bool? Atendida)
        {
            var result = new ResponseViewModel(true);
            try
            {
                string stringHtml = string.Empty;
                var dataReport = new DataReport();
                var vm = new HistorialCitasUtilitiesResponse
                {
                    Fecha1 = Fecha1,
                    Fecha2 = Fecha2
                };

                var request = new CitaGridRequest();
                request.Pagina = 0;
                request.Registros = 0;
                request.Busqueda = search;
                request.ColumnaOrdenamiento = sortColumn;
                request.Ordenamiento = sortColumnDirection;
                DateTime fecharesult;
                string formatoFecha = "dd/MM/yyyy";
                if (DateTime.TryParseExact(Fecha1, formatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecharesult))
                {
                    request.Fecha1 = fecharesult;
                }
                else
                {
                    request.Fecha1 = null;
                }
                if (DateTime.TryParseExact(Fecha2, formatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecharesult))
                {
                    request.Fecha2 = fecharesult;
                }
                else
                {
                    request.Fecha2 = null;
                }


                var resultPost = await CitasHelper.Consulta(_proxyWebAPI, request);
                var jsonReponse = (ResponseGrid<HistorialCitasResponse>)resultPost.Result ?? new();
                vm.Rows = JsonConvert.DeserializeObject<List<HistorialCitasRowResponse>>(JsonConvert.SerializeObject(jsonReponse.Data));

                if (PDF)
                {
                    var getdoc = await _pdfBuider.GetDocumentoHistorialCitas(vm);
                    HttpContext.Session.SetString("FileName", getdoc.Response.NombreDocumento);
                    HttpContext.Session.Set("FileXlsx", getdoc.Response.DocumentoPDF);
                }
                else
                {
                    var getdoc = await _excelBuilder.GetDocumentoHistorialCitas(vm);
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
        // [HttpGet]
        // public FileResult DescargarReportePDF()
        // {
        //     var fileName = HttpContext.Session.GetString("FileName");
        //     var bytes = HttpContext.Session.Get("FileXlsx");

        //     HttpContext.Session.Remove("FileName");
        //     HttpContext.Session.Remove("FileXlsx");

        //     return File(bytes, "application/pdf", fileName);
        // }
        [HttpGet]
        public async Task<IActionResult> DescargarDocumentoExpediente(string url)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var urlSplit = url.Split("/");
                var nombreArchivo = urlSplit[urlSplit.Length - 1];
                //var archivo = new StorageManager().ObtenerArchivo(url);
                //if (string.IsNullOrEmpty(archivo))
                //{
                //    result.IsSuccessFully = false;
                //    throw new Exception("No sé encontró el documento.");
                //}
                url = _blobStorage._url + "/" + url;
                var archivo = await _blobStorage.DownloadFileAsync(url, true);
                if (!archivo.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    throw new Exception("No sé encontró el documento.");
                }
                result.Result = new
                {
                    FileName = nombreArchivo,
                    Base64 = archivo.Result,
                    Ext = nombreArchivo.Split('.')[1]
                };
            }
            catch (Exception ex)
            {
                if (result.IsSuccessFully)
                    result.Message = "Error al descargar el documento.";
                else
                    result.Message = ex.Message;
                result.IsSuccessFully = false;
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> ReiniciarCita(ReiniciarCitaRequest req)
        {
            var result = new ResponseViewModel(true);
            try
            {
                result = await CitasHelper.ReiniciarCita(_proxyWebAPI, req);
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
