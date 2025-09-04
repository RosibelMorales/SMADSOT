using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Models.Entities.CargaMasiva.Response;
using Smadot.Models.Entities.ConfiguracionLayout.Request;
using Smadot.Models.Entities.ConfiguracionLayout.Response;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Request;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Testificacion.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Models.EquipoTipoCalibracion;
using System.Text;

namespace Smadot.Web.Helper.Operaciones.DirectorioCentrosVerificacion
{
    public class DirectorioCentrosVerificacionHelper : Controller
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, DirectorioCentrosVerificacionListRequest req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                var request = await _proxyWebAPI.GetAsyncData<List<DirectorioCentrosVerificacionResponse>>("gateway/DirectorioCentrosVerificacion" + query);

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


        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<DirectorioCentrosVerificacionResponse>>($"gateway/DirectorioCentrosVerificacion/{id}");

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

        public static async Task<ResponseViewModel> EditCVV(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<NuevoVerificentroResponse>($"gateway/DirectorioCentrosVerificacion/EditCVV/{id}");

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
        public static async Task<ResponseViewModel> ConsultaFechas(IProxyWebAPI _proxyWebAPI, ConfiguradorCitaRequestList req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                //var request = await _proxyWebAPI.GetAsyncData<List<FoliosCanceladosResponse>>("api/FoliosCancelados" + query);
                var request = await _proxyWebAPI.GetAsyncData<ResponseGrid<ConfiguradorCitaResponse>>("gateway/ConfiguradorCitaList" + query);

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

        public static async Task<ResponseViewModel> GetConfiguradorByCVV(IProxyWebAPI _proxyWebAPI, long id)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<ConfiguradorCitaResponse>("gateway/DirectorioCentrosVerificacion/GetConfiguradorByCVV?id=" + id);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response.Id > 0, request.Response, request.mensaje));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, "Servicio no disponible, contacte con el administrador."));
                }
            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }


        public static async Task<ResponseViewModel> ConfiguradorCita(IProxyWebAPI _proxyWebAPI, List<ConfiguradorCitaDiasRequest> req)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(req);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("gateway/DirectorioCentrosVerificacion/ConfiguradorCita"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response,request.CurrentException, request.mensaje));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, "Ocurrió un error al consultar el servicio."));
                }   
            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }
        public static async Task<ResponseViewModel> ConfiguradorCita(IProxyWebAPI _proxyWebAPI, ConfiguradorCitaEditRequest req)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(req);  
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<EditFechaResponse>("gateway/DirectorioCentrosVerificacion/EditFecha", stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: true, request.Response, request.mensaje));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, "Ocurrió un error al consultar el servicio."));
                }
            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }

        public static async Task<ResponseViewModel> RegistroDiaNoLaboral(IProxyWebAPI _proxyWebAPI, DiaNoLaboralRequest req)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(req);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<string>(string.Format("gateway/DirectorioCentrosVerificacion/RegistroDiaNoLaboral"), stringContent);

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


        public static async Task<ResponseViewModel> ConfiguracionLayout(IProxyWebAPI _proxyWebAPI)
        {
            try
            {
                var request = await _proxyWebAPI.GetAsyncData<List<ConfiguracionLayoutResponse>>($"gateway/ConfiguracionLayout");

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

        public static async Task<ResponseViewModel> ConfiguracionLayout(IProxyWebAPI _proxyWebAPI, List<ConfiguracionLayoutResponse> registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<long>(string.Format("gateway/ConfiguracionLayout"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response > 0, request.Response));
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

        public static async Task<ResponseViewModel> RegistroCVV(IProxyWebAPI _proxyWebAPI, NuevoVerificentroResponse registro)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(registro);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("gateway/DirectorioCentrosVerificacion/RegistroCVV"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response, request.Response));
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

        public static async Task<ResponseViewModel> CargaMasiva(IProxyWebAPI _proxyWebAPI, DataCarga req)
        {
            try
            {
                var jsonSerialize = JsonConvert.SerializeObject(req);
                var stringContent = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
                var request = await _proxyWebAPI.PostAsyncData<bool>(string.Format("gateway/DirectorioCentrosVerificacion/GuardarCarga"), stringContent);

                if (request != null && request.Status == ResponseStatus.Success)
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: request.Response, request.CurrentException, request.mensaje));
                }
                else
                {
                    return await Task.FromResult(new ResponseViewModel(isSucces: false, "Ocurrió un error al consultar el servicio."));
                }
            }
            catch (Exception ex)
            {
                return new ResponseViewModel(ex);
            }
        }
    }
}
