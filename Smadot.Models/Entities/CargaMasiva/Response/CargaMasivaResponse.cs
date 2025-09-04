using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.CargaMasiva.Response
{
    public class CargaMasivaResponse
    {
        public long Id { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string? NombreArchivo { get; set; }

        public int? NumeroRegistros { get; set; }

        public long IdUserRegistro { get; set; }
    }
}
