using Amazon.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Roles.Request;
using Smadot.Models.Entities.Roles.Response;
using Smadot.Models.Entities.SeguimientoCVV.Request;
using Smadot.Models.Entities.SeguimientoCVV.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.SeguimientoCVV.Model.Negocio
{
    public class RolesNegocio : IRolesNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        public RolesNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public async Task<ResponseGeneric<List<RolesResponse>>> Consulta(RolesListRequest request)
        {
            try
            {
                var seguimiento = _context.Rols.Where(x => !x.Nombre.Equals("ADMIN SMADSOT"));


                if (!string.IsNullOrEmpty(request.Busqueda))
                {

                    seguimiento = seguimiento.Where(x => x.Nombre.ToLower().Contains(request.Busqueda.ToLower()) || x.Id.ToString().Contains(request.Busqueda.ToLower()) || x.Alias.ToLower().Contains(request.Busqueda.ToLower()) || x.AccesoTotalVerificentros.ToString().Contains(request.Busqueda.ToLower()));
                }

                var tot = seguimiento.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    seguimiento = seguimiento.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    seguimiento = seguimiento.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }


                DateTime now = DateTime.Now;
                var result = await seguimiento.Select(x => new RolesResponse
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Alias = x.Alias,
                    AccesoTotalVerificentros = x.AccesoTotalVerificentros,
                    Total = tot

                }).ToListAsync();

                return new ResponseGeneric<List<RolesResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<RolesResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<RolesResponse>>> GetById(long Id)
        {
            try
            {
                var result = new List<RolesResponse>();
                var roles = _context.Rols.Where(x => x.Id == Id);

                if (roles != null)
                {
                    string t = JsonConvert.SerializeObject(roles);
                    result = JsonConvert.DeserializeObject<List<RolesResponse>>(t);
                }

                return new ResponseGeneric<List<RolesResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<RolesResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> Registro(List<RolesResponse> request)
        {
            try
            {
                var rol = new Rol();
                var rols = _context.Rols.FirstOrDefault(x => x.Id == request[0].Id);
                if (rols != null)
                {
                    rols.Nombre = request[0].Nombre;
                    rols.Alias = request[0].Nombre;
                    rols.AccesoTotalVerificentros = (bool)request[0].AccesoTotalVerificentros;
                    _context.Update(rols);

                    var result = await _context.SaveChangesAsync() > 0;
                    return result ? new ResponseGeneric<long>(rols.Id) : new ResponseGeneric<long>();
                }
                else
                {
                    rol = new Rol
                    {
                        Nombre = request[0].Nombre,
                        Alias = request[0].Nombre,
                        AccesoTotalVerificentros = (bool)request[0].AccesoTotalVerificentros,
                    };
                    _context.Rols.Add(rol);
                    var result = await _context.SaveChangesAsync() > 0;
                    return result ? new ResponseGeneric<long>(rol.Id) : new ResponseGeneric<long>();

                }

            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex);
            }
        }
    }
}

public interface IRolesNegocio
{
    public Task<ResponseGeneric<List<RolesResponse>>> Consulta(RolesListRequest request);

    public Task<ResponseGeneric<List<RolesResponse>>> GetById(long Id);

    Task<ResponseGeneric<long>> Registro(List<RolesResponse> request);
}