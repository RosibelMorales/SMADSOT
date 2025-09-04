using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Utilities.Modelos;
using System.Linq.Dynamic.Core;
using System.Net.NetworkInformation;

namespace Smadot.Catalogo.Model.Negocio
{
    public class TipoCertificadoNegocio : ITipoCertificado
    {
        private SmadotDbContext _context;

        public TipoCertificadoNegocio(SmadotDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseGeneric<List<TipoCertificadoResponse>>> Consulta (TipoCertificadoRequest request)
        {
            try
            {
                var catalogo = _context.CatTipoCertificados.AsQueryable();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);

                var tot = catalogo.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x => x.Id.ToString().Contains(request.Busqueda.ToLower()) || x.Nombre.ToLower().Contains(request.Busqueda.ToLower()));
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var resut = await catalogo.Select(x => new TipoCertificadoResponse
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Descripcion = x.Descripcion,
                    ClaveCertificado = x.ClaveCertificado,
                    Activo = x.Activo,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<TipoCertificadoResponse>>(resut);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<TipoCertificadoResponse>>(ex);
            }
        }
    }

    public interface ITipoCertificado
    {
        public Task<ResponseGeneric<List<TipoCertificadoResponse>>> Consulta(TipoCertificadoRequest request);
    }
}
