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
    public class DahsboardVerificacionesHelper
    {
        public static async Task<ResponseViewModel> ConsultaEstadisticasDashboard(IProxyWebAPI _proxyWebAPI, EstadisticaUsoFormaValoradaGridRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryStringDateTimes(req);

                var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<EstadisiticaUsoFormaValoradaResponseGridAcciones>>("gateway/DashboardVerificaciones/ConsultaEstadisticasDashboard" + query);

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

        public static async Task<ResponseViewModel> ConsultaEstadisticaUsoFormaValorada(IProxyWebAPI _proxyWebAPI, EstadisticaUsoFormaValoradaRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryStringDateTimes(req);

                var request = await _proxyWebAPI.GetAsyncData<DashboardIndicadores>($"gateway/DashboardVerificaciones/ConsultaEstadisticaUsoFormaValorada" + query);

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
        public static async Task<ResponseViewModel> GetConteoCertificados(IProxyWebAPI _proxyWebAPI, EstadisticaUsoFormaValoradaRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryStringDateTimes(req);

                var request = await _proxyWebAPI.GetAsyncData<List<CertificadoMapeo>>($"gateway/DashboardVerificaciones/GetConteoCertificados" + query);

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


        public static async Task<ResponseViewModel> ConsultaGraficaBarras(IProxyWebAPI _proxyWebAPI, EstadisticaUsoFormaValoradaRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryStringDateTimes(req);

                var request = await _proxyWebAPI.GetAsyncData<List<VerificacionCVVRangoFecha>>($"gateway/DashboardVerificaciones/ConsultaGraficaBarras" + query);

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
        public static async Task<ResponseViewModel> ConsultaGraficaPie(IProxyWebAPI _proxyWebAPI, EstadisticaUsoFormaValoradaRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryStringDateTimes(req);

                var request = await _proxyWebAPI.GetAsyncData<List<DataPieChart>>($"gateway/DashboardVerificaciones/ConsultaGraficaPie" + query);

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
        public static async Task<ResponseViewModel> ConsultaGraficaGauge(IProxyWebAPI _proxyWebAPI, EstadisticaUsoFormaValoradaRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryStringDateTimes(req);

                var request = await _proxyWebAPI.GetAsyncData<List<DataGaugeChart>>($"gateway/DashboardVerificaciones/ConsultaGraficaGauge" + query);

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
