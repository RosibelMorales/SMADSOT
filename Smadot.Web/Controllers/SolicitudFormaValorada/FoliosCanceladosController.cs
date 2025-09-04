using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Utilities;
using static Smadot.Models.Entities.FoliosCancelados.Response.FoliosCanceladosResponseData;
using System.Linq.Dynamic.Core;
using System.Linq;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Azure.Core;
using Smadot.Models.DataBase;
using static Smadot.Models.Entities.FoliosCancelados.Request.FoliosCanceladosRequestData;
using Smadot.Web.Models;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Models.Dicts;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Smadot.Web.Controllers.SolicitudFormaValorada
{
    [Authorize]
    public class FoliosCanceladosController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;

        //Mensaje genérico de error
        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public FoliosCanceladosController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
        }

        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(PermisosMenu.FoliosCancelados))
                return new UnauthorizedResult();
            return View("~/Views/FoliosCancelados/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Consulta()
        {
            JsonResult result = new JsonResult("");
            //Invoca al método que se encarga de realizar la petición Api
            var request = new RequestList();

            int totalRecord = 0;
            int filterRecord = 0;

            var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

            var gridData = new ResponseGrid<FoliosCanceladosResponseGrid>();
            var listaResult = await FoliosCanceladosHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<FoliosCanceladosResponse> ?? new List<FoliosCanceladosResponse>();

            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total;

            filterRecord = string.IsNullOrEmpty(searchValue) ? totalRecord : data.Count();

            var List = new List<FoliosCanceladosResponseGrid>();

            foreach (var row in data)
            {
                List.Add(new FoliosCanceladosResponseGrid
                {
                    Folio = row.Folio,
                    FechaCancelacion = row.FechaCancelacion,
                    Motivo = (row.IdCatMotivoCancelacion == 3) ? string.IsNullOrEmpty(row.OtroMotivo) ? "Sin especificar" : row.OtroMotivo : row.Motivo,
                    UsuarioAprobo = row.UsuarioAprobo,
                    TipoTramite = row.TipoTramite,
                    DatosVehiculo = string.IsNullOrWhiteSpace(row.DatosVehiculo) ? null : row.DatosVehiculo,
                    PersonaRealizoTramite = row.PersonaRealizoTramite
                });
            }
            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            //var List = GetList();
            //var List = new List<FoliosCanceladosResponseGrid>();


            //if (!string.IsNullOrEmpty(request.Busqueda))
            //{
            //    List = List.Where(
            //        x => 
            //        x.Folio.ToLower().Contains(request.Busqueda.ToLower()) ||
            //        x.Motivo.ToLower().Contains(request.Busqueda.ToLower()) ||
            //        x.UsuarioAprobo.ToLower().Contains(request.Busqueda.ToLower()) || 
            //        x.TipoTramite.ToLower().Contains(request.Busqueda.ToLower())
            //        ).ToList();
            //}
            //if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento)) {
            //    var quer = List.AsQueryable();
            //    List = quer.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento).ToList();

            //}

            //List.Add(new EstadisiticaUsoFormaValoradaResponse() { }, new EstadisiticaUsoFormaValoradaResponse() { });
            //gridData.Data = List;
            //gridData.RecordsTotal = totalRecord;
            //gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
            //filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
            //gridData.RecordsFiltered = filterRecord;
            //gridData.Draw = draw;

            var returnObj = new
            {
                draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = List
            };

            return Json(returnObj);
        }

        public async Task<IActionResult> Registro()
        {
			var catalogo = await TipoCertificadoHelper.Consulta(_proxyWebAPI, new TipoCertificadoRequest { Activo = true });
            var user = _userResolver.GetUser();
            var certificados = (catalogo.Result as List<TipoCertificadoResponse> ?? new List<TipoCertificadoResponse>());
            if (!user.ClaveVerificentro.Equals("SMADSOT-00"))
                certificados = certificados.Where(x => x.Id != TipoCertificado.Exentos && x.Id != TipoCertificado.Testificacion).ToList();
			ViewBag.Certificados = certificados.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
			return View("~/Views/FoliosCancelados/Registro.cshtml");
        }


        [HttpPost]
        public async Task<JsonResult> ConsultaFolioAutocomplete(string prefix)
        {
            //var folio = GetList().Where(x => x.Folio.ToLower().Contains(prefix)).Take(10).ToList();
            //var folio = GetList().Take(10).ToList();
            var listaResult = await FoliosCanceladosHelper.AutocompleteFolio(_proxyWebAPI, prefix);
            var folio = listaResult.Result as List<FoliosCanceladosAutoCompleteResponse> ?? new List<FoliosCanceladosAutoCompleteResponse>();
            var result = folio.Select(x => new { Id = x.Id, Nombre = x.Folio });
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> DetalleFolio(int id)
        {
            //var data = GetById(id);
            var helper = await FoliosCanceladosHelper.GetById(_proxyWebAPI, id);
            var data = helper.Result as FoliosCanceladosResponse ?? new FoliosCanceladosResponse();
            //var titular = model.GetTitular(id);            
            return Json(new
            {
                Error = false,
                Data =
                           new
                           {
                               data.Id,
                               data.Folio,
                               data.FechaCancelacion,
                               data.Motivo,
                               DatosUsuario = data.UsuarioAprobo,
                               data.TipoTramite,
                               data.DatosVehiculo,
                               data.PersonaRealizoTramite,
                               data.Fecha,
                               data.IdCatMotivoCancelacion,
                               data.OtroMotivo
                           }
            });
        }

        [HttpPost]
        public async Task<JsonResult> DetalleFolioImpresion(int id)
        {
            //var data = GetById(id);
            var helper = await FoliosCanceladosHelper.GetById(_proxyWebAPI, id);
            var data = helper.Result as FoliosCanceladosResponse ?? new FoliosCanceladosResponse();
            //var titular = model.GetTitular(id);            
            return Json(new
            {
                Error = !helper.IsSuccessFully,
                Html = !helper.IsSuccessFully ? "" : await this.RenderViewToStringAsync("~/Views/FoliosCancelados/_RegistroCancelado.cshtml", data)

            });
        }

        [HttpPost]
        public async Task<JsonResult> CancelarFolio(FolioCanceladosRequest request)
        {
            var result = new JsonResultDto();
            try
            {
                var usuarioActual = _userResolver.GetUser();

                if (usuarioActual != null)
                {
                    request.IdUser = usuarioActual.IdUser;
                    request.PersonaAprobo = usuarioActual.Nombre;
                }

                var helper = await FoliosCanceladosHelper.CancelarFolio(_proxyWebAPI, request);

                if (!helper.IsSuccessFully)
                {
                    result.Error = true;
                    result.ErrorDescription = helper.Message ?? _mensajeErrorCatch;
                    return Json(result);
                }
                result.Error = false;
                result.ErrorDescription = helper.Message ?? "El Folio fue cancelado correctamente";

            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }
            return Json(result);
        }
        [HttpPost]
        public async Task<JsonResult> ReimpresionFolio(FolioCanceladosRequest request)
        {
            var result = new JsonResultDto();
            try
            {
                var usuarioActual = _userResolver.GetUser();

                // if (usuarioActual != null)
                // {
                //     request.IdUser = usuarioActual.IdUser;
                //     request.PersonaAprobo = usuarioActual.Nombre;
                // }
                var helper = await FoliosCanceladosHelper.ReimprimirFolio(_proxyWebAPI, request);

                if (!helper.IsSuccessFully)
                {
                    result.Error = true;
                    result.ErrorDescription = helper.Message ?? _mensajeErrorCatch;
                    return Json(result);
                }
                var resultHelper = helper.Result as ResponseGeneric<bool> ?? new ResponseGeneric<bool>();
                result.Error = !resultHelper.Response;
                result.ErrorDescription = resultHelper.mensaje ?? (resultHelper.Response ? "El Folio se generó correctamente" : "No se pudo generar el folio");

            }
            catch (Exception e)
            {
                result.Error = true;
                result.ErrorDescription = _mensajeErrorCatch;
            }
            return Json(result);
        }

        public int GetRandomDay()
        {
            Random rd = new Random();

            int rand_num = rd.Next(-20, 0);

            return rand_num;
        }

        public List<FoliosCanceladosResponseGrid> GetList()
        {
            var List = new List<FoliosCanceladosResponseGrid>()
            {
                new FoliosCanceladosResponseGrid() {
                    Id = 1,
                    Folio = "XBG-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Nissan Sentra LKL-5963",
                    PersonaRealizoTramite = "Pablo Lopez Herrera",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 2,
                    Folio = "ABC-3327",
                    FechaCancelacion = DateTime.Now.ToString("dd/MM/yyyy"),
                    Motivo = "Folio Defectuoso",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Nissan Tsuru JSO-8596",
                    PersonaRealizoTramite = "Gerardo Ortega Juarez",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 3,
                    Folio = "PEL-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Ford Explorer KSI-856",
                    PersonaRealizoTramite = "Juan Olvera Huerta",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 4,
                    Folio = "HHS-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Renault Koleos PWP-2266",
                    PersonaRealizoTramite = "Eduardo Escobar Herrera",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 5,
                    Folio = "KSJ-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Renault Clio ISU-7785",
                    PersonaRealizoTramite = "Edmundo Santos Morales",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 6,
                    Folio = "KWK-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Nissan Murano OWO-7855",
                    PersonaRealizoTramite = "Edgar Flores Ochoa",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 7,
                    Folio = "PWK-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Nissan Pathfinder IIE-7854",
                    PersonaRealizoTramite = "Domingo Ortiz Alvarez",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 8,
                    Folio = "SIS-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Nissan Juke PWP-4855",
                    PersonaRealizoTramite = "Cesar Torres Gomez",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 9,
                    Folio = "SJS-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Nissan X-Trail OWS-7458",
                    PersonaRealizoTramite = "Amanda Ramirez Chacon",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 10,
                    Folio = "PLE-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Jeep Cherokee UWJ-4963",
                    PersonaRealizoTramite = "Alejandro Rivera Chavez",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 11,
                    Folio = "PSL-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Jeep Renegade KSI-7515",
                    PersonaRealizoTramite = "Vicente Barrios Quevedo",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 12,
                    Folio = "PWK-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Ford EcoSport HSU-1856",
                    PersonaRealizoTramite = "Bernardo Soto Ramirez",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 13,
                    Folio = "JQN-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Ford Focus JET-8425",
                    PersonaRealizoTramite = "Oscar Huerta Flores",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 14,
                    Folio = "LWJ-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Ford Fiesta JSU-9374",
                    PersonaRealizoTramite = "Teresa Sanchez Sanchez",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 15,
                    Folio = "PIK-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Chevrolet Aveo JEU-7496",
                    PersonaRealizoTramite = "Julieta Ponce Marquez",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 16,
                    Folio = "IWK-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Chevrolet Spark KSJ-8544",
                    PersonaRealizoTramite = "Valentina Torres de la Rosa",
                    Fecha = ""
                },
                new FoliosCanceladosResponseGrid() {
                    Id = 17,
                    Folio = "WOL-3327",
                    FechaCancelacion = DateTime.Now.AddDays(GetRandomDay()).ToString("dd/MM/yyyy"),
                    Motivo = "Mala Impresion",
                    UsuarioAprobo = "Hector Martinez Perez",
                    TipoTramite = "Cancelación",
                    DatosVehiculo = "Chevrolet Trax LWP-9412",
                    PersonaRealizoTramite = "Martin Martinez Marquez",
                    Fecha = ""
                },
            };
            return List;
        }

        public FoliosCanceladosResponse GetById(int id)
        {
            var folio = new FoliosCanceladosResponse();

            var getRes = GetList().FirstOrDefault(x => x.Id == id);
            if (getRes != null)
            {
                folio.Folio = getRes.Folio;
                folio.TipoTramite = getRes.TipoTramite;

                folio.Fecha = getRes.Fecha ?? DateTime.Now.ToString("dd/MM/yyyy");

                folio.UsuarioAprobo = getRes.UsuarioAprobo;
                folio.DatosVehiculo = getRes.DatosVehiculo;
                folio.PersonaRealizoTramite = getRes.PersonaRealizoTramite;
                folio.FechaCancelacion = getRes.FechaCancelacion;
                folio.Motivo = getRes.Motivo;
                folio.UsuarioAprobo = getRes.UsuarioAprobo;
            }

            return folio;
        }
    }
}
