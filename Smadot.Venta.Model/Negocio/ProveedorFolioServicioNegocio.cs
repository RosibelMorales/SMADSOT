using Smadot.Models.DataBase;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Smadot.Models.Entities.ProveedorFolioServicio.Response.ProveedorFolioServicioResponseData;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using static Smadot.Models.Entities.ProveedorFolioServicio.Request.ProveedorFolioServicioRequestData;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Smadot.Venta.Model.Negocio
{
    public class ProveedorFolioServicioNegocio : IProveedorFolioServicioNegocio
    {
        private SmadotDbContext _context;

        public ProveedorFolioServicioNegocio(SmadotDbContext context)
        {
            _context= context;
        }

        public async Task<ResponseGeneric<List<ProveedorFolioServicioResponse>>> Consulta(RequestList request)
        {
            try
            {
                ///Se agrego ordenamiento debido a que el grid mostraba los registros desordenados
                ///REVISAR OPCIONES
                var catalogo = _context.vProveedorFolios.AsQueryable();
                //var catalogo = GetList().AsQueryable();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }
                //if (request.Activo.HasValue)
                //{
                //    catalogo = catalogo.Where(x => x.Activo == request.Activo);
                //}

                //SE OBTIENE EL TOTAL DE REGISTROS PARA INFORMACION EN EL GRID
                var tot = catalogo.Count();
                var totFilt = 0;
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x =>
                                                    x.FolioPF.ToString().ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.ProveedorEmpresa.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Proveedor.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Motivo.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Equipo.ToLower().Contains(request.Busqueda.ToLower()));

                    totFilt = catalogo.Count();
                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new ProveedorFolioServicioResponse
                {
                    Id = x.Id,
                    ProveedorEmpresa = x.ProveedorEmpresa,
                    Equipo = x.Equipo,
                    EstatusFolio = x.EstatusFolio ?? false,
                    FechaRegistro = x.FechaRegistro.HasValue ? x.FechaRegistro.Value.ToString("dd/MM/yyyy") : string.Empty,
                    FolioOS = x.FolioOS,
                    Motivo = x.Motivo,
                    Proveedor = x.Proveedor,
                    FolioPF = x.FolioPF,
                    EsLaboratorio= x.EsLaboratorio,
                    Total = tot,
                    TotalFilter = totFilt
                }).ToListAsync();

                return new ResponseGeneric<List<ProveedorFolioServicioResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<ProveedorFolioServicioResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<ProveedorFolioServicioResponse>> GetById(long Id)
        {
            try
            {
                var prov = _context.vProveedorFolios.FirstOrDefault(x => x.Id == Id);
                //var prov = GetList().FirstOrDefault(x => x.Id == Id);

                var result = new ProveedorFolioServicioResponse
                {
                    Id = prov.Id,
                    ProveedorEmpresa = prov.ProveedorEmpresa,
                    Equipo = prov.Equipo,
                    EstatusFolio = prov.EstatusFolio ?? false,
                    FechaRegistro = prov.FechaRegistro?.ToString("dd/MM/yyyy"),
                    FolioOS = prov.FolioOS,
                    Motivo = prov.Motivo,
                    Proveedor = prov.Proveedor,
                    FolioPF = prov.FolioPF,
                    EsLaboratorio = prov.EsLaboratorio,
                };
                //var result = prov;

                return new ResponseGeneric<ProveedorFolioServicioResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ProveedorFolioServicioResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<string>> GuardarProveedorFolio(ProveedorFolioServicioRequest request)
        {
            try
            {
                var proveedorF = new ProveedorFolio();


                if (request.FolioInicial < 1)
                    return new ResponseGeneric<string>("El valor del Folio Inicial debe ser mayor a 0.", true);
                if (request.FolioFinal < request.FolioInicial)
                    return new ResponseGeneric<string>("El valor del Folio Final no debe ser menor al valor del Folio Inicial.", true);



                

                var rangos = _context.ProveedorFolios.Any(x => (x.Folio >= request.FolioInicial &&  x.Folio <= request.FolioFinal));

                if (rangos)
                    return new ResponseGeneric<string>("Algunos valores dentro del rango Folio Inicial - Folio Final ya se encuentran registrados", true);

                var proveedor = _context.Proveedors.FirstOrDefault(x => x.Id == request.IdProveedor);
                if (proveedor == null)
                    return new ResponseGeneric<string>("No se encontro el proveedor seleccionado.", true);

                //proveedorF.FolioInicial = request.FolioInicial;
                //proveedorF.FolioFinal = request.FolioFinal;
                //proveedorF.IdProveedor = request.IdProveedor;


                //_context.ProveedorFolios.Add(proveedorF);
                //_context.SaveChanges();
                var sp = await _context.Database.ExecuteSqlRawAsync(
                    "exec [dbo].[spInsertarRangoFolios] @FolioInicial, @FolioFinal, @IdProveedor",
                    new SqlParameter[]
                    {
                        new SqlParameter("@FolioInicial",SqlDbType.BigInt){ Value =  request.FolioInicial },
                        new SqlParameter("@FolioFinal",SqlDbType.BigInt){ Value =  request.FolioFinal },
                        new SqlParameter("@IdProveedor",SqlDbType.BigInt){ Value =  request.IdProveedor }
                    }
                    );

                if (sp < 1)
                {
                    return new ResponseGeneric<string>("Ocurrió un error al registrar la información", true);
                }

                return new ResponseGeneric<string>("", true);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<string>(ex);
            }
        }


        /// <summary>
        /// PRUEBA FRONTEND
        /// </summary>
        /// <returns></returns>
        //private IQueryable<ProveedorFolioServicioResponse> GetList()
        //{
        //    var listaObj = new List<ProveedorFolioServicioResponse>();

        //    listaObj.Add(new ProveedorFolioServicioResponse { Id = 1, Folio = "1", Empresa = "Fake S.A. de C.V.", FechaRegistro = DateTime.Now.ToString("dd/MM/yyyy"), Proveedor = "Gerardo Torres", Equipo ="Equipo 1", Motivo ="Desconocido", EstatusFolio = true });
        //    listaObj.Add(new ProveedorFolioServicioResponse { Id = 2, Folio = "2", Empresa = "Fake S.A. de C.V.", FechaRegistro = DateTime.Now.ToString("dd/MM/yyyy"), Proveedor = "Juan Lopez", Equipo = "Equipo 2", Motivo = "Desconocido 2", EstatusFolio = false });
        //    listaObj.Add(new ProveedorFolioServicioResponse { Id = 3, Folio = "3", Empresa = "Fake S.A. de C.V.", FechaRegistro = DateTime.Now.ToString("dd/MM/yyyy"), Proveedor = "Pedro Martinez", Equipo = "Equipo 3", Motivo = "Desconocido 3", EstatusFolio = true });
        //    listaObj.Add(new ProveedorFolioServicioResponse { Id = 4, Folio = "4", Empresa = "Fake S.A. de C.V.", FechaRegistro = DateTime.Now.ToString("dd/MM/yyyy"), Proveedor = "Gerardo Torres", Equipo = "Equipo 1", Motivo = "Desconocido", EstatusFolio = true });
        //    listaObj.Add(new ProveedorFolioServicioResponse { Id = 5, Folio = "5", Empresa = "Fake S.A. de C.V.", FechaRegistro = DateTime.Now.ToString("dd/MM/yyyy"), Proveedor = "Juan Lopez", Equipo = "Equipo 2", Motivo = "Desconocido 2", EstatusFolio = false });
        //    listaObj.Add(new ProveedorFolioServicioResponse { Id = 6, Folio = "6", Empresa = "Fake S.A. de C.V.", FechaRegistro = DateTime.Now.ToString("dd/MM/yyyy"), Proveedor = "Pedro Martinez", Equipo = "Equipo 3", Motivo = "Desconocido 3", EstatusFolio = true });
        //    listaObj.Add(new ProveedorFolioServicioResponse { Id = 7, Folio = "7", Empresa = "Fake S.A. de C.V.", FechaRegistro = DateTime.Now.ToString("dd/MM/yyyy"), Proveedor = "Gerardo Torres", Equipo = "Equipo 1", Motivo = "Desconocido", EstatusFolio = true });
        //    listaObj.Add(new ProveedorFolioServicioResponse { Id = 8, Folio = "8", Empresa = "Fake S.A. de C.V.", FechaRegistro = DateTime.Now.ToString("dd/MM/yyyy"), Proveedor = "Juan Lopez", Equipo = "Equipo 2", Motivo = "Desconocido 2", EstatusFolio = false });
        //    listaObj.Add(new ProveedorFolioServicioResponse { Id = 9, Folio = "9", Empresa = "Fake S.A. de C.V.", FechaRegistro = DateTime.Now.ToString("dd/MM/yyyy"), Proveedor = "Pedro Martinez", Equipo = "Equipo 3", Motivo = "Desconocido 3", EstatusFolio = true });
        //    listaObj.Add(new ProveedorFolioServicioResponse { Id = 10, Folio = "10", Empresa = "Fake S.A. de C.V.", FechaRegistro = DateTime.Now.ToString("dd/MM/yyyy"), Proveedor = "Gerardo Torres", Equipo = "Equipo 1", Motivo = "Desconocido", EstatusFolio = true });

        //    return listaObj.AsQueryable();
        //}

    }

    public interface IProveedorFolioServicioNegocio
    {
        public Task<ResponseGeneric<List<ProveedorFolioServicioResponse>>> Consulta(RequestList request);
        public Task<ResponseGeneric<ProveedorFolioServicioResponse>> GetById(long Id);
        public Task<ResponseGeneric<string>> GuardarProveedorFolio(ProveedorFolioServicioRequest request);
    }
}
