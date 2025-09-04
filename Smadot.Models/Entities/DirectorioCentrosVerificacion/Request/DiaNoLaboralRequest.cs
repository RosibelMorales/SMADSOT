using Newtonsoft.Json;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.DirectorioCentrosVerificacion.Request
{
    public class DiaNoLaboralRequest
    {
        public long Id { get; set; }

        public DateTime Fecha { get; set; }

        public long IdUserRegistro { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long IdCVV { get; set; }

    }

    public class DeleteDiaNoLaboralRequest
    {
        public long Id { get; set; }

    }

    public class DiaNoLaboralGridRequest : RequestList
    {
        [JsonProperty("idCvv")]
        public long IdCvv { get; set; }
    }
}
