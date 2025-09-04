using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos.Documentos
{
    public class ComprobanteCitaResponse
    {
        public string NombrePersona { get; set; }

        public DateTime Fecha { get; set; }

        public string NombreCentroVerificacion { get; set; }

        public string DireccionCentroVerificacion { get; set; }

        public string Folio { get; set; }

        public string UrlWeb { get; set; }
    }
}
