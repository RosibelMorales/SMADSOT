using Smadot.Models.DataBase;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Smadot.Models.Entities.FoliosCancelados.Response.FoliosCanceladosResponseData;
using System.Linq.Dynamic.Core;
using static Smadot.Models.Entities.FoliosCancelados.Request.FoliosCanceladosRequestData;
using Microsoft.EntityFrameworkCore;
using Smadot.Utilities.GestionTokens;
using Smadot.Models.Dicts;
using System.Globalization;
using System.Transactions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Smadot.SolicitudFormaValorada.Model.Negocio
{
    public class FoliosCanceladosNegocio : IFoliosCanceladosNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;

        public FoliosCanceladosNegocio(SmadotDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public async Task<ResponseGeneric<List<FoliosCanceladosResponse>>> Consulta(RequestList request)
        {
            try
            {
                ///Se agrego ordenamiento debido a que el grid mostraba los registros desordenados
                ///REVISAR OPCIONES
                var catalogo = _context.vFoliosFormaValoradaVerificentros.Where(x => x.Cancelado).AsQueryable();
                //var catalogo = _context.vFoliosCancelados.AsQueryable();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    if (request.ColumnaOrdenamiento.Equals("usuarioAprobo"))
                        catalogo = catalogo.OrderBy("UserCancelo " + request.Ordenamiento);
                    else if (request.ColumnaOrdenamiento.Equals("TipoTramite"))
                        catalogo = catalogo.OrderBy("IdCatTipoTramite " + request.Ordenamiento);
                    else if (request.ColumnaOrdenamiento.Equals("DatosVehiculo"))
                        catalogo = catalogo.OrderBy("Marca " + request.Ordenamiento).ThenBy("Anio " + request.Ordenamiento).ThenBy("ColorVehiculo " + request.Ordenamiento).ThenBy("Placa " + request.Ordenamiento);
                    else if (request.ColumnaOrdenamiento.Equals("PersonaRealizoTramite"))
                        catalogo = catalogo.OrderBy("NombreDueñoVeh " + request.Ordenamiento);
                    else if (request.ColumnaOrdenamiento.Equals("motivo"))
                        catalogo = catalogo.OrderBy("CatMotivoCancelacion " + request.Ordenamiento).ThenBy("OtroMotivo " + request.Ordenamiento);
                    else
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
                    var prefixBool = long.TryParse(request.Busqueda, out long prefix);
                    var segundaBusqueda = await _context.vFoliosFormaValoradaExentosImpresions.Where(x =>
                        x.Marca.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Modelo.ToString().ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Propietario.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Placa.ToLower().Contains(request.Busqueda.ToLower())).Select(x => x.Id).ToListAsync();
                    catalogo = catalogo.Where(x =>
                                                    x.CatMotivoCancelacion.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.OtroMotivo.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.UserCancelo.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    //x.TipoTramite.ToLower().Contains(request.Busqueda.ToLower()) || 
                                                    x.Marca.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Anio.ToString().ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.ColorVehiculo.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.Placa.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    segundaBusqueda.Contains(x.Id) ||
                                                    (prefixBool ? x.Folio.ToString().Contains(prefix.ToString()) : x.Folio.ToString().Contains(request.Busqueda.ToLower()))
                                                    );


                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new FoliosCanceladosResponse
                {
                    Folio = x.Folio.ToString("000000000"),
                    FechaCancelacion = x.FechaCancelacion.HasValue ? x.FechaCancelacion.Value.ToString("dd/MM/yyyy") : "",
                    Motivo = x.CatMotivoCancelacion,
                    UsuarioAprobo = x.UserCancelo,
                    TipoTramite = "Ventanilla",//Hardcode Ventanilla
                    DatosVehiculo = $"{x.Marca} {x.Anio} {x.ColorVehiculo} {x.Placa}",
                    PersonaRealizoTramite = x.NombreDueñoVeh,
                    Id = x.Id,

                    Fecha = x.FechaCitaVerificacion.HasValue ? x.FechaCitaVerificacion.Value.ToString("dd/MM/yyyy") : "",
                    IdCatMotivoCancelacion = x.IdCatMotivoCancelacion,
                    IdUserCancelo = x.IdUserCancelo,
                    IdCatTipoTramite = x.IdCatMotivoCancelacion != null ? 1 : 0,
                    OtroMotivo = x.OtroMotivo,

                    Total = tot,
                }).ToListAsync();

                var idsExentos = await catalogo.Where(x => x.IdVerificacion == null).Select(x => x.Id).ToListAsync();
                if (idsExentos.Count > 0)
                {
                    var exento = await _context.vFoliosFormaValoradaExentosImpresions.Where(x => idsExentos.Contains(x.Id)).ToListAsync();
                    foreach (var e in exento)
                    {
                        result.FirstOrDefault(x => x.Id == e.Id).DatosVehiculo = $"{e.Marca} {e.Modelo} {e.Placa}";
                        result.FirstOrDefault(x => x.Id == e.Id).PersonaRealizoTramite = e.Propietario;
                    }
                }

                return new ResponseGeneric<List<FoliosCanceladosResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<FoliosCanceladosResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> CancelarFolio(FolioCanceladosRequest request)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var user = _userResolver.GetUser();
                    var result = new ResponseGeneric<bool>();
                    DateTime parseDateTime;
                    var date = DateTime.Now;

                    var fv = _context.FoliosFormaValoradaVerificentros.FirstOrDefault(x => x.Id == request.IdFolio);
                    if (fv == null && request.FolioExists)
                    {
                        result.mensaje = "No se encontró el Folio indicado";
                        result.Response = false;
                        return result;
                    }
                    if (string.IsNullOrEmpty(request.FechaCancelacion))
                    {
                        parseDateTime = DateTime.Now;
                    }
                    else
                    {
                        request.FechaCancelacion = request.FechaCancelacion;
                        var tryparseDate = DateTime.TryParseExact(request.FechaCancelacion, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parseDateTime);
                    }
                    if (request.MotivoCancelacion == 3)
                    {
                        request.OtroMotivo = string.IsNullOrEmpty(request.OtroMotivo) ? "Sin especificar" : request.OtroMotivo;
                    }
                    else
                    {
                        request.OtroMotivo = null;
                    }
                    // Habilitar cuando la operacón este al 100%
                    // var isInStock = _context.vMovimientosInventarioEnStocks.Any(x => x.IdVerificentro == user.IdVerificentro && request.FolioNuevo >= x.FolioInicial && request.FolioNuevo <= x.FolioFinal && x.IdCatTipoCertificado == request.IdCatTipoCertificado);
                    // if (!isInStock)
                    // {
                    //     result.mensaje = "El Folio no esta en stock del centro de verificación.";
                    //     result.Response = false;
                    //     // throw new Exception($"No se encontró folio {request.FolioNuevo} de {TipoCertificado.DictNombreCertificado[request.IdCatTipoCertificado ?? 0]} en stock. No se puede cancelar.");
                    // }
                    if (!request.FolioExists)
                    {

                        var ciclo = _context.CicloVerificacions.FirstOrDefault(x => x.Activo && x.FechaInicio <= date && x.FechaFin >= date);
                        var claveTramite = TipoTramite.CV;
                        if (request.IdCatTipoCertificado == TipoCertificado.Exentos)
                            claveTramite = TipoTramite.CE;
                        else if (request.IdCatTipoCertificado == TipoCertificado.Testificacion)
                            claveTramite = TipoTramite.CT;
                        else if (user.ClaveVerificentro.Equals("SMADSOT-00"))
                            claveTramite = TipoTramite.CA;
                        var claveTramiteString = TipoTramite.Dict[claveTramite];
                        var importe = 0M;
                        switch (claveTramite)
                        {
                            case TipoTramite.CV:
                                importe = ciclo.ImporteFv;
                                break;
                            case TipoTramite.CE:
                                importe = ciclo.ImporteExento;
                                break;
                            case TipoTramite.CA:
                                importe = ciclo.ImporteAdministrativo;
                                break;
                            case TipoTramite.CT:
                                importe = ciclo.ImporteTestificacion;
                                break;
                        }
                        fv = new FoliosFormaValoradaVerificentro
                        {
                            IdCatTipoTramite = claveTramite == TipoTramite.CV ? TipoTramiteDict.CVV : TipoTramiteDict.Ventanilla,
                            IdCatTipoCertificado = request.IdCatTipoCertificado,
                            IdVerificentro = user.IdVerificentro.Value,
                            Folio = request.FolioNuevo.Value,
                            ImporteActual = importe,
                            FechaRegistro = date,
                            IdUserRegistro = user.IdUser,
                            FechaEmisionRef = date,
                            FechaPago = date,
                            ClaveTramite = string.Empty,
                            ConsecutivoTramite = 0,
                            EntidadProcedencia = string.Empty,
                            ServidorPublico = string.Empty,
                            Reposicion = false,
                        };

                        // var inventarioDB = new Almacen();
                        // var almacen = new Almacen();
                        // if (claveTramite != TipoTramite.CV)
                        //     almacen = await _context.Almacens.FirstOrDefaultAsync(x => x.IdVerificentro == null);
                        // else
                        //     almacen = await _context.Almacens.FirstOrDefaultAsync(x => x.IdVerificentro == user.IdVerificentro.Value);

                        // if (almacen == null)
                        // {
                        //     result.Response = false;
                        //     result.mensaje = $"No se encontró el almacen del centro.";
                        //     return result;
                        // }
                        // var inventarioDestinoDB = await _context.Inventarios.FirstOrDefaultAsync(x => x.IdAlmacen == almacen.Id && x.IdCatTipoCertificado == request.IdCatTipoCertificado);

                        // if (inventarioDestinoDB == null)
                        // {
                        //     result.Response = false;
                        //     result.mensaje = $"No se encontró el inventario del folio.";
                        //     return result;
                        // }

                        // inventarioDestinoDB.CantidadStock -= 1;

                        // if (inventarioDestinoDB.CantidadStock < 0)
                        // {
                        //     result.Response = false;
                        //     result.mensaje = $"Se ha agotado el stock del certificado.";
                        //     return result;
                        // }
                    }

                    var count = _context.FoliosFormaValoradaVerificentros.Where(x => x.Cancelado && x.IdCatTipoTramite != null).Count() + 1;
                    fv.FechaCancelacion = parseDateTime;
                    fv.IdCatMotivoCancelacion = request.MotivoCancelacion;
                    fv.OtroMotivo = request.OtroMotivo;
                    fv.Cancelado = true;
                    fv.ClaveTramiteCancelado = $"{TipoTramite.Dict[TipoTramite.CANCELADO]}";
                    fv.IdUserCancelo = user.IdUser;
                    fv.ConsecutivoTramiteCancelado = count;

                    if (!request.FolioExists)
                        _context.FoliosFormaValoradaVerificentros.Add(fv);

                    var res = await _context.SaveChangesAsync();
                    result.mensaje = "El Folio fue cancelado correctamente";
                    result.Response = true;

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                JObject data = new JObject();
                data["Exception"] = JsonConvert.SerializeObject(ex);
                data["Tipo"] = "ExcepcionCancelarFolio";
                _context.Errors.Add(new()
                {
                    Created = DateTime.Now,
                    Values = JsonConvert.SerializeObject(data)
                });
                await _context.SaveChangesAsync();
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<List<FoliosCanceladosAutoCompleteResponse>>> ConsultaAutocomplete(string prefix)
        {
            try
            {
                //var result = _context.FormaValorada.Where(x => x.Folio.ToLower().Contains(prefix.ToLower()) && x.IdCatEstatusFV != 2).Take(10).ToList();
                prefix = int.Parse(prefix).ToString();
                var user = _userResolver.GetUser();
                var folios = await _context.FoliosFormaValoradaVerificentros.Where(x => x.Folio.ToString().Contains(prefix) && !x.Cancelado && x.IdCatTipoTramite != null && x.IdVerificentro == user.IdVerificentro).Take(10).ToListAsync();
                var result = folios.Select(x => new FoliosCanceladosAutoCompleteResponse
                {
                    Id = x.Id,
                    Cancelado = x.Cancelado,
                    Folio = x.Folio.ToString("000000000") + " / " + TipoCertificado.DictNombreCertificado[x.IdCatTipoCertificado ?? 1]
                }).ToList();
                return new ResponseGeneric<List<FoliosCanceladosAutoCompleteResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<FoliosCanceladosAutoCompleteResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<FoliosCanceladosResponse>> GetById(long Id)
        {
            try
            {
                var formaV = _context.vFoliosFormaValoradaVerificentros.FirstOrDefault(x => x.Id == Id);

                var result = new FoliosCanceladosResponse
                {
                    Id = formaV.Id,
                    Folio = formaV.Folio.ToString("000000000"),
                    FechaCancelacion = formaV.FechaCancelacion.HasValue ? formaV.FechaCancelacion.Value.ToString("dd/MM/yyyy") : "",
                    Motivo = formaV.CatMotivoCancelacion,
                    UsuarioAprobo = formaV.UserCancelo,
                    TipoTramite = "Ventanilla",//Hardcode Ventanilla
                    DatosVehiculo = $"{formaV.Marca} {formaV.Anio} {formaV.ColorVehiculo} {formaV.Placa}",
                    PersonaRealizoTramite = formaV.NombreDueñoVeh,
                    Fecha = formaV.FechaCitaVerificacion.HasValue ? formaV.FechaCitaVerificacion.Value.ToString("dd/MM/yyyy") : "",
                    IdCatMotivoCancelacion = formaV.IdCatMotivoCancelacion,
                    IdCatTipoTramite = formaV.IdCatMotivoCancelacion != null ? 1 : 0,
                    IdCatEstatusFV = 2,
                    OtroMotivo = formaV.OtroMotivo

                };
                if (!formaV.IdVerificacion.HasValue)
                {
                    var exento = _context.vFoliosFormaValoradaImpresions.FirstOrDefault(x => x.Id == formaV.Id);
                    result.UsuarioAprobo = exento.NombreCapturista;
                    result.DatosVehiculo = $"{exento.Marca} {exento.Modelo} {exento.Anio} {exento.Placa}";
                    result.Fecha = exento.FechaRegistro.HasValue ? exento.FechaRegistro.Value.ToString("dd/MM/yyyy") : "";
                    result.PersonaRealizoTramite = exento.Propietario;
                }

                return new ResponseGeneric<FoliosCanceladosResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<FoliosCanceladosResponse>(ex);
            }
        }

    }

    public interface IFoliosCanceladosNegocio
    {
        public Task<ResponseGeneric<List<FoliosCanceladosResponse>>> Consulta(RequestList request);
        public Task<ResponseGeneric<bool>> CancelarFolio(FolioCanceladosRequest request);
        public Task<ResponseGeneric<List<FoliosCanceladosAutoCompleteResponse>>> ConsultaAutocomplete(string request);
        public Task<ResponseGeneric<FoliosCanceladosResponse>> GetById(long Id);
    }
}
