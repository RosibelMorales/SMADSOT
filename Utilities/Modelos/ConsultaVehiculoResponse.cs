using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos
{
    public class ConsultaVehiculoResponse
    {
        public bool BResultado { get; set; }
        public int IEstatusProceso { get; set; }
        public string VchMensajeProceso { get; set; }
        public string VchMensajeCiudadano { get; set; }
        public string VchPlaca { get; set; }
        public string VchSerie { get; set; }
        public string VchMarca { get; set; }
        public string VchLinea { get; set; }
        public int SmAnioModelo { get; set; }
        public int TiEstatusAdeudo { get; set; }
        public string VchMensajeAdeudo { get; set; }
        public int TiEstatusFotomulta { get; set; }
        public string VchMensajeFotomulta { get; set; }

    }
}
