using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;
using static Smadot.Models.Entities.FoliosCancelados.Request.FoliosCanceladosRequestData;
using static Smadot.Models.Entities.FoliosCancelados.Response.FoliosCanceladosResponseData;

namespace Smadot.Web.Helper.Operaciones.SolicitudFormaValorada
{
    public class FoliosCanceladosHelper
    {

        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, RequestList req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                //var request = await _proxyWebAPI.GetAsyncData<List<FoliosCanceladosResponse>>("api/FoliosCancelados" + query);
                var request = await _proxyWebAPI.GetAsyncData<List<FoliosCanceladosResponse>>("gateway/FoliosCancelados" + query);

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


        public static async Task<ResponseViewModel> CancelarFolio(IProxyWebAPI _proxyWebAPI, FolioCanceladosRequest registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("/gateway/FoliosCancelados"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response, request.Response, request.mensaje ?? "Ocurrió un error al cancelar el folio."));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, "Ocurrió un error al cancelar el folio.", request.CurrentException));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex) { Message = "Ocurrió un error al consultar el servicio." };
            }
        }
        public static async Task<ResponseViewModel> CalcularResultados(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format($"/gateway/CalcularResultados/{id}"), null);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response, request.Response, request.mensaje));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException, "Ocurrió un error al consultar el servicio."));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }
        public static async Task<ResponseViewModel> ReimprimirFolio(IProxyWebAPI _proxyWebAPI, FolioCanceladosRequest registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("gateway/EventosVerificentro/ReimprimirFolio"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: true, request));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, "Ocurrió un error al intentar generar el nuevo folio.", request.CurrentException));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex) { Message = "Ocurrió un error al consultar el servicio." };
            }
        }

        public static async Task<ResponseViewModel> GetById(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<FoliosCanceladosResponse>($"gateway/FoliosCancelados/{id}");

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Status == ResponseStatus.Success ? true : false, request.Response));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, "Ocurrió un error al intentar generar el nuevo folio.", request.CurrentException));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex) { Message = "Ocurrió un error al obtener la información." };
            }
        }

        public static async Task<ResponseViewModel> AutocompleteFolio(IProxyWebAPI _proxyWebAPI, string prefix)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<FoliosCanceladosAutoCompleteResponse>>($"gateway/FoliosAutocomplete?request={prefix}");

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Status == ResponseStatus.Success ? true : false, request.Response));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, "Ocurrió un error al obtener la información.", request.CurrentException));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex) { Message = "Ocurrió un error al consultar el servicio." };
            }
        }

    }
}
