using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.Linq.Dynamic.Core;
using System.Net.NetworkInformation;


namespace Smadot.Catalogo.Model.Negocio
{
    public class AlmacenNegocio : IAlmacenNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        public AlmacenNegocio(SmadotDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public async Task<ResponseGeneric<List<AlmacenResponse>>> Consulta(AlmacenRequest request)
        {
            try
            {
                var catalogo = _context.Almacens.AsQueryable();
                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                var user = _userResolver.GetUser();
                var rol = user.RoleNames.FirstOrDefault();
                var acceso = _context.Rols.ToList().Where(o => o.Nombre.Equals(rol, StringComparison.Ordinal)).FirstOrDefault();
                if (!acceso.AccesoTotalVerificentros)
                {
                    if (user.ClaveVerificentro.Equals("SMADSOT-00"))
                        catalogo = catalogo.Where(x => x.IdVerificentro == null).AsQueryable();
                    else
                        catalogo = catalogo.Where(x => x.IdVerificentro == user.IdVerificentro).AsQueryable();
                }

                //if (request.VerificentrosNulos)
                //{
                //    catalogo = catalogo.Where(x => (x.IdVerificentro == null || x.IdVerificentro == _userResolver.GetUser().IdVerificentro));
                //}
                //if (request.IdVerificentroDestino != null)
                //{
                //    catalogo = catalogo.Where(x => x.IdVerificentro == request.IdVerificentroDestino);
                //}

                var tot = catalogo.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x => x.Id.ToString().Contains(request.Busqueda.ToLower()) || x.Nombre.ToLower().Contains(request.Busqueda.ToLower()));
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var resut = await catalogo.Select(x => new AlmacenResponse
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Activo = x.Activo,
                    IdVerificentro = x.IdVerificentro,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<AlmacenResponse>>(resut) { AccesoTotalVerificentros = acceso.AccesoTotalVerificentros };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<AlmacenResponse>>(ex);
            }
        }
        public async Task<ResponseGeneric<AlmacenResponse>> Consulta(long Id)
        {
            try
            {
                var catalogo = _context.Almacens.Select(obj => new AlmacenResponse
                {
                    Id = obj.Id,
                    Nombre = obj.Nombre,
                    Activo = obj.Activo,
                    IdVerificentro = obj.IdVerificentro,
                }).FirstOrDefault(x => x.Id == Id);
                if (catalogo == null)
                    return new ResponseGeneric<AlmacenResponse>(new Exception("No se encontro el almacen"));

                return new ResponseGeneric<AlmacenResponse>(catalogo);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<AlmacenResponse>(ex);
            }
        }
    }

    public interface IAlmacenNegocio
    {
        public Task<ResponseGeneric<List<AlmacenResponse>>> Consulta(AlmacenRequest request);
        public Task<ResponseGeneric<AlmacenResponse>> Consulta(long Id);
    }
}
