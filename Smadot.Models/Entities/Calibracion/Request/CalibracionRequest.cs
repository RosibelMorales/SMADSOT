using Newtonsoft.Json;
using Smadot.Models.Entities.Calibracion.Response;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.Calibracion.Request
{
    public class CalibracionListRequest : RequestList
    {
        [JsonProperty("idVerificentro")]
        public long? IdVerificentro { get; set; }
    }
    public class CalibracionRequest
    {
        public long Id { get; set; }
        public long IdRealiza { get; set; }
        public DateTime FechaHora { get; set; }
        public long IdLinea { get; set; }
        public long IdEquipo { get; set; }
        public long? IdTecnico { get; set; }
        public string? NombreTecnico { get; set; }
        public int? NumeroFolio { get; set; }
        public string? Nota { get; set; }
        public DateTime? FechaEmisionCertificado { get; set; }
        public List<CalibracionFile>? Files { get; set; }
    }
}
