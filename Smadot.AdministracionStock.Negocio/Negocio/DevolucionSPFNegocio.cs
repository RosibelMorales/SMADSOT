using Smadot.Models.DataBase;
using Smadot.Models.Entities.DevolucionSPF.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Smadot.Models.Entities.DevolucionSPF.Request;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.BlobStorage;
using Microsoft.Extensions.Configuration;
using System;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Response;
using Smadot.Models.Entities.Catalogos.Response;

namespace Smadot.AdministracionStock.Negocio.Negocio
{
    public class DevolucionSPFNegocio : IDevolucionSPFNegocio
    {
        private SmadotDbContext _context;
        private IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        public DevolucionSPFNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }
        public async Task<ResponseGeneric<ResponseGrid<ListDevolucionSPFResponse>>> Consulta(RequestList request)
        {
            try
            {

                var _ = new ResponseGrid<ListDevolucionSPFResponse>();
                var data = _context.vDevolucionSPFs.AsQueryable();
                _.RecordsTotal = data.Count();
                _.RecordsFiltered = data.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    data = data.Where(asv => asv.UserAprobo.Contains(request.Busqueda) || asv.Nombre.Contains(request.Busqueda) || asv.Id.ToString().Contains(request.Busqueda));
                    _.RecordsFiltered = data.Count();
                }
                data = data.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                _.Data = data.Skip(request.Pagina.Value).Take(request.Registros.Value).Select(x => new ListDevolucionSPFResponse
                {
                    Id = x.Id,
                    FechaEntrega = x.FechaEntrega,
                    FechaRegistro = x.FechaRegistro,
                    IdAlmacen = x.IdAlmacen,
                    IdUserRegistro = x.IdUserRegistro,
                    UserAprobo = x.UserAprobo,
                    Nombre = x.Nombre,
                    NumeroDevolucion = x.NumeroDevolucion,
                    RecibioSPF = x.RecibioSPF,
                    ResponsableEntrega = x.ResponsableEntrega,
                    NumeroSolicitud = x.NumeroSolicitud,
                    Cantidad = x.Cantidad,
                }).ToList();
                //_.RecordsFiltered = _.Data.Count();
                return new ResponseGeneric<ResponseGrid<ListDevolucionSPFResponse>>(_);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<ListDevolucionSPFResponse>>(ex);

            }
        }

        public async Task<ResponseGeneric<List<SolicitudFV>>> Autocomplete(string prefix)
        {
            try
            {
                var list = _context.SolicitudFVs;
                var result = list.Where(x => x.Id.ToString().ToLower().Contains(prefix.ToLower())).Take(10).ToList();
                return new ResponseGeneric<List<SolicitudFV>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<SolicitudFV>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<User>>> AutocompleteResponsable(string prefix)
        {
            try
            {
                var list = _context.Users;
                var result = list.Where(x => x.Nombre.ToLower().Contains(prefix.ToLower())).Take(10).ToList();
                return new ResponseGeneric<List<User>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<User>>(ex);
            }
        }

        public async Task<ResponseGeneric<string>> Guardar(DevolucionSPFRequest request)
        {
            try
            {
                using (var dbContextTransaction = _context.Database.BeginTransaction())
                {
                    var checkNumDev = GenerarNumeroDevolucion(request.NumeroDevolucion.ToString());
                    //Obtenemos el almacen para obtener los inventarios
                    var ingresoCertificados = _context.IngresoCertificados.Include(x => x.IdIngresoFVNavigation).Where(x => x.IdIngresoFVNavigation.IdSolicitudFV == request.NumeroSolicitud
                                       ).ToList();
                    var idAlmacen = ingresoCertificados.FirstOrDefault()?.IdIngresoFVNavigation.IdAlmacen;
                    var Almacen = _context.Almacens.Include(i => i.Inventarios).FirstOrDefault(x => x.Id == idAlmacen);
                    if (Almacen == null)
                        return new ResponseGeneric<string>($"No existe el almacen seleccionado.", true);
                    var devolucionSPF = new DevolucionSPF
                    {
                        IdAlmacen = Almacen.Id,
                        FechaEntrega = (DateTime)request.FechaEntrega,
                        FechaRegistro = DateTime.Now,
                        //NumeroDevolucion = request.NumeroDevolucion.ToString(),
                        NumeroDevolucion = checkNumDev,
                        NumeroSolicitud = request.NumeroSolicitud.ToString(),
                        ResponsableEntrega = request.ResponsableEntrega,
                        RecibioSPF = request.RecibioEnSPF,

                        IdUserRegistro = _userResolver.GetUser().IdUser,
                        UserAprobo = _userResolver.GetUser().Nombre
                    };
                    //Creamos la asignación a ventanilla
                    _context.DevolucionSPFs.Add(devolucionSPF);
                    _context.SaveChanges();

                    if (request.Files != null)
                    {
                        var i = 0;
                        foreach (var f in request.Files)
                        {
                            var url = await _blobStorage.UploadFileAsync(new byte[0], "DevolucionSPF/" + devolucionSPF.Id + "/" + f.Nombre, f.Base64);
                            if (!string.IsNullOrEmpty(url))
                            {
                                switch (i)
                                {
                                    case 0:
                                        devolucionSPF.UrlNombreDocumento1 = url; break;
                                    case 1:
                                        devolucionSPF.UrlNombreDocumento2 = url; break;
                                    case 2:
                                        devolucionSPF.UrlNombreDocumento3 = url; break;
                                }
                            }
                            i++;
                        }
                        _context.SaveChanges();
                    }

                    // Validamos que se hayan ingresado cantidades
                    if (!request.TablaFijaViewModel.Any())
                        return new ResponseGeneric<string>("No se agregaron devoluciones de certificado.", true);

                    // Recorremos las asignaciones de cada certificado
                    foreach (var item in request.TablaFijaViewModel)
                    {
                        // Obtenemos los datos del tipo de certificado y validamos que existe
                        var tipoCertificado = _context.CatTipoCertificados.FirstOrDefault(x => x.Id == item.IdTipoCertificado);
                        if (tipoCertificado == null)
                            return new ResponseGeneric<string>($"El tipo de certificado no existe.", true);
                        //Validamos que no se hay registrado algún folio en el rango

                        var folioPrevio = _context.DevolucionCertificados.Include(x => x.IdDevolucionSPFNavigation).FirstOrDefault(x => (item.FolioInicial >= x.FolioInicial && item.FolioInicial <= x.FolioFinal) ||
                                            (item.FolioFinal >= x.FolioInicial && item.FolioFinal <= x.FolioFinal) && x.IdDevolucionSPFNavigation.NumeroSolicitud.Equals(request.NumeroSolicitud));
                        if (folioPrevio != null)
                            return new ResponseGeneric<string>($"Uno o varios folios de {tipoCertificado.Nombre} ya han sido devueltos en el rango {folioPrevio.FolioInicial} - {folioPrevio.FolioFinal}.", true);
                        // Sumamos la cantidad acumulada de los folios devueltos por solicitud
                        var cantidadAcumulada = _context.DevolucionCertificados.Include(x => x.IdDevolucionSPFNavigation).Where(x => x.IdDevolucionSPFNavigation.NumeroSolicitud == null ? false :
                        x.IdDevolucionSPFNavigation.NumeroSolicitud.Trim() == request.NumeroSolicitud.ToString()).Sum(f => f.Cantidad);
                        cantidadAcumulada += item.Cantidad;
                        var ingresoPorTipo = ingresoCertificados.Where(x => x.IdCatTipoCertificado == item.IdTipoCertificado);
                        // Obtenemos la cantidad total del ingreso de la solicitud
                        if (!ingresoPorTipo.Any(x => (item.FolioInicial >= x.FolioInicial && item.FolioInicial <= x.FolioFinal) &&
                                            (item.FolioFinal >= x.FolioInicial && item.FolioFinal <= x.FolioFinal)))
                        {
                            return new ResponseGeneric<string>($"El rango de folios a devolver de {tipoCertificado.Nombre} no se encuentra en el rango de folios recibidos de la solicitud No. <b>{request.NumeroSolicitud}</b>.", true);
                        }
                        var cantidadIngreso = ingresoPorTipo.Sum(x => x.CantidadRecibida);
                        if (cantidadAcumulada >= cantidadIngreso)
                        {
                            return new ResponseGeneric<string>($"Se está devolviendo más de la cantidad recibida de <b>{tipoCertificado.Nombre}</b>, la cantidad recibida de la solicitud es <b>{cantidadIngreso}</b>. La cantidad acumulada de devoluciones es: <b>{cantidadAcumulada}</b>.", true);
                        }

                        // Obtenemos el inventario del tipo de certificado en el almacen seleccionado
                        var inventario = Almacen.Inventarios.FirstOrDefault(x => x.IdCatTipoCertificado == item.IdTipoCertificado);
                        // var cantidadStock = inventario?.CantidadStock ?? 0;
                        // Validamos que existe el inventario para el tipo de certificad
                        if (inventario == null)
                        {
                            return new ResponseGeneric<string>($"No existe inventario del certificado <b>{tipoCertificado.Nombre}</b> en almacen seleccionado.", true);
                        }

                        // Validamos que hay suficiente inventario para resguardo de ventanilla
                        //if (cantidadStock < 0)
                        //    return new ResponseGeneric<string>($"No hay suficiente inventario para cubrir las asignaciones.", true);
                        // asignamos la nueva cantidad del stock
                        //inventario.CantidadStock = cantidadStock;
                        // Creamos la asignacion de certificado
                        var _ = new DevolucionCertificado
                        {
                            Cantidad = item.Cantidad,
                            FolioFinal = item.FolioFinal,
                            FolioInicial = item.FolioInicial,
                            IdDevolucionSPF = devolucionSPF.Id,
                            IdCatTipoCertificado = item.IdTipoCertificado
                        };
                        // Validamos que la cantidad ingresada sea mayor a 0
                        //if (_.Cantidad < 1)
                        //    return new ResponseGeneric<string>($"No se agrego una cantidad para el tipo de certificado {tipoCertificado.Nombre}", true);
                        // Validamos que los folios correspondan con la cantidad
                        var sumaFolios = _.FolioFinal - _.FolioInicial;
                        if (sumaFolios != (_.Cantidad - 1))
                        {
                            return new ResponseGeneric<string>($"Los folios del certificado <b>{tipoCertificado.Nombre}</b> no corresponden con la cantidad ingresada.", true);
                        }
                        _context.DevolucionCertificados.Add(_);

                        // inventario.CantidadStock = cantidadStock + _.Cantidad;

                        // Creamos el movimiento de inventario
                        var movimiento = new MovimientosInventario
                        {
                            IdDevolucionCertificado = _.Id,
                            IdInventario = inventario.Id,
                        };
                        _context.MovimientosInventarios.Add(movimiento);
                    }
                    _context.SaveChanges();
                    dbContextTransaction.Commit();
                    return new ResponseGeneric<string>("", true);
                }
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<string>(ex);
            }
        }

        public async Task<ResponseGeneric<DevolucionSPFResponse>> ConsultaById(long id)
        {
            try
            {

                var registro = _context.DevolucionSPFs.Include(x => x.IdUserRegistroNavigation).Include(d => d.DevolucionCertificados)
                    .FirstOrDefault(x => x.Id == id);
                if (registro.Id == 0)
                {
                    return new ResponseGeneric<DevolucionSPFResponse>(new Exception("No se encontró el registro."));

                }
                var _ = new DevolucionSPFResponse
                {
                    IdDevolucionSPF = (int)id,
                    NumeroDevolucion = int.Parse(registro.NumeroDevolucion),
                    ResponsableEntrega = registro.ResponsableEntrega,
                    UserAprobo = registro.UserAprobo,
                    IdAlmacen = registro.IdAlmacen,
                    RecibioEnSPF = registro.RecibioSPF,
                    FechaEntrega = (DateTime)registro.FechaEntrega,
                    FechaEntregaString = registro.FechaEntrega?.ToString("dd/MM/yyyy"),
                    NumeroSolicitud = int.Parse(registro.NumeroSolicitud),
                    TablaFijaViewModel = new TablaFijaViewModel.TablaFijaMain().Consultar(),
                    UrlDoc1 = registro.UrlNombreDocumento1,
                    UrlDoc2 = registro.UrlNombreDocumento2,
                    UrlDoc3 = registro.UrlNombreDocumento3
                };
                foreach (var item in registro.DevolucionCertificados)
                {
                    var row = _.TablaFijaViewModel.FirstOrDefault(x => x.IdTipoCertificado == item.IdCatTipoCertificado);
                    row.Cantidad = item.Cantidad;
                    row.FolioInicial = item.FolioInicial;
                    row.FolioFinal = item.FolioFinal;
                }

                _.isReadOnly = true;
                return new ResponseGeneric<DevolucionSPFResponse>(_);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DevolucionSPFResponse>(ex);

            }
        }


        public async Task<ResponseGeneric<List<TablaFijaViewModel.TablaFijaRow>>> GetInventarioByAlmacen(long id)
        {
            try
            {
                var _ = new TablaFijaViewModel.TablaFijaMain().Consultar();
                var registro = _context.Inventarios.Where(x => x.IdAlmacen == id);
                if (registro == null && registro.Count() == 0)
                {
                    return new ResponseGeneric<List<TablaFijaViewModel.TablaFijaRow>>(new Exception("No se encontró el registro."));

                }
                foreach (var item in registro)
                {
                    //var row = registro.FirstOrDefault(x => x.IdTipoCertificado == item.IdCatTipoCertificado);
                    var row = _.FirstOrDefault(x => x.IdTipoCertificado == item.IdCatTipoCertificado);
                    row.Cantidad = item.CantidadStock;
                    row.FolioInicial = item.FolioInicial ?? 0;
                    row.FolioFinal = item.FolioFinal ?? 0;
                }


                return new ResponseGeneric<List<TablaFijaViewModel.TablaFijaRow>>(_);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<TablaFijaViewModel.TablaFijaRow>>(ex);

            }
        }

        public async Task<ResponseGeneric<int>> GetNumeroDevolucion()
        {
            try
            {
                int _;
                var registro = _context.DevolucionSPFs.OrderByDescending(x => x.NumeroDevolucion).FirstOrDefault();
                _ = registro == null ? 1 : Convert.ToInt32(registro.NumeroDevolucion) + 1;

                // var random = new Random().Next(1000, 2000);
                // _ = (registro == null) ? random : (int.Parse(registro) + 1);

                return new ResponseGeneric<int>(_);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<int>(ex);

            }
        }

        private string GenerarNumeroDevolucion(string numeroDevolucion)
        {
            var exist = _context.DevolucionSPFs.Any(x => x.NumeroDevolucion == numeroDevolucion);
            if (exist)
            {
                var getLast = _context.DevolucionSPFs.OrderByDescending(x => x.NumeroDevolucion).FirstOrDefault().NumeroDevolucion;
                var newNumero = (int.Parse(getLast) + 1);
                var newN = GenerarNumeroDevolucion(newNumero.ToString());
                return newN;
            }
            return numeroDevolucion;
        }


        public async Task<ResponseGeneric<List<DevolucionSPFResponse>>> GetDevolucionSPFDocumento(RequestList request)
        {
            try
            {
                Random rdm = new Random();

                var folios = _context.vDevolucionSPFs.AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    folios = folios.Where(x => x.NumeroDevolucion.ToLower().Contains(request.Busqueda.ToLower())
                                            || x.UserAprobo.ToLower().Contains(request.Busqueda.ToLower())
                                            || x.NumeroSolicitud.ToLower().Contains(request.Busqueda.ToLower())
                                            || x.FechaEntrega.ToString().Contains(request.Busqueda.ToLower())
                                            || x.FechaRegistro.ToString().Contains(request.Busqueda.ToLower())
                                            || x.RecibioSPF.ToLower().Contains(request.Busqueda.ToLower())
                                            || x.ResponsableEntrega.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var total = folios.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    folios = folios.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    folios = folios.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                if ((bool)!request.Pagination)
                {
                    if (request.Pagina > 0 && request.Registros > 0)
                    {
                        folios = folios.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                    }
                }

                var result = await folios.Select(x => new DevolucionSPFResponse
                {
                    NumeroDevolucion = Int32.Parse(x.NumeroDevolucion),
                    FechaRegistro = (DateTime)x.FechaRegistro,
                    FechaEntrega = (DateTime)x.FechaEntrega,
                    NumeroSolicitud = Int32.Parse(x.NumeroSolicitud),
                    UsuarioAprobo = x.UserAprobo,
                    RecibioSPF = x.RecibioSPF,
                    ResponsableEntrega = x.ResponsableEntrega,
                    Cantidad = x.Cantidad,
                }).ToListAsync();


                var verificentro = _context.Verificentros.Where(x => x.Id == _userResolver.GetUser().IdVerificentro).AsQueryable();

                var result2 = await verificentro.Select(x => new DevolucionSPFResponse
                {
                    Folio = x.Clave,
                    Direccion = x.Direccion,
                    Telefono = x.Telefono,
                    NombreEncargado = x.DirectorGestionCalidadAire,
                }).ToListAsync();

                foreach (var item in result)
                {
                    item.Folio = result2[0].Folio;
                    item.Direccion = result2[0].Direccion;
                    item.Telefono = result2[0].Telefono;
                    item.NombreEncargado = result2[0].NombreEncargado;
                }



                return new ResponseGeneric<List<DevolucionSPFResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<DevolucionSPFResponse>>(ex);
            }
        }


    }

    public interface IDevolucionSPFNegocio
    {
        public Task<ResponseGeneric<ResponseGrid<ListDevolucionSPFResponse>>> Consulta(RequestList request);
        public Task<ResponseGeneric<List<SolicitudFV>>> Autocomplete(string prefix);
        public Task<ResponseGeneric<List<User>>> AutocompleteResponsable(string prefix);
        public Task<ResponseGeneric<string>> Guardar(DevolucionSPFRequest request);
        public Task<ResponseGeneric<DevolucionSPFResponse>> ConsultaById(long id);
        public Task<ResponseGeneric<List<TablaFijaViewModel.TablaFijaRow>>> GetInventarioByAlmacen(long id);
        public Task<ResponseGeneric<int>> GetNumeroDevolucion();
        public Task<ResponseGeneric<List<DevolucionSPFResponse>>> GetDevolucionSPFDocumento(RequestList request);
    }

}