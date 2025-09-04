using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.DirectorioCentrosVerificacion.Response
{
    public class DiaNoLaboralResponse
    {
        public long Id { get; set; }

        public DateTime Fecha { get; set; }

        public string FechaString { get; set; }

        public long IdUserRegistro { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long IdCVV { get; set; }

        public int? Total { get; set; }

    }

    public class DiaNoLaboralResponseGrid : DiaNoLaboralResponse
    {
        public string Acciones { get; set; }
    }
}
