using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.ConsultaTablaMaestra.Response;
using Smadot.Utilities.Modelos;

namespace Smadot.Catalogo.Model.Negocio
{
    public class CatalogosVerificacion:ICatalogosVerificacion
    {
        private SmadotDbContext _context;

        public CatalogosVerificacion(SmadotDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseGeneric<List<Models.Entities.ConsultaTablaMaestra.Response.Catalogo>>> GetCatalogoTipoServicio()
        {
            try
            {
                var catalogo = _context.CatTipoServicioCita;


                var resut = await catalogo.Select(x => new Models.Entities.ConsultaTablaMaestra.Response.Catalogo
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                }).ToListAsync();

                return new ResponseGeneric<List<Models.Entities.ConsultaTablaMaestra.Response.Catalogo>>(resut);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<Models.Entities.ConsultaTablaMaestra.Response.Catalogo>>(ex);
            }
        }
        public async Task<ResponseGeneric<List<Models.Entities.ConsultaTablaMaestra.Response.Catalogo>>> GetCatalogoMotivoVerificacion()
        {
            try
            {
                var catalogo = _context.MotivoVerificacions.Where(x => x.Activo);


                var resut = await catalogo.Select(x => new Models.Entities.ConsultaTablaMaestra.Response.Catalogo
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                }).ToListAsync();

                return new ResponseGeneric<List<Models.Entities.ConsultaTablaMaestra.Response.Catalogo>>(resut);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<Models.Entities.ConsultaTablaMaestra.Response.Catalogo>>(ex);
            }
        }
    }
    public interface ICatalogosVerificacion
    {
        Task<ResponseGeneric<List<Models.Entities.ConsultaTablaMaestra.Response.Catalogo>>> GetCatalogoTipoServicio();
        Task<ResponseGeneric<List<Models.Entities.ConsultaTablaMaestra.Response.Catalogo>>> GetCatalogoMotivoVerificacion();
    }
}