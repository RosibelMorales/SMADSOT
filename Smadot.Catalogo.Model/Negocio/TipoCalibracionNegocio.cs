using Microsoft.EntityFrameworkCore;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Utilities.Modelos;
using System.Linq.Dynamic.Core;

namespace Smadot.Catalogo.Model.Negocio
{
    public class TipoCalibracionNegocio : ITipoCalibracionNegocio
    {
        private SmadotDbContext _context;

        public TipoCalibracionNegocio(SmadotDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseGeneric<List<TipoCalibracionResponse>>> Consulta(TipoCalibracionRequest request)
        {
            try
            {
                var catalogo = _context.CatTipoCalibracions.AsQueryable();

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

                var resut = await catalogo.Select(x => new TipoCalibracionResponse
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Activo = x.Activo,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<TipoCalibracionResponse>>(resut);
            }
            catch(Exception ex)
            {
                return new ResponseGeneric<List<TipoCalibracionResponse>>(ex);
            }
        }
    }

    public interface ITipoCalibracionNegocio
    {
        public Task<ResponseGeneric<List<TipoCalibracionResponse>>> Consulta(TipoCalibracionRequest request);
    }
}
