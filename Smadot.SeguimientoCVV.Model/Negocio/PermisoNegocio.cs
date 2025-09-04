using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Permiso.Request;
using Smadot.Models.Entities.Permiso.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.SeguimientoCVV.Model.Negocio
{
    public class PermisoNegocio : IPermisoNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        public PermisoNegocio(SmadotDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public async Task<ResponseGeneric<List<PermisoTreeRolesRequest>>> GetRoles()
        {
            try
            {
                var roles = await _context.Rols.Select(x => new PermisoTreeRolesRequest
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                }).ToListAsync();

                return new ResponseGeneric<List<PermisoTreeRolesRequest>>(roles);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<PermisoTreeRolesRequest>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<PermisoTreeResponse>>> GetPermisosTreeByRol(int rol)
        {
            try
            {
                var result = new List<PermisoTreeResponse>();
                var rolPermisos = _context.Rols.Include(x => x.IdPermisos).FirstOrDefault(x => x.Id == rol)?.IdPermisos.Select(x => x.Id).ToList() ?? new List<long>();
                result = await _context.Permisos.Select(x => new PermisoTreeResponse
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Descripcion= x.Descripcion,
                    IdPermisoPadre = x.IdPermisoPadre,
                    Selected = rolPermisos.Contains(x.Id)
                }).ToListAsync();
                return new ResponseGeneric<List<PermisoTreeResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<PermisoTreeResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> SavePermisos(PermisoTreeRequest request)
        {
            try
            {
                var result = false;
                var rol = _context.Rols.Include(x => x.IdPermisos).FirstOrDefault(x => x.Id == request.Rol) ?? new Rol();
                if (rol.IdPermisos!= null &&  rol.IdPermisos.Count > 0)
                {
                    rol.IdPermisos.Clear();
                    result = await _context.SaveChangesAsync() > 0;
                }

                var permisos = await _context.Permisos.Where(x => request.Permisos.Contains(x.Id)).ToListAsync();
                foreach (var p in permisos)
                {
                    rol.IdPermisos.Add(p);
                }
                result = await _context.SaveChangesAsync() > 0;

                return result ? new ResponseGeneric<long>(1) : new ResponseGeneric<long>();
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex);
            }
        }
    }

    public interface IPermisoNegocio
    {
        public Task<ResponseGeneric<List<PermisoTreeRolesRequest>>> GetRoles();
        public Task<ResponseGeneric<List<PermisoTreeResponse>>> GetPermisosTreeByRol(int Id);
        public Task<ResponseGeneric<long>> SavePermisos(PermisoTreeRequest request);
    }
}
