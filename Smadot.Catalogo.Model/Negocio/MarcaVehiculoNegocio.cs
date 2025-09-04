using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Smadot.Models.Entities.Generic.Response;
using System.Web.Mvc;

namespace Smadot.Catalogo.Model.Negocio
{
    public class MarcaVehiculoNegocio : IMarcaVehiculoNegocio
    {
        private SmadotDbContext _context;

        public MarcaVehiculoNegocio(SmadotDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseGeneric<ResponseGrid<SelectListItem>>> Consulta(MarcaVehiculoRequest request)
        {
            try
            {
                var catalogo = _context.vCatMarcaVehiculoNombres.AsQueryable();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);

                // if (!string.IsNullOrEmpty(request.Busqueda))
                // {
                //     catalogo = catalogo.Where(x => x.Id.ToString().Contains(request.Busqueda.ToLower()) || x.Nombre.ToLower().Contains(request.Busqueda.ToLower()));
                // }

                if (!string.IsNullOrEmpty(request.NombreMarca))
                {
                    request.NombreMarca = request.NombreMarca.Replace(" ", "").ToLower();
                    catalogo = catalogo.Where(x => x.Nombre.Replace("_", "").Replace("-", "").ToLower().Contains(request.NombreMarca.ToLower()) || request.NombreMarca.Contains(x.Nombre.Replace("_", "").Replace(" ", "").Replace("-", "").ToLower()));
                    var r = catalogo.ToList();
                }
                var tot = catalogo.Count();

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new SelectListItem
                {
                    Text = x.Nombre,
                    Value = x.Nombre
                }).ToListAsync();

                return new ResponseGeneric<ResponseGrid<SelectListItem>>(new ResponseGrid<SelectListItem>
                {
                    Data = result,
                    RecordsTotal = tot,
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<SelectListItem>>(ex);
            }
        }
    }

    public interface IMarcaVehiculoNegocio
    {
        Task<ResponseGeneric<ResponseGrid<SelectListItem>>> Consulta(MarcaVehiculoRequest request);
    }
}
