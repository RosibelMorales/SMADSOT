using Newtonsoft.Json;
using Smadot.Models.Entities.ConsultaSock.Request;
using Smadot.Models.Entities.ConsultaSock.Response;
using Smadot.Models.Entities.EstadisticasUsoFormaValorada.Request;
using Smadot.Models.Entities.FoliosRegresadosSPF.Request;
using Smadot.Models.Entities.FoliosRegresadosSPF.Response;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Request;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Response;
using Smadot.Models.Entities.FoliosVendidosCentrosVerificacion.Request;
using Smadot.Models.Entities.FoliosVendidosCentrosVerificacion.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Models.Entities.StockMinimo.Request;
using Smadot.Models.Entities.StockMinimo.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;

namespace Smadot.Web.Helper.Operaciones.SolicitudFormaValorada
{
    public class SolicitudFormaValoradaHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, SolicitudFormaValoradaListRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<SeguimientoSolicitudResponse>>("gateway/SolicitudFormaValorada" + query);

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
                var request = await _proxyWebAPI.GetAsyncData<List<SolicitudFormaValoradaResponse>>($"gateway/SolicitudFormaValorada/{id}");

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

        public static async Task<ResponseViewModel> Edit(IProxyWebAPI _proxyWebAPI, SolicitudFormaValoradaRequest registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("gateway/SolicitudFormaValorada"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response > 0, request.Response, request.mensaje));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.mensaje ?? "No se pudo establecer conexión con el servicio.", request?.CurrentException ?? ""));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }

        public static async Task<ResponseViewModel> ConsultaFoliosUsadosVentanilla(IProxyWebAPI _proxyWebAPI, FoliosUsadosEnVentanillaRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<FoliosUsadosEnVentanillaResponse>>("gateway/SolicitudFormaValorada/FoliosUsadosVentanilla" + query);

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

        public static async Task<ResponseViewModel> DetalleFoliosVentanilla(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<FoliosUsadosEnVentanillaResponse>($"gateway/SolicitudFormaValorada/FoliosUsadosVentanilla/{id}");

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

        public static async Task<ResponseViewModel> ConsultaFoliosRegresadosSPF(IProxyWebAPI _proxyWebAPI, FoliosRegresadosSPFRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<FoliosRegresadosSPFResponse>>("gateway/SolicitudFormaValorada/FoliosRegresadosSPF" + query);

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
        public static async Task<ResponseViewModel> DetalleFolioRegresadoSPF(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<FoliosRegresadosSPFResponse>($"gateway/SolicitudFormaValorada/FoliosRegresadosSPF/{id}");

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
        public static async Task<ResponseViewModel> ConsultaFoliosVendidosCentrosVerificacion(IProxyWebAPI _proxyWebAPI, FoliosVendidosCentrosVerificacionRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<FoliosVendidosCentrosVerificacionResponse>>("gateway/SolicitudFormaValorada/FoliosVendidosCentrosVerificacion" + query);

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
        public static async Task<ResponseViewModel> DetalleFoliosVendidosCentrosVerificacion(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<FoliosVendidosCentrosVerificacionResponse>($"gateway/SolicitudFormaValorada/FoliosVendidosCentrosVerificacion/{id}");

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

        public static async Task<ResponseViewModel> InformeConsultaStockDVRF(IProxyWebAPI _proxyWebAPI, ConsultaStockDVRFRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<ConsultaStockDVRFResponse>>("gateway/SolicitudFormaValorada/ConsultaStockDVRF" + query);

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

        public static async Task<ResponseViewModel> GetCatalogo(IProxyWebAPI _proxyWebAPI, int IdAlmacen)
        {
            try
            {
                var response = await _proxyWebAPI.GetAsyncData<ConsultaStockDVRFResponse>(string.Format($"gateway/SolicitudFormaValorada/ConsultaStockDVRF/{IdAlmacen}"));

                if (response != null && response.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: response.Status == ResponseStatus.Success ? true : false, response.Response));
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

        public static async Task<ResponseViewModel> GetStockMinimo(IProxyWebAPI _proxyWebAPI, StockMinimoRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);
                var response = await _proxyWebAPI.GetAsyncData<List<StockMinimoResponse>>(string.Format("gateway/SolicitudFormaValorada/StockMinimo" + query));

                if (response != null && response.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: response.Status == ResponseStatus.Success ? true : false, response.Response));
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

        public static async Task<ResponseViewModel> RegistroStockMinimo(IProxyWebAPI _proxyWebAPI, List<StockMinimoResponse> registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("gateway/SolicitudFormaValorada/StockMinimo"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response > 0, request.mensaje));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.CurrentException ?? "", request?.mensaje ?? "Error al establecer conexión con el servicio de consulta de stock."));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }





    }
}
