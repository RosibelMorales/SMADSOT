using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Proveedor.Request
{
    public class ProveedorRequestData
    {
        public class ProveedorRequest
        {
            public string? Nombre { get; set; }

            public string? CorreoElectronico { get; set; }

            public string? Telefono { get; set; }


            public string? Direccion { get; set; }

            public string? Empresa { get; set; }

            public bool? Autorizado { get; set; }
            public bool? EsLaboratorio { get; set; }
        }

        public class EstatusProveedor
        {
            public long IdEstatus { get; set; }
        }

    }
}
