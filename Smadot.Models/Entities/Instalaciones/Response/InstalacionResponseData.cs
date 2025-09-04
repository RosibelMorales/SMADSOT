using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Instalaciones.Response
{
    public class InstalacionResponseData
    {
        public class InstalacionResponse
        {
            public long? IdInstalacion { get; set; }
            public string? UserRegistro { get; set; }
            public string? Proveedor { get; set; }
            public string? UrlDocumento { get; set; }
            public string? FechaRegistro { get; set; }
            public int Total { get; set; }
        }
        public class InstalacionResponseGrid : InstalacionResponse
        {
            public string Acciones { get; set; }
        }
    }
}
