using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.POIFS.Crypt.Dsig;
using Polly;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static Smadot.Utilities.ServicioEventosVerificentro;

namespace Smadot.Utilities
{
    public class ServicioEventosVerificentro : IDisposable
    {
        private HttpClient httpClient;
        protected readonly string _apiEndPoint;
        private readonly string _apiKey;
        private bool disposed = false;

        public ServicioEventosVerificentro(string apiEndPoint, string apiKey)
        {
            _apiEndPoint = apiEndPoint;
            _apiKey = apiKey;
            httpClient = CreateHttpClient(_apiEndPoint, _apiKey);
        }
        protected virtual HttpClient CreateHttpClient(string serviceBaseAddress, string apiKey)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(serviceBaseAddress);
            httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            return httpClient;
        }
        public async Task<ResponseGeneric<TResponse>> PostAsync<TResponse, TType>(string actionName, TType data, string cvv, string errorBase = "")
        {
            var mensaje = "";
            try
            {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Local
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var responseMessage = await httpClient.PostAsync(actionName, content);
                //var problemDetailsHeader = responseMessage.Headers.TryGetValues("ProblemDetails", out var problemDetailsValues)
                //                             ? string.Join(", ", problemDetailsValues)
                //                             : null;

                var statusCode = (int)responseMessage.StatusCode;

                var responseContent = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.IsSuccessStatusCode)
                {
                    if (ValidateJson(responseContent))
                    {
                        var responseObject = JsonConvert.DeserializeObject<TResponse>(responseContent);
                        return new ResponseGeneric<TResponse>(responseObject);
                    }
                    return new ResponseGeneric<TResponse>() { Status = ResponseStatus.Success, mensaje = responseContent };

                }
                else
                {
                    var datosError = string.Empty;
                    JObject problemDetailsJson = new JObject();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        try
                        {
                            problemDetailsJson = JObject.Parse(responseContent);
                            datosError = problemDetailsJson["title"]?.ToString();
                        }
                        catch (Exception ex)
                        {
                            datosError = "El servicio esta en status 500.";
                        }
                    }
                    return new ResponseGeneric<TResponse>(mensaje) { mensaje = $"{mensaje} {datosError}", CurrentException = responseContent };
                }
                //var responseString = await responseMessage.Content.ReadAsStringAsync();
                //if (string.IsNullOrEmpty(responseString))
                //{
                //    return new ResponseGeneric<TResponse>();

