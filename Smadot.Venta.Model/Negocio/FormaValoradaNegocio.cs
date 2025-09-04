using Smadot.Models.DataBase;
using System.Linq.Dynamic.Core;
using Smadot.Models.Entities.FoliosFormaValorada.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Smadot.Models.Entities.Generic.Response;
using NPOI.POIFS.Crypt.Dsig;
using Smadot.Models.Entities.FoliosFormaValorada.Request;
using Newtonsoft.Json;
using Smadot.Models.Dicts;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Smadot.Venta.Model.Negocio
{
    public class FormaValoradaNegocio : IFormaValoradaNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;

        public FormaValoradaNegocio(SmadotDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public async Task<ResponseGeneric<ResponseGrid<FolioPendienteImprimirResponse>>> Consulta(FolioFormaValoradaGridRequest request)
        {
            try
            {
                var user = _userResolver.GetUser();
                var today = DateTime.Now.Date;
                var otherDay = today.AddDays(1);
#if DEBUG
                var folios = _context.vPendientesImprimirs
                .Where(x => !(x.Impreso ?? false) && x.IdVerificentro == user.IdVerificentro)
                    .AsQueryable();
#else
                 var folios = _context.vPendientesImprimirs
                    .Where(x => !(x.Impreso ?? false) && x.IdVerificentro == user.IdVerificentro && x.Fecha >= today && x.Fecha < otherDay)
                    .AsQueryable();

#endif
                var total = folios.Count();
                // var rol = user.RoleNames.FirstOrDefault();
                // var acceso = _context.Rols.ToList().Where(o => o.Nombre.Equals(rol, StringComparison.Ordinal)).FirstOrDefault();

                // if (!acceso.AccesoTotalVerificentros)
                // {
                //     folios = folios.Where(x => x.IdVerificentro == user.IdVerificentro).AsQueryable();
                //     filtered = folios.Count();
                // }

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    //folios = folios.Where(x => x.Id.ToString().Contains(request.Busqueda.ToLower()) || x.Folio.ToString("000000000").Contains(request.Busqueda.ToLower()));
                    folios = folios.Where(x =>
                        EF.Functions.Like(x.FolioFoliosFormaValoradaVerificentro == null ? "" : x.FolioFoliosFormaValoradaVerificentro.ToString(), $"%{request.Busqueda}%") ||
                        x.FolioAnterior == null ? false : x.FolioAnterior.Contains(request.Busqueda) ||
                        EF.Functions.Like(x.Serie.ToLower(), $"%{request.Busqueda}%") ||
                        EF.Functions.Like(x.Placa.ToLower(), $"%{request.Busqueda}%") ||
                        EF.Functions.Like(x.Propietario.ToLower(), $"%{request.Busqueda}%") ||
                        EF.Functions.Like(x.Marca.ToLower(), $"%{request.Busqueda}%") ||
                        EF.Functions.Like(x.Propietario.ToLower(), $"%{request.Busqueda}%") ||
                        EF.Functions.Like(x.Modelo.ToString(), $"%{request.Busqueda}%") ||
                        EF.Functions.Like(x.TarjetaCirculacion, $"%{request.Busqueda}%"));

                }
                // if (request.TipoCertificado > 0 && request.TipoCertificado != null)
                // {
                //     folios = folios.Where(x => x.IdCatTipoCertificado == request.TipoCertificado);
                //     filtered = folios.Count();
                // }
                // if (request.TipoTramite > 0 && request.TipoTramite != null)
                // {
                //     folios = folios.Where(x => x.IdCatTipoTramite == request.TipoTramite);
                //     filtered = folios.Count();
                // }
                var filtered = folios.Count();

                folios = folios.OrderBy(x => x.FolioFoliosFormaValoradaVerificentro);

                //if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                //{
                //    folios = folios.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                //}

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    folios = folios.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }
                string r = JsonConvert.SerializeObject(await folios.ToListAsync());
                var result = JsonConvert.DeserializeObject<List<FolioPendienteImprimirResponse>>(r);

                return new ResponseGeneric<ResponseGrid<FolioPendienteImprimirResponse>>(new ResponseGrid<FolioPendienteImprimirResponse> { Data = result, RecordsFiltered = filtered, RecordsTotal = total });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<FolioPendienteImprimirResponse>>(ex);
            }
        }
        public async Task<ResponseGeneric<FoliosProximosResponse>> ProximosFolios()
        {
            try
            {
                var user = _userResolver.GetUser();
                // var rol = user.RoleNames.FirstOrDefault();
                // var acceso = _context.Rols.ToList().Where(o => o.Nombre.Equals(rol, StringComparison.Ordinal)).FirstOrDefault();
                var foliosActuales = await _context.vFoliosFormaValoradaVerificentroActuales.Where(x => x.IdVerificentro == user.IdVerificentro).ToListAsync();

                FoliosProximosResponse foliosProximosResponse = new()
                {
                    FolioDoblecero = (foliosActuales.FirstOrDefault(x => x.IdCatTipoCertificado == TipoCertificado.DobleCero)?.Folio ?? 0) + 1,
                    FolioCero = (foliosActuales.FirstOrDefault(x => x.IdCatTipoCertificado == TipoCertificado.Cero)?.Folio ?? 0) + 1,
                    FolioUno = (foliosActuales.FirstOrDefault(x => x.IdCatTipoCertificado == TipoCertificado.Uno)?.Folio ?? 0) + 1,
                    FolioDos = (foliosActuales.FirstOrDefault(x => x.IdCatTipoCertificado == TipoCertificado.Dos)?.Folio ?? 0) + 1,
                    FolioCNA = (foliosActuales.FirstOrDefault(x => x.IdCatTipoCertificado == TipoCertificado.ConstanciasNoAprobado)?.Folio ?? 0) + 1,
                    FolioExento = (foliosActuales.FirstOrDefault(x => x.IdCatTipoCertificado == TipoCertificado.Exentos)?.Folio ?? 0) + 1,
                    FolioTestificacion = (foliosActuales.FirstOrDefault(x => x.IdCatTipoCertificado == TipoCertificado.Testificacion)?.Folio ?? 0) + 1,
                };

                return new ResponseGeneric<FoliosProximosResponse>(foliosProximosResponse);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<FoliosProximosResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> RecalcularFolio(long idFolioFormaValorada, long idVerificentro)
        {
            try
            {
                if (idFolioFormaValorada > 0)
                {
                    var aux = await _context.FoliosFormaValoradaVerificentros.FirstOrDefaultAsync(x => x.Id == idFolioFormaValorada);

                    if (aux == null)
                    {
                        throw new ValidationException("No se encontró el registro.");
                    }
                    else
                    {
                        var consulta = await _context.FoliosFormaValoradaActuales.Where(x => x.IdCatTipoCertificado == aux.IdCatTipoCertificado && x.IdVerificentro == idVerificentro).FirstOrDefaultAsync();
                        if (consulta != null && consulta.Id != idFolioFormaValorada)
                        {
                            aux.Folio = consulta.Folio + 1;
                        }
                        _context.SaveChanges();
                    }
                    return new ResponseGeneric<bool>(true);
                }
                else
                {
                    return new ResponseGeneric<bool>(false);
                }
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<bool>(ex, ex.Message);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex, "Ocurrió un error al actualizar la información.");
            }
        }

    }
    public interface IFormaValoradaNegocio
    {
        Task<ResponseGeneric<ResponseGrid<FolioPendienteImprimirResponse>>> Consulta(FolioFormaValoradaGridRequest request);
        Task<ResponseGeneric<FoliosProximosResponse>> ProximosFolios();
        public Task<ResponseGeneric<bool>> RecalcularFolio(long idFolioFormaValorada, long idVerificentro);
    }
}
