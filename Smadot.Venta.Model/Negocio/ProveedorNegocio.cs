using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using static Smadot.Models.Entities.Proveedor.Response.ProveedorResponseData;
using Smadot.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using Smadot.Models.Entities.DevolucionSPF.Response;
using static Smadot.Models.Entities.Proveedor.Request.ProveedorRequestData;
using static Smadot.Models.Entities.ProveedorFolioServicio.Request.ProveedorFolioServicioRequestData;
using static Smadot.Models.Entities.ProveedorFolioServicio.Response.ProveedorFolioServicioResponseData;
using NPOI.SS.Formula.Functions;

namespace Smadot.Venta.Model.Negocio
{
    public class ProveedorNegocio : IProveedorNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;

        public ProveedorNegocio(SmadotDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public async Task<ResponseGeneric<List<ProveedorResponse>>> Consulta(RequestList request)
        {
            try
            {
                ///Se agrego ordenamiento debido a que el grid mostraba los registros desordenados
                ///REVISAR OPCIONES
                /// Remover filtro esLaboratorio después de la presentación
                var catalogo = _context.Proveedors.Where(l => l.EsLaboratorio == request.Activo).AsQueryable();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    if (request.ColumnaOrdenamiento == "Telefono")
                    {
                        request.ColumnaOrdenamiento = "NumeroTelefono";
                    }
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }
                //if (request.Activo.HasValue)
                //{
                //    catalogo = catalogo.Where(x => x.Activo == request.Activo);
                //}

                //SE OBTIENE EL TOTAL DE REGISTROS PARA INFORMACION EN EL GRID
                var tot = catalogo.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x =>
                                                    x.Nombre.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.CorreoElectronico.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Direccion.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.NumeroTelefono.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Empresa.ToLower().Contains(request.Busqueda.ToLower()));
                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new ProveedorResponse
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    CorreoElectronico = x.CorreoElectronico,
                    Telefono = x.NumeroTelefono,
                    Direccion = x.Direccion,
                    Empresa = x.Empresa,
                    Autorizado = x.Autorizado,
                    Total = tot,
                }).ToListAsync();

                return new ResponseGeneric<List<ProveedorResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<ProveedorResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<ProveedorResponse>> GetById(long Id)
        {
            try
            {
                var prov = _context.Proveedors.FirstOrDefault(x => x.Id == Id);

                var result = new ProveedorResponse
                {
                    Id = prov.Id,
                    Nombre = prov.Nombre,
                    CorreoElectronico = prov.CorreoElectronico,
                    Telefono = prov.NumeroTelefono,
                    Direccion = prov.Direccion,
                    Empresa = prov.Empresa,
                    Autorizado = prov.Autorizado
                };

                return new ResponseGeneric<ProveedorResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ProveedorResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<string>> GuardarProveedor(ProveedorRequest request)
        {
            try
            {
                var proveedor = new Proveedor();
                proveedor.Nombre = request.Nombre;
                proveedor.CorreoElectronico = request.CorreoElectronico;
                proveedor.NumeroTelefono = request.Telefono;
                proveedor.Direccion = request.Direccion;
                proveedor.Empresa = request.Empresa;
                proveedor.Autorizado = true;
                if (request.EsLaboratorio.HasValue)
                    proveedor.EsLaboratorio = request.EsLaboratorio.Value;

                _context.Proveedors.Add(proveedor);
                _context.SaveChanges();

                return new ResponseGeneric<string>("", true);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<string>(ex);
            }
        }

        public async Task<ResponseGeneric<string>> CambiarAutorizacion(EstatusProveedor request)
        {
            try
            {
                var prov = _context.Proveedors.FirstOrDefault(x => x.Id == request.IdEstatus);
                if (prov == null)
                    return new ResponseGeneric<string>($"No existe el proveedor seleccionado.", true);

                prov.Autorizado = !prov.Autorizado;

                _context.SaveChanges();

                return new ResponseGeneric<string>("", true);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<string>(ex);
            }
        }

        public async Task<ResponseGeneric<List<ProveedorFolioServicioAutocompleteResponse>>> Autocomplete(ProveedorFolioServicioAutocompletRequest request)
        {
            try
            {
                var verificaciones = _context.Proveedors.Where(x => x.Nombre.ToLower().Contains(request.Term.ToLower()) || x.Empresa.ToLower().Contains(request.Term.ToLower()) && x.Autorizado).AsQueryable();
                if (request.EsLaboratorio)
                {
                    verificaciones = verificaciones.Where(x => x.EsLaboratorio);
                }
                var aux = verificaciones.Where(x => x.EsLaboratorio);
                var tot = verificaciones.Count();
                verificaciones = verificaciones.Skip(request.Start).Take(request.End);
                var result = await verificaciones.Select(x => new ProveedorFolioServicioAutocompleteResponse
                {
                    Id = x.Id,
                    Text = string.Format("{0}/{1} {2}", x.Nombre, x.Empresa, x.EsLaboratorio ? "(Laboratorio)" : "(Proveedor)")
                }).ToListAsync();
                return new ResponseGeneric<List<ProveedorFolioServicioAutocompleteResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<ProveedorFolioServicioAutocompleteResponse>>(ex);
            }
        }


        private static string BytesToString(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

    }

    public interface IProveedorNegocio
    {
        public Task<ResponseGeneric<List<ProveedorResponse>>> Consulta(RequestList request);
        public Task<ResponseGeneric<ProveedorResponse>> GetById(long Id);
        public Task<ResponseGeneric<string>> CambiarAutorizacion(EstatusProveedor request);
        public Task<ResponseGeneric<string>> GuardarProveedor(ProveedorRequest request);
        public Task<ResponseGeneric<List<ProveedorFolioServicioAutocompleteResponse>>> Autocomplete(ProveedorFolioServicioAutocompletRequest request);
    }
}
