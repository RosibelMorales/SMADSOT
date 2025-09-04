using Newtonsoft.Json;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.PortalCitas.Request;
using Smadot.Models.Entities.PortalCitas.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;

namespace Smadot.Web.Helper.Operaciones.SeguimientoCVV
{
    public class PortalCitaHelper
    {
        public static async Task<ResponseViewModel> GetPortalCitasCalendarioByIdCvv(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<PortalCitasResponse>("gateway/PortalCita/GetPortalCitasCalendarioByIdCvv?id=" + id);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Status == ResponseStatus.Success ? true : false, request.Response));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException, request.mensaje));
                }
            }
            catch (Exception ex)
            {
                return new ResponseViewModel(false, ex, "No se ha podido estableces comunicación con el servicio.");
            }
        }



        public static async Task<ResponseViewModel> GetCitaData(IProxyWebAPI _proxyWebAPI, PortalConsultaCitaRequest requestparams)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(requestparams);
                var request = await _proxyWebAPI.GetAsyncData<PortalCitasComprobanteResponse>($"gateway/PortalCita/GetCitaData{query}");

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Status == ResponseStatus.Success ? true : false, request.Response, msj: request.mensaje));
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

        public static async Task<ResponseViewModel> CancelarCita(IProxyWebAPI _proxyWebAPI, PortalCitaCancelarRequest req)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(req);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("gateway/PortalCita/CancelarCita"), stringContent);

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

        public static async Task<ResponseViewModel> Registro(IProxyWebAPI _proxyWebAPI, PortalCitaRequest req)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(req);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<PortalCitasRegistroResponse>(string.Format("gateway/PortalCita"), stringContent);

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

        public static async Task<ResponseViewModel> ConfirmarCita(IProxyWebAPI _proxyWebAPI, ConfirmarCita req)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(req);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<Response>(string.Format("gateway/PortalCita/ConfirmarCita"), stringContent);

                    if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Status == ResponseStatus.Success ? true : false, request.Response, request.Response.respuesta));
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
