
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using System.Net.Http.Headers;
using System.Reflection;
using Smadot.Utilities.GestionTokens;

namespace Smadot.Utilities.ProxyWebAPI.Implementacion
{
    /// <summary>
    /// Clase encargada de ejecutar el contrato de IProxyWebAPI
    /// </summary>
    public class ProxyWebAPI : IProxyWebAPI
    {
        #region Propiedades
        protected readonly IHttpClientFactory _proxyFactory;
        private HttpClient _client;
        private string _clientUrlAPI;
        private IUserResolver _userResolver;
        #endregion

        #region Constructors
        /// <summary>
        /// Contructor encargado de inyectarla clase el canal de comunicación con los endpoist
        /// </summary>
        /// <param name="httpClientFactory">Fabrica de canales con los endpoins</param>
        /// <param name="clientUrlAPI">Url de la apii</param>
        public ProxyWebAPI(IHttpClientFactory httpClientFactory,
                            string clientUrlAPI,
                            IUserResolver userResolver)
        {
            _proxyFactory = httpClientFactory;
            _clientUrlAPI = clientUrlAPI;
            _userResolver = userResolver;
        }
        #endregion

        #region Métodos publicos
        /// <summary>
        ///  Método encargado ejecutar acciones get con los endpoins 
        /// </summary>
        /// <typeparam name="T">Tipo generico </typeparam>
        /// <param name="serviceUrl">Url donde a puntara los endpoins</param>
        /// <param name="token">JWT de la api</param>
        /// <param name="goAuthenticated">Bandera encarga para enviar JWT</param>
        /// <returns></returns>
        public async Task<ResponseGeneric<T>> GetAsyncData<T>(string serviceUrl)
        {
            ResponseGeneric<T> value;
            _client = _proxyFactory.CreateClient(_clientUrlAPI);
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            if (!string.IsNullOrEmpty(_userResolver.GetToken()))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userResolver.GetToken());

            var asyncPoly = CreateAsyncPolicy();
            try
            {
                return await asyncPoly.ExecuteAsync(async () =>
                {
                    using var response = await _client.GetAsync(serviceUrl);
                    using (HttpContent content = response.Content)
                    {
                        var contenResponse = await content.ReadAsStringAsync();
                        //value = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<ResponseGeneric<T>>(contenResponse) : new ResponseGeneric<T>(new Exception(contenResponse));
                        value = JsonConvert.DeserializeObject<ResponseGeneric<T>>(contenResponse) ?? new ResponseGeneric<T>(new Exception(contenResponse));
                        return value;
                    }
                });
            }
            catch (HttpRequestException ex)
            {
                return new ResponseGeneric<T>(ex);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<T>(ex);
            }
            finally
            {
                _client.Dispose();
            }
        }

        /// <summary>
        /// Método encargado ejecutar acciones Post con los endpoins 
        /// </summary>
        /// <typeparam name="T">Tipo generico </typeparam>
        /// <param name="serviceUrl">Url donde a puntara los endpoins</param>
        /// <param name="dataObject">Objecto generico a serializar</param>
        /// <param name="token">JWT de la api</param>
        /// <param name="goAuthenticated">Bandera encarga para enviar JWT</param>
        /// <returns></returns>
        public async Task<ResponseGeneric<T>> PostAsyncData<T>(string serviceUrl, HttpContent dataObject, bool application_json = true)
        {
            ResponseGeneric<T> value;
            _client = _proxyFactory.CreateClient(_clientUrlAPI);
            if (application_json)
                _client.DefaultRequestHeaders.Add("Accept", "application/json");

            if (!string.IsNullOrEmpty(_userResolver.GetToken()))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userResolver.GetToken());

