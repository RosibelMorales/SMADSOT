using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Administrativa.Request
{
    public class AdministrativaRequestData
    {
        public class AdministrativaApiRequest
        {
            [JsonProperty("idCatMotivoTramite")]
            public int IdCatMotivoTramite { get; set; }

            [JsonProperty("placa")]
            public string? Placa { get; set; }

            [JsonProperty("serie")]
            public string? Serie { get; set; }

            [JsonProperty("numeroReferencia")]
            public string? NumeroReferencia { get; set; }

            [JsonProperty("documento1")]
            public string? Documento1 { get; set; }

            [JsonProperty("documento2")]
            public string? Documento2 { get; set; }

            [JsonProperty("documento3")]
            public string? Documento3 { get; set; }

            [JsonProperty("folioAsignado")]
            public string? FolioAsignado { get; set; }

            [JsonProperty("idUserRegistro")]
            public long? IdUserRegistro { get; set; }

            [JsonProperty("fechaEmisionReferencia")]
            public DateTime FechaEmisionReferencia { get; set; }

            [JsonProperty("fechaPago")]
            public DateTime FechaPago { get; set; }

            [JsonProperty("idCatTipoCertificado")]
            public int IdCatTipoCertificado { get; set; }
            [JsonProperty("nombrePropietario")]
            public string? NombrePropietario { get; set; }
            [JsonProperty("marca")]
            public string? Marca { get; set; }
            [JsonProperty("submarca")]
            public string? Submarca { get; set; }
            [JsonProperty("modelo")]
            public int? Modelo { get; set; }
            [JsonProperty("tarjetaCirculacion")]
            public string? TarjetaCirculacion { get; set; }
            [JsonProperty("entidadProcedencia")]
            public string EntidadProcedencia { get; set; }
            [JsonProperty("idTipoCombustible")]
            public int IdTipoCombustible { get; set; }
            public string? FilesString { get; set; }
            public List<AdministrativaFiles> Files { get; set; }
        }

        public class AdministrativaRequest
        {
            [JsonProperty("idCatMotivoTramite")]
            public int IdCatMotivoTramite { get; set; }

            [JsonProperty("placa")]
            public string? Placa { get; set; }

            [JsonProperty("serie")]
            public string? Serie { get; set; }

            [JsonProperty("numeroReferencia")]
            public string? NumeroReferencia { get; set; }

            [JsonProperty("documento1")]
            public IFormFile? Documento1 { get; set; }

            [JsonProperty("documento2")]
            public IFormFile? Documento2 { get; set; }

            [JsonProperty("documento3")]
            public IFormFile? Documento3 { get; set; }

            [JsonProperty("folioAsignado")]
            public string? FolioAsignado { get; set; }

            [JsonProperty("idUserRegistro")]
            public long? IdUserRegistro { get; set; }
            public string? FilesString { get; set; }

        }
        public class AdministrativaFiles
        {
            public string Nombre { get; set; }
            public string Tipo { get; set; }
            public string Base64 { get; set; }
        }
    }
}
