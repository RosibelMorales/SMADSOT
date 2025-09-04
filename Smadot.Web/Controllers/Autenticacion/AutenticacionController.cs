using System.Text;
using Smadot.Web.Helper.Identida;
using Smadot.Models.Entities.Generic.Response;
using Microsoft.AspNetCore.Mvc;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Utilities.Seguridad;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.Autenticacion;
using Smadot.Utilities.GestionTokens;
using Smadot.Models.Entities.Autenticacion.Request;
using Smadot.Utilities.Seguridad.Modelo;
using Smadot.Models.Entities.Autenticacion.Response;
using Smadot.Utilities.Modelos;

namespace Smadot.Web.Controllers
{
    /// <summary>
    /// Controller encargado de la auenticacion (Inicio de sesion)
    /// </summary>
    public class AutenticacionController : Controller
    {
        #region Propiedades IoC
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly ILogger<AutenticacionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserResolver _userResolver;
        #endregion

        #region Contructor
        /// <summary>
        /// Contructor inicial de auenticacion
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="proxyWebAPI"></param>
        public AutenticacionController(ILogger<AutenticacionController> logger,
                                       IProxyWebAPI proxyWebAPI,
                                       IConfiguration configuration, IUserResolver userResolver)
        {
            _logger = logger;
            _proxyWebAPI = proxyWebAPI;
            _configuration = configuration;
            _userResolver = userResolver;
        }
        #endregion

        #region Eventos Controller
        /// <summary>
        /// Método de regresar la vista principal del autenticacion
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var esAutenticado = HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            if (esAutenticado)
            {
                return RedirectToAction("Index", "EstadisiticaUsoFormaValorada");
            }

            return View("~/Views/Autenticacion/Index.cshtml");
        }

