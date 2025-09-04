using Newtonsoft.Json;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;

namespace Smadot.Web.Helper.Operaciones.Catalogo
{
    public class CatalogosVerificacionHelper
    {
        public static async Task<ResponseViewModel> ConsultaTipoServicio(IProxyWebAPI _proxyWebAPI)
        {
            try
            {

                var request = await _proxyWebAPI.GetAsyncData<List<Smadot.Models.Entities.ConsultaTablaMaestra.Response.Catalogo>>("/gateway/Verificacion/Catalogos/TipoServicio");

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
        public static async Task<ResponseViewModel> ConsultaMotivosVerificacion(IProxyWebAPI _proxyWebAPI)
        {
            try
            {

                var request = await _proxyWebAPI.GetAsyncData<List<Smadot.Models.Entities.ConsultaTablaMaestra.Response.Catalogo>>("/gateway/Verificacion/Catalogos/MotivoVerificacion");

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
