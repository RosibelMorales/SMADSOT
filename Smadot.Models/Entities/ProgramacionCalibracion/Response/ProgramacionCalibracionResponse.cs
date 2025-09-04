using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.ProgramacionCalibracion.Response
{
    public class ProgramacionCalibracionResponse
    {
        public long Id { get; set; }

        public long IdEquipo { get; set; }

        public int IdCatTipoCalibracion { get; set; }

        public string? UrlDocumento1 { get; set; }

        public DateTime PrimeraFechaCalibracion { get; set; }

        public string? Nota { get; set; }

        public long IdUserRegistro { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long? IdUserValido { get; set; }
    }

    public class AuxiliarEstatusResponse
    {
        public long Id { get; set; }

        public string? Nota { get; set; }

        public long? IdUserValido { get; set; }
    }
}
