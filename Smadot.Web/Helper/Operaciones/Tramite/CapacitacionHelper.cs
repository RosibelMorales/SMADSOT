using Newtonsoft.Json;
using Smadot.Models.Entities.Capacitacion.Request;
using Smadot.Models.Entities.Capacitacion.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Personal.Response;
using Smadot.Models.Entities.ReporteMensual.Request;
using Smadot.Models.Entities.ReporteMensual.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;
using System.Web.Mvc;

namespace Smadot.Web.Helper.Operaciones.Tramite
{
    public class CapacitacionHelper : Controller
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, CapacitacionListRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<CapacitacionResponse>>("/gateway/Capacitacion" + query);

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
				var request = await _proxyWebAPI.GetAsyncData<List<CapacitacionResponse>>($"/gateway/Capacitacion/{id}");

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

		public static async Task<ResponseViewModel> Edit(IProxyWebAPI _proxyWebAPI, List<CapacitacionResponse> registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("/gateway/Capacitacion"), stringContent);

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

        public static async Task<ResponseViewModel> GetByIdEmpleado(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<CapacitacionResponse>>("gateway/Capacitacion/GetByIdEmpleado?id=" + id);

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
