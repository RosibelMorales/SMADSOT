using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.AsignacionStock.Request;
using Smadot.Models.Entities.AsignacionStock.Response;
using Smadot.Models.Entities.DevolucionSPF.Request;
using Smadot.Models.Entities.DevolucionSPF.Response;
using Smadot.Models.Entities.FoliosFormaValorada.Response;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Request;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Refrendo.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Collections.Generic;
using System.Text;
using static Smadot.Models.Entities.ConsultaTablaMaestra.Response.ConsultaTablaMaestraResponseData;

namespace Smadot.Web.Helper.Operaciones.AdministracionStock
{
    public class AdministracionStockHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, RequestList req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<ListAsignacionStockResponse>>("gateway/AdministracionStock/AsignacionStock" + query);

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
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI,long id)
        {
            try
            {

                var request = await _proxyWebAPI.GetAsyncData<RegistroStockventanillaResponse>($"gateway/AdministracionStock/AsignacionStock/{id}" );

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
        public static async Task<ResponseViewModel> ConsultaDevlucionSPF(IProxyWebAPI _proxyWebAPI, RequestList req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<ListDevolucionSPFResponse>>("/gateway/AdministracionStock/DevolucionSPF" + query);

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
        public static async Task<ResponseViewModel> Guardar(IProxyWebAPI _proxyWebAPI, RegistroStockventanillaRequest registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var response = await _proxyWebAPI.PostAsyncData<string>(string.Format("gateway/AdministracionStock/AsignacionStock"), stringContent);

                if (response != null && response.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: string.IsNullOrEmpty(response.Response), response.Response));
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

        public static async Task<ResponseViewModel> GuardarDevolucionSPF(IProxyWebAPI _proxyWebAPI, DevolucionSPFRequest registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var response = await _proxyWebAPI.PostAsyncData<string>(string.Format("gateway/AdministracionStock/DevolucionSPF"), stringContent);

                if (response != null && response.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: string.IsNullOrEmpty(response.Response), response.Response));
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

        public static async Task<ResponseViewModel> AutocompleteSolicitudFV(IProxyWebAPI _proxyWebAPI, string prefix)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<SolicitudFV>>($"gateway/AutocompleteSolicitud?prefix={prefix}");

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

        public static async Task<ResponseViewModel> AutocompleteResponsable(IProxyWebAPI _proxyWebAPI, string prefix)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<User>>($"gateway/AutocompleteResponsable?prefix={prefix}");

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

        public static async Task<ResponseViewModel> ConsultaByIdDevolucion(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {

                var request = await _proxyWebAPI.GetAsyncData<DevolucionSPFResponse>($"gateway/AdministracionStock/DevolucionSPF/{id}");

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

		public static async Task<ResponseViewModel> GetInventarioByAlmacen(IProxyWebAPI _proxyWebAPI, long almacen)
		{
			try
			{
				var request = await _proxyWebAPI.GetAsyncData<List<TablaFijaViewModel.TablaFijaRow>>($"gateway/GetInventarioByAlmacen?id={almacen}");

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

		public static async Task<ResponseViewModel> GetNumeroDevolucion(IProxyWebAPI _proxyWebAPI)
		{
			try
			{
				var request = await _proxyWebAPI.GetAsyncData<int>($"gateway/GetNumeroDevolucion");

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

        public static async Task<ResponseViewModel> GetDevolucionSPFDocumento(IProxyWebAPI _proxyWebAPI, RequestList req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<DevolucionSPFResponse>>("gateway/AdministracionStock/DevolucionSPF/GetDoc" + query);

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
