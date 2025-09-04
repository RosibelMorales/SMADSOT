using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.ReposicionCredencial.Request;
using Smadot.Models.Entities.ReposicionCredencial.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.Linq.Dynamic.Core;

namespace Smadot.SeguimientoCVV.Model.Negocio
{
    public class ReposicionCredencialNegocio : IReposicionCredencialNegocio
    {
		private SmadotDbContext _context;
		private readonly IUserResolver _userResolver;
		private readonly BlobStorage _blobStorage;

		public ReposicionCredencialNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
		{
			_context = context;
			_userResolver = userResolver;
			_blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
		}


        public async Task<ResponseGeneric<List<UserPuestoAutoCompleteResponse>>> Autocomplete(UserPuestoAutoCompleteRequest request)
        {
            try
            {
                var userPuestoVerificentro = _context.vPersonalAutorizacions.Where(x => x.NumeroTrabajador == request.Term);
                
                var tot = userPuestoVerificentro.Count();
                userPuestoVerificentro = userPuestoVerificentro.Skip(request.Start).Take(request.End);
                var result = await userPuestoVerificentro.Select(x => new UserPuestoAutoCompleteResponse
                {
                    Id = x.IdPuestoVerificentro,
                    Text = x.Nombre + " / " + x.NumeroTrabajador
                }).ToListAsync();
                return new ResponseGeneric<List<UserPuestoAutoCompleteResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<UserPuestoAutoCompleteResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Registro(ReposicionCredencialRequest request)
        {
            try
            {
                ReporteCredencial reporteCredencial = new()
                {
                    Denuncia = request.Denuncia.ToUpper(),
                    FechaRegistro = DateTime.Now,
                    IdCatEstatusReporteCredencial = 1,
                    IdCatMotivoReporteCredencial = request.IdCatMotivoReporteCredencial,
                    IdUserPuestoVerificentro = request.IdUserPuestoVerificentro
                };
                _context.Add(reporteCredencial);
                int response = await _context.SaveChangesAsync();

                foreach (var file in request.Files)
                {
                    var url = await _blobStorage.UploadFileAsync(new byte[0], "ReposicionCredencial/" + reporteCredencial.Id + "/" + file.Nombre, file.Base64);
                    if (!string.IsNullOrEmpty(url))
                    {
                        reporteCredencial.UrlCredencial = url;
                        break;
                    }
                }

                await _context.SaveChangesAsync();
                return new ResponseGeneric<bool>(response > 0);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<List<vReposicionCredencialResponse>>> Consulta(ReposicionCredencialListRequest request)
        {
            try
            {
                var reposiciones = _context.vReposicionCredencials.AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    reposiciones = reposiciones.Where(x => x.Nombre.ToLower().Contains(request.Busqueda.ToLower()) || x.NumeroTrabajador.Contains(request.Busqueda.ToLower()) || x.MotivoReporteCredencial.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var tot = reposiciones.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    reposiciones = reposiciones.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    reposiciones = reposiciones.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }
                DateTime now = DateTime.Now;

                var result = await reposiciones.Select(x => new vReposicionCredencialResponse
                {
                    Id = x.Id,
                    IdCatEstatusReporteCredencial = x.IdCatEstatusReporteCredencial,
                    Denuncia = x.Denuncia,
                    FechaRegistro = x.FechaRegistro,
                    IdCatMotivoReporteCredencial = x.IdCatEstatusReporteCredencial,
                    IdUserPuestoVerificentro = x.IdUserPuestoVerificentro,
                    Nombre = x.Nombre,
                    NumeroTrabajador = x.NumeroTrabajador,
                    UrlCredencial = x.UrlCredencial,
                    MotivoReporteCredencial = x.MotivoReporteCredencial,
                    EstatusReporteCredencial = x.EstatusReporteCredencial,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<vReposicionCredencialResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<vReposicionCredencialResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<vReposicionCredencialResponse>> Detalle(long Id)
        {
            try
            {
                var reposicion = await _context.vReposicionCredencials.FirstOrDefaultAsync(x => x.Id == Id);

                var result = new vReposicionCredencialResponse
                {
                    Id = reposicion.Id,
                    IdCatEstatusReporteCredencial = reposicion.IdCatEstatusReporteCredencial,
                    Denuncia = reposicion.Denuncia,
                    FechaRegistro = reposicion.FechaRegistro,
                    IdCatMotivoReporteCredencial = reposicion.IdCatEstatusReporteCredencial,
                    IdUserPuestoVerificentro = reposicion.IdUserPuestoVerificentro,
                    Nombre = reposicion.Nombre,
                    NumeroTrabajador = reposicion.NumeroTrabajador,
                    UrlCredencial = reposicion.UrlCredencial,
                    EstatusReporteCredencial = reposicion.EstatusReporteCredencial,
                    MotivoReporteCredencial = reposicion.MotivoReporteCredencial
                };

                return new ResponseGeneric<vReposicionCredencialResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<vReposicionCredencialResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Rechazar(long Id)
        {
            try
            {
                var reporteCredencial =await _context.ReporteCredencials.FirstOrDefaultAsync(x => x.Id == Id);
                reporteCredencial.IdCatEstatusReporteCredencial = CatEstatusReporteCredencialDic.RechazadoDVRF;

                _context.ReporteCredencials.Update(reporteCredencial);

                int response = await _context.SaveChangesAsync();

                await _context.SaveChangesAsync();
                return new ResponseGeneric<bool>(response > 0);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Aprobar(long Id)
        {
            try
            {
                var reporteCredencial = await _context.ReporteCredencials.FirstOrDefaultAsync(x => x.Id == Id);
                reporteCredencial.IdCatEstatusReporteCredencial = CatEstatusReporteCredencialDic.ApruebaDVRF;

                _context.ReporteCredencials.Update(reporteCredencial);

                int response = await _context.SaveChangesAsync();

                await _context.SaveChangesAsync();
                return new ResponseGeneric<bool>(response > 0);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }
    }
}

public interface IReposicionCredencialNegocio
{
    public Task<ResponseGeneric<List<UserPuestoAutoCompleteResponse>>> Autocomplete(UserPuestoAutoCompleteRequest request);
    public Task<ResponseGeneric<bool>> Registro(ReposicionCredencialRequest request);

    public Task<ResponseGeneric<List<vReposicionCredencialResponse>>> Consulta(ReposicionCredencialListRequest request);

    public Task<ResponseGeneric<vReposicionCredencialResponse>> Detalle(long Id);
    public Task<ResponseGeneric<bool>> Rechazar(long Id);
    public Task<ResponseGeneric<bool>> Aprobar(long Id);
}