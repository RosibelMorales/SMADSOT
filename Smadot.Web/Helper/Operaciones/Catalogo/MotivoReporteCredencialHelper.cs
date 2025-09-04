using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;

namespace Smadot.Web.Helper.Operaciones.Catalogo
{
    public class MotivoReporteCredencialHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, MotivoReporteCredencialRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);
                var request = await _proxyWebAPI.GetAsyncData<List<MotivoReporteCredencialResponse>>("gateway/Catalogo/MotivoReporteCredencial" + query);

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
