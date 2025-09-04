using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Equipo.Request;
using Smadot.Models.Entities.Equipo.Response;
using Smadot.Models.Entities.Linea.Request;
using Smadot.Models.Entities.Linea.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;
using Smadot.Models.Entities.Refrendo.Request;
using Smadot.Utilities.ProxyWebAPI.Implementacion;

namespace Smadot.Web.Helper.Operaciones.AdministracionStock
{
    public class EquipoHelper
    {

        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, EquipoRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<vEquipoResponse>>("gateway/AdministracionStock/Equipo" + query);

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

        public static async Task<ResponseViewModel> Detalle(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<vEquipoResponse>($"gateway/AdministracionStock/Equipo/{id}");
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

        public static async Task<ResponseViewModel> Registro(IProxyWebAPI _proxyWebAPI, EquipoResponse registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("gateway/AdministracionStock/Equipo"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: true, request.Response, "La operación se realizó exitosamente."));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException, request.mensaje ?? "Ocurrió un error al establecer conexión con el servicio."));
                }
            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }

        public static async Task<ResponseViewModel> Validar(IProxyWebAPI _proxyWebApi, AuxValidarResponse registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var response = await _proxyWebApi.PutAsyncData<bool>(string.Format("gateway/AdministracionStock/Equipo"), stringContent);

                if (response != null && response.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: response.Response, response.Response));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, response.CurrentException));
                }
            }
            catch (Exception e)
            {
                return new ResponseViewModel(e);
            }
        }

        public static async Task<ResponseViewModel> ConsultaLinea(IProxyWebAPI _proxyWebAPI, LineaRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);
                var response = await _proxyWebAPI.GetAsyncData<List<LineaResponse>>(string.Format("gateway/AdministracionStock/Equipo/Linea" + query));

                if (response != null && response.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: response.Status == ResponseStatus.Success ? true : false, response.Response));
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

        public static async Task<ResponseViewModel> ActualizarEstatus(IProxyWebAPI _proxyWebApi, AuxEstatusResponse registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebApi.PostAsyncData<bool>(string.Format("gateway/AdministracionStock/Equipo/Validacion"), stringContent);

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
    }
}
