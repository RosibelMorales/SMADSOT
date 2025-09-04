using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Ocsp;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Implementacion;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Smadot.Web.Helper.Operaciones.Catalogo
{
    public class MotivoTramiteHelper
    {
        public static async Task<ResponseViewModel> Consulta(IProxyWebAPI _proxyWebAPI, RequestList req)
        {
            try
            {
                var query = _proxyWebAPI.ToQueryString(req);

                //var request = await _proxyWebAPI.GetAsyncData<List<FoliosCanceladosResponse>>("api/FoliosCancelados" + query);
                var request = await _proxyWebAPI.GetAsyncData<List<MotivoTramiteResponse>>("gateway/MotivoTramite" + query);

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
                var request = await _proxyWebApi.GetAsyncData<List<MotivoTramiteResponse>>("gateway/Catalogo/MotivoTramite" + query);
                if (request.Status != ResponseStatus.Success)
                    throw new Exception(request.CurrentException);

                selectlist = request.Response.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Nombre
                }).ToList();
            }
            catch
            {
                throw new Exception("Error al cargar el catálogo");
            }
            return selectlist;
        }
    }
}
