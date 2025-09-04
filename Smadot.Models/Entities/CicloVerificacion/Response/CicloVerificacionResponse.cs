using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.CicloVerificacion.Response
{
    public class CicloVerificacionResponse
    {
        public long Id { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }

        public decimal? ImporteFv { get; set; }
    }
}
