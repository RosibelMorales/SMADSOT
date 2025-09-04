using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Verificacion.Response
{
    public class ConsultaVerificacionResponse<T>
    {
        public int IdEstatusPrueba { get; set; }
        public T Resultados { get; set; }    
    }
}
