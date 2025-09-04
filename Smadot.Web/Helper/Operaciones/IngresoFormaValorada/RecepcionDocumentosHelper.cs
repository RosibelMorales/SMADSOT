using Newtonsoft.Json;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.PortalCitas.Response;
using Smadot.Models.Entities.RecepcionDocumentos.Request;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;
using static Smadot.Models.Entities.OrdenServicio.Response.OrdenServicioResponseData;
using static Smadot.Models.Entities.ProveedorFolioServicio.Request.ProveedorFolioServicioRequestData;
using static Smadot.Models.Entities.ProveedorFolioServicio.Response.ProveedorFolioServicioResponseData;
using static Smadot.Models.Entities.RecepcionDocumentos.Request.RecepcionDocumentosRequestData;
using static Smadot.Models.Entities.RecepcionDocumentos.Response.RecepcionDocumentosResponseData;

namespace Smadot.Web.Helper.Operaciones.IngresoFormaValorada
{
    public class RecepcionDocumentosHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, RequestList req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<RecepcionDocumentosGridResponse>>("gateway/RecepcionDocumentos" + query);

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

        public static async Task<ResponseViewModel> GetById(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<RecepcionDocumentosGridResponse>($"gateway/RecepcionDocumentos/{id}");

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

        public static async Task<ResponseViewModel> Documentos(IProxyWebAPI _proxyWebAPI, RecepcionDocumentosRequest req)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(req);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<string>(string.Format("gateway/RecepcionDocumentos"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Status == ResponseStatus.Success ? true : false, request.mensaje));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request.mensaje ?? "Ocurrió un error en la conexión con el servidor."));
                }
            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }

        public static async Task<ResponseViewModel> Reagendar(IProxyWebAPI _proxyWebAPI, ReagendarCitaRequest req)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(req);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<PortalCitasRegistroResponse>(string.Format("gateway/RecepcionDocumentos/Reagendar"), stringContent);

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

        public static async Task<ResponseViewModel> ConsultaTablaMaestra(IProxyWebAPI _proxyWebAPI, CatalogoTablaMaestraRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<CatalogoTablaMaestraResponse>>("gateway/RecepcionDocumentos/ConsultaTablaMaestra" + query);

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

        public static async Task<ResponseViewModel> Autocomplete(IProxyWebAPI _proxyWebAPI, GenericSelect2AutocompleRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<DieselAutocompleteResponse>>("gateway/RecepcionDocumentos/Autocomplete" + query);

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

        public static async Task<ResponseViewModel> ConsultaSubDiesel(IProxyWebAPI _proxyWebAPI, SubDieselRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);
                var request = await _proxyWebAPI.GetAsyncData<List<SubDieselResponse>>("gateway/RecepcionDocumentos/ConsultaSubDiesel" + query);

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

        public static async Task<ResponseViewModel> CambioLinea(IProxyWebAPI _proxyWebAPI, long idVerificacion)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<LineaPendientes>>($"gateway/RecepcionDocumentos/CambiarLinea/{idVerificacion}");

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

        public static async Task<ResponseViewModel> CambiarLinea(IProxyWebAPI _proxyWebAPI, RecepcionDocumentosCambiarLineaRequest registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("gateway/RecepcionDocumentos/CambiarLinea"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response > 0, request.Response));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.mensaje ?? "Ocurrió un error al realizar la operación."));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }
    }
}
