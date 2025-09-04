using Newtonsoft.Json;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;
using static Smadot.Models.Entities.ConsultaCircular.Request.ConsultaCircularRequestData;
using static Smadot.Models.Entities.ConsultaCircular.Response.ConsultaCircularResponseData;

namespace Smadot.Web.Helper.Operaciones.SolicitudFormaValorada
{
	public class ConsultaCircularHelper
	{
		public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, RequestList req)
		{
			try
			{
				var query = _proxyWebAPI.ToQueryString(req);

				//var request = await _proxyWebAPI.GetAsyncData<List<FoliosCanceladosResponse>>("api/FoliosCancelados" + query);
				var request = await _proxyWebAPI.GetAsyncData<List<ConsultaCircularResponse>>("gateway/ConsultaCircular" + query);

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
				var request = await _proxyWebAPI.GetAsyncData<ConsultaCircularResponse>($"gateway/ConsultaCircular/{id}");

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

		public static async Task<ResponseViewModel> EnvioEmail(IProxyWebAPI _proxyWebAPI, ConsultaCircularRequest registro)
		{
			try
			{
				var jsonSerialize = JsonConvert.SerializeObject(registro);
				var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
				var response = await _proxyWebAPI.PostAsyncData<bool>(string.Format("gateway/ConsultaCircular"), stringContent);

				if (response != null && response.Status == ResponseStatus.Success)
				{
					return await Task.FromResult(new ResponseViewModel(isSucces: response.Response, response.Response));
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

        public static async Task<ResponseViewModel> ConfirmarCircular(IProxyWebAPI _proxyWebAPI, ConfirmarCircularRequest registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var response = await _proxyWebAPI.PostAsyncData<bool>(string.Format("gateway/ConfirmarCircular"), stringContent);

                if (response != null && response.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: response.Response, response.Response));
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

    }
}
