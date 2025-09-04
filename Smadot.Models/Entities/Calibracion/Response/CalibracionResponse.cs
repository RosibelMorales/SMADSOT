using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.EquipoTipoCalibracion.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Smadot.Models.Entities.Calibracion.Response
{
    public class CalibracionResponse
    {
        public long Id { get; set; }
        public long IdLinea { get; set; }
        public long IdEquipo { get; set; }

        public long IdEquipoTipoCalibracion { get; set; }

        public int Consecutivo { get; set; }

        public DateTime FechaCalibracion { get; set; }

        public DateTime FechaProgramada { get; set; }

        public DateTime FechaProximaCalibracion { get; set; }

        public long IdUserRegistro { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long? IdUserValido { get; set; }

        public string? Nota { get; set; }

        public int IdCatEstatusCalibracion { get; set; }

        public long? IdTecnicoLaboratorio { get; set; }

        public int? NumeroCertificado { get; set; }

        public DateTime? FechaEmisionCertificad { get; set; }

        public string? UrlResultado { get; set; }

        public string? Laboratorio { get; set; }

        public List<CalibracionFile> Files { get; set; }
    }

    public class CalibracionFile
    {
        public string? Base64 { get; set; }

        public string? Nombre { get; set; }

        public string? Tipo { get; set; }
    }
}
