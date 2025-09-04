using Newtonsoft.Json;
using Smadot.Models.Entities.AcreditacionTecnicoVerificador.Request;
using Smadot.Models.Entities.AcreditacionTecnicoVerificador.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Personal.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Text;

namespace Smadot.Web.Helper.Operaciones.DirectorioCentrosVerificacion
{
    public class AcreditacionTecnicoVerificadorHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, SolicitudFormaValoradaListRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<AcreditacionTecnicoVerificadorGridResponse>>("gateway/AcreditacionTecnicoVerificador" + query);

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

        public static async Task<ResponseViewModel> Registro(IProxyWebAPI _proxyWebAPI, AcreditacionTecnicoSolicitudRequest req)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(req);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<string>(string.Format("gateway/AcreditacionTecnicoVerificador"), stringContent);

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

        public static async Task<ResponseViewModel> RegistroEvidencia(IProxyWebAPI _proxyWebAPI, AcreditacionTecnicoEvidenciaRequest req)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(req);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("gateway/AcreditacionTecnicoVerificador/RegistroEvidencia"), stringContent);

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

        public static async Task<ResponseViewModel> AutorizarAcreditacion(IProxyWebAPI _proxyWebAPI, AutorizarAcreditacionRequest req)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(req);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("gateway/AcreditacionTecnicoVerificador/AutorizarAcreditacion"), stringContent);

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
                var request = await _proxyWebAPI.GetAsyncData<AcreditacionTecnicoVerificadorGridResponse>("gateway/AcreditacionTecnicoVerificador/" + id);

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

        public static async Task<ResponseViewModel> GetEvidenciaBySolicitud(IProxyWebAPI _proxyWebAPI, string solicitud)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<AcreditacionTecnicoEvidenciaResponse>("gateway/AcreditacionTecnicoVerificador/GetEvidenciaBySolicitud?solicitud=" + solicitud);

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

        public static async Task<ResponseViewModel> Autocomplete(IProxyWebAPI _proxyWebAPI, EmpleadoAutocompletRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<EmpleadoAutocompleteResponse>>("gateway/AcreditacionTecnicoVerificador/Autocomplete" + query);

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

        //public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, long id, bool verificacion)
        //{
        //    try
        //    {
        //        var request = await _proxyWebAPI.GetAsyncData<RefrendoGridResponse>($"gateway/Refrendo/{id}/{verificacion}");

        //        if (request != null && request.Status == ResponseStatus.Success)
        //        {
        //            return await Task.FromResult(new ResponseViewModel(isSucces: request.Status == ResponseStatus.Success ? true : false, request.Response));
        //        }
        //        else
        //        {
        //            return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseViewModel(ex);
        //    }
        //}

        //public static async Task<ResponseViewModel> Edit(IProxyWebAPI _proxyWebAPI, RefrendoRequest registro)
        //{
        //    try
        //    {
        //        var jsonSerialize = JsonConvert.SerializeObject(registro);
        //        var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
        //        var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("gateway/Refrendo"), stringContent);

        //        if (request != null && request.Status == ResponseStatus.Success)
        //        {
        //            return await Task.FromResult(new ResponseViewModel(isSucces: request.Response > 0, request.Response));
        //        }
        //        else
        //        {
        //            return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException));
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseViewModel(ex);
        //    }
        //}

        //public static async Task<ResponseViewModel> Autocomplete(IProxyWebAPI _proxyWebAPI, RefrendoAutocompletRequest req)
        //{
        //    try
        //    {
        //        var query = _proxyWebAPI.ToQueryString(req);

        //        var request = await _proxyWebAPI.GetAsyncData<List<RefrendoAutocompleteResponse>>("gateway/Refrendo/Autocomplete" + query);

        //        if (request != null && request.Status == ResponseStatus.Success)
        //        {
        //            return await Task.FromResult(new ResponseViewModel(isSucces: request.Status == ResponseStatus.Success ? true : false, request.Response));
        //        }
        //        else
        //        {
        //            return await Task.FromResult(new ResponseViewModel(isSucces: false, request.CurrentException));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseViewModel(ex);
        //    }
        //}
    }
}
