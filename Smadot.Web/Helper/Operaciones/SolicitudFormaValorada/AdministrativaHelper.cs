using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;
using static Smadot.Models.Entities.Administrativa.Request.AdministrativaRequestData;
using static Smadot.Models.Entities.Administrativa.Response.AdministrativaResponseData;

namespace Smadot.Web.Helper.Operaciones.SolicitudFormaValorada
{
    public class AdministrativaHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, RequestList req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                //var request = await _proxyWebAPI.GetAsyncData<List<FoliosCanceladosResponse>>("api/FoliosCancelados" + query);
                var request = await _proxyWebAPI.GetAsyncData<List<AdministrativaResponse>>("gateway/Administrativa" + query);

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


        public static async Task<ResponseViewModel> Guardar(IProxyWebAPI _proxyWebAPI, AdministrativaApiRequest registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("/gateway/Administrativa"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response, request.Response) { });
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.CurrentException ?? request.mensaje));
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
                var request = await _proxyWebAPI.GetAsyncData<AdministrativaResponse>($"gateway/Administrativa/{id}");

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

        public static async Task<ResponseViewModel> AutocompleteFolio(IProxyWebAPI _proxyWebAPI, string prefix)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<PruebaAutocompletePlacaApi>>($"gateway/PlacaAutocomplete?request={prefix}");

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

        public static async Task<ResponseViewModel> Eliminar(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.DeleteAsync<bool>($"gateway/Administrativa/{id}");

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

    }
}
