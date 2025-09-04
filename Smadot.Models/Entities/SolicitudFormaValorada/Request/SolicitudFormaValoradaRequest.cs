using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Smadot.Models.Entities.SolicitudFormaValorada.Request
{
    public class SolicitudCertificadoRequest
    {
        [JsonProperty("idCatTipoCertificado")]
        public int IdCatTipoCertificado { get; set; }
        [JsonProperty("cantidad")]
        public int Cantidad { get; set; }
    }
    public class SolicitudFormaValoradaRequest
    {
        [JsonProperty("idSolicitudFV")]
        public long IdSolicitudFV { get; set; }
        [JsonProperty("fechaSolicitudFV")]
        public DateTime FechaSolicitudFV { get; set; }
        [JsonProperty("idAlmacenFV")]
        public long IdAlmacenFV { get; set; }
        [JsonProperty("solicitudesCertificado")]
        public List<SolicitudCertificadoRequest> SolicitudesCertificado { get; set; }
    }
}