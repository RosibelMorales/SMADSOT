using Newtonsoft.Json;
using Smadot.Models.Entities.Capacitacion.Response;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Request;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.IngresoFormaValorada.Response;
using Smadot.Models.Entities.Linea.Request;
using Smadot.Models.Entities.Linea.Response;
using Smadot.Models.Entities.Motivos.Response;
using Smadot.Models.Entities.SeguimientoCVV.Request;
using Smadot.Models.Entities.SeguimientoCVV.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;
using System.Web.Mvc;

namespace Smadot.Web.Helper.Operaciones.SeguimientoCVV
{
	public class CierreAperturaLineaHelper : Controller
	{
		public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, SeguimientoCVVListRequest req)
		{
			try
			{
				var query = _proxyWebAPI.ToQueryString(req);

				var request = await _proxyWebAPI.GetAsyncData<List<SeguimientoCVVResponse>>("/gateway/CierreAperturaLinea" + query);

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

		public static async Task<ResponseViewModel> DetalleLinea(IProxyWebAPI _proxyWebAPI, RequestListLinea req)
		{
			try
			{
				var query = _proxyWebAPI.ToQueryString(req);

				var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<LineaDetalle>>("/gateway/CierreAperturaLinea/GetDetalle" + query);

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
				var request = await _proxyWebAPI.GetAsyncData<SeguimientoCVVResponse>($"/gateway/SeguimientoCVV/CierreAperturaLinea/{id}");

				if (request != null && request.Status == ResponseStatus.Success)
				{
					return await Task.FromResult(new ResponseViewModel(isSucces: true, request.Response));
				}
				else
				{
					return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.mensaje ?? "No se pudo establecer conexión con el servicio.", request?.CurrentException ?? ""));
				}
			}
			catch (Exception ex)
			{
				return new ResponseViewModel(ex){Message="No se pudo establecer conexión con el servicio."};
			}
		}

		public static async Task<ResponseViewModel> Edit(IProxyWebAPI _proxyWebAPI, List<SeguimientoCVVResponse> registro)
		{
			try
			{
				var jsonSerialize = JsonConvert.SerializeObject(registro);
				var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
				var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("/gateway/CierreAperturaLinea"), stringContent);

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


		public static async Task<ResponseViewModel> GetByIdLineaCA(IProxyWebAPI _proxyWebAPI, long id)
		{
			try
			{
				var request = await _proxyWebAPI.GetAsyncData<SeguimientoCVVResponse>("gateway/SeguimientoCVV/CierreAperturaLinea/GetByCA?id=" + id);

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

		public static async Task<ResponseViewModel> DashboardLineaAperturaCierre(IProxyWebAPI _proxyWebAPI, DashboardLineaRequest registro)
		{
			try
			{
				var jsonSerialize = JsonConvert.SerializeObject(registro);
				var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
				var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("gateway/SeguimientoCVV/CierreAperturaLinea/DashboardLineaAperturaCierre"), stringContent);

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
