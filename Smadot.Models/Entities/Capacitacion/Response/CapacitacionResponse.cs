using Smadot.Models.DataBase;
using Smadot.Models.Entities.ReporteMensual.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Capacitacion.Response
{
    public class CapacitacionResponse
    {
        public int? Id { get; set; }

        public int? IdEmpleado { get; set; }

        public string? Nombre {get; set;}

        public int? IdCapacitacion { get; set; }

        public int? IdVerificentro { get; set; }

        public string? NumeroTrabajador { get; set; }

		public DateTime? FechaCapacitacion { get; set; }

        public string? FechaCapacitacionFormat { get; set; }

        public string? TemaCapacitacion { get; set; }

        public long? IdCatEstatusCapacitacion { get; set; }

        public string? NombreCatEstatusCapacitacion { get; set; }

        public bool? ActivoCatEstatusCapacitacion { get; set; }
        
        public long? IdUserPuestoVerificentro { get; set; }

        public long? IdPuestoVerificentro { get; set; }

        public bool? Asistio { get; set; }

        public string? UrlFotografia { get; set; }

        public string? UrlEvaluacion { get; set; }

        public List<CapacitacionFiles>? Files { get; set; }

        public int? Total { get; set; }

		public int? TotalAsistentes { get; set; }

		public int? IdCapEmp { get; set; }

        public int? AcceptOrDenied { get; set; }

        public string? NombreVerificentro { get; set; }

        public string? UrlFotografiaBase64 { get; set; }

        public string? UrlEvaluacionBase64 { get; set; }
    }

    public class CapacitacionResponseGrid : CapacitacionResponse
    {
        public string Acciones { get; set; }
    }

    public class CapacitacionFiles
    {
        public string? Nombre { get; set; }
        public string? Tipo { get; set; }
        public string? Base64 { get; set; }
    }

}
