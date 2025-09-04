using Smadot.Utilities.Modelos;

namespace Smadot.Utilities.ProxyWebAPI.Interfaces
{
    /// <summary>
    /// Contrato encargado de ejecutar la acciones de comunicacion con los endpoins de la api
    /// </summary>
    public interface IProxyWebAPI
    {
        /// <summary>
        ///  Método encargado ejecutar acciones get con los endpoins 
        /// </summary>
        /// <typeparam name="T">Tipo generico </typeparam>
        /// <param name="serviceUrl">Url donde a puntara los endpoins</param>
        /// <param name="token">JWT de la api</param>
        /// <param name="goAuthenticated">Bandera encarga para enviar JWT</param>
        /// <returns></returns>
        Task<ResponseGeneric<T>> GetAsyncData<T>(string serviceUrl);


        /// <summary>
        /// Método encargado ejecutar acciones Post con los endpoins 
        /// </summary>
        /// <typeparam name="T">Tipo generico </typeparam>
        /// <param name="serviceUrl">Url donde a puntara los endpoins</param>
        /// <param name="dataObject">Objecto generico a serializar</param>
        /// <param name="token">JWT de la api</param>
        /// <param name="goAuthenticated">Bandera encarga para enviar JWT</param>
        /// <returns></returns>
        Task<ResponseGeneric<T>> PostAsyncData<T>(string serviceUrl, HttpContent dataObject, bool application_json = true);

        /// <summary>
        /// Método encargado ejecutar acciones Put con los endpoins 
        /// </summary>
        /// <typeparam name="T">Tipo generico </typeparam>
        /// <param name="serviceUrl">Url donde a puntara los endpoins</param>
        /// <param name="dataObject">Objecto generico a serializar</param>
        /// <param name="token">JWT de la api</param>
        /// <param name="goAuthenticated">Bandera encarga para enviar JWT</param>
        /// <returns></returns>
        Task<ResponseGeneric<T>> PutAsyncData<T>(string serviceUrl, StringContent? dataObject);

        /// <summary>
        /// Método encargado ejecutar acciones Patch con los endpoins 
        /// </summary>
        /// <typeparam name="T">Tipo generico </typeparam>
        /// <param name="serviceUrl">Url donde a puntara los endpoins</param>
        /// <param name="dataObject">Objecto generico a serializar</param>
        /// <param name="token">JWT de la api</param>
        /// <param name="goAuthenticated">Bandera encarga para enviar JWT</param>
        /// <returns></returns>
        Task<ResponseGeneric<T>> PatchAsync<T>(string serviceUrl, StringContent dataObject);

        /// <summary>
        /// Método encargado ejecutar acciones Delete con los endpoins 
        /// </summary>
        /// <typeparam name="T">Tipo generico </typeparam>
        /// <param name="serviceUrl">Url donde a puntara los endpoins</param>
        /// <param name="dataObject">Objecto generico a serializar</param>
        /// <param name="token">JWT de la api</param>
        /// <param name="goAuthenticated">Bandera encarga para enviar JWT</param>
        /// <returns></returns>
        Task<ResponseGeneric<T>> DeleteAsync<T>(string serviceUrl);

        string ToQueryString<T>(T parameters);
        string ToQueryStringDateTimes<T>(T parameters);
    }

}
