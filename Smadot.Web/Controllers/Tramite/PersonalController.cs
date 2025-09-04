using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Personal.Request;
using Smadot.Models.Entities.Personal.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Helper.Operaciones.Tramite;
using Smadot.Web.Utilities;
using System.Linq.Dynamic.Core;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Models.Entities.Personal.Extensions;
using Smadot.Web.Helper.Operaciones.SeguimientoCVV;

namespace Smadot.Web.Controllers.Tramite
{
    [Authorize]
    public class PersonalController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuilder;
        private readonly BlobStorage _blobStorage;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public PersonalController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuilder, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuilder = pdfBuilder;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuPersonal.PermisoMenuPadre))
                return new UnauthorizedResult();
            return View("~/Views/Personal/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Consulta(long? id)
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
            var request = new PersonalListRequest()
            {
                Pagina = page,
                Registros = pageSize,
                Busqueda = searchValue,
                ColumnaOrdenamiento = sortColumn,
                Ordenamiento = sortColumnDirection,
                IdVerificentro = id
            };

            var response = await PersonalHelper.Consulta(_proxyWebAPI, request);

            if (response.IsSuccessFully)
            {
                var jsonReponse = (ResponseGrid<PersonalGridResponse>)response.Result;
                jsonReponse.Data.ForEach(async x =>
                {
                    x.Iniciales = GetIniciales(x.Nombre);
                    x.Actions = await this.RenderViewToStringAsync("~/Views/Personal/_Acciones.cshtml", x);
                });
                //if (jsonReponse.Data.Count() >0 && jsonReponse.Data != null)
                //{
                //    foreach (var x in jsonReponse.Data)
                //    {
                //        x.UrlFotoBase64 = "";
                //        if (!string.IsNullOrEmpty(x.UrlFoto))
                //        {
                //            var arrDoc = x.UrlFoto.Split("/");
                //            var NameDoc1 = arrDoc[arrDoc.Length - 1];
                //            var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                //            var archivo = await _blobStorage.DownloadFileAsync(x.UrlFoto, true);
                //            if (archivo.IsSuccessFully)
                //            {
                //                var imageDoc3 = string.Format("{0}{1}", ContentType1, archivo.Result);
                //                x.UrlFotoBase64 = imageDoc3;
                //            }
                //        }
                //    }
                //}
                jsonReponse.Draw = draw;
                return Json(jsonReponse);
            }

            return Json("");
        }

        [HttpGet]
        public async Task<IActionResult> Registro()
        {
            PersonalRequest vm = new()
            {
                FechaCapacitacionInicio = DateTime.Now,
                FechaCapacitacionFinal = DateTime.Now,
                FechaIncorporacion = DateTime.Now,
                FechaNacimiento = DateTime.Now,
                HorarioRequest = new List<HorarioUserRequest>()
                {
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Lunes, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes ] },
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Martes, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ] },
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Miercoles, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles ] },
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Jueves, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves ] },
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Viernes, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes ] },
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Sabado, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado ] },
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Domingo, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo ] },
                }
            };
            ViewBag.Generos = new List<SelectListItem>() { new SelectListItem { Text = "Masculino", Value = "M" }, new SelectListItem { Text = "Femenino", Value = "F" } };
            var catalogoTipoPuesto = await TipoPuestoHelper.Consulta(_proxyWebAPI, new TipoPuestoRequest { Activo = true });
            ViewBag.TipoPuesto = (catalogoTipoPuesto.Result as List<TipoPuestoResponse> ?? new List<TipoPuestoResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
            return View("~/Views/Personal/Registro.cshtml", vm);
        }

        [HttpPost]
        public async Task<JsonResult> Registro([FromBody] PersonalRequest request)
        {
            var result = new ResponseViewModel();
            try
            {
                request.Files = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesString ?? "");
                request.FilesFoto = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesFotoString ?? "");
                request.FilesSeguroSocial = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesSeguroSocialString ?? "");
                request.FilesFirma = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesFirmaString ?? "");
                //request.FilesContrato = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesContratoString ?? "");

                var registroUsuario = await PersonalHelper.Registro(_proxyWebAPI, request);
                if (!registroUsuario.IsSuccessFully)
                {
                    result.Message = "Ocurrió un error al registrar el usuario.";
                    return new JsonResult(result);
                }
                result.IsSuccessFully = registroUsuario.IsSuccessFully;
                result.Result = registroUsuario.Result;
                var existoso = registroUsuario.Result as bool? ?? false;
                if (!existoso)
                {
                    result.IsSuccessFully = existoso;
                    result.Message = registroUsuario.Message;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(long id)
        {
            try
            {
                //var response = await PersonalHelper.Detalle(_proxyWebAPI, id);
                var response = await PersonalHelper.GetByIdPuestoVerificentro(_proxyWebAPI, id, true);
                if (!response.IsSuccessFully)
                    return RedirectToAction("Index");

                var vm = response.Result as PersonalResponse ?? new PersonalResponse();

                var puestos = await PersonalHelper.ConsultaPuestos(_proxyWebAPI, vm.IdUsuario);
                var res = puestos.Result as List<PersonalResponse> ?? new List<PersonalResponse>();

                res = res.Where(x => x.IdPuestoVerificentro != id).ToList();

                ViewBag.HasPuestos = res.Any();
                ViewBag.Puestos = res;

                var VBUrlDoc1 = "";
                var VBUrlDoc2 = "";
                var VBUrlDoc3 = "";
                var VBUrlDoc4 = "";
                if (!string.IsNullOrEmpty(vm.UrlIne))
                {
                    var arrDoc = vm.UrlIne.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm.UrlIne, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageUrlIne = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc1 = imageUrlIne;
                    }
                }
                if (!string.IsNullOrEmpty(vm.UrlFoto))
                {
                    var arrDoc = vm.UrlFoto.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm.UrlFoto, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageUrlFoto = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc2 = imageUrlFoto;
                    }
                }
                if (!string.IsNullOrEmpty(vm.UrlSeguroSocial))
                {
                    var arrDoc = vm.UrlSeguroSocial.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm.UrlSeguroSocial, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageUrlSeguroSocial = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc3 = imageUrlSeguroSocial;
                    }
                }
                if (!string.IsNullOrEmpty(vm.UrlFirma))
                {
                    var arrDoc = vm.UrlFirma.Split("/");
                    var NameDoc1 = arrDoc[arrDoc.Length - 1];
                    var ContentType1 = NameDoc1.Contains(".pdf") ? "data:application/pdf;base64," : "data:image/png;base64,";
                    var archivo = await _blobStorage.DownloadFileAsync(vm.UrlFirma, true);
                    if (archivo.IsSuccessFully)
                    {
                        var imageUrlFirma = string.Format("{0}{1}", ContentType1, archivo.Result);
                        VBUrlDoc4 = imageUrlFirma;
                    }
                }
                ViewBag.UrlIne = VBUrlDoc1;
                ViewBag.UrlFoto = VBUrlDoc2;
                ViewBag.UrlSeguroSocial = VBUrlDoc3;
                ViewBag.UrlFirma = VBUrlDoc4;

                return View("~/Views/Personal/Detalle.cshtml", vm);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<JsonResult> Baja(string nombre, string nombrePuesto)
        {
            var result = new ResponseViewModel(true);
            var vm = new BajaPersonalResponse()
            {
                Nombre = nombre,
                NombrePuesto = nombrePuesto
            };
            try
            {
                result.Result = await this.RenderViewToStringAsync("~/Views/Personal/_ModalBaja.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<JsonResult> Baja([FromBody] BajaPersonalRequest request)
        {
            var result = new ResponseViewModel();
            try
            {
                var validacionUsuario = await PersonalHelper.Baja(_proxyWebAPI, request);
                if (!validacionUsuario.IsSuccessFully)
                {
                    result.Message = "Ocurrió un error al dar de baja al usuario.";
                    return new JsonResult(result);
                }
                bool usuarioRegistrado = (bool)validacionUsuario.Result;
                result.IsSuccessFully = validacionUsuario.IsSuccessFully;
                result.Result = usuarioRegistrado;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(long id)
        {

            //var helper = await PersonalHelper.GetPersonalById(_proxyWebAPI, id);
            var helper = await PersonalHelper.GetByIdPuestoVerificentro(_proxyWebAPI, id, true);
            if (!helper.IsSuccessFully)
                return RedirectToAction("Index");

            var response = helper.Result as PersonalResponse ?? new PersonalResponse();

            if (response.IdCatEstatusPuesto is not CatEstatusPuestoDic.PermiteModificar)
                return RedirectToAction("Index");

            PersonalRequest vm = new()
            {
                FechaCapacitacionInicio = response.FechaCapacitacionInicio,
                FechaCapacitacionFinal = response.FechaCapacitacionFinal,
                FechaIncorporacion = response.FechaIncorporacion,
                FechaNacimiento = response.FechaNacimiento,
                HorarioRequest = response.HorarioRequest,
                CorreoUsuario = response.CorreoUsuario,
                Curp = response.Curp,
                FechaAcreditacionNorma = response.FechaAcreditacionNorma,
                Genero = response.Genero,
                IdPuesto = response.IdPuesto,
                Nombre = response.Nombre,
                IdUsuario = response.IdUsuario,
                NumeroTrabajador = response.NumeroTrabajador,
                Rfc = response.Rfc,
                TelefonoUsuario = response.TelefonoUsuario
            };
            ViewBag.Generos = new List<SelectListItem>() { new SelectListItem { Text = "Masculino", Value = "M" }, new SelectListItem { Text = "Femenino", Value = "F" } };
            var catalogoTipoPuesto = await TipoPuestoHelper.Consulta(_proxyWebAPI, new TipoPuestoRequest { Activo = true });
            ViewBag.TipoPuesto = (catalogoTipoPuesto.Result as List<TipoPuestoResponse> ?? new List<TipoPuestoResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
            return View("~/Views/Personal/Edit.cshtml", vm);
        }

        [HttpPost]
        public async Task<JsonResult> Editar([FromBody] PersonalRequest request)
        {
            var result = new ResponseViewModel();
            try
            {
                request.Files = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesString ?? "");
                request.FilesFoto = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesFotoString ?? "");
                request.FilesSeguroSocial = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesSeguroSocialString ?? "");
                request.FilesFirma = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesFirmaString ?? "");
                //request.FilesContrato = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesContratoString ?? "");

                var registroUsuario = await PersonalHelper.Registro(_proxyWebAPI, request);
                if (!registroUsuario.IsSuccessFully)
                {
                    result.Message = registroUsuario?.Message.ObtenerMensajeExcepcion() ?? "Ocurrió un error al registrar el usuario.";
                    return new JsonResult(result);
                }
                result.IsSuccessFully = registroUsuario.IsSuccessFully;
                result.Result = registroUsuario.Result;
                var existoso = registroUsuario.Result as bool? ?? false;
                if (!existoso)
                {
                    result.IsSuccessFully = existoso;

                    result.Message = registroUsuario.Message;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> Movimiento(long id)
        {

            var helper = await PersonalHelper.GetByIdPuestoVerificentro(_proxyWebAPI, id);
            if (!helper.IsSuccessFully)
                return RedirectToAction("Index");

            var response = helper.Result as PersonalResponse ?? new PersonalResponse();

            if (response.IdCatEstatusPuesto is not CatEstatusPuestoDic.PermiteModificar)
                return RedirectToAction("Index");

            PersonalRequest vm = new()
            {
                FechaNacimiento = response.FechaNacimiento,
                CorreoUsuario = response.CorreoUsuario,
                Curp = response.Curp,
                Genero = response.Genero,
                Nombre = response.Nombre,
                IdUsuario = response.IdUsuario,
                Rfc = response.Rfc,
                TelefonoUsuario = response.TelefonoUsuario,
                HorarioRequest = new List<HorarioUserRequest>()
                {
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Lunes, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes] },
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Martes, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes] },
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Miercoles, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles] },
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Jueves, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves] },
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Viernes, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes] },
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Sabado, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado] },
                    new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Domingo, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo] },
                },
                FechaCapacitacionFinal = DateTime.Now,
                FechaCapacitacionInicio = DateTime.Now,
                FechaIncorporacion = DateTime.Now,
                FechaAcreditacionNorma = DateTime.Now
            };

            ViewBag.Generos = new List<SelectListItem>() { new SelectListItem { Text = "Masculino", Value = "M" }, new SelectListItem { Text = "Femenino", Value = "F" } };
            var catalogoTipoPuesto = await TipoPuestoHelper.Consulta(_proxyWebAPI, new TipoPuestoRequest { Activo = true });
            ViewBag.TipoPuesto = (catalogoTipoPuesto.Result as List<TipoPuestoResponse> ?? new List<TipoPuestoResponse>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
            return View("~/Views/Personal/Movimiento.cshtml", vm);
        }

        [HttpPost]
        public async Task<JsonResult> Movimiento([FromBody] PersonalRequest request)
        {
            var result = new ResponseViewModel();
            try
            {
                request.Files = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesString ?? "");
                request.FilesFoto = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesFotoString ?? "");
                request.FilesSeguroSocial = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesSeguroSocialString ?? "");
                request.FilesFirma = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesFirmaString ?? "");
                //request.FilesContrato = JsonConvert.DeserializeObject<List<PersonalFiles>>(request.FilesContratoString ?? "");

                var registroUsuario = await PersonalHelper.Movimiento(_proxyWebAPI, request);
                if (!registroUsuario.IsSuccessFully)
                {
                    result.Message = registroUsuario?.Message.ObtenerMensajeExcepcion() ?? "Ocurrió un error al registrar el usuario.";
                    return new JsonResult(result);
                }
                result.IsSuccessFully = registroUsuario.IsSuccessFully;
                result.Result = registroUsuario.Result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> GetPDF([FromBody] PersonalVistaPreviaResponse registro)
        {
            var result = new ResponseViewModel(true);
            if (registro == null) registro = new PersonalVistaPreviaResponse();
            try
            {
                string stringHtml = string.Empty;
                registro.PuestosResult = new List<PuestosVistaPreviaResponse>();
                var dataReport = new DataReport();
                if (registro.IdUsuario < 1)
                {
                    registro.PuestosResult.Add(new PuestosVistaPreviaResponse
                    {
                        FechaAcreditacionNorma = registro.FechaAcreditacionNorma,
                        FechaCapacitacionFinal = registro.FechaCapacitacionFinal,
                        FechaCapacitacionInicio = registro.FechaCapacitacionInicio,
                        FechaIncorporacion = registro.FechaIncorporacion,
                        IdPuesto = registro.IdPuesto > 0 ? registro.IdPuesto : 0,
                        NumeroTrabajador = registro.NumeroTrabajador ?? "",
                        //Puesto = registro.Puesto
                        Puesto = string.IsNullOrEmpty(registro.Puesto) ? "" : registro.Puesto.ToLower().Contains("seleccione") ? "" : registro.Puesto
                    });
                }
                if (registro.IdUsuario > 0)
                {
                    var puestosResult = await PersonalHelper.ConsultaPuestos(_proxyWebAPI, registro.IdUsuario);
                    var res = puestosResult.Result as List<PersonalResponse> ?? new List<PersonalResponse>();



                    if (res.Count() > 0)
                    {
                        var list = res.Select(x => new PuestosVistaPreviaResponse
                        {
                            Puesto = x.NombrePuesto,
                            NumeroTrabajador = x.NumeroTrabajador,
                            FechaIncorporacion = x.FechaIncorporacion,
                            FechaCapacitacionInicio = x.FechaCapacitacionInicio,
                            FechaCapacitacionFinal = x.FechaCapacitacionFinal,
                            FechaAcreditacionNorma = x.FechaAcreditacionNorma
                        });

                        registro.PuestosResult.AddRange(list);
                    }
                }
                var getdoc = await _pdfBuilder.GetDocumentoPersonal(registro);

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

        [HttpGet]
        public async Task<IActionResult> DescargarDocumento(string url)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var urlSplit = url.Split("/");
                var nombreArchivo = urlSplit[urlSplit.Length - 1];
                url = _blobStorage._url + "/Personal/" + url;
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

        [HttpPut]
        public async Task<IActionResult> ModificarEstatusPuesto(long idUserPuestoVerificentro, int idCatEstatusPuesto)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var request = new EstatusPuestoPersonalRequest
                {
                    IdCatEstatusPuesto = idCatEstatusPuesto,
                    IdUserPuestoVerificentro = idUserPuestoVerificentro
                };
                var response = await PersonalHelper.ModificarEstatusPuesto(_proxyWebAPI, request);
                if (!response.IsSuccessFully)
                {
                    result.IsSuccessFully = false;
                    result.Message = response.Message.ObtenerMensajeExcepcion() ?? "Ocurrió un error al modificar el estatus";
                    return new JsonResult(result);
                }
                result.Message = "Se ha modificado correctamente";
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }
            return new JsonResult(result);
        }
        public string GetIniciales(string nombre)
        {
            var resultado = "";

            try
            {
                List<char> iniciales = new List<char>();
                var split = nombre.Split(' ');
                for (int i = 0; i < split.Count(); i++)
                {
                    if (iniciales.Count() == 2)
                    {
                        break;
                    }
                    iniciales.Add(char.Parse(split[i].Substring(0, 1)));
                }

                resultado = string.Join("", iniciales);
            }
            catch (Exception e)
            {

            }

            return resultado;
        }

    }
}