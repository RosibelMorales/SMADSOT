using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Caja.Request;
using Smadot.Models.Entities.Caja.Response;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Response;
using Smadot.Models.Entities.IngresoFormaValorada.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;

namespace Smadot.IngresoFormaValorada.Model.Negocio
{
    public class CajaNegocio : ICajaNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        public CajaNegocio(SmadotDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }
        public async Task<ResponseGeneric<long>> Registro(List<CajaResponse> request)
        {
            try
            {
                var solicitud = new Caja();
                var result = false;
                var ingresoFV = new IngresoFV();
                ingresoFV = _context.IngresoFVs.FirstOrDefault(x => x.Id == request[0].Id);
                //UPDATE
                if (request[0].Id > 0)
                {
                    int i = 0;

                    foreach (var c in request)
                    {
                        solicitud = new Caja
                        {
                            IdIngresoFV = request[i].IdIngresoFV,
                            IdCatTipoCertificado = request[i].IdCatTipoCertificado,
                            NumeroCaja = request[i].NumeroCaja,
                            FolioInicial = request[i].FolioInicial,
                            FolioFinal = request[i].FolioFinal,
                        };
                        _context.Update(solicitud);
                        result = await _context.SaveChangesAsync() > 0;
                        i++;
                    }
                    return result ? new ResponseGeneric<long>(solicitud.Id) : new ResponseGeneric<long>();
                }
                //ADD
                else
                {
                    int i = 0;
                    foreach (var c in request)
                    {
                        solicitud = new Caja
                        {
                            IdIngresoFV = request[0].IdIngresoFV,
                            IdCatTipoCertificado = c.IdCatTipoCertificado,
                            NumeroCaja = c.NumeroCaja,
                            FolioInicial = c.FolioInicial,
                            FolioFinal = c.FolioFinal,

                        };
                        _context.Cajas.Add(solicitud);
                        result = await _context.SaveChangesAsync() > 0;
                        i++;
                    }
                    return result ? new ResponseGeneric<long>(solicitud.Id) : new ResponseGeneric<long>();
                }
                //var result = await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex);
            }
        }
        public async Task<ResponseGeneric<List<IngresoFormaValoradaResponse>>> GetById(long Id)
        {
            try
            {
                var result = new List<IngresoFormaValoradaResponse>();
                var result2 = new List<IngresoFormaValoradaResponse>();

                if (Id > 0)
                {
                    var solicitud = _context.vIngresoFVs.Where(x => x.IdSolicitudFV == Id);
                    var solicitudFV = _context.vSolicitudFormaValorada.Where(x => x.IdSolicitudFV == Id);

                    if (solicitud != null)
                    {
                        var soli = _context.vSolicitudFormaValorada.Where(x => x.IdSolicitudFV == Id);

                        string r = JsonConvert.SerializeObject(solicitud);
                        result = JsonConvert.DeserializeObject<List<IngresoFormaValoradaResponse>>(r);

                        string r2 = JsonConvert.SerializeObject(soli);
                        result2 = JsonConvert.DeserializeObject<List<IngresoFormaValoradaResponse>>(r2);
                        result2 = await soli.Select(x => new IngresoFormaValoradaResponse
                        {
                            FechaSolicitud = x.FechaSolicitudFV
                        }).ToListAsync();


                        result[0].FechaSolicitud = result2[0].FechaSolicitud;

                    }

                }
                else
                {
                    var catTipoCert = _context.CatTipoCertificados.Where(x => x.Activo).OrderBy(x => Id).ToList();
                    foreach (var c in catTipoCert)
                    {
                        result.Add(new IngresoFormaValoradaResponse
                        {
                            IdCatTipoCertificadoSC = c.Id,
                            TipoCertificadoSC = c.Nombre,
                            ClaveCertificadoSC = c.ClaveCertificado,
                            FolioInicialSC = 0
                        });
                    }

                }

                return new ResponseGeneric<List<IngresoFormaValoradaResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<IngresoFormaValoradaResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<Caja>>> GetCajaById(long Id)
        {
            try
            {
                var result = new List<Caja>();
                var cajas = _context.Cajas.Include(x => x.IdIngresoFVNavigation).Where(x => x.IdIngresoFVNavigation.IdSolicitudFV == Id);
                if (cajas != null && cajas.Count() > 0)
                {
                    //string r = JsonConvert.SerializeObject(ingresoCert);
                    //result = JsonConvert.DeserializeObject<List<IngresoCertificado>>(r);
                    //result = ingresoCert.ToList();

                    result = await cajas.Select(x => new Caja
                    {
                        Id = x.Id,
                        IdIngresoFV = x.IdIngresoFV,
                        IdCatTipoCertificado = x.IdCatTipoCertificado,
                        NumeroCaja = x.NumeroCaja,
                        FolioInicial = x.FolioInicial,
                        FolioFinal = x.FolioFinal
                    }).ToListAsync();

                }

                return new ResponseGeneric<List<Caja>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<Caja>>(ex);
            }
        }

    }
    public interface ICajaNegocio
    {
        Task<ResponseGeneric<long>> Registro(List<CajaResponse> request);
        public Task<ResponseGeneric<List<IngresoFormaValoradaResponse>>> GetById(long Id);
        public Task<ResponseGeneric<List<Caja>>> GetCajaById(long Id);
    }
}