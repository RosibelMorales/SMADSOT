using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Utilities.Modelos;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Smadot.Catalogo.Model.Negocio
{
    public class NormaAcreditacionNegocio : INormaAcreditacionNegocio
    {
        private SmadotDbContext _context;

        public NormaAcreditacionNegocio(SmadotDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseGeneric<List<NormaAcreditacionResponse>>> Consulta(NormaAcreditacionRequest request)
        {
            try
            {
                var catalogo = _context.NormaAcreditacions.AsQueryable();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);

                var tot = catalogo.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x => x.Id.ToString().Contains(request.Busqueda.ToLower()) || x.Clave.ToLower().Contains(request.Busqueda.ToLower()) || x.Descripcion.ToLower().Contains(request.Busqueda.ToLower()));
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var resut = await catalogo.Select(x => new NormaAcreditacionResponse
                {
                    Id = x.Id,
                    Clave = x.Clave,
                    Descripcion = x.Descripcion,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<NormaAcreditacionResponse>>(resut);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<NormaAcreditacionResponse>>(ex);
            }
        }
    }

    public interface INormaAcreditacionNegocio
    {
        public Task<ResponseGeneric<List<NormaAcreditacionResponse>>> Consulta(NormaAcreditacionRequest request);
    }
}
