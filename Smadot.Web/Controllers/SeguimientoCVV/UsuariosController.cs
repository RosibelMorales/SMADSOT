using Microsoft.AspNetCore.Mvc;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Roles.Response;
using Smadot.Models.Entities.Usuarios.Request;
using Smadot.Models.Entities.Usuarios.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Utilities.Seguridad.Modelo;
using Smadot.Utilities.Seguridad;
using Smadot.Web.Helper.Operaciones.Roles;
using Smadot.Web.Helper.Operaciones.Usuarios;
using Smadot.Web.Utilities;
using System.Linq.Dynamic.Core;
using Smadot.Models.DataBase;
using Polly;
using Smadot.Models.Dicts;
using Smadot.Web.Handler.Autorizacion;

namespace Smadot.Web.Controllers.SeguimientoCVV
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuider;
        private readonly IConfiguration _configuration;
        private readonly BlobStorage _blobStorage;

        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public UsuariosController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver, IPdfBuider pdfBuider, IConfiguration configuration)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
            _pdfBuider = pdfBuider;

            _configuration = configuration;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }
        public IActionResult Index()
        {
            if (!_userResolver.HasPermission(MenuUsuarios.PermisoMenuPadre))
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
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            //Invoca al método que se encarga de realizar la petición Api
            var request = new UsuariosListRequest();
            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();
            //long i;
            //if (long.TryParse(Request.Form["idAlmacen"].FirstOrDefault(), out i)) request.IdAlmacen = i;
            //else request.IdAlmacen = null;

            var listaResult = await UsuariosHelper.Consulta(_proxyWebAPI, request);
            var data = listaResult.Result as List<UsuariosResponse> ?? new List<UsuariosResponse>();

            ////Almacena el total de registros
            totalRecord = data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().Total.Value;

            ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? totalRecord : data.Count();

            var List = new List<UsuariosResponseGrid>();
            foreach (var row in data)
            {
                List.Add(new UsuariosResponseGrid
                {
                    Id = row.Id,
                    NombreUsuario = row.NombreUsuario,
                    NombreRol = row.NombreRol,
                    CorreoElectronico = row.CorreoElectronico,
                    LockoutEnabled = row.LockoutEnabled,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Usuarios/_Acciones.cshtml", row)
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
        public async Task<IActionResult> Edit(long? id)
        {

            var result = new ResponseViewModel(true);
            var vm = new List<UsuariosResponse>
            {
                new UsuariosResponse
                {
                    Id = 0
                }
            };
            try
            {
                var response = await UsuariosHelper.Consulta(_proxyWebAPI, id ?? 0);
                vm = response.Result as List<UsuariosResponse> ?? vm;
                result.Result = await this.RenderViewToStringAsync("~/Views/Usuarios/_Registro.cshtml", vm);
            }
            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = ex.Message;
            }

            return new JsonResult(result);



        }

        [HttpPost]
        public async Task<IActionResult> Edit(List<UsuariosResponse> registro)
        {
            var result = new ResponseViewModel(true);
            try
            {
                if (registro[0].IdUsuario > 0)
                {

                    result = await UsuariosHelper.Edit(_proxyWebAPI, registro);

                }
                else
                {
                    registro[0].Contrasenia = GestioEncriptacion.Cifrar(new SeguridadModelo
                    {
                        Valor = (registro[0].Contrasenia ?? "").ToLower(),
                        LlaveCifrado = _configuration["JWT:ClaveSecreta"]
                    });
                    result = await UsuariosHelper.Edit(_proxyWebAPI, registro);

                }
                if (result.IsSuccessFully)
                    result.Message = "La información se guardó correctamente.";

            }

            catch (Exception ex)
            {
                result.IsSuccessFully = false;
                result.Message = "Ocurrió un error al intentar guardar la información.";
            }

            return new JsonResult(result);

        }


        [HttpPost]
        public async Task<ActionResult> ActivarDesactivar(UsuarioActivarDesRequest request)
        {
            var result = new ResponseViewModel(true);
            try
            {
                result = await UsuariosHelper.ActivarDesactivar(_proxyWebAPI, request);
            }
            catch (Exception e)
            {
                result.IsSuccessFully = false;
                result.Message = e.Message;
            }

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> CambiarContrasenia(UpdatePasswordRequest request)
        {
            var result = new ResponseViewModel(true);
            try
            {
                result = await UsuariosHelper.ReestablecerContrasenia(_proxyWebAPI, request);
                if (result.IsSuccessFully)
                    result.Message = "Se ha reestablecido la contraseña con éxito.";
            }
            catch (Exception e)
            {
                result.IsSuccessFully = false;
                result.Message = e.Message;
            }

            return new JsonResult(result);
        }
    }
}
