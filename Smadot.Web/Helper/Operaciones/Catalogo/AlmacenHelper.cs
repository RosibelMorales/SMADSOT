using Newtonsoft.Json;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;
using Smadot.Utilities.ProxyWebAPI.Implementacion;
using Smadot.Web.Models;

namespace Smadot.Web.Helper.Operaciones.Catalogo
{
    public class AlmacenHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, AlmacenRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<AlmacenResponse>>("gateway/Almacen" + query);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Status == ResponseStatus.Success ? true : false, request.Response) { AccesoTotalVerificentros = request.AccesoTotalVerificentros });
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
