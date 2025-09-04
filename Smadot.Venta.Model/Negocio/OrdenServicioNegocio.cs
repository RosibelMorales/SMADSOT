using Microsoft.Extensions.Configuration;
using Smadot.Models.DataBase;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.Linq.Dynamic.Core;
using static Smadot.Models.Entities.OrdenServicio.Response.OrdenServicioResponseData;
using Microsoft.EntityFrameworkCore;
using Smadot.Models.Entities.Instalaciones.Request;
using Smadot.Models.Entities.OrdenServicio.Request;
using System.Globalization;

namespace Smadot.Venta.Model.Negocio
{
    public class OrdenServicioNegocio : IOrdenServicioNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        public OrdenServicioNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public async Task<ResponseGeneric<List<OrdenServicioResponse>>> Consulta(RequestList request)
        {
            try
            {
                var catalogo = _context.vOrdenServicios.AsQueryable();
                catalogo = catalogo.Where(o => o.IdVerificentro == _userResolver.GetUser().IdVerificentro);

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                var tot = catalogo.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x =>
                                                x.NombreVerificentro.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.NombreEquipo.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.NombreLinea.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.FolioOS.ToString().Contains(request.Busqueda) ||
                                                x.NombreUsuario.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                x.Solucion.ToLower().Contains(request.Busqueda.ToLower()));
                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new OrdenServicioResponse
                {
                    Id = x.Id,
                    Verificentro = x.NombreVerificentro,
                    Equipo = x.NombreEquipo,
                    Linea = x.NombreLinea,
                    FechaRegistro = x.FechaRegistro.ToString("dd/MM/yyyy"),
                    FolioServicio = x.FolioOS,
                    UserRegistro = x.NombreUsuario,
                    Solución = x.Solucion,
                    Total = tot,
                }).ToListAsync();

                return new ResponseGeneric<List<OrdenServicioResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<OrdenServicioResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<OrdenServicioResponse>> GetById(long Id)
        {
            try
            {
                var catalogo = _context.vOrdenServicios.AsQueryable();
                catalogo = catalogo.Where(x => x.Id == Id);
                var result = await catalogo.Select(x => new OrdenServicioResponse
                {
                    Id = x.Id,
                    CatTipoOrden = x.CatNombreTipoOrden,
                    FolioServicio = x.FolioOS,
                    UrlDocumento = x.UrlOS,
                    FechaRegistro = x.FechaRegistro.ToString("dd/MM/yyyy"),
                    UserRegistro = x.NombreUsuario,
                    Equipo = x.NombreEquipo,
                    Solución = x.Solucion,
                    NoSerieActual = x.NumeroSerieActual,
                    NoSerieAnterior = x.NumeroSerieAnterior,
                    Motivo = x.Motivo

                }).FirstOrDefaultAsync();

                return new ResponseGeneric<OrdenServicioResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<OrdenServicioResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<List<BusquedaProveedorRequest>>> ConsultaAutocomplete(string prefix)
        {
            try
            {
                var user = _userResolver.GetUser();
                var catalogo = _context.Equipos.Include(x => x.IdLineaNavigation).Where(x => (x.IdLineaNavigation != null && x.IdLineaNavigation.IdVerificentro == user.IdVerificentro) || x.IdVerificentro == user.IdVerificentro);
                catalogo = catalogo.Where(x => x.Nombre.ToLower().Contains(prefix.ToLower()) || x.NumeroSerie.ToLower().Contains(prefix.ToLower())).Take(10);
                var result = await catalogo.Select(x => new BusquedaProveedorRequest
                {
                    id = x.Id,
                    Text = x.Nombre + " / " + x.NumeroSerie + " / " + x.IdLineaNavigation.Nombre
                }).ToListAsync();

                return new ResponseGeneric<List<BusquedaProveedorRequest>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<BusquedaProveedorRequest>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<BusquedaProveedorRequest>>> CatTipoOrdenServicio()
        {
            try
            {
                var catalogo = _context.CatTipoOrdenServicios.AsQueryable();
                var result = await catalogo.Select(x => new BusquedaProveedorRequest
                {
                    id = x.Id,
                    Text = x.Nombre
                }).ToListAsync();

                return new ResponseGeneric<List<BusquedaProveedorRequest>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<BusquedaProveedorRequest>>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> GuardarOrdenServicio(OrdenServicioApiRequestData request)
        {
            try
            {
                var result = new ResponseGeneric<bool>();
                var folioPF = await _context.vProveedorFolios.FirstOrDefaultAsync(x => x.FolioPF == request.Folio);
                if (folioPF is null)
                {
                    result.Status = ResponseStatus.Failed;
                    result.mensaje = "No sé encontró el Folio ingresado.";
                    return result;
                }
                if (folioPF.FolioOS == request.Folio)
                {
                    result.Status = ResponseStatus.Failed;
                    result.mensaje = "El Folio ya se encuentra asignado a una Orden de Servicio.";
                    return result;
                }

                var data = new OrdenServicio
                {
                    IdCatTipoOrdenServicio = request.TipoOrdenServicio,
                    FolioOS = request.Folio,
                    UrlOS = string.Empty,
                    FechaRegistro = DateTime.Now,
                    IdUserRegistro = request.IdUserRegistro,
                    IdEquipo = request.IdEquipo,
                    Solucion = request.Solucion ?? string.Empty,
                    //data.NumeroSerieActual = request.NoSerieActual;
                    //data.NumeroSerieAnterior = request.NoSerieAnterior;
                    NumeroSerieActual = null,
                    NumeroSerieAnterior = null,
                    Motivo = request.Motivo ?? string.Empty,
                    IdFolioProveedor = folioPF.Id
                };
                _context.OrdenServicios.Add(data);
                var resultSave = await _context.SaveChangesAsync() > 0;

                if (resultSave)
                {
                    if (!string.IsNullOrEmpty(request.NoSerieActual))
                    {
                        var equipo = _context.Equipos.FirstOrDefault(x => x.Id == request.IdEquipo);
                        var numActual = equipo.NumeroSerie;

                        data.NumeroSerieActual = request.NoSerieActual;
                        data.NumeroSerieAnterior = numActual;

                        equipo.NumeroSerie = request.NoSerieActual;

                        await _context.SaveChangesAsync();
                    }
                }

                var url = await _blobStorage.UploadFileAsync(new byte[0], "OrdenServicio/" + data.Id + "/" + request.File.FirstOrDefault().Nombre, request.File.FirstOrDefault().Base64);
                data.UrlOS = url;
                resultSave = await _context.SaveChangesAsync() > 0;

                result.Response = true;
                return result;
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }
    }

    public interface IOrdenServicioNegocio
    {
        public Task<ResponseGeneric<List<OrdenServicioResponse>>> Consulta(RequestList request);
        public Task<ResponseGeneric<OrdenServicioResponse>> GetById(long Id);
        public Task<ResponseGeneric<List<BusquedaProveedorRequest>>> ConsultaAutocomplete(string prefix);
        public Task<ResponseGeneric<List<BusquedaProveedorRequest>>> CatTipoOrdenServicio();
        public Task<ResponseGeneric<bool>> GuardarOrdenServicio(OrdenServicioApiRequestData request);
    }
}
