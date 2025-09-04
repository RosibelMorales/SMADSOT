using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.ConstanciaUltimaVerificacion.Request;
using Smadot.Models.Entities.ConstanciaUltimaVerificacion.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Refrendo.Request;
using Smadot.Models.Entities.Refrendo.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Transactions;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Smadot.Models.Entities.Equipo.Response;

namespace Smadot.SolicitudFormaValorada.Model.Negocio
{
    public class ConstanciaUltimaVerificacionNegocio : IConstanciaUltimaVerificacionNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        private readonly IPdfBuider _pdfBuider;
        private readonly IConfiguration _configuration;
        public ConstanciaUltimaVerificacionNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration, IPdfBuider pdfBuider)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _pdfBuider = pdfBuider;
            _configuration = configuration;
        }

        public async Task<ResponseGeneric<ResponseGrid<ConstanciaUltimaVerificacionGridResponse>>> Consulta(RequestList request)
        {
            try
            {
                var constancias = _context.vConstanciaUltimaVerificacions.AsQueryable();

                var tot = constancias.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    var date = new DateTime();
                    var dateDef = new DateTime();
                    DateTime.TryParseExact(request.Busqueda, "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out date);
                    var dateMax = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                    constancias = constancias.Where(x => x.PlacaSerie.ToLower().Contains(request.Busqueda.ToLower()) || (date != dateDef && (x.FechaRegistro >= date && x.FechaRegistro <= dateMax)) || x.NumeroReferencia.ToLower().Contains(request.Busqueda.ToLower()) || x.Nombre.ToLower().Contains(request.Busqueda.ToLower()) || x.Placa.ToLower().Contains(request.Busqueda.ToLower()) || x.Serie.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var filtered = string.IsNullOrEmpty(request.Busqueda) ? tot : constancias.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    constancias = constancias.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    constancias = constancias.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }
                DateTime now = DateTime.Now;
                var result = new ResponseGrid<ConstanciaUltimaVerificacionGridResponse>
                {
                    RecordsTotal = tot,
                    RecordsFiltered = filtered,
                    Data = JsonConvert.DeserializeObject<List<ConstanciaUltimaVerificacionGridResponse>>(JsonConvert.SerializeObject(await constancias.ToListAsync()))
                };
                return new ResponseGeneric<ResponseGrid<ConstanciaUltimaVerificacionGridResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<ConstanciaUltimaVerificacionGridResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<ConstanciaUltimaVerificacionGridResponse>> GetById(long Id, bool verificacion)
        {
            try
            {
                var result = new ConstanciaUltimaVerificacionGridResponse() { verificaciones = new List<vDataTramitesVentanilla>() };
                if (Id > 0)
                {

                    if (verificacion)
                    {
                        var verif = _context.vVerificacionReposicions.FirstOrDefault(x => x.Id == Id);

                        if (verif == null)
                            throw new Exception("No sé encontró una verificación.");
                        //result.verificaciones.Add((await _context.vVerificacionReposicions.FirstOrDefaultAsync(x => (x.Placa == verif.Placa || x.Serie == verif.Serie) && !(x.Cancelado ?? false && x.IdConstanciaUltimaVerificacion == null))) ?? new vVerificacionReposicion());
                    }
                    else
                    {
                        var constancia = _context.vConstanciaUltimaVerificacions.FirstOrDefault(x => x.Id == Id);
                        //if (constancia != null)
                        //{
                        //    string r = JsonConvert.SerializeObject(constancia);
                        //    result = JsonConvert.DeserializeObject<ConstanciaUltimaVerificacionGridResponse>(r);
                        //    if (result != null)
                        //    {

                        //        result.verificaciones = new List<vVerificacionReposicion>();
                        //        result.verificaciones.Add(await _context.vVerificacionReposicions.FirstOrDefaultAsync(x => (x.IdConstanciaUltimaVerificacion == constancia.Id || x.Placa == constancia.Placa) && !(x.Cancelado ?? false)) ?? new vVerificacionReposicion());
                        //        //result.verificaciones = await _context.vVerificacionReposicions.Where(x => x.Id == constancia.IdVerificacion && !(x.Cancelado ?? false)).ToListAsync();
                        //    }

                        //}
                    }
                }
                else
                {
                    throw new Exception("No sé encontró una constancia.");
                }
                return new ResponseGeneric<ConstanciaUltimaVerificacionGridResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ConstanciaUltimaVerificacionGridResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> Registro(ConstanciaUltimaVerificacionRequest request)
        {
            try
            {
                var constancia = new ConstanciaUltimaVerificacion();
                var verificacion = new vDataTramitesVentanilla();
                using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                var CVVSecertaria = await _context.vVerificentros.FirstOrDefaultAsync(x => x.Clave == "SMADSOT-00");
                if (request.Id > 0)
                {
                    constancia = _context.ConstanciaUltimaVerificacions.FirstOrDefault(x => x.Id == request.Id);
                    if (constancia == null)
                        throw new ValidationException("No sé encontró una constancia.");
                }
                else
                {

                    verificacion = _context.vDataTramitesVentanillas.FirstOrDefault(x => x.Id == request.IdFv);
                    if (verificacion == null)
                        throw new ValidationException("No sé encontró la verificación.");
                    var count = _context.ConstanciaUltimaVerificacions.Count() + 1;
                    var aux = _context.CicloVerificacions.FirstOrDefault(x => x.Activo && x.FechaInicio <= DateTime.Now && x.FechaFin >= DateTime.Now);
                    var date = DateTime.Now;
                    constancia = new ConstanciaUltimaVerificacion
                    {
                        //IdVerificacion = request.IdVerificacion,
                        NumeroReferencia = request.NumeroReferencia,
                        UrlDoc1 = request.UrlDoc1,
                        UrlDoc2 = request.UrlDoc2,
                        UrlDoc3 = request.UrlDoc3,
                        Placa = verificacion.Placa,
                        FechaRegistro = DateTime.Now,
                        FechaVerificacion = verificacion.Fecha,
                        IdUserRegistro = _userResolver.GetUser().IdUser,
                        Serie = verificacion.Serie,
                        ConsecutivoTramite = count,
                        ClaveTramite = $"COV-{date.Year}/{count}",
                        FechaEmisionRef = request.FechaEmisionRef,
                        FechaPago = request.FechaPago,
                        EntidadProcedencia = verificacion.EntidadProcedencia,
                        ServidorPublico = "",
                        Vigencia = (DateTime)verificacion.Vigencia,
                        ImporteActual = aux.ImporteConstanciaUltimaVer,
                        Marca = verificacion.Marca,
                        Combustible = verificacion.Combustible ?? string.Empty,
                        IdVerificentro = CVVSecertaria.Id,
                        IdCatTipoCertificado = verificacion.IdCatTipoCertificado ?? 0,
                        NombrePropietario = verificacion.Propietario,
                        Modelo = verificacion.Modelo,
                        Submarca = verificacion.Submarca,
                        TarjetaCirculacion = verificacion.TarjetaCirculacion,
                        FolioVerificacion = (verificacion.FolioCertificado ?? 0).ToString()

                    };
                    _context.ConstanciaUltimaVerificacions.Add(constancia);
                }
                var result = await _context.SaveChangesAsync() > 0;

                var i = 0;
                foreach (var f in request.Files)
                {
                    var url = await _blobStorage.UploadFileAsync(new byte[0], "ConstanciaUltimaVerificacion/" + constancia.Id + "/" + f.Nombre, f.Base64);
                    if (!string.IsNullOrEmpty(url))
                    {
                        switch (i)
                        {
                            case 0:
                                constancia.UrlDoc1 = url; break;
                            case 1:
                                constancia.UrlDoc2 = url; break;
                            case 2:
                                constancia.UrlDoc3 = url; break;
                        }
                    }
                    i++;
                }
                var vConstancia = new ConstanciaUltimaVerificacionGridResponse
                {
                    Id = constancia.Id,
                    IdVerificacion = constancia?.IdVerificacion ?? 0,
                    NumeroReferencia = constancia?.NumeroReferencia ?? string.Empty,
                    //NombreC = verificacion.NombreDueñoVeh,
                    NombreEncargado = CVVSecertaria.DirectorGestionCalidadAire,
                    Telefono = CVVSecertaria.Telefono,
                    Direccion = CVVSecertaria.Direccion,
                    //Folio = verificacion.FolioCertificado,
                    UrlRoot = _configuration["SiteUrl"],
                    verificaciones = new List<vDataTramitesVentanilla>()
                };
                vConstancia.verificaciones.Add(verificacion);

                var vm = new ConstanciaUltimaVerificacionResponse
                {
                    //Id = constancia.Id,
                    IdVerificacion = constancia.IdVerificacion,
                    Placa = constancia.Placa,
                    Serie = constancia.Serie,
                    Marca = constancia.Marca,
                    UrlDoc1 = constancia.UrlDoc1,
                    UrlDoc2 = constancia.UrlDoc2,
                    UrlDoc3 = constancia.UrlDoc3,
                    //FechaCartaFactura = constancia.FechaCartaFactura,
                    FechaRegistro = verificacion.Fecha,
                    TipoCertificado = verificacion.TipoCertificadoFV,
                    VigenciaHoloAnterior = constancia.Vigencia,
                    IdUserRegistro = constancia.IdUserRegistro,
                    Nombre = constancia.NombrePropietario,
                    Vigencia = constancia.Vigencia,
                    Modelo = constancia.Submarca,
                    ClaveTramite = constancia.ClaveTramite,
                    Anio = constancia.Modelo,                   
                    //NombreC = constancia.NombreC,
                    NombreEncargado = CVVSecertaria.DirectorGestionCalidadAire,
                    Telefono = CVVSecertaria.Telefono,
                    Direccion = CVVSecertaria.Direccion,
                    Folio = constancia.FolioVerificacion,
                    UrlRoot = _configuration["SiteUrl"],
                    //verificaciones = constancia.verificaciones

                };
                //JsonConvert.DeserializeObject<ConstanciaUltimaVerificacionResponse>(JsonConvert.SerializeObject(vConstancia));
                var getdoc = await _pdfBuider.GetConstanciaUltimaVerificacion(vm);
                if (getdoc.Response.NombreDocumento != null)
                {
                    var url = await _blobStorage.UploadFileAsync(getdoc.Response.DocumentoPDF, "ConstanciaUltimaVerificacion/" + constancia.Id + "/Certificado.pdf");
                    constancia.CodigoQr = url;
                }

                result = await _context.SaveChangesAsync() > 0;

                scope.Complete();
                return result ? new ResponseGeneric<long>(constancia.Id) : new ResponseGeneric<long>();
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<long>(ex) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex) { mensaje = "La información de la constancia no pudo ser guardada." };
            }
        }

        public async Task<ResponseGeneric<List<RefrendoAutocompleteResponse>>> Autocomplete(RefrendoAutocompletRequest request)
        {
            try
            {
                var verificaciones = _context.vPlacaSerieBusqueda.Where(x => request.Placa ? x.Placa.ToLower().Contains(request.Term.ToLower()) : x.Serie.ToLower().Contains(request.Term.ToLower())).GroupBy(x => x.Placa).AsQueryable();
                var tot = verificaciones.Count();
                verificaciones = verificaciones.Skip(request.Start).Take(request.End);
                var result = await verificaciones.Select(x => new RefrendoAutocompleteResponse
                {
                    Id = x.FirstOrDefault().Id,
                    Text = x.FirstOrDefault().Placa + " / " + x.FirstOrDefault().Serie
                }).ToListAsync();
                return new ResponseGeneric<List<RefrendoAutocompleteResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<RefrendoAutocompleteResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<ConstanciaUltimaVerificacionDetalleResponse>> Detalle(long id)
        {
            try
            {
                var cons = await _context.vConstanciaUltimaVerificacions.FirstOrDefaultAsync(x => x.Id == id);

                if (cons is null)
                    return new ResponseGeneric<ConstanciaUltimaVerificacionDetalleResponse>();

                var result = new ConstanciaUltimaVerificacionDetalleResponse
                {
                    Id = cons.Id,
                    FechaRegistro = cons.FechaRegistro,
                    IdUserRegistro = cons.IdUserRegistro,
                    Placa = cons.Placa,
                    Serie = cons.Serie,
                    NumeroReferencia = cons.NumeroReferencia,
                    UrlDoc1 = cons.UrlDoc1,
                    UrlDoc2 = cons.UrlDoc2,
                    UrlDoc3 = cons.UrlDoc3,
                    Combustible = cons.Combustible,
                    FechaEmisionRef = cons.FechaEmisionRef,
                    EntidadProcedencia = cons.EntidadProcedencia,
                    FechaPago = cons.FechaPago,
                    Marca = cons.Marca,
                    Modelo = cons.Modelo,
                    Vigencia = cons.Vigencia,
                    TipoCertificado = cons.TipoCertificado,
                    Anio = cons.Anio,
                    TarjetaCirculacion = cons.TarjetaCirculacion,
                    Fecha = cons.Fecha,
                    Semestre = cons.Semestre,
                    NombreRazonSocial = cons.NombreRazonSocial,
                    FolioCertificado= cons.FolioCertificado
                };

                return new ResponseGeneric<ConstanciaUltimaVerificacionDetalleResponse>(result);
            }
            catch(Exception ex)
            {
                return new ResponseGeneric<ConstanciaUltimaVerificacionDetalleResponse>(ex);
            }
        }
    }

    public interface IConstanciaUltimaVerificacionNegocio
    {
        public Task<ResponseGeneric<ResponseGrid<ConstanciaUltimaVerificacionGridResponse>>> Consulta(RequestList request);
        public Task<ResponseGeneric<ConstanciaUltimaVerificacionGridResponse>> GetById(long Id, bool verificacion);
        public Task<ResponseGeneric<long>> Registro(ConstanciaUltimaVerificacionRequest request);
        public Task<ResponseGeneric<List<RefrendoAutocompleteResponse>>> Autocomplete(RefrendoAutocompletRequest request);

        public Task<ResponseGeneric<ConstanciaUltimaVerificacionDetalleResponse>> Detalle(long Id);
    }
}
