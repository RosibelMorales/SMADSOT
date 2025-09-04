using Newtonsoft.Json;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.ReporteMensual.Response;
using Smadot.Models.Entities.Roles.Request;
using Smadot.Models.Entities.Roles.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;
using static Smadot.Models.Entities.Reportes.Response.ReporteTercerTantoResponseData;

namespace Smadot.Web.Helper.Operaciones.Roles
{
    public class RolesHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, RolesListRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);
                var request = await _proxyWebAPI.GetAsyncData<List<RolesResponse>>("gateway/SeguimientoCVV/Roles" + query);

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

        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<RolesResponse>>($"/gateway/SeguimientoCVV/Roles/{id}");

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

        public static async Task<ResponseViewModel> Edit(IProxyWebAPI _proxyWebAPI, List<RolesResponse> registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("/gateway/SeguimientoCVV/Roles"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response > 0, request.Response));
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
