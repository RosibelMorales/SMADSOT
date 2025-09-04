using Smadot.Models.DataBase;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Exento.Request;
using Smadot.Models.Entities.Exento.Response;
using Smadot.Models.Entities.FoliosVendidosCentrosVerificacion.Response;
using Microsoft.Extensions.Configuration;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using System.Runtime.Intrinsics.Arm;
using Smadot.Models.Dicts;
using System.Transactions;
using Smadot.Models.GenericProcess;
using NPOI.SS.Formula.Functions;

namespace Smadot.SolicitudFormaValorada.Model.Negocio
{
    public class ExentoNegocio : IExentoNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;
        public ExentoNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration, SmadsotGenericInserts smadsotGenericInserts)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _smadsotGenericInserts = smadsotGenericInserts;
        }
        public async Task<ResponseGeneric<List<vExentoResponse>>> Consulta(ExentoRequest request)
        {
            try
            {
                var exentos = _context.vExentos.Where(x => x.IdCatTipoCertificado == TipoCertificado.Exentos).AsQueryable();
                var tot = exentos.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    exentos = exentos.Where(x => x.Placa.ToLower().Contains(request.Busqueda.ToLower())
                                                || x.Serie.ToLower().Contains(request.Busqueda.ToLower())
                                                || x.Marca.ToLower().Contains(request.Busqueda.ToLower())
                                                || x.Submarca.ToLower().Contains(request.Busqueda.ToLower())
                                                || x.Modelo.ToString().Contains(request.Busqueda.ToLower())
                                                || (x.Vigencia != null && x.Vigencia.Value.Date.ToString().Contains(request.Busqueda.ToLower()))
                                                );
                }


                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    exentos = exentos.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    exentos = exentos.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await exentos.Select(x => new vExentoResponse
                {
                    Id = x.Id,
                    Placa = x.Placa,
                    Serie = x.Serie,
                    Marca = x.Marca,
                    Submarca = x.Submarca,
                    Modelo = x.Modelo,
                    Fecha = x.FechaRegistro,
                    IdCatTipoCertificado = x.IdCatTipoCertificado,
                    Nombre = x.TipoCertificado,
                    Vigencia = x.Vigencia.Value,
                    Permanente = x.Combustible.Equals(TipoCombustible.DictTipoCombustible[TipoCombustible.Hibridos]),
                    Total = tot,
                    IdFoliosFormaValoradaVerificentro = x.IdFolioFormaValoradaVerificentro,
                    Folio = x.Folio ?? 0,
                    ClaveTramite = x.ClaveTramite
                }).ToListAsync();

                return new ResponseGeneric<List<vExentoResponse>>(result);

            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<vExentoResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Registro(ExentoResponse request)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var exento = new Exento();
                    if (request.Id > 0)
                    {

                        exento = _context.Exentos.FirstOrDefault(x => x.Id == request.Id);
                        if (exento != null)
                        {
                            exento.Marca = request.Marca;
                            exento.Submarca = request.Submarca;
                            exento.Modelo = request.Modelo;
                            exento.Placa = request.Placa.Trim();
                            exento.Serie = request.Serie.Trim();
                            //exento.ResultadoPrueba = request.ResultadoPrueba ?? string.Empty;
                            //exento.IdCatTipoCertificado = request.IdCatTipoCertificado;
                            exento.IdCatTipoCertificado = TipoCertificado.Exentos;
                            //exento.Vigencia = request.Vigencia;
                            exento.FechaCartaFactura = request.FechaCartaFactura;
                            exento.Propietario = request.Propietario;
                            exento.Combustible = request.Combustible;
                            exento.NumTarjetaCirculacion = request.NumTarjetaCirculacion;
                            exento.UltimoFolio = request.UltimoFolio ?? string.Empty;
                            exento.VigenciaHoloAnterior = request.VigenciaHoloAnterior;
                            exento.NumeroReferencia = request.NumeroReferencia;

                            _context.Update(exento);
                            _context.SaveChanges();

                            var i = 0;
                            foreach (var file in request.Files)
                            {
                                var url = await _blobStorage.UploadFileAsync(new byte[0], "Exentos/" + exento.Id + "/" + file.Nombre, file.Base64);
                                if (!string.IsNullOrEmpty(url))
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            exento.UrlDoc1 = url; break;
                                        case 1:
                                            exento.UrlDoc2 = url; break;
                                        case 2:
                                            exento.UrlDoc3 = url; break;
                                        case 3:
                                            exento.UrlDoc4 = url; break;
                                    }
                                }
                                i++;
                            }
                            _context.SaveChanges();

                            return new ResponseGeneric<bool>(true);

                        }
                        return new ResponseGeneric<bool>(false);
                    }
                    else
                    {
                        var user = _userResolver.GetUser();
                        var exentoVigente = _context.vExentos.Any(x => x.Serie.Equals(request.Serie));
                        if (exentoVigente)
                        {
                            return new ResponseGeneric<bool>(false) { mensaje = "El vehículo ya cuenta con un registro de Exento, haga un refrendo si necesita generar uno nuevo." };

                        }

                        var ciclo = _context.CicloVerificacions.FirstOrDefault(x => x.Activo && x.Id == user.IdCicloVerificacion);

                        // exento.IdFolioFormaValoradaVerificentro = newFolio.Id;
                        exento = new Exento
                        {
                            Marca = request.Marca,
                            Submarca = request.Submarca,
                            Modelo = request.Modelo,
                            Placa = request.Placa.Trim(),
                            Serie = request.Serie.Trim(),
                            //IdCatTipoCertificado = request.IdCatTipoCertificado,
                            IdCatTipoCertificado = TipoCertificado.Exentos,
                            //Vigencia = request.Vigencia,
                            Vigencia = request.FechaCartaFactura.AddYears(8),
                            FechaCartaFactura = request.FechaCartaFactura,
                            Propietario = request.Propietario,
                            Combustible = request.Combustible,
                            NumTarjetaCirculacion = request.NumTarjetaCirculacion,
                            UltimoFolio = request.UltimoFolio ?? string.Empty,
                            VigenciaHoloAnterior = request.VigenciaHoloAnterior,
                            NumeroReferencia = request.NumeroReferencia,
                            FechaRegistro = DateTime.Now,
                            IdUser = user.IdUser
                        };
                        var i = 0;
                        foreach (var file in request.Files)
                        {
                            var url = await _blobStorage.UploadFileAsync(new byte[0], "Exentos/" + exento.Id + "/" + file.Nombre, file.Base64);
                            if (!string.IsNullOrEmpty(url))
                            {
                                switch (i)
                                {
                                    case 0:
                                        exento.UrlDoc1 = url; break;
                                    case 1:
                                        exento.UrlDoc2 = url; break;
                                    case 2:
                                        exento.UrlDoc3 = url; break;
                                    case 3:
                                        exento.UrlDoc4 = url; break;
                                }
                            }
                            i++;
                        }
                        _context.Exentos.Add(exento);
                        var resultp = await _context.SaveChangesAsync() > 0;
                        if (!resultp)
                        {
                            return new ResponseGeneric<bool>(false) { mensaje = "No se pudo registar el exento." };
                        }
                        var verificentro = await _context.vVerificentros.FirstOrDefaultAsync(x => x.Clave.Equals("SMADSOT-00"));

                        var validado = await _smadsotGenericInserts.ValidateFolio(request.IdCatTipoCertificado, verificentro.Id, TipoTramite.CE, user.IdUser, request.EntidadProcedencia, null, null, exento.Id);
                        if (!validado.IsSucces)
                        {
                            return new ResponseGeneric<bool>(false) { mensaje = validado.Description ?? "" };

                        }
                    }


                    var result = await _context.SaveChangesAsync() > 0;

                    scope.Complete();
                    return new ResponseGeneric<bool>(true);
                }
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex) { mensaje = "No se pudo realizar el registro del Exento." };
            }
        }

        public async Task<ResponseGeneric<ExentoResponse>> Detalle(long id)
        {
            try
            {
                var exento = await _context.Exentos.FirstOrDefaultAsync(x => x.Id == id);
                if (exento is null)
                    return new ResponseGeneric<ExentoResponse>();

                var result = new ExentoResponse
                {
                    Id = exento.Id,
                    Marca = exento.Marca,
                    Submarca = exento.Submarca,
                    Modelo = exento.Modelo,
                    Placa = exento.Placa,
                    Serie = exento.Serie,
                    // ResultadoPrueba = exento.ResultadoPrueba,
                    IdCatTipoCertificado = exento.IdCatTipoCertificado,
                    Vigencia = exento.Vigencia,
                    FechaCartaFactura = exento.FechaCartaFactura,
                    Propietario = exento.Propietario,
                    Combustible = exento.Combustible,
                    NumTarjetaCirculacion = exento.NumTarjetaCirculacion,
                    UltimoFolio = exento.UltimoFolio,
                    VigenciaHoloAnterior = exento.VigenciaHoloAnterior,
                    NumeroReferencia = exento.NumeroReferencia,
                    UrlDoc1 = exento.UrlDoc1,
                    UrlDoc2 = exento.UrlDoc2,
                    UrlDoc3 = exento.UrlDoc3,
                    UrlDoc4 = exento.UrlDoc4

                };

                return new ResponseGeneric<ExentoResponse>(result);

            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ExentoResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Eliminar(long id)
        {
            try
            {
                using(var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var exento = await _context.Exentos.FirstOrDefaultAsync(x => x.Id == id);

                    if(exento == null)
                    {
                        return new ResponseGeneric<bool>(false) { mensaje = "Ocurrio un error al encontrar el registro." };
                    }

                    var folioFormaValorada = _context.FoliosFormaValoradaVerificentros.Where(x => x.IdExento == id).ToList();
                    var aux = folioFormaValorada.Select(x => x.Id).ToList();
                    var folioActual = _context.FoliosFormaValoradaActuales.Where(x => aux.Contains((long)x.IdFolioFormaValoradaVerificentro));
                    

                    _context.FoliosFormaValoradaActuales.RemoveRange(folioActual);
                    _context.FoliosFormaValoradaVerificentros.RemoveRange(folioFormaValorada);
                    _context.Exentos.Remove(exento);
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

    public interface IExentoNegocio
    {
        public Task<ResponseGeneric<List<vExentoResponse>>> Consulta(ExentoRequest request);

        public Task<ResponseGeneric<bool>> Registro(ExentoResponse request);

        public Task<ResponseGeneric<ExentoResponse>> Detalle(long id);

        public Task<ResponseGeneric<bool>> Eliminar(long id);
    }
}
