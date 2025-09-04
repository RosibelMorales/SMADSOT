using Smadot.Models.DataBase;
using Smadot.Utilities.Modelos;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using System.Transactions;
using Smadot.Models.Dicts;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.GenericProcess;
using Newtonsoft.Json;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Testificacion.Request;
using Smadot.Models.Entities.Testificacion.Response;

namespace Smadot.SolicitudFormaValorada.Model.Negocio
{
    public class TestificacionNegocio : ITestificacionNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;
        public TestificacionNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration, SmadsotGenericInserts smadsotGenericInserts)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _smadsotGenericInserts = smadsotGenericInserts;
        }
        public async Task<ResponseGeneric<ResponseGrid<TestificacionResponseGrid>>> Consulta(RequestList request)
        {
            try
            {
                var tot = await _context.vVerificacionReposicions.CountAsync(x => x.IdTestificacion != null && !(x.Cancelado ?? false));
                var catalogo = _context.vVerificacionReposicions.Where(x => x.IdTestificacion != null && !(x.Cancelado ?? false) && (string.IsNullOrEmpty(request.Busqueda)
                               || EF.Functions.Like(x.Placa.ToLower(), $"%{request.Busqueda}%")
                               || EF.Functions.Like(x.Serie.ToLower(), $"%{request.Busqueda}%")
                               || EF.Functions.Like(x.NumeroReferencia.ToLower(), $"%{request.Busqueda}%")
                               || EF.Functions.Like(x.FolioCertificado.ToString(), $"%{request.Busqueda}%")
                               || EF.Functions.Like(x.NombreTecnico.ToLower(), $"%{request.Busqueda}%")));
                var filtered = await catalogo.CountAsync();
                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = new ResponseGrid<TestificacionResponseGrid>
                {
                    RecordsTotal = tot,
                    RecordsFiltered = filtered,
                    Data = JsonConvert.DeserializeObject<List<TestificacionResponseGrid>>(JsonConvert.SerializeObject(await catalogo.ToListAsync()))
                };
                return new ResponseGeneric<ResponseGrid<TestificacionResponseGrid>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<TestificacionResponseGrid>>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Guardar(TestificacionApiRequest request)
        {
            try
            {
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = new ResponseGeneric<bool>();

                    var user = _userResolver.GetUser();
                    var idUser = user.IdUser;
                    var verificentro = await _context.vVerificentros.FirstOrDefaultAsync(x => x.Clave.Equals("SMADSOT-00"));
                    Testificacion testificacion = new()
                    {
                        FechaRegistro = DateTime.Now,
                        FolioOrigen = request.FolioAsignado,
                        IdTipoCertificadoOrigen = request.IdCatTipoCertificado,
                        Marca = request.Marca,
                        SubMarca = request.Submarca,
                        Modelo = request.Modelo ?? 0,
                        Combustible = Combustible.DictCombustible[request.IdTipoCombustible],
                        IdTipoCombustible = request.IdTipoCombustible,
                        Propietario = request.NombrePropietario,
                        PersonaTramite = request.PersonaTramite,
                        NumeroRef = request.NumeroReferencia,
                        Placa = request.Placa.Trim(),
                        Serie = request.Serie.Trim(),
                        TarjetaCirculacion = request.TarjetaCirculacion,
                        VigenciaOrigen = request.VigenciaOrigen,

                    };


                    _context.Testificacions.Add(testificacion);
                    await _context.SaveChangesAsync();
                    var validado = await _smadsotGenericInserts.ValidateFolio(TipoCertificado.Testificacion, verificentro.Id, TipoTramite.CT, idUser, request.EntidadProcedencia, null, null, null, testificacion.Id, false);
                    if (!validado.IsSucces)
                    {
                        result.Status = ResponseStatus.Failed;
                        result.mensaje = validado.Description ?? "";
                        result.Response = true;
                        return result;
                    }


                    var i = 0;
                    foreach (var file in request.Files)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], $"Testificacion/{i}/" + testificacion.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            switch (i)
                            {
                                case 0:
                                    testificacion.UrlCertificado = url; break;
                                case 1:
                                    testificacion.UrlTarjetaCirculacion = url; break;
                                case 2:
                                    testificacion.UrlIdentificacion = url; break;                             
                            }
                        }
                        i++;
                    }

                    await _context.SaveChangesAsync();
                    ts.Complete();
                    ts.Dispose();
                    result.mensaje = "La información se guardo correctamente";
                    result.Response = true;

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<TestificacionResponse>> GetById(long Id)
        {
            try
            {
                var folio = await _context.vTestificacions.FirstOrDefaultAsync(x => x.IdFolio == Id);

                var result = JsonConvert.DeserializeObject<TestificacionResponse>(JsonConvert.SerializeObject(folio));
                return new ResponseGeneric<TestificacionResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<TestificacionResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Eliminar(long id)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var registroTestificacion = await _context.Testificacions.FirstOrDefaultAsync(x => x.Id == id);

                    if (registroTestificacion == null)
                    {
                        return new ResponseGeneric<bool>($"Ocurrio un error al encontrar el registro.", true) { Status = ResponseStatus.Failed, mensaje = $"Ocurrio un error al encontrar el registro." };
                    }

                    var folioFormaValorada = _context.FoliosFormaValoradaVerificentros.Where(x => x.IdTestificacion == id).ToList();
                    var aux = folioFormaValorada.Select(x => x.Id).ToList();
                    var folioActual = _context.FoliosFormaValoradaActuales.Where(x => aux.Contains((long)x.IdFolioFormaValoradaVerificentro));


                    _context.FoliosFormaValoradaActuales.RemoveRange(folioActual);
                    _context.FoliosFormaValoradaVerificentros.RemoveRange(folioFormaValorada);
                    _context.Testificacions.Remove(registroTestificacion);
                    _context.SaveChanges();
                    transaction.Complete();

                    return new ResponseGeneric<bool>(true);

                }

            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

    }

    public interface ITestificacionNegocio
    {
        public Task<ResponseGeneric<ResponseGrid<TestificacionResponseGrid>>> Consulta(RequestList request);
        public Task<ResponseGeneric<bool>> Guardar(TestificacionApiRequest request);
        // public Task<ResponseGeneric<List<PruebaAutocompletePlacaApi>>> ConsultaAutocomplete(string request);
        public Task<ResponseGeneric<TestificacionResponse>> GetById(long Id);

        public Task<ResponseGeneric<bool>> Eliminar(long id);
    }
}
