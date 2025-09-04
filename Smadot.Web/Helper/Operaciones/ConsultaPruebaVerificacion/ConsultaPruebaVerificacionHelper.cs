using Smadot.Models.Entities.ConsultaPruebaVerificacion.Request;
using Smadot.Models.Entities.ConsultaPruebaVerificacion.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;

namespace Smadot.Web.Helper.Operaciones.ConsultaPruebaVerificacion
{
    public class ConsultaPruebaVerificacionHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, ConsultaPruebaVerificacionRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<ConsultaPruebaVerificacionResponse>>("gateway/ConsultaPruebaVerificacion" + query);

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

        public static async Task<ResponseViewModel> BusquedaPlacaSerie(IProxyWebAPI _proxyWebAPI, string prefix, string flag)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<BusquedaPlacaSerieRequest>>("gateway/ConsultaPruebaVerificacion/BuscarPlacaSerie?prefix=" + prefix + "&flag=" + flag);

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
