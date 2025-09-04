using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Cita.Request;
using Smadot.Models.Entities.FoliosFormaValorada.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.PortalCita.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Implementacion;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;

namespace Smadot.Web.Helper.Operaciones
{
    public class CitasHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, CitaGridRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryStringDateTimes(req);

                var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<HistorialCitasResponse>>("gateway/PortalCita/Consulta" + query);

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
        public static async Task<ResponseViewModel> Expediente(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<vVerificacionExpediente>($"gateway/Verificacion/Expediente/{id}");

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

        public static async Task<ResponseViewModel> VolverImprimir(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(id);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var response = await _proxyWebAPI.PutAsyncData<bool>(string.Format("gateway/Verificacion/VolverImprimir"), stringContent);

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

        public static async Task<ResponseViewModel> ReiniciarCita(IProxyWebAPI _proxyWebAPI, ReiniciarCitaRequest registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("gateway/PortalCita/ReiniciarCita"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response > 0, request.Response));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.CurrentException ?? "", request?.mensaje ?? "Ocurrió un error al establecer la conexión con el servicio."));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }
    }
}