        /// <summary>
        /// Método  ejecutar el inicio de sesion 
        /// </summary>
        /// <param name="inicioSesion">Objeto de transporte</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public async Task<IActionResult> Autorizacion(AutenticacionRequest inicioSesion)
        {
            try
            {
                inicioSesion.Contrasenia = GestioEncriptacion.Cifrar(new SeguridadModelo
                {
                    Valor = inicioSesion.Contrasenia,
                    LlaveCifrado = _configuration["JWT:ClaveSecreta"]
                });

                var result = await AutenticacionHelper.Autenticacion(_proxyWebAPI, inicioSesion);
                if (result.IsSuccessFully)
                {
                    var autenticacion = result.Result as TokenResponse;


                    HttpContext.Session.SetString("WebSiteSMADSOT", IdentidaHelper.CreacionTokenWebSite(_configuration, autenticacion));
                    return new JsonResult(new ResponseViewModel(isSucces: true, msj: ""));
                }
                else
                {
                    return new JsonResult(new ResponseViewModel(isSucces: false, msj: result.Message));
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseViewModel(isSucces: false, msj: ex.Message));
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Registro()
        {
            return View(new RegistroRequest());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Registro(RegistroRequest registro)
        {
            try
            {
                registro.Contrasenia = GestioEncriptacion.Cifrar(new SeguridadModelo
                {
                    Valor = registro.Contrasenia,
                    LlaveCifrado = _configuration["JWT:ClaveSecreta"]
                });

                var result = await AutenticacionHelper.Registro(_proxyWebAPI, registro);
                if (result.IsSuccessFully)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CambiarContrasenia()
        {
            try
            {
                var usuarioActual = LecturaToken.UsuarioActualEnSesion(HttpContext);
                var model = new CambioContraseniaRequest
                {
                    IdUser = usuarioActual.IdUser
                };
                return View(model);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CambiarContrasenia(CambioContraseniaRequest viewModel)
        {
            try
            {
                viewModel.ContraseniaActual = GestioEncriptacion.Cifrar(new SeguridadModelo
                {
                    Valor = viewModel.ContraseniaActual,
                    LlaveCifrado = _configuration["JWT:ClaveSecreta"]
                });
                viewModel.ContraseniaConfirmar = GestioEncriptacion.Cifrar(new SeguridadModelo
                {
                    Valor = viewModel.ContraseniaConfirmar,
                    LlaveCifrado = _configuration["JWT:ClaveSecreta"]
                });

                var result = await AutenticacionHelper.CambiarContrasenia(_proxyWebAPI, viewModel);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CerrarSesion()
        {
            try
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Autenticacion");
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        /// <summary>
        /// Método  ejecutar el select de verificentros 
        /// </summary>
        /// <param name="usuario">Objeto de transporte</param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public async Task<IActionResult> GetVerificentros(string usuario)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario))
                    return new JsonResult(new ResponseViewModel(isSucces: true));
                var result = await AutenticacionHelper.GetVerificentrosByUsername(_proxyWebAPI, usuario);
                if (result.IsSuccessFully)
                {
                    return new JsonResult(new ResponseViewModel(isSucces: true, msj: "", result: result.Result));
                }
                else
                {
                    return new JsonResult(new ResponseViewModel(isSucces: false, msj: result.Message));
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseViewModel(isSucces: false, msj: ex.Message));
            }
        }

        /// <summary>
        /// Método para obtener 
        /// </summary>
        /// <param name="usuario">Objeto de transporte</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GetVerificentroChange(string searchParam)
        {
            try
            {
                var usuario = _userResolver.GetUser();
                if (string.IsNullOrEmpty(usuario.UserName))
                    return new JsonResult(new ResponseViewModel(isSucces: true));
                var result = await AutenticacionHelper.GetVerificentrosByUsername(_proxyWebAPI, usuario.UserName);

                if (result.IsSuccessFully)
                {
                    var listCvv = (List<AutenticacionVerificentrosResponse>)result.Result;
                    if (!string.IsNullOrEmpty(searchParam))
                    {
                        listCvv = listCvv.Where(x =>
                            x.Nombre.Normalize(NormalizationForm.FormD).ToLower().Contains(searchParam.ToLower())).ToList();
                        result.Result = listCvv;
                    }

                    usuario = _userResolver.GetUser();
                    if (!usuario.IdVerificentro.HasValue)
                        return new JsonResult(new ResponseViewModel(isSucces: false, msj: "El usuario no tiene Verificentro asignado."));
                    listCvv.Remove(listCvv.FirstOrDefault(x => x.Id == usuario.IdVerificentro.Value));
                    result.Result = listCvv;
                    return new JsonResult(new ResponseViewModel(isSucces: true, msj: "", result: result.Result));
                }
                else
                {
                    return new JsonResult(new ResponseViewModel(isSucces: false, msj: result.Message));
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseViewModel(isSucces: false, msj: ex.Message));
            }
        }
        /// <summary>
        /// Método para obtener 
        /// </summary>
        /// <param name="usuario">Objeto de transporte</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeVerificentro(int id)
        {
            try
            {
                var currentUserData = _userResolver.GetUser();
                var request = new AutenticacionResponse
                {
                    IdUser = currentUserData.IdUser,
                    UserName = currentUserData.UserName,
                    Email = currentUserData.Email ?? "",
                    Nombre = currentUserData.Nombre,
                    IdVerificentro = id,
                    ClaveVerificentro = currentUserData.ClaveVerificentro,
                    IdPuesto = currentUserData.IdPuesto,
                    IdNivelConsulta = currentUserData.IdNivelConsulta,
                    Roles = currentUserData.Roles,
                    RoleNames = currentUserData.RoleNames,
                    NombreVerificentro = string.Empty,
                    Permisos = currentUserData.Permisos,
                    CodigoError = currentUserData.CodigoError,
                    CicloVerificacion = currentUserData.CicloVerificacion,
                    IdCicloVerificacion = currentUserData.IdCicloVerificacion,
                    Semestre = currentUserData.Semestre
                };

                var result = await AutenticacionHelper.CambiarVerificentro(_proxyWebAPI, request);
                if (result.IsSuccessFully)
                {
                    HttpContext.Session.Clear();
                    HttpContext.Session.SetString("WebSiteSMADSOT", IdentidaHelper.CreacionTokenWebSite(_configuration, (TokenResponse)result.Result));
                    return Json(new ResponseViewModel(true));
                }
                var msj = ((TokenResponse)result.Result).Mensaje;
                return Json(new ResponseViewModel(false, null, msj));

            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel(true, null, "Ocurrió un error al realizar la operación."));
            }
        }
        #endregion
    }
}
