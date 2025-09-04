using Smadot.Models.Entities.DashboardLineas.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.PortalCitas.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;

namespace Smadot.Web.Helper.Operaciones.SeguimientoCVV
{
    public class VerificacionHelper
    {
        public static async Task<ResponseViewModel> ObetenerInformacionLineas(IProxyWebAPI _proxyWebAPI)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<DashboardLineaResponseData>>("/gateway/Verificacion/InfoLineas");

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: true, request.Response));
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
