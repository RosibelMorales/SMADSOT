using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Smadot.Models.Entities.Administrativa.Response.AdministrativaResponseData;
using static Smadot.Models.Entities.ConsultaTablaMaestra.Response.ConsultaTablaMaestraResponseData;
using Smadot.Models.DataBase;
using Newtonsoft.Json;
using System.Text;
using Smadot.Models.Entities.ConsultaTablaMaestra.Request;
using Smadot.Models.Entities.ConsultaTablaMaestra.Response;
using Smadot.Models.Entities.Catalogos.Request;

namespace Smadot.Web.Helper.Operaciones.Tramite
{
    public class ConsultaTablaMaestraHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, RequestList req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                //var request = await _proxyWebAPI.GetAsyncData<List<FoliosCanceladosResponse>>("api/FoliosCancelados" + query);
                var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<ConsultaTablaMaestraResponseGrid>>("gateway/ConsultaTablaMaestra" + query);

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
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, SubMarcaVehiculoRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                //var request = await _proxyWebAPI.GetAsyncData<List<FoliosCanceladosResponse>>("api/FoliosCancelados" + query);
                var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<Smadot.Models.Entities.ConsultaTablaMaestra.Response.Catalogo>>("gateway/TablaMaestra/ConsultaSubMarca" + query);

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

        public static async Task<ResponseViewModel> ConsultaCicloVerificacion(IProxyWebAPI _proxyWebAPI, RequestList req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                //var request = await _proxyWebAPI.GetAsyncData<List<FoliosCanceladosResponse>>("api/FoliosCancelados" + query);
                var request = await _proxyWebAPI.GetAsyncData<List<ConsultaTablaMaestraCicloVerificacionResponse>>("gateway/ConsultaCicloVerificacion" + query);

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

        public static async Task<List<SelectListItem>> SelectList(IProxyWebAPI _proxyWebApi, RequestList req)
        {
            List<SelectListItem> selectlist = new();
            try
            {
                var query = _proxyWebApi.ToQueryString(req);
                var request = await _proxyWebApi.GetAsyncData<List<ConsultaTablaMaestraCicloVerificacionResponse>>("gateway/ConsultaCicloVerificacion" + query);
                if (request.Status != ResponseStatus.Success)
                    throw new Exception(request.CurrentException);

                selectlist = request.Response.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Nombre,
                    Selected = x.Activo.Value
                }).ToList();
            }
            catch
            {
                throw new Exception("Error al cargar la información");
            }
            return selectlist;
        }

        public static async Task<ResponseViewModel> AutocompleteMarca(IProxyWebAPI _proxyWebAPI, string prefix)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<TablaMaestraAutocompleteResponse>>($"gateway/AutocompleteMarca?prefix={prefix}");

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

        public static async Task<ResponseViewModel> AutocompleteSubmarca(IProxyWebAPI _proxyWebAPI, string prefix)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<TablaMaestraAutocompleteResponse>>($"gateway/AutocompleteSubmarca?prefix={prefix}");

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

        public static async Task<ResponseViewModel> AutocompleteLinea(IProxyWebAPI _proxyWebAPI, string prefix)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<TablaMaestraAutocompleteResponse>>($"gateway/AutocompleteLinea?prefix={prefix}");

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

        public static async Task<ResponseViewModel> AutocompleteModelo(IProxyWebAPI _proxyWebAPI, string prefix)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<TablaMaestraAutocompleteResponse>>($"gateway/AutocompleteModelo?prefix={prefix}");

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
        public static async Task<ResponseViewModel> Registro(IProxyWebAPI _proxyWebAPI, long Id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<RegistroTablaMaestraResponse>(string.Format($"/gateway/TablaMaestra/{Id}"));

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: true, request.Response, request.mensaje));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.CurrentException ?? "", request?.mensaje ?? "Ocurrió al intentar actualizar la tabla maestra."));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex) { Message = "Ocurrió un error al procesar la información." };
            }
        }
        public static async Task<ResponseViewModel> Registro(IProxyWebAPI _proxyWebAPI, TablaMaestraRequest requestdata)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(requestdata);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("/gateway/TablaMaestra"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: true, request.Response, request.mensaje));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.CurrentException ?? "", request?.mensaje ?? "Ocurrió al intentar actualizar la tabla maestra."));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex) { Message = "Ocurrió un error al procesar la información." };
            }
        }
        public static async Task<ResponseViewModel> Guardar(IProxyWebAPI _proxyWebAPI, TablaMaestraDbfRequest registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("/gateway/ActualizarTablaMaestra"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response, request.Response) { Message = request.mensaje });
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, request?.CurrentException ?? "", request?.mensaje ?? "Ocurrió al intentar actualizar la tabla maestra."));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }
    }
}
