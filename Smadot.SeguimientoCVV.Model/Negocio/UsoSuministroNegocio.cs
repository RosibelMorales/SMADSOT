using Smadot.Models.DataBase;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Smadot.Models.Entities.UsoSuministro.Request;
using Smadot.Models.Entities.UsoSuministro.Response;
using Microsoft.Extensions.Configuration;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Newtonsoft.Json;
using System.Globalization;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace Smadot.SeguimientoCVV.Model.Negocio
{
    public class UsoSuministroNegocio : IUsoSuministroNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;


        public UsoSuministroNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public async Task<ResponseGeneric<List<vUsoSuministroResponse>>> Consulta(UsoSuministroRequest request)
        {
            try
            {
                var suministro = _context.vUsoSuministros.Where( x => x.IdVerificentro == _userResolver.GetUser().IdVerificentro).AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    var date = new DateTime();
                    var dateDef = new DateTime();
                    DateTime.TryParseExact(request.Busqueda, "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out date);
                    suministro = suministro.Where(x => x.Id.ToString().Contains(request.Busqueda.ToLower()) || (date != dateDef && x.Fecha == date) || x.NombreUser.ToLower().Contains(request.Busqueda.ToLower()) || x.NombreUsoSuministro.ToLower().Contains(request.Busqueda.ToLower()) || x.UrlFactura.ToLower().Contains(request.Busqueda.ToLower()) || x.Cantidad.ToString().Contains(request.Busqueda.ToLower()));

                }

                var tot = suministro.Count();
                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    suministro = suministro.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    suministro = suministro.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }
                DateTime now = DateTime.Now;

                var result = await suministro.Select(x => new vUsoSuministroResponse
                {
                    Id = x.Id,
                    NombreUsoSuministro = x.NombreUsoSuministro,
                    Cantidad = x.Cantidad,
                    Fecha = x.Fecha,
                    IdUserRegistro = x.IdUserRegistro,
                    NombreUser = x.NombreUser,
                    UrlFactura = x.UrlFactura,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<vUsoSuministroResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<vUsoSuministroResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Registro(UsoSuministroResponse request)
        {
            try
            {
                var suministro = new UsoSuministro();
                if(request.Id > 0)
                {
                    suministro = _context.UsoSuministros.FirstOrDefault( x => x.Id == request.Id);
                    if (suministro == null)
                        throw new Exception("No se encontró registro");
                }
                else
                {
                    suministro = new UsoSuministro
                    {
                        IdVerificentro = (long)_userResolver.GetUser().IdVerificentro,
                        Nombre = request.Nombre,
                        Cantidad = request.Cantidad,
                        Fecha = request.Fecha,
                        Nota = request.Nota,
                        UrlFactura = request.UrlFactura,
                        Proveedor = request.Proveedor,
                        FechaRegistro = DateTime.Now,
                        IdUserRegistro = _userResolver.GetUser().IdUser
                    };
                    _context.UsoSuministros.Add(suministro);
                    await _context.SaveChangesAsync();

                    //Registrar UrlDocument
                    foreach (var file in request.Files)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "UsoSuministro/" + suministro.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            suministro.UrlFactura = url; break;
                        }
                    }
                   
                }
                var result = await _context.SaveChangesAsync() > 0;
                return new ResponseGeneric<bool>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<vUsoSuministroResponse>> Detalle(long id)
        {
            try
            {
                var suministro = await _context.vUsoSuministros.FirstOrDefaultAsync(x => x.Id == id);
                if (suministro is null)
                    return new ResponseGeneric<vUsoSuministroResponse>();

                var result = new vUsoSuministroResponse
                {
                    Id = suministro.Id,
                    IdVerificentro = suministro.IdVerificentro,
                    NombreVerificentro = suministro.NombreVerificentro,
                    NombreUsoSuministro = suministro.NombreUsoSuministro,
                    Cantidad = suministro.Cantidad,
                    Fecha = suministro.Fecha,
                    Nota = suministro.Nota,
                    UrlFactura = suministro.UrlFactura,
                    Proveedor = suministro.Proveedor,
                    FechaRegistro = suministro.FechaRegistro,
                    IdUserRegistro = suministro.IdUserRegistro,
                    NombreUser = suministro.NombreUser
                };

                return new ResponseGeneric<vUsoSuministroResponse>(result);
            }
            catch(Exception ex)
            {
                return new ResponseGeneric<vUsoSuministroResponse>(ex);
            }
        }
    }

    public interface IUsoSuministroNegocio
    {
        public Task<ResponseGeneric<List<vUsoSuministroResponse>>> Consulta(UsoSuministroRequest request);

        public Task<ResponseGeneric<bool>> Registro(UsoSuministroResponse request);

        public Task<ResponseGeneric<vUsoSuministroResponse>> Detalle(long id);
    }
}
