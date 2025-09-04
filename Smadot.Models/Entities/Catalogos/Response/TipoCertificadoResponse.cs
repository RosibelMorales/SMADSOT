using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Catalogos.Response
{
    public class TipoCertificadoResponse
    {
        public int Id { get; set; }

        public string? Nombre { get; set; } 

        public string? Descripcion { get; set; }

        public string? ClaveCertificado { get; set; }

        public bool Activo { get; set; }
        public int Total { get; set; }
    }
}