            var asyncPoly = CreateAsyncPolicy();
            try
            {
                return await asyncPoly.ExecuteAsync(async () =>
                {
                    using var response = await _client.PostAsync(serviceUrl, dataObject);
                    using (HttpContent content = response.Content)
                    {
                        var contenResponse = await content.ReadAsStringAsync();
                        value = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<ResponseGeneric<T>>(contenResponse) : new ResponseGeneric<T>(new Exception(contenResponse));
                        return value;
                    }
                });

            }
            catch (HttpRequestException ex)
            {
                return new ResponseGeneric<T>(ex);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<T>(ex);
            }
            finally
            {
                _client.Dispose();
            }
        }

        /// <summary>
        /// Método encargado ejecutar acciones Put con los endpoins 
        /// </summary>
        /// <typeparam name="T">Tipo generico </typeparam>
        /// <param name="serviceUrl">Url donde a puntara los endpoins</param>
        /// <param name="dataObject">Objecto generico a serializar</param>
        /// <param name="token">JWT de la api</param>
        /// <param name="goAuthenticated">Bandera encarga para enviar JWT</param>
        /// <returns></returns>
        public async Task<ResponseGeneric<T>> PutAsyncData<T>(string serviceUrl, StringContent? dataObject)
        {
            ResponseGeneric<T> value;
            _client = _proxyFactory.CreateClient(_clientUrlAPI);
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            if (!string.IsNullOrEmpty(_userResolver.GetToken()))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userResolver.GetToken());

            var asyncPoly = CreateAsyncPolicy();
            try
            {
                return await asyncPoly.ExecuteAsync(async () =>
                {
                    using var response = await _client.PutAsync(serviceUrl, dataObject);
                    using (HttpContent content = response.Content)
                    {
                        var contenResponse = await content.ReadAsStringAsync();
                        value = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<ResponseGeneric<T>>(contenResponse) : new ResponseGeneric<T>(new Exception(contenResponse));
                        return value;
                    }
                });

            }
            catch (HttpRequestException ex)
            {
                return new ResponseGeneric<T>(ex);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<T>(ex);
            }
            finally
            {
                _client.Dispose();
            }
        }

        /// <summary>
        /// Método encargado ejecutar acciones Patch con los endpoins 
        /// </summary>
        /// <typeparam name="T">Tipo generico </typeparam>
        /// <param name="serviceUrl">Url donde a puntara los endpoins</param>
        /// <param name="dataObject">Objecto generico a serializar</param>
        /// <param name="token">JWT de la api</param>
        /// <param name="goAuthenticated">Bandera encarga para enviar JWT</param>
        /// <returns></returns>
        public async Task<ResponseGeneric<T>> PatchAsync<T>(string serviceUrl, StringContent dataObject)
        {
            ResponseGeneric<T> value;
            _client = _proxyFactory.CreateClient(_clientUrlAPI);
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            if (!string.IsNullOrEmpty(_userResolver.GetToken()))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userResolver.GetToken());

            var asyncPoly = CreateAsyncPolicy();
            try
            {
                return await asyncPoly.ExecuteAsync(async () =>
                {
                    using var response = await _client.PatchAsync(serviceUrl, dataObject);
                    using (HttpContent content = response.Content)
                    {
                        var contenResponse = await content.ReadAsStringAsync();
                        value = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<ResponseGeneric<T>>(contenResponse) : new ResponseGeneric<T>(new Exception(contenResponse));
                        return value;
                    }
                });

            }
            catch (HttpRequestException ex)
            {
                return new ResponseGeneric<T>(ex);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<T>(ex);
            }
            finally
            {
                _client.Dispose();
            }
        }

        /// <summary>
        /// Método encargado ejecutar acciones Delete con los endpoins 
        /// </summary>
        /// <typeparam name="T">Tipo generico </typeparam>
        /// <param name="serviceUrl">Url donde a puntara los endpoins</param>
        /// <param name="dataObject">Objecto generico a serializar</param>
        /// <param name="token">JWT de la api</param>
        /// <param name="goAuthenticated">Bandera encarga para enviar JWT</param>
        /// <returns></returns>
        public async Task<ResponseGeneric<T>> DeleteAsync<T>(string serviceUrl)
        {
            ResponseGeneric<T> value;
            _client = _proxyFactory.CreateClient(_clientUrlAPI);
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            if (!string.IsNullOrEmpty(_userResolver.GetToken()))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userResolver.GetToken());

            var asyncPoly = CreateAsyncPolicy();
            try
            {
                return await asyncPoly.ExecuteAsync(async () =>
                {
                    using var response = await _client.DeleteAsync(serviceUrl);
                    using (HttpContent content = response.Content)
                    {
                        var contenResponse = await content.ReadAsStringAsync();
                        value = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<ResponseGeneric<T>>(contenResponse) : new ResponseGeneric<T>(new Exception(contenResponse));
                        return value;
                    }
                });

            }
            catch (HttpRequestException ex)
            {
                return new ResponseGeneric<T>(ex);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<T>(ex);
            }
            finally
            {
                _client.Dispose();
            }
        }
        #endregion

        #region Rutinas Axuliares
        /// <summary>
        /// Crea la peticion de los numeros de intentos para la solicitud.
        /// </summary>
        /// <returns></returns>
        private AsyncRetryPolicy CreateAsyncPolicy()
        {
            return Policy
                  .Handle<Exception>()
                  .Or<HttpRequestException>()
                  .Or<TimeoutException>()
                  .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(.5 * Math.Pow(2, retryAttempt - 1)));
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
        public string ToQueryStringDateTimes<T>(T parameters)
        {
            var lista = new List<string>();
            PropertyInfo[] props = parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetValue(parameters, null) != null).ToArray();

            foreach (PropertyInfo prop in props)
            {
                var propertyName = prop.GetCustomAttribute<JsonPropertyAttribute>().PropertyName;
                var propertyValue = prop.GetValue(parameters);

                // Verificar si la propiedad es de tipo DateTime?
                if (prop.PropertyType == typeof(DateTime?)||prop.PropertyType == typeof(DateTime))
                {
                    var dateTimeValue = (DateTime?)propertyValue;
                    if (dateTimeValue.HasValue)
                    {
                        var dateTimeString = dateTimeValue.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                        lista.Add($"{propertyName}={dateTimeString}");
                    }
                    else
                    {
                        lista.Add($"{propertyName}=null");
                    }
                }
                else
                {
                    lista.Add($"{propertyName}={propertyValue}");
                }
            }

            return $"?{string.Join("&", lista)}";
        }
        #endregion

    }
}
