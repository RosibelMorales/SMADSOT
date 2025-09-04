using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Reportes.Request;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.Linq.Dynamic.Core;
using static Smadot.Models.Entities.Reportes.Response.ReporteTercerTantoResponseData;

namespace Smadot.DirectorioCentrosVerificacion.Model.Negocio
{
    public class ReporteTercerTantoNegocio : IReporteTercerTantoNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        public ReporteTercerTantoNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public async Task<ResponseGeneric<List<ReporteTercerTantoResponse>>> Consulta(RequestList request)
        {
            try
            {
                var catalogo = _context.ReporteEntregaTercers.AsQueryable();
                catalogo = catalogo.Where(o => o.IdVerificentro == _userResolver.GetUser().IdVerificentro);

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                var tot = catalogo.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x => x.Numero.ToString().Contains(request.Busqueda.ToLower()));
                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new ReporteTercerTantoResponse
                {
                    Id = x.Id,
                    NumeroEntrega = x.Numero.ToString(),
                    FechaInicial = x.FechaInicial.ToString("dd/MM/yyyy"),
                    FechaFinal = x.FechaFinal.ToString("dd/MM/yyyy"),
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<ReporteTercerTantoResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<ReporteTercerTantoResponse>>(ex);
            }
        }
        public async Task<ResponseGeneric<bool>> GuardarReporteCertificado(ReporteCertificadoRequest request)
        {
            var result = new ResponseGeneric<bool>();

            try
            {
                var reporte = _context.ReporteEntregaTercers.Where(x => x.IdVerificentro == _userResolver.GetUser().IdVerificentro).AsQueryable();
                if (reporte.Any(o => o.Numero == request.NumeroCuenta))
                {
                    result.Status = ResponseStatus.Failed;
                    result.mensaje = "El Numero de Entrega ya se encuentra registrado.";
                    return result;
                }
                if (reporte.Any(o => request.FechaInicial >= o.FechaInicial && request.FechaInicial <= o.FechaFinal))
                {
                    result.Status = ResponseStatus.Failed;
                    result.mensaje = "La Fecha Inicial no se debe encontrar dentro del rango ya registrado.";
                    return result;
                }
                if (reporte.Any(o => request.FechaFinal >= o.FechaInicial && request.FechaFinal <= o.FechaFinal))
                {
                    result.Status = ResponseStatus.Failed;
                    result.mensaje = "La Fecha Final no se debe encontrar dentro del rango ya registrado.";
                    return result;
                }
                if (reporte.Any(o => request.FechaInicial <= o.FechaInicial && request.FechaFinal >= o.FechaFinal))
                {
                    result.Status = ResponseStatus.Failed;
                    result.mensaje = "El rango de Fechas no se debe encontrar dentro del rango ya registrado.";
                    return result;
                }

                var reporteEntregaTercer = new ReporteEntregaTercer();
                reporteEntregaTercer.Numero = request.NumeroCuenta;
                reporteEntregaTercer.FechaInicial = request.FechaInicial;
                reporteEntregaTercer.FechaFinal = request.FechaFinal;
                reporteEntregaTercer.UrlEntrega = "Initial";
                reporteEntregaTercer.FechaRegistro = DateTime.Now;
                reporteEntregaTercer.IdUserRegistro = request.IdUserRegistro;
                reporteEntregaTercer.IdVerificentro = request.IdVerificentro.Value;

                _context.ReporteEntregaTercers.Add(reporteEntregaTercer);
                var resultSave = await _context.SaveChangesAsync() > 0;

                var url = await _blobStorage.UploadFileAsync(new byte[0], "ReporteTercerTanto/" + reporteEntregaTercer.Id + "/" + request.File.FirstOrDefault().Nombre, request.File.FirstOrDefault().Base64);
                reporteEntregaTercer.UrlEntrega = url;
                resultSave = await _context.SaveChangesAsync() > 0;

                foreach (var values in request.ReporteCertificados)
                {
                    var entregaTercerCertificado = new ReporteEntregaTercerCertificado()
                    {
                        IdReporte = reporteEntregaTercer.Id,
                        IdCatTipoCertificado = values.IdCatTipoCertificado,
                        Cantidad = values.Cantidad,
                        SerieInicial = values.SerieInicial,
                        SerieFinal = values.SerieFinal,
                    };
                    _context.ReporteEntregaTercerCertificados.Add(entregaTercerCertificado);
                    await _context.SaveChangesAsync();
                }

                result.Response = true;
                return result;
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<ReporteTercerTantoResponse>> GetById(long Id)
        {
            try
            {
                var catalogo = _context.vReporteEntregaTercers.AsQueryable();
                catalogo = catalogo.Where(x => x.IdReporte == Id);
                var result = await catalogo.Select(x => new ReporteTercerTantoResponse
                {
                    Id = x.IdReporte,
                    NumeroEntrega = x.NumeroReporte.ToString(),
                    FechaInicial = x.FechaInicial.ToString("dd/MM/yyyy"),
                    FechaFinal = x.FechaFinal.ToString("dd/MM/yyyy"),
                    UrlDocumento = x.UrlEntrega,
                    FechaRegistro = x.FechaRegistro.ToString("dd/MM/yyyy"),
                    UsuarioRegistro = x.UserRegistro,

                }).FirstOrDefaultAsync();

                var certificados = await catalogo.Select(x => new ReporteTercerTantoCertificados
                {
                    ClaveCertificado = x.ClaveCertificado,
                    NombreCatTipoReporte = x.NombreCatTipoReporte,
                    Cantidad = x.Cantidad.Value,
                    SerieInicial = x.SerieInicial.Value,
                    SerieFinal = x.SerieFinal.Value,
                }).ToListAsync();
                result.reporteTercerTantoCertificados = certificados;

                return new ResponseGeneric<ReporteTercerTantoResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ReporteTercerTantoResponse>(ex);
            }
        }
    }

    public interface IReporteTercerTantoNegocio
    {
        public Task<ResponseGeneric<List<ReporteTercerTantoResponse>>> Consulta(RequestList request);
        public Task<ResponseGeneric<bool>> GuardarReporteCertificado(ReporteCertificadoRequest request);
        public Task<ResponseGeneric<ReporteTercerTantoResponse>> GetById(long Id);
    }
}
