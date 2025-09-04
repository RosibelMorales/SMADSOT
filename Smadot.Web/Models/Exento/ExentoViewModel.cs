using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Exento.Response;
using Smadot.Models.DataBase;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Smadot.Models.Entities.VentaCVV.Response;

namespace Smadot.Web.Models.Exento
{
    public class ExentoViewModel
    {
        public long Id { get; set; }
        public string? Marca { get; set; }
        public string? Submarca { get; set; }
        public int Modelo { get; set; }
        public string? Placa { get; set; }
        public string? Serie { get; set; }
        public string? ResultadoPrueba { get; set; }

        public int IdCatTipoCertificado { get; set; }

        public DateTime Vigencia { get; set; }

        public DateTime FechaCartaFactura { get; set; }

        public string? Propietario { get; set; }

        public string? Combustible { get; set; }

        public string? NumTarjetaCirculacion { get; set; }

        public string? UltimoFolio { get; set; }

        public DateTime? VigenciaHoloAnterior { get; set; }

        public string? NumeroReferencia { get; set; }

        public string? UrlDoc1 { get; set; }

        public string? UrlDoc2 { get; set; }

        public string? UrlDoc3 { get; set; }
        public List<SelectListItem>? Certificados { get; set; }
        public int Total { get; set; }
        public string? FilesString { get; set; }
        public List<ExentosFiles> Files { get; set; }

        public DateTime FechaEmisionRef { get; set; }
        public DateTime FechaPago { get; set; }
        public string EntidadProcedencia { get; set; }
        public string? ServidorPublico { get; set; }
    }
    public class ExentoResponseGrid : ExentoViewModel
    {
        public string Acciones { get; set; }
    }

    public class ExentoViewModelPrueba
    {
        public long Id { get; set; }
        public string? Marca { get; set; }
        public string? Submarca { get; set; }
        public int Modelo { get; set; }
        public string? Placa { get; set; }
        public string? Serie { get; set; }
        public string? ResultadoPrueba { get; set; }

        public int IdCatTipoCertificado { get; set; }

        public DateTime Vigencia { get; set; }

        public DateTime FechaCartaFactura { get; set; }

        public string? Propietario { get; set; }

        public string? Combustible { get; set; }

        public string? NumTarjetaCirculacion { get; set; }

        public string? UltimoFolio { get; set; }

        public DateTime VigenciaHoloAnterior { get; set; }

        public string? NumeroReferencia { get; set; }

        public string? UrlDoc1 { get; set; }

        public string? UrlDoc2 { get; set; }

        public string? UrlDoc3 { get; set; }
    }

    public class ExentosFiles
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }
}
