using Newtonsoft.Json;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.IngresoFormaValorada.Response;
using Smadot.Models.Entities.Refrendo.Request;
using static Smadot.Models.Entities.Generic.Response.TablaFijaViewModel;

namespace Smadot.Models.Entities.VentaCVV.Response
{
    public class VentaCVVListResponse
    {
        public long IdVenta { get; set; }
        public long IdAlmacen { get; set; }
        public string NumeroVenta { get; set; }
        public DateTime Fecha { get; set; }
        public string Verificentro { get; set; }
        public string UserRegistro { get; set; }
        public int? CantidadHologramas { get; set; }
        public int Total { get; set; }


        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public string? FolioCertificado { get; set; }


    }
    public class VentaCVVGridResponse : VentaCVVListResponse
    {
        public string Acciones { get; set; }
    }
    public class InventarioListResponse
    {
        public long Id { get; set; }

        public long IdAlmacen { get; set; }

        public int CantidadStock { get; set; }

        public int IdCatTipoCertificado { get; set; }

        public int? FolioInicial { get; set; }

        public int? FolioFinal { get; set; }

        public string? NombreCertificado { get; set; }

        public decimal? ImporteTotal { get; set; }

    }

    public class InventarioVentaCVVResponse
    {
        public decimal? ImporteFV { get; set; }
        public DateTime? FechaReporteRevalidado { get; set; }
        public DateTime? FechaReporteValidado { get; set; }
        public int DobleCeroCantidadStock { get; set; }
        public int DobleCeroFolioInicial { get; set; }
        public int DobleCeroFolioFinal { get; set; }
        public int CeroCantidadStock { get; set; }
        public int CeroFolioInicial { get; set; }
        public int CeroFolioFinal { get; set; }
        public int UnoCantidadStock { get; set; }
        public int UnoFolioInicial { get; set; }
        public int UnoFolioFinal { get; set; }
        public int DosCantidadStock { get; set; }
        public int DosFolioInicial { get; set; }
        public int DosFolioFinal { get; set; }
        public int ExentosCantidadStock { get; set; }
        public int ExentosFolioInicial { get; set; }
        public int ExentosFolioFinal { get; set; }
        public int ConstanciasNoAprobadosCantidadStock { get; set; }
        public int ConstanciasNoAprobadosFolioInicial { get; set; }
        public List<CertificadoDataTotal> certificadosTotalData { get; set; }

        public List<CertificadoGrupo> certificados { get; set; }
        public int ConstanciasNoAprobadosFolioFinal { get; set; }
    }

    public class CertificadosTotalData
    {
        public int CantidadSolicitada { get; set; }
        public int CantidadRecibida { get; set; }
        public int IdCatTipoCertificado { get; set; }
        public string Clave
        {
            get
            {
                return TipoCertificado.DictNombreClave.FirstOrDefault(x => x.Key == IdCatTipoCertificado).Value ?? "";
            }
        }
        public string NombreCertificado
        {
            get
            {
                return TipoCertificado.DictNombreCertificado.FirstOrDefault(x => x.Key == IdCatTipoCertificado).Value ?? "";
            }
        }
    }

    public class CertificadoGrupo
    {
        public int IdCatTipoCertificado { get; set; }
        public string NombreCertificado
        {
            get
            {
                return TipoCertificado.DictNombreCertificado.FirstOrDefault(x => x.Key == IdCatTipoCertificado).Value ?? "";
            }
        }
        public string Clave
        {
            get
            {
                return TipoCertificado.DictNombreClave.FirstOrDefault(x => x.Key == IdCatTipoCertificado).Value ?? "";
            }
        }
        public List<CertificadoData> Certificados { get; set; }
    }

    public class CertificadoData
    {
        public long IdIngresoCertificado { get; set; }
        public int IdCatTipoCertificado { get; set; }
        public int FolioInicial { get; set; }
        public int FolioFinal { get; set; }
        public int CantidadRecibida { get; set; }
        public string Clave
        {
            get
            {
                return TipoCertificado.DictNombreClave.FirstOrDefault(x => x.Key == IdCatTipoCertificado).Value ?? "";
            }
        }
        public string NombreCertificado
        {
            get
            {
                return TipoCertificado.DictNombreCertificado.FirstOrDefault(x => x.Key == IdCatTipoCertificado).Value ?? "";
            }
        }
    }
    public class VentaCVVRequest : InventarioVentaCVVResponse
    {
        public long IdVenta { get; set; }
        public long IdAlmacen { get; set; }
        public long IdVerificentro { get; set; }
        public DateTime? FechaVenta { get; set; }
        public string NumeroReferencia { get; set; }
        public string ReferenciaBancaria { get; set; }
        public string NumeroCompra { get; set; }
        public decimal? DobleCeroImporteTotal { get; set; }
        public decimal? CeroImporteTotal { get; set; }
        public decimal? UnoImporteTotal { get; set; }
        public decimal? DosImporteTotal { get; set; }
        public decimal? ExentosImporteTotal { get; set; }
        public decimal? ConstanciasNoAprobadosImporteTotal { get; set; }
        public string? FilesString { get; set; }
        public List<VentaCVVFiles> Files { get; set; }

    }
    public class VentaApiRequest
    {
        public VentaApiRequest()
        {
            VentaCertificados = new List<VentaCertificados>();
            Files = new List<VentaCVVFiles>();
        }

        public long IdVenta { get; set; }
        public long IdAlmacen { get; set; }
        public long IdVerificentro { get; set; }
        public DateTime? FechaVenta { get; set; }
        public string NumeroReferencia { get; set; }
        public string ReferenciaBancaria { get; set; }
        public string NumeroCompra { get; set; }
        public List<VentaCertificados> VentaCertificados { get; set; }
        public List<VentaCVVFiles> Files { get; set; }

    }
    public class VentaCertificadoResponse : InventarioVentaCVVResponse
    {
        public string NombreAlmacen { get; set; }
        public string NombreVerificentro { get; set; }
        public string? NumeroReferencia { get; set; }
        public string? ReferenciaBancaria { get; set; }
        public string? NumeroCompra { get; set; }
        public DateTime FechaVenta { get; set; }
        public string? Clave { get; set; }
        public string? Rfc { get; set; }
        public Decimal? SumaImporteTotal { get; set; }
        public int? Cantidad { get; set; }
        public string? UrlDoc1 { get; set; }
        public string? UrlDoc2 { get; set; }
        public string? UrlDoc3 { get; set; }

    }
    public class CertificadoDataTotal
    {
        public string NombreHolograma { get; set; }
        public string FolioInicial { get; set; }
        public string FolioFinal { get; set; }
        public int Cantidad { get; set; }
        public decimal ImporteTotal { get; set; }
    }
    public class VentaCVVFiles
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }

    public class VentaCVVAutocompletRequest
    {
        [JsonProperty("term")]
        public string Term { get; set; }

        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("end")]
        public int End { get; set; }

        [JsonProperty("idAlmacen")]
        public int IdAlmacen { get; set; }
    }
}
