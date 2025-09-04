using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Instalaciones.Request;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.Linq.Dynamic.Core;
using static Smadot.Models.Entities.Instalaciones.Response.InstalacionResponseData;

namespace Smadot.DirectorioCentrosVerificacion.Model.Negocio
{
    public class InstalacionNegocio : IInstalacionNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        public InstalacionNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public async Task<ResponseGeneric<List<InstalacionResponse>>> Consulta(RequestList request)
        {
            try
            {
                var catalogo = _context.vInstalacions.AsQueryable();
                catalogo = catalogo.Where(o => o.IdVerificentro == _userResolver.GetUser().IdVerificentro);

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                var tot = catalogo.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x => x.NombreProveedor.ToLower().Contains(request.Busqueda.ToLower()) || x.NombreUsuario.ToLower().Contains(request.Busqueda.ToLower()));
                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new InstalacionResponse
                {
                    IdInstalacion = x.Id,
                    UserRegistro = x.NombreUsuario,
                    FechaRegistro = x.FechaRegistro.ToString("dd/MM/yyyy"),
                    Proveedor = x.NombreProveedor,
                    UrlDocumento = x.UrlDocumento,
                    Total = tot,
                }).ToListAsync();

                return new ResponseGeneric<List<InstalacionResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<InstalacionResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<InstalacionResponse>> GetById(long Id)
        {
            try
            {
                var catalogo = _context.vInstalacions.AsQueryable();
                catalogo = catalogo.Where(x => x.Id == Id);
                var result = await catalogo.Select(x => new InstalacionResponse
                {
                    IdInstalacion = x.Id,
                    UserRegistro = x.NombreUsuario,
                    FechaRegistro = x.FechaRegistro.ToString("dd/MM/yyyy"),
                    Proveedor = x.NombreProveedor,
                    UrlDocumento = x.UrlDocumento,
                }).FirstOrDefaultAsync();

                return new ResponseGeneric<InstalacionResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<InstalacionResponse>(ex);
            }
        }
        public async Task<ResponseGeneric<List<BusquedaProveedorRequest>>> ConsultaAutocomplete(string prefix)
        {
            try
            {
                var catalogo = _context.Proveedors.AsQueryable();
                catalogo = catalogo.Where(x => x.Nombre.ToLower().Contains(prefix.ToLower()) || x.CorreoElectronico.ToLower().Contains(prefix.ToLower())).Take(10);
                var result = await catalogo.Select(x => new BusquedaProveedorRequest
                {
                    id = x.Id,
                    Text = x.Nombre + " / " + x.CorreoElectronico
                }).ToListAsync();
                
                return new ResponseGeneric<List<BusquedaProveedorRequest>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<BusquedaProveedorRequest>>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> GuardarInstalacion(InstalacionApiRequestData request)
        {
            try
            {
                var result = new ResponseGeneric<bool>();
                var data = new Instalacion();
                data.FechaRegistro = DateTime.Now;
                data.IdUserRegistro = request.IdUserRegistro;
                data.IdReporte = null;
                data.IdProveedor = request.IdProveedor;
                data.UrlDocumento = "Initial";
                data.IdVerificentro = request.IdVerificentro.Value;

                _context.Instalacions.Add(data);
                var resultSave = _context.SaveChanges() > 0;

                var url = await _blobStorage.UploadFileAsync(new byte[0], "Instalacion/" + data.Id + "/" + request.File.FirstOrDefault().Nombre, request.File.FirstOrDefault().Base64);
                data.UrlDocumento = url;
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
    public interface IInstalacionNegocio
    {
        public Task<ResponseGeneric<List<InstalacionResponse>>> Consulta(RequestList request);
        public Task<ResponseGeneric<InstalacionResponse>> GetById(long Id);
        public Task<ResponseGeneric<List<BusquedaProveedorRequest>>> ConsultaAutocomplete(string prefix);
        public Task<ResponseGeneric<bool>> GuardarInstalacion(InstalacionApiRequestData request);
    }
}
