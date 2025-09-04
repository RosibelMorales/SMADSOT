using Newtonsoft.Json;
using Smadot.Models.Entities.ConstanciaUltimaVerificacion.Request;
using Smadot.Models.Entities.ConstanciaUltimaVerificacion.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Personal.Response;
using Smadot.Models.Entities.Refrendo.Request;
using Smadot.Models.Entities.Refrendo.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;

namespace Smadot.Web.Helper.Operaciones.Tramite
{
	public class ConstanciaUltimaVerificacionHelper
	{

		public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, SolicitudFormaValoradaListRequest req)
		{
			try
			{
				var query = _proxyWebAPI.ToQueryString(req);

				var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<ConstanciaUltimaVerificacionGridResponse>>("gateway/ConstanciaUltimaVerificacion" + query);

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

		public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, long id, bool verificacion)
		{
			try
			{
				var request = await _proxyWebAPI.GetAsyncData<ConstanciaUltimaVerificacionGridResponse>($"gateway/ConstanciaUltimaVerificacion/{id}/{verificacion}");

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
                var request = await _proxyWebAPI.GetAsyncData<ConstanciaUltimaVerificacionDetalleResponse>("gateway/ConstanciaUltimaVerificacion/" + id);

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
        public static async Task<ResponseViewModel> Edit(IProxyWebAPI _proxyWebAPI, ConstanciaUltimaVerificacionRequest registro)
		{
			try
			{
				var jsonSerialize = JsonConvert.SerializeObject(registro);
				var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
				var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("gateway/ConstanciaUltimaVerificacion"), stringContent);

				if (request != null && request.Status == ResponseStatus.Success)
				{
					return await Task.FromResult(new ResponseViewModel(isSucces: request.Response > 0, request.Response,request.mensaje));
				}
				else
				{
					return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.mensaje??"Ocurrió un error al estableccer conexión.",request?.CurrentException??""));
				}

			}
			catch (Exception ex)
			{
				return new ResponseViewModel(ex);
			}
		}

		public static async Task<ResponseViewModel> Autocomplete(IProxyWebAPI _proxyWebAPI, RefrendoAutocompletRequest req)
		{
			try
			{
				var query = _proxyWebAPI.ToQueryString(req);

				var request = await _proxyWebAPI.GetAsyncData<List<RefrendoAutocompleteResponse>>("gateway/ConstanciaUltimaVerificacion/Autocomplete" + query);

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