                //}
                //var response = await responseMessage.Content.ReadFromJsonAsync<TResponse>();
                //return new ResponseGeneric<TResponse>(response);
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException is TimeoutException)
                {
                    mensaje = $"La solicitud al servicio del {cvv} excedió el tiempo de espera.";
                }
                else
                {
                    mensaje = $"No se pudo establecer comunicación al servicio del {cvv}.";
                }
                return new ResponseGeneric<TResponse>(mensaje) { mensaje = $"{errorBase} {mensaje}" };

            }
            catch (Exception e)
            {
                JObject dataEx = new JObject();
                dataEx["Exception"] = JsonConvert.SerializeObject(e);
                dataEx["Tipo"] = "ExepcionServicioCVV";
                return new ResponseGeneric<TResponse>(e) { mensaje = $"{errorBase}", CurrentException = JsonConvert.SerializeObject(dataEx) };

            }
        }

        public async Task<ResponseGeneric<TResponse>> PutAsync<TResponse, TType>(string actionName, TType data, string cvv, string errorBase = "")
        {
            var mensaje = errorBase;
            try
            {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Local
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var responseMessage = await httpClient.PutAsync(actionName, content);
                //var problemDetailsHeader = responseMessage.Headers.TryGetValues("ProblemDetails", out var problemDetailsValues)
                //                             ? string.Join(", ", problemDetailsValues)
                //                             : null;

                var statusCode = (int)responseMessage.StatusCode;

                var responseContent = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseObject = JsonConvert.DeserializeObject<TResponse>(responseContent);
                    // Procesa la respuesta y retorna una respuesta exitosa
                    return new ResponseGeneric<TResponse>(responseObject);

                }
                else
                {
                    var datosError = string.Empty;
                    JObject problemDetailsJson = new JObject();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        try
                        {
                            problemDetailsJson = JObject.Parse(responseContent);
                            datosError = problemDetailsJson["title"]?.ToString();
                        }
                        catch (Exception ex)
                        {
                            datosError = "El servicio esta en status 500.";
                        }
                    }
                    return new ResponseGeneric<TResponse>(mensaje) { mensaje = $"{mensaje} {datosError}", CurrentException = responseContent };
                }
                //var responseString = await responseMessage.Content.ReadAsStringAsync();
                //if (string.IsNullOrEmpty(responseString))
                //{
                //    return new ResponseGeneric<TResponse>();

                //}
                //var response = await responseMessage.Content.ReadFromJsonAsync<TResponse>();
                //return new ResponseGeneric<TResponse>(response);
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException is TimeoutException)
                {
                    mensaje = $"La solicitud al servicio del {cvv} excedió el tiempo de espera.";
                }
                else
                {
                    mensaje = $"No se pudo establecer comunicación al servicio del {cvv}.";
                }
                return new ResponseGeneric<TResponse>(mensaje) { mensaje = $"{errorBase} {mensaje}" };

            }
            catch (Exception e)
            {
                JObject dataEx = new();
                dataEx["Exception"] = JsonConvert.SerializeObject(e);
                dataEx["Tipo"] = "ExepcionServicioCVV";
                return new ResponseGeneric<TResponse>(e) { mensaje = $"{errorBase} {mensaje}", CurrentException = JsonConvert.SerializeObject(dataEx) };

            }
        }
        public async Task<ResponseGeneric<TResponse>> GetAsync<TResponse>(string actionName, string clave, string errorBase)
        {
            var mensaje = string.Empty;
            try
            {
                //     var json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
                //     {
                //         DateTimeZoneHandling = DateTimeZoneHandling.Local
                //     });
                //     var content = new StringContent(json, Encoding.UTF8, "application/json");
                var responseMessage = await httpClient.GetAsync(actionName);
                var statusCode = (int)responseMessage.StatusCode;
                var responseContent = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseObject = JsonConvert.DeserializeObject<TResponse>(responseContent);
                    // Procesa la respuesta y retorna una respuesta exitosa
                    return new ResponseGeneric<TResponse>(responseObject);

                }
                else
                {
                    var datosError = string.Empty;
                    JObject problemDetailsJson = new JObject();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        try
                        {
                            problemDetailsJson = JObject.Parse(responseContent);
                            datosError = problemDetailsJson["title"]?.ToString();
                        }
                        catch (Exception ex)
                        {
                            datosError = "El servicio esta en status 500.";
                        }
                    }
                    return new ResponseGeneric<TResponse>(mensaje) { mensaje = $"{mensaje} {datosError}", CurrentException = responseContent };
                }
            }
            catch (HttpRequestException e)
            {
                JObject dataEx = new JObject();
                dataEx["Exception"] = JsonConvert.SerializeObject(e);
                dataEx["Tipo"] = "ExepcionServicioCVV";
                if (e.InnerException is TimeoutException)
                {
                    mensaje = $"La solicitud al servicio del {clave} excedió el tiempo de espera.";
                }
                else
                {
                    mensaje = $"No se pudo establecer comunicación al servicio del {clave}.";
                }
                return new ResponseGeneric<TResponse>(mensaje) { mensaje = $"{errorBase} {mensaje}", CurrentException = JsonConvert.SerializeObject(dataEx) };

            }
            catch (Exception e)
            {
                JObject dataEx = new JObject();
                dataEx["Exception"] = JsonConvert.SerializeObject(e);
                dataEx["Tipo"] = "ExepcionServicioCVV";
                return new ResponseGeneric<TResponse>(e) { mensaje = $"{errorBase} {mensaje}", CurrentException = JsonConvert.SerializeObject(dataEx) };

            }


        }
        public async Task<ResponseGeneric<TResponse>> DeleteAsync<TResponse, TType>(string actionName, string clave, TType request, string errorBase)
        {
            var mensaje = string.Empty;
            try
            {
                //     var json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
                //     {
                //         DateTimeZoneHandling = DateTimeZoneHandling.Local
                //     });
                //     var content = new StringContent(json, Encoding.UTF8, "application/json");
                var query = ToQueryString(request);
                var responseMessage = await httpClient.DeleteAsync(actionName + query);
                var statusCode = (int)responseMessage.StatusCode;
                var responseContent = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseObject = JsonConvert.DeserializeObject<TResponse>(responseContent);
                    // Procesa la respuesta y retorna una respuesta exitosa
                    return new ResponseGeneric<TResponse>(responseObject);

                }
                else
                {
                    var datosError = string.Empty;
                    JObject problemDetailsJson = new JObject();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        try
                        {
                            problemDetailsJson = JObject.Parse(responseContent);
                            datosError = problemDetailsJson["title"]?.ToString();
                        }
                        catch (Exception ex)
                        {
                            datosError = "El servicio esta en status 500.";
                        }
                    }
                    return new ResponseGeneric<TResponse>(mensaje) { mensaje = $"{mensaje} {datosError}", CurrentException = responseContent };
                }
            }
            catch (HttpRequestException e)
            {
                JObject dataEx = new JObject();
                dataEx["Exception"] = JsonConvert.SerializeObject(e);
                dataEx["Tipo"] = "ExepcionServicioCVV";
                if (e.InnerException is TimeoutException)
                {
                    mensaje = $"La solicitud al servicio del {clave} excedió el tiempo de espera.";
                }
                else
                {
                    mensaje = $"No se pudo establecer comunicación al servicio del {clave}.";
                }
                return new ResponseGeneric<TResponse>(mensaje) { mensaje = $"{errorBase} {mensaje}", CurrentException = JsonConvert.SerializeObject(dataEx) };

            }
            catch (Exception e)
            {
                JObject dataEx = new JObject();
                dataEx["Exception"] = JsonConvert.SerializeObject(e);
                dataEx["Tipo"] = "ExepcionServicioCVV";
                return new ResponseGeneric<TResponse>(e) { mensaje = $"{errorBase} {mensaje}", CurrentException = JsonConvert.SerializeObject(dataEx) };

            }

        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (httpClient != null)
                {
                    httpClient.Dispose();
                }
                disposed = true;
            }
        }
        public string ToQueryString<T>(T parameters)
        {
            var lista = new List<string>();
            PropertyInfo[] props = parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetValue(parameters, null) != null).ToArray();
            foreach (PropertyInfo prop in props)
            {
                var propertyName = prop.GetCustomAttribute<JsonPropertyAttribute>().PropertyName;

                lista.Add($"{propertyName}={prop.GetValue(parameters)}");
            }

            return $"?{String.Join("&", lista.ToArray())}";
        }
        private bool ValidateJson(string json)
        {
            var valid = true;
            try
            {
                var tmpObj = JsonValue.Parse(json);
            }
            catch (FormatException)
            {
                valid = false;
            }
            catch (Exception) //some other exception
            {
                valid = false;
            }
            return valid;
        }
    }
}
