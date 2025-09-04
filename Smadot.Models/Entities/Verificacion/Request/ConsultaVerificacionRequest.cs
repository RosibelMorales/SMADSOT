using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Verificacion.Request
{
    public class ConsultaVerificacionRequest
    {
        public long IdTipoPrueba { get; set; }
        public long IdVerificacion { get; set; }
        public long IdUserTecnico { get; set; }
        public long IdVerificentro { get; set; }
        public int ClaveRechazo { get; set; }
    
    }
}
