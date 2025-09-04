using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Smadot.Catalogo.Model.Negocio
{
    public class MotivoTramiteNegocio : IMotivoTramiteNegocio
    {

        private SmadotDbContext _context;

        public MotivoTramiteNegocio(SmadotDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseGeneric<List<MotivoTramiteResponse>>> Consulta(RequestList request)
        {
            try
            {
                ///Se agrego ordenamiento debido a que el grid mostraba los registros desordenados
                ///REVISAR OPCIONES
                var catalogo = _context.CatMotivoTramites.AsQueryable();
                //var catalogo = _context.vFoliosCancelados.AsQueryable();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }
                if (request.Activo)
                {
                    catalogo = catalogo.Where(x => x.Activo == request.Activo);
                }

                //SE OBTIENE EL TOTAL DE REGISTROS PARA INFORMACION EN EL GRID
                var tot = catalogo.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x =>
                                                    x.Nombre.Contains(request.Busqueda.ToLower()));
                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new MotivoTramiteResponse
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Activo = x.Activo,

                    Total = tot,
                }).ToListAsync();

                return new ResponseGeneric<List<MotivoTramiteResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<MotivoTramiteResponse>>(ex);
            }
        }
    }

    public interface IMotivoTramiteNegocio
    {
        public Task<ResponseGeneric<List<MotivoTramiteResponse>>> Consulta(RequestList request);
    }
}
