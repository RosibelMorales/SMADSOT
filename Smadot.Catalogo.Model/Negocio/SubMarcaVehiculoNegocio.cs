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
    public class SubMarcaVehiculoNegocio : ISubMarcaVehiculoNegocio
    {
        private SmadotDbContext _context;

        public SubMarcaVehiculoNegocio(SmadotDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseGeneric<ResponseGrid<SelectListItem>>> Consulta(SubMarcaVehiculoRequest request)
        {
            try
            {
                var catalogo = _context.vTablaMaestras.AsQueryable();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);

                if (string.IsNullOrEmpty(request.Marca))
                {
                    return new ResponseGeneric<ResponseGrid<SelectListItem>>(new ResponseGrid<SelectListItem>());
                }
                // if (request.Anio != null)
                // {
                //     catalogo = catalogo.Where(x => request.Anio >= x.ANO_DESDE && request.Anio <= x.ANO_HASTA);
                // }

                // request.Marca = request?.Marca?.Replace(" ", "").ToLower();
                var busqueda = request?.Busqueda?.Replace(" ", "").ToLower();
                var catalogoAux = new List<vTablaMaestra>().AsQueryable();
                catalogoAux = catalogo.Where(x => (x.Marca.Equals(request.Marca)) &&
                (!string.IsNullOrEmpty(busqueda) ? (x.SubMarca.Replace("_", "").Replace(" ", "").Replace("-", "").ToLower().Contains(busqueda.ToLower())
                || busqueda.Contains(x.SubMarca.Replace("_", "").Replace(" ", "").Replace(" ", "").Replace("-", "").ToLower())) : true || x.IdCatSubmarcaVehiculo.Equals(busqueda))
                );
                var r = catalogo.ToList();
                if (!catalogoAux.Any() && !string.IsNullOrEmpty(request?.Busqueda))
                {
                    busqueda = request?.Busqueda?.Split(" ")[0].ToLower();
                    catalogoAux = catalogo.Where(x => (x.Marca.Equals(request.Marca)) &&
                                    (!string.IsNullOrEmpty(busqueda) ? (x.SubMarca.Replace("_", "").Replace(" ", "").Replace("-", "").ToLower().Contains(busqueda.ToLower())
                                    || busqueda.Contains(x.SubMarca.Replace("_", "").Replace(" ", "").Replace(" ", "").Replace("-", "").ToLower())) : true)
                                    );
                }
                catalogo = catalogoAux;
                var tot = catalogo.Count();
                if (request?.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new SelectListItem
                {
                    Text = $"{x.SubMarca} {x.ANO_DESDE} - {x.ANO_HASTA}",
                    //Value = request.SubmarcaClave ? x.IdCatSubmarcaVehiculo.ToString() : x.Id.ToString()
                    Value = x.IdRegistroSubMarca.ToString()
                }).ToListAsync();

                return new ResponseGeneric<ResponseGrid<SelectListItem>>(new ResponseGrid<SelectListItem>
                {
                    RecordsTotal = tot,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<SelectListItem>>(ex);
            }
        }
        public async Task<ResponseGeneric<ResponseGrid<SelectListItem>>> ConsultaSecundaria(SubMarcaVehiculoRequest request)
        {
            try
            {
                var catalogo = _context.vTablaMaestras.AsQueryable();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);

                // if (string.IsNullOrEmpty(request.Marca))
                // {
                //     return new ResponseGeneric<ResponseGrid<SelectListItem>>(new ResponseGrid<SelectListItem>());
                // }
                // if (request.Anio != null)
                // {
                //     catalogo = catalogo.Where(x => request.Anio >= x.ANO_DESDE && request.Anio <= x.ANO_HASTA);
                // }

                // request.Marca = request?.Marca?.Replace(" ", "").ToLower();
                // var busqueda = request?.Busqueda?.Replace(" ", "").ToLower();
                var catalogoAux = new List<vTablaMaestra>().AsQueryable();
                request.Busqueda ??= string.Empty;
                catalogoAux = catalogo.Where(x =>
                x.SubMarca.Replace("_", "").Replace(" ", "").Replace("-", "").ToLower().Contains(request.Busqueda.ToLower())
                || request.Busqueda.Contains(x.SubMarca.Replace("_", "").Replace(" ", "").Replace(" ", "").Replace("-", "").ToLower())
                || x.Marca.Replace("_", "").Replace(" ", "").Replace("-", "").ToLower().Contains(request.Busqueda.ToLower())
                || request.Busqueda.Contains(x.Marca.Replace("_", "").Replace(" ", "").Replace(" ", "").Replace("-", "").ToLower())
                || x.IdCatSubmarcaVehiculo.Equals(request.Busqueda));
                // var r = catalogo.ToList();
                // if (!catalogoAux.Any() && !string.IsNullOrEmpty(request?.Busqueda))
                // {
                //     busqueda = request?.Busqueda?.Split(" ")[0].ToLower();
                //     catalogoAux = catalogo.Where(x => (x.Marca.Equals(request.Marca)) &&
                //                     (!string.IsNullOrEmpty(busqueda) ? (x.SubMarca.Replace("_", "").Replace(" ", "").Replace("-", "").ToLower().Contains(busqueda.ToLower())
                //                     || busqueda.Contains(x.SubMarca.Replace("_", "").Replace(" ", "").Replace(" ", "").Replace("-", "").ToLower())) : true)
                //                     );
                // }
                catalogo = catalogoAux;
                var tot = catalogo.Count();
                if (request?.Pagina >= 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip(request.Pagina.Value * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new SelectListItem
                {
                    Text = $"{x.Marca}/{x.SubMarca} {x.ANO_DESDE} - {x.ANO_HASTA}",
                    //Value = request.SubmarcaClave ? x.IdCatSubmarcaVehiculo.ToString() : x.Id.ToString()
                    Value = x.IdRegistroSubMarca.ToString()
                }).ToListAsync();

                return new ResponseGeneric<ResponseGrid<SelectListItem>>(new ResponseGrid<SelectListItem>
                {
                    RecordsTotal = tot,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<SelectListItem>>(ex);
            }
        }
    }

    public interface ISubMarcaVehiculoNegocio
    {
        Task<ResponseGeneric<ResponseGrid<SelectListItem>>> ConsultaSecundaria(SubMarcaVehiculoRequest request);
        public Task<ResponseGeneric<ResponseGrid<SelectListItem>>> Consulta(SubMarcaVehiculoRequest request);
    }
}
