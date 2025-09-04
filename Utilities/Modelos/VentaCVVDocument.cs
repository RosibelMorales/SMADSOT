using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos
{
    public class VentaCVVDocument
    {
        public int? Id { get; set; }

        public string? NumeroVenta { get; set; }

        public string? Verificentro { get; set; }

        public string? UserRegistro { get; set; }

        public int? CantidadHologramas { get; set; }

        public DateTime FechaVenta { get; set; }



        public string? UrlRoot { get; set; }

        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public string? FolioCertificado { get; set; }
    }
}
