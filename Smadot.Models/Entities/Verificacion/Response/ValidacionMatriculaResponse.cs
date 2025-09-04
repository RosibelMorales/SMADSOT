using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Verificacion.Response
{
    public class ValidacionMatriculaResponse
    {
        public int IdTipoPrueba { get; set; }
        public int IdTipoCombustible { get; set; }
        public ConsultaVehiculoResponse Vehiculo { get; set; }
        public long IdVerificacion { get; set; }
    }
}
