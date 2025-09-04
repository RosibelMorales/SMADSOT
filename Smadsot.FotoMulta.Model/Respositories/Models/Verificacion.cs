using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smadsot.FotoMulta.Model.Respositories.Models
{
    public class Verificacion
    {
        public long IdRegistro { get; set; }

        public DateTime Fecha { get; set; }

        public string? Placa { get; set; }

        public string Serie { get; set; } = null!;

        public DateTime? Vigencia { get; set; }

        public string Marca { get; set; } = null!;

        public string Submarca { get; set; } = null!;
    }
}