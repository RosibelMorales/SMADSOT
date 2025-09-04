using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Polly;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.AsignacionStock.Request;
using Smadot.Models.Entities.AsignacionStock.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks.Dataflow;

namespace Smadot.AdministracionStock.Negocio.Negocio
{
    public class AsignacionStock : IAsignacionStock
    {
        private SmadotDbContext _context;

        private IUserResolver _userResolver;
        public AsignacionStock(SmadotDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }
        public async Task<ResponseGeneric<ResponseGrid<ListAsignacionStockResponse>>> Consulta(RequestList request)
        {
            try
            {

                var _ = new ResponseGrid<ListAsignacionStockResponse>();
                var data = _context.vAsignacionStocks.AsQueryable();
                _.RecordsTotal = data.Count();
                _.RecordsFiltered = data.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    data = data.Where(asv => asv.Nombre.Contains(request.Busqueda) || asv.NombreRecibio.Contains(request.Busqueda) || asv.Id.ToString().Contains(request.Busqueda));
                    _.RecordsFiltered = data.Count();
                }
                data = data.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                _.Data = data.Skip(request.Pagina.Value).Take(request.Registros.Value).Select(x => new ListAsignacionStockResponse
                {
                    Id = x.Id,
                    FechaEntrega = x.FechaEntrega,
                    FechaRegistro = x.FechaRegistro,
                    Nombre = x.NombreRecibio,
                    UserRecibe = x.UserRecibe,
                    IdAlmacen = x.IdAlmacen,
                    IdUserRegistro = x.IdUserRegistro,
                    UserAprobo = x.UserAprobo,
                }).ToList();

                return new ResponseGeneric<ResponseGrid<ListAsignacionStockResponse>>(_);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<ListAsignacionStockResponse>>(ex);

            }
        }
        public async Task<ResponseGeneric<RegistroStockventanillaResponse>> Consulta(long id)
        {
            try
            {

                var registro = _context.AsignacionSVs.Include(x => x.IdUserRegistroNavigation).Include(d => d.AsignacionCertificados)
                    .FirstOrDefault(x => x.Id == id);
                if (registro.Id == 0)
                {
                    return new ResponseGeneric<RegistroStockventanillaResponse>(new Exception("No se encontró el registro."));

                }
                var _ = new RegistroStockventanillaResponse
                {
                    Id = id,
                    FechaEntrega = registro.FechaEntrega,
                    IdAlmacen = registro.IdAlmacen,
                    UserRecibe = registro.UserRecibe,
                    UserSolicita = registro.IdUserRegistroNavigation.Nombre,
                    FechaSolicitud = registro.FechaRegistro,
                    IdUserSolicita = registro.IdUserRegistro,
                    TablaFijaViewModel = new TablaFijaViewModel.TablaFijaMain().Consultar()
                };
                foreach (var item in registro.AsignacionCertificados)
                {
                    var row = _.TablaFijaViewModel.FirstOrDefault(x => x.IdTipoCertificado == item.IdCatTipoCertificado);
                    row.Cantidad = item.Cantidad;
                    row.FolioInicial = item.FolioInicial;
                    row.FolioFinal = item.FolioFinal;
                }

                _.isReadOnly = true;
                return new ResponseGeneric<RegistroStockventanillaResponse>(_);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<RegistroStockventanillaResponse>(ex);

            }
        }
        public async Task<ResponseGeneric<string>> Guardar(RegistroStockventanillaRequest request)
        {
            try
            {
                using (var dbContextTransaction = _context.Database.BeginTransaction())
                {
                    var asignacionSV = new AsignacionSV
                    {
                        IdAlmacen = request.IdAlmacen,
                        FechaEntrega = (DateTime)request.FechaEntrega,
                        FechaRegistro = DateTime.Now,
                        IdUserRegistro = _userResolver.GetUser().IdUser,
                        NombreRecibio = request.UserRecibe,
                        UserAprobo = _userResolver.GetUser().Nombre,
                        UserRecibe = request.UserRecibe
                    };
                    //Creamos la asignación a ventanilla
                    _context.AsignacionSVs.Add(asignacionSV);
                    _context.SaveChanges();
                    // Validamos que se hayan ingresado cantidades
                    if (!request.Asignaciones.Any())
                        return new ResponseGeneric<string>("No se agregaron asignaciones de certificado.", true);
                    //Obtenemos el almacen para obtener los inventarios
                    var Almacen = _context.Almacens.Include(i => i.Inventarios).FirstOrDefault(x => x.Id == request.IdAlmacen);
                    if (Almacen == null)
                        return new ResponseGeneric<string>($"No existe el almacen seleccionado.", true);
                    // Recorremos las asignaciones de cada certificado
                    foreach (var item in request.Asignaciones)
                    {

                        // Obtenemos los datos del tipo de certificado y validamos que existe
                        var tipoCertificado = _context.CatTipoCertificados.FirstOrDefault(x => x.Id == item.IdTipoCertificado);
                        if (tipoCertificado == null)
                            return new ResponseGeneric<string>($"El tipo de certificado no existe.", true);
                        var isInStock = _context.vMovimientosInventarioEnStocks.Any(x => x.IdVerificentro == null && item.FolioInicial >= x.FolioInicial && item.FolioFinal <= x.FolioFinal && item.FolioInicial >= x.FolioInicial && item.FolioFinal <= x.FolioFinal && x.IdCatTipoCertificado == item.IdTipoCertificado);
                        if (isInStock)
                        {
                            return new ResponseGeneric<string>($"El rango de folios {tipoCertificado.Nombre} no se encuentra en Stock.", true);
                        }
                        //Validamos que no se hay registrado algún folio en el rango
                        var folioPrevio = _context.AsignacionCertificados.FirstOrDefault(x => (item.FolioInicial >= x.FolioInicial && item.FolioInicial <= x.FolioFinal) ||
                        (item.FolioFinal >= x.FolioInicial && item.FolioFinal <= x.FolioFinal));
                        if (folioPrevio != null)
                            return new ResponseGeneric<string>($"Uno o varios folios de {tipoCertificado.Nombre} ya se han asignado a ventanilla en el rango {folioPrevio.FolioInicial} - {folioPrevio.FolioFinal}.", true);
                        // Obtenemos el inventario del tipo de certificado en el almacen seleccionado
                        var inventario = Almacen.Inventarios.FirstOrDefault(x => x.IdCatTipoCertificado == item.IdTipoCertificado);
                        var cantidadStock = (inventario?.CantidadStock ?? 0) - item.Cantidad;
                        // Validamos que existe el inventario para el tipo de certificado
                        if (inventario == null)
                            return new ResponseGeneric<string>($"No existe inventario del certificado {tipoCertificado.Nombre} en almacen seleccionado.", true);
                        // Validamos que hay suficiente inventario para resguardo de ventanilla
                        if (cantidadStock < 0)
                            return new ResponseGeneric<string>($"No hay suficiente inventario de {tipoCertificado.Nombre} para cubrir las asignaciones.", true);
                        // Validamos que la cantidad ingresada sea mayor a 0
                        if (item.Cantidad < 1)
                            return new ResponseGeneric<string>($"No se agrego una cantidad para el tipo de certificado {tipoCertificado.Nombre}", true);
                        // Validamos que los folios correspondan con la cantidad
                        var restaFolios = item.FolioFinal - item.FolioInicial;
                        if (restaFolios != (item.Cantidad - 1))
                        {
                            return new ResponseGeneric<string>($"Los folios del certificado {tipoCertificado.Nombre} no corresponden con la cantidad ingresada.", true);
                        }
                        // asignamos la nueva cantidad del stock
                        inventario.CantidadStock = cantidadStock;
                        // Creamos la asignacion de certificado
                        var _ = new AsignacionCertificado
                        {
                            Cantidad = item.Cantidad,
                            FolioFinal = item.FolioFinal,
                            FolioInicial = item.FolioInicial,
                            IdAsignacionSV = asignacionSV.Id,
                            IdCatTipoCertificado = item.IdTipoCertificado
                        };

                        _context.AsignacionCertificados.Add(_);
                        await _context.SaveChangesAsync();
                        // Creamos el movimiento de inventario
                        var movimiento = new MovimientosInventario
                        {
                            IdAsignacionCertificado = _.Id,
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
    }

    public interface IAsignacionStock
    {
        public Task<ResponseGeneric<ResponseGrid<ListAsignacionStockResponse>>> Consulta(RequestList request);
        public Task<ResponseGeneric<RegistroStockventanillaResponse>> Consulta(long id);
        public Task<ResponseGeneric<string>> Guardar(RegistroStockventanillaRequest request);
    }

}