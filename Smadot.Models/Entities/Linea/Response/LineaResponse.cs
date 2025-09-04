using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Linea.Response
{
    public class LineaResponse
    {
        public long Id { get; set; }

        public long IdVerificentro { get; set; }

        public string Nombre { get; set; } = null!;

        public int IdCatEstatusLinea { get; set; }

        public long IdUserRegistro { get; set; }

        public DateTime FechaRegistro { get; set; }

        public int Total { get; set; }
    }
}
