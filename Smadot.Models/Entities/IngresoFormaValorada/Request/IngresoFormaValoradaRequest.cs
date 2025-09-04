using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.IngresoFormaValorada.Response;

namespace Smadot.Models.Entities.IngresoFormaValorada.Request
{
    public class IngresoFormaValoradaRequest
    {
        [JsonProperty("idSolicitudFV")]
        public long IdSolicitudFV { get; set; }
        [JsonProperty("idIngresoFV")]
        public long IdIngresoFV { get; set; }
        [JsonProperty("fechaSolicitud")]
        public DateTime FechaSolicitud { get; set; }
        [JsonProperty("fechaEntrega")]
        public DateTime FechaEntrega { get; set; }
        [JsonProperty("idAlmacen")]
        public long IdAlmacen { get; set; }
        [JsonProperty("nombreRecibio")]
        public string NombreRecibio { get; set; }
        [JsonProperty("documento1")]
        public IngresoFormaValoradaFiles? Documento1 { get; set; }
        [JsonProperty("documento2")]
        public IngresoFormaValoradaFiles? Documento2 { get; set; }
        [JsonProperty("documento3")]
        public IngresoFormaValoradaFiles? Documento3 { get; set; }
        [JsonProperty("certificados")]
        public List<CertificadoData> certificados { get; set; }

    }
}