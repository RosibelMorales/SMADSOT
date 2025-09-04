using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Smadot.Models.Entities.VentaCVV.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Reposicion.Request
{
    public class ReposicionRequestData
    {


        public class ReposicionApiRequest
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("idFv")]
            public long IdFv { get; set; }

            [JsonProperty("idUserRegistro")]
            public long IdUserRegistro { get; set; }

            [JsonProperty("numeroReferencia")]
            public string? NumeroReferencia { get; set; }

            [JsonProperty("documento1")]
            public string? Documento1 { get; set; }

            [JsonProperty("documento2")]
            public string? Documento2 { get; set; }

            [JsonProperty("documento3")]
            public string? Documento3 { get; set; }
            public List<ReposicionFiles>? Files { get; set; }

            [JsonProperty("fechaEmisionRef")]
            public DateTime FechaEmisionRef { get; set; }

            [JsonProperty("fechaPago")]
            public DateTime FechaPago { get; set; }

            [JsonProperty("entidadProcedencia")]
            public string EntidadProcedencia { get; set; }

            [JsonProperty("filesString")]
            public string FilesString { get; set; }

        }
        public class ReposicionFiles
        {
            public string Nombre { get; set; }
            public string Tipo { get; set; }
            public string Base64 { get; set; }
        }
    }
}
