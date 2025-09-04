using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.DevolucionSPF.Response;
using Smadot.Models.Entities.FoliosFormaValorada.Request;
using Smadot.Models.Entities.FoliosFormaValorada.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.ProgramacionCalibracion.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.ProxyWebAPI.Implementacion;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;

namespace Smadot.Web.Helper.Operaciones.FoliosFormaValorada
{
    public class FoliosFormaValoradaHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, FolioFormaValoradaGridRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<FolioPendienteImprimirResponse>>("gateway/FolioFormaValorada/Consulta" + query);

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
        public static async Task<ResponseViewModel> ProximosFolios(IProxyWebAPI _proxyWebAPI)
        {
            try
            {

                var request = await _proxyWebAPI.GetAsyncData<FoliosProximosResponse>("/gateway/FolioFormaValorada/ProximosFolios");

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
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, GenerateImpresionRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);
                var request = await _proxyWebAPI.GetAsyncData<FolioFormaValoradaImpresionResponse>($"gateway/Verificacion/DataImpresion{query}");

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: true, request.Response) { Message = request.mensaje });
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException, request.mensaje ?? "Error en la conexión con el servicio."));
                }
            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }

        public static async Task<ResponseViewModel> DetalleCertificado(IProxyWebAPI _proxyWebAPI, GenerateImpresionRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<FolioFormaValoradaImpresionResponse>($"gateway/Verificacion/DataImpresion{query}");

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Status == ResponseStatus.Success ? true : false, request.Response));
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

        public static async Task<ResponseViewModel> Recalcular(IProxyWebAPI _proxyWebAPI, long idFolioFormaValorada, long idVerificentro)
        {
            try
            {
                var url = $"gateway/FolioFormaValorada/Recalcular?idFolioFormaValorada={idFolioFormaValorada}&idVerificentro={idVerificentro}";
                var request = await _proxyWebAPI.PutAsyncData<bool>(url, new StringContent(string.Empty));
                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: true, request.Response, "La operación se realizó exitosamente."));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException, request.mensaje ?? "Ocurrió un error al establecer conexión con el servicio."));
                }
            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }
    }
}
