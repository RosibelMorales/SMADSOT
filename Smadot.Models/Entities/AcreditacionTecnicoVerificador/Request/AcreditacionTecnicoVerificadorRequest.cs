using Newtonsoft.Json;
using Smadot.Models.Entities.Refrendo.Request;

namespace Smadot.Models.Entities.AcreditacionTecnicoVerificador.Request
{
    public class AcreditacionTecnicoSolicitudRequest
    {
        public long Id { get; set; }

        public string NumeroSolicitud { get; set; } = null!;

        //public int IdCatEstatusAcreditacion { get; set; }

        //public string? UrlAprobacion { get; set; }

        //public List<RefrendoFiles>? Files { get; set; }

        public List<long>? Empleados { get; set; }
        public string EmpleadosString { get; set; } 
    }

    public class AcreditacionTecnicoEvidenciaRequest
    {
        public long Id { get; set; }

        public string NumeroSolicitud { get; set; }

        public string TipoTramite { get; set; }

        public string NumeroAcreditacion { get; set; }

        public string? NumeroReferencia { get; set; }

        public DateTime FechaAcreditacion { get; set; }

        public DateTime? FechaAmpliacion { get; set; }

        public DateTime FechaEmision { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int IdCatEstatusAcreditacion { get; set; }

        public List<AcreditacionTecnicoFile> UrlAcreditacion { get; set; }

        public string UrlAcreditacionString { get; set; }

        public List<EvidenciaEmpleados> EvidenciaEmpleados { get; set; }

        public string? DocumentoUrl { get; set; }
    }

    public class AutorizarAcreditacionRequest
    {
        public long Id { get; set; }

        public List<AcreditacionTecnicoFile> UrlAprobacion { get; set; }

        public string UrlAprobacionString { get; set; }

        public bool Estatus { get; set; }
    }

    public class AcreditacionTecnicoFile
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }

    public class EvidenciaEmpleados
    {
        public long IdUserPuesto { get; set; }

        public string Nombre { get; set; }

        public bool Eliminado { get; set; }

        public int? TipoAcreditacion { get; set; }

        public int? NormaAcreditacion { get; set; }
    }

    public class EmpleadoAutocompletRequest
    {
        [JsonProperty("term")]
        public string Term { get; set; }

        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("end")]
        public int End { get; set; }

        [JsonProperty("emp")]
        public string Emp { get; set; }

    }
    public class EmpleadoTermRequest
    {
        [JsonProperty("id")]
        public string Id { get; set; }

    }
}
