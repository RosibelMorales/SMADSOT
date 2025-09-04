using Smadot.Models.Entities.Generic.Response;
using Newtonsoft.Json;
using System.Text;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Utilities.Modelos;
using Smadot.Models.Entities.Autenticacion.Request;
using Smadot.Models.Entities.Autenticacion.Response;

namespace Smadot.Web.Helper.Operaciones.Autenticacion
{
    /// <summary>
    /// Helper encargardo de procesar el inicio de sesion
    /// </summary>
    public class AutenticacionHelper
    {
        #region Operaciones API
        /// <summary>
        /// Método de procesar la solitud ala api de autenticacion
        /// </summary>
        /// <param name="_proxyWebAPI">Cannal Proxy API</param>
        /// <param name="inicioSesion">Objeto del view model</param>
        /// <returns></returns>
        public static async Task<ResponseViewModel> Autenticacion(IProxyWebAPI _proxyWebAPI, AutenticacionRequest inicioSesion)
        {
            try
            {
                var sonValidos = ValidacionCampos(inicioSesion);

                if (!sonValidos.IsSuccessFully)
                    return sonValidos;

                var jsonSerialize = JsonConvert.SerializeObject(new { usuario = inicioSesion.Usuario, contrasenia = inicioSesion.Contrasenia, idVerificentro = inicioSesion.IdVerificentro });
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<TokenResponse>(string.Format("/gateway/Autenticacion/Login"), stringContent);
                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: !string.IsNullOrEmpty(request.Response.Token), request.Response));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }
        public static async Task<ResponseViewModel> Registro(IProxyWebAPI _proxyWebAPI, RegistroRequest registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("/gateway/Autenticacion/Registro"), stringContent);
                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response, request.Response));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }

        public static async Task<ResponseViewModel> CambiarContrasenia(IProxyWebAPI _proxyWebAPI, CambioContraseniaRequest viewModel)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(viewModel);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("/gateway/Autenticacion/CambiarContrasenia"), stringContent);
                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response, request.Response));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }

        public static async Task<ResponseViewModel> CambiarVerificentro(IProxyWebAPI _proxyWebAPI, AutenticacionResponse viewModel)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(viewModel);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var response = await _proxyWebAPI.PostAsyncData<TokenResponse>(string.Format("/gateway/Autenticacion/CambiarVerificentro"), stringContent);
                if (response != null && response.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: string.IsNullOrEmpty(response.mensaje), response.Response));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, response.CurrentException));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }

        public static async Task<ResponseViewModel> GetVerificentrosByUsername(IProxyWebAPI _proxyWebAPI, string userName)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<AutenticacionVerificentrosResponse>>($"/gateway/Autenticacion/GetVerificentrosByUsername/{userName}");
                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Status == ResponseStatus.Success ? true : false, request.Response));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }
        #endregion

        #region Rutinas axuliraes
        /// <summary>
        /// Método de validar que los campos no se encuentren vacios
        /// </summary>
        /// <param name="inicioSesion"></param>
        /// <returns></returns>
        private static ResponseViewModel ValidacionCampos(AutenticacionRequest inicioSesion)
        {
            ResponseViewModel validacionRespuesta;
            if (inicioSesion == null)
            {
                validacionRespuesta = new ResponseViewModel(isSucces: false, msj: "¡Ocurrio un error, los datos se encuentran vacios!");
            }
            else if (string.IsNullOrEmpty(inicioSesion.Contrasenia))
            {
                validacionRespuesta = new ResponseViewModel(isSucces: false, msj: "¡Ocurrio un error, el campo de la contraseña se encuentra vacio!");
            }
            else if (string.IsNullOrEmpty(inicioSesion.Usuario))
            {
                validacionRespuesta = new ResponseViewModel(isSucces: false, msj: "¡Ocurrio un error, el campo del usuario se encuentra vacio!");
            }
            else if (inicioSesion.IdVerificentro == 0)
            {
                validacionRespuesta = new ResponseViewModel(isSucces: false, msj: "¡Ocurrio un error, el campo del verificentro se encuentra vacio!");
            }
            else
            {
                validacionRespuesta = new ResponseViewModel(isSucces: true);
            }

            return validacionRespuesta;
        }

        #endregion
    }
}
