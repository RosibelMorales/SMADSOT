using Newtonsoft.Json;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Instalaciones.Request;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;
using static Smadot.Models.Entities.Instalaciones.Response.InstalacionResponseData;

namespace Smadot.Web.Helper.Operaciones.Mantenimiento
{
    public class InstalacionHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, RequestList req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);
                var request = await _proxyWebAPI.GetAsyncData<List<InstalacionResponse>>("gateway/Instalacion" + query);

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

        public static async Task<ResponseViewModel> GetById(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<InstalacionResponse>($"gateway/Instalacion/{id}");

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

        public static async Task<ResponseViewModel> ProveedorAutocomplete(IProxyWebAPI _proxyWebAPI, string prefix)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<BusquedaProveedorRequest>>($"gateway/ProveedorAutocomplete?request={prefix}");

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

        public static async Task<ResponseViewModel> GuardarInstalacion(IProxyWebAPI _proxyWebAPI, InstalacionApiRequestData registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("/gateway/Instalacion"), stringContent);

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
