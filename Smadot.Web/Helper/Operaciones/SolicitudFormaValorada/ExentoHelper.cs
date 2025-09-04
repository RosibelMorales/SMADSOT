using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Exento.Request;
using Smadot.Models.Entities.Exento.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Web.Models.Exento;

namespace Smadot.Web.Helper.Operaciones.SolicitudFormaValorada
{
    public class ExentoHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, ExentoRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<vExentoResponse>>("gateway/Exento" + query);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: true, request.Response, request.mensaje));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.mensaje ?? "Ocurrió un error al establecer conexión con el servicio.", request.CurrentException));
                }
            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }

        public static async Task<ResponseViewModel> Registro(IProxyWebAPI _proxyWebAPI, ExentoViewModel registro)
        {
            try
            {
                registro.UrlDoc1 = "A";
                registro.UrlDoc2 = "B";
                registro.UrlDoc3 = "C";
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("gateway/Exento"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response, request.Response, request.mensaje));
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
                var request = await _proxyWebAPI.GetAsyncData<ExentoResponse>($"gateway/Exento/{id}");

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
                var request = await _proxyWebAPI.DeleteAsync<bool>($"gateway/Exento/{id}");

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
