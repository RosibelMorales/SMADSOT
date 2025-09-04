using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.ConsultaSock.Request;
using Smadot.Models.Entities.ConsultaSock.Response;
using Smadot.Models.Entities.FoliosRegresadosSPF.Request;
using Smadot.Models.Entities.FoliosRegresadosSPF.Response;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Request;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Response;
using Smadot.Models.Entities.FoliosVendidosCentrosVerificacion.Request;
using Smadot.Models.Entities.FoliosVendidosCentrosVerificacion.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Refrendo.Request;
using Smadot.Models.Entities.Refrendo.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Models.Entities.StockMinimo.Request;
using Smadot.Models.Entities.StockMinimo.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;

namespace Smadot.Web.Helper.Operaciones.SolicitudFormaValorada
{
    public class RefrendoHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {

                var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<RefrendoGridResponse>>($"gateway/Refrendo/{id}");

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: true, request.Response));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.mensaje ?? "No se pudo establecer conexión con el servicio.", request.CurrentException));
                }
            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }

        public static async Task<ResponseViewModel> Detalle(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<RefrendoGridResponse>($"gateway/Refrendo/Detalle/{id}");

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

        public static async Task<ResponseViewModel> Edit(IProxyWebAPI _proxyWebAPI, RefrendoRequest registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("gateway/Refrendo"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response > 0, request.Response, request.mensaje ?? "La información se guardó correctamente."));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.mensaje ?? "Ocurrió un error al establecer conexión con el servicio.", request.CurrentException));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }

        public static async Task<ResponseViewModel> Autocomplete(IProxyWebAPI _proxyWebAPI, RefrendoAutocompletRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<RefrendoAutocompleteResponse>>("gateway/Refrendo/Autocomplete" + query);

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
        // No se usa
        // public static async Task<ResponseViewModel> GetPDFFormaValorada(IProxyWebAPI _proxyWebAPI, long id)
        // {
        //     try
        //     {
        //         var request = await _proxyWebAPI.GetAsyncData<vFoliosFormaValoradaExentosImpresion>($"gateway/Refrendo/PDFFormaValorada/{id}");

        //         if (request != null && request.Status == ResponseStatus.Success)
        //         {
        //             return await Task.FromResult(new ResponseViewModel(isSucces: request.Status == ResponseStatus.Success ? true : false, request.Response));
        //         }
        //         else
        //         {
        //             return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException));
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ResponseViewModel(ex);
        //     }
        // }
    }
}
