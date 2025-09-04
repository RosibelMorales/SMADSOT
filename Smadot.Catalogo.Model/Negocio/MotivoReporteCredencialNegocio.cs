using Microsoft.EntityFrameworkCore;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Utilities.Modelos;
using System.Linq.Dynamic.Core;

namespace Smadot.Catalogo.Model.Negocio
{
    public class MotivoReporteCredencialNegocio : IMotivoReporteCredencialNegocio
    {
        private SmadotDbContext _context;

        public MotivoReporteCredencialNegocio(SmadotDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseGeneric<List<MotivoReporteCredencialResponse>>> Consulta (MotivoReporteCredencialRequest request)
        {
            try
            {
                var catalogo = _context.CatMotivoReporteCredencials.AsQueryable();

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

                var resut = await catalogo.Select(x => new MotivoReporteCredencialResponse
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Activo = x.Activo,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<MotivoReporteCredencialResponse>>(resut);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<MotivoReporteCredencialResponse>>(ex);
            }
        }
    }

    public interface IMotivoReporteCredencialNegocio
    {
        public Task<ResponseGeneric<List<MotivoReporteCredencialResponse>>> Consulta(MotivoReporteCredencialRequest request);
    }
}
