using Smadot.Models.DataBase;
using Smadot.Models.Entities.AcreditacionTecnicoVerificador.Request;

namespace Smadot.Models.Entities.AcreditacionTecnicoVerificador.Response
{
    public class AcreditacionTecnicoVerificadorGridResponse : AceditacionTecnicoSolicitud
    {
        public string Acciones { get; set; }
        
        public List<EmpleadoDataResponse> ListIdUserPuestoVerificentros { get; set; }

        public string CatEstatus { get; set; }

        public string? Estatus { get; set; }

        public string UrlAprobacionString => string.IsNullOrEmpty(UrlAprobacion) ? "" : UrlAprobacion.Split("/")[UrlAprobacion.Split("/").Length - 1];

        public string UrlAprobacionBase64 { get; set; }
        //public List<vExento> Exentos { get; set; }

        //public bool VerificacionBool { get; set; }
    }

    public class EmpleadoDataResponse
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
    }

    public class EmpleadoAutocompleteResponse
    {
        public long Id { get; set; }
        public string Text { get; set; }
    }

    public class AcreditacionTecnicoEvidenciaResponse : AcreditacionTecnicoEvidencium
    {

        public List<EvidenciaEmpleados> ListIdUserPuestoVerificentros { get; set; } 

        public long EstatusAcreditacionSolicitud { get; set; }
        //public List<vExento> Exentos { get; set; }

        //public bool VerificacionBool { get; set; }
    }
}
