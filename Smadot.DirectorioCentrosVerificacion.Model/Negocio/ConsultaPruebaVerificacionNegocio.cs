using Microsoft.EntityFrameworkCore;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.ConsultaPruebaVerificacion.Request;
using Smadot.Models.Entities.ConsultaPruebaVerificacion.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.DirectorioCentrosVerificacion.Model.Negocio
{
    public class ConsultaPruebaVerificacionNegocio : IConsultaPruebaVerificacionNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;

        public ConsultaPruebaVerificacionNegocio(SmadotDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }
        public async Task<ResponseGeneric<List<ConsultaPruebaVerificacionResponse>>> GetPruebaVerificacion(ConsultaPruebaVerificacionRequest request)
        {
            try
            {
                var pruebaVerificacion = _context.vVerificacions.AsQueryable();
                var user = _userResolver.GetUser();
                var rol = user.RoleNames.FirstOrDefault();
                var acceso = _context.Rols.ToList().Where(o => o.Nombre.Equals(rol, StringComparison.Ordinal)).FirstOrDefault();
                if (!acceso.AccesoTotalVerificentros)
                {
                    pruebaVerificacion = pruebaVerificacion.Where(o => o.IdVereficentro == _userResolver.GetUser().IdVerificentro);
                }
                if (request.placa)
                {
                    pruebaVerificacion = pruebaVerificacion.Where(x => x.Placa.ToLower().Contains(request.placaSerie.ToLower())).Distinct();
                }
                else
                {
                    pruebaVerificacion = pruebaVerificacion.Where(x => x.Serie.ToLower().Contains(request.placaSerie.ToLower())).Distinct();
                }

                var tot = pruebaVerificacion.Count();
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    pruebaVerificacion = pruebaVerificacion.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await pruebaVerificacion.Select(x => new ConsultaPruebaVerificacionResponse
                {
                    Fecha = x.Fecha,
                    Placa = x.Placa,
                    Serie = x.Serie,
                    FolioCertificado = x.FolioCertificado,
                    Vigencia = x.Vigencia,
                    ResultadosPrueba = x.IdResultadosVerificacion,
                    TipoCertificado = x.TipoCertificado,
                    Semestre = x.Semestre,
                    Marca = x.Marca,
                    Modelo = x.Modelo,
                    Anio = x.Anio,
                    Combustible = x.Combustible,
                    TarjetaCirculacion = x.TarjetaCirculacion,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<ConsultaPruebaVerificacionResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<ConsultaPruebaVerificacionResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<BusquedaPlacaSerieRequest>>> ConsultaAutocomplete(string prefix, string flag)
        {
            try
            {
                var pruebaVerificacion = _context.vPlacaSerieBusquedaVerificaciones.AsQueryable();
                var user = _userResolver.GetUser();
                var rol = user.RoleNames.FirstOrDefault();
                var acceso = _context.Rols.ToList().Where(o => o.Nombre.Equals(rol, StringComparison.Ordinal)).FirstOrDefault();

                if (!acceso.AccesoTotalVerificentros)
                {
                    pruebaVerificacion = pruebaVerificacion.Where(o => o.IdVerificentro == _userResolver.GetUser().IdVerificentro);
                }
                if (flag == "true")
                {
                    pruebaVerificacion = pruebaVerificacion.Where(x => x.Placa.ToLower().Contains(prefix.ToLower()));
                }
                else
                {
                    pruebaVerificacion = pruebaVerificacion.Where(x => x.Serie.ToLower().Contains(prefix.ToLower())).Distinct();
                }

                var result = await pruebaVerificacion.Select(x => new BusquedaPlacaSerieRequest
                {
                    id = flag == "true" ? x.Placa : x.Serie,
                    Text = flag == "true" ? x.Placa : x.Serie,
                }).ToListAsync();

                result = result.DistinctBy(p => new { p.id, p.Text }).ToList();

                return new ResponseGeneric<List<BusquedaPlacaSerieRequest>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<BusquedaPlacaSerieRequest>>(ex);
            }
        }
    }
}


public interface IConsultaPruebaVerificacionNegocio
{
    public Task<ResponseGeneric<List<BusquedaPlacaSerieRequest>>> ConsultaAutocomplete(string prefix, string flag);
    public Task<ResponseGeneric<List<ConsultaPruebaVerificacionResponse>>> GetPruebaVerificacion(ConsultaPruebaVerificacionRequest request);
}
