using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.InfraccionesPendientesPago.Response
{
    public class InfraccionesPendientesResponse
    {

        
            public int codigo { get; set; }
            public string desc { get; set; }
            public Veh[] vehs { get; set; }
        

        public class Veh
        {
            public string placa { get; set; }
            public string serie { get; set; }
            public int cuentaInfr { get; set; }
        }

    }
}
