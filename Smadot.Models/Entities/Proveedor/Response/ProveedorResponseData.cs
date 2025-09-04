using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Smadot.Models.Entities.Reposicion.Response.ReposicionResponseData;

namespace Smadot.Models.Entities.Proveedor.Response
{
    public class ProveedorResponseData
    {
        public class ProveedorResponse
        {
            public long? Id { get; set; }

            public string? Nombre { get; set; }

            public string? CorreoElectronico { get; set; }

            public string? Telefono { get; set; }


            public string? Direccion { get; set; }

            public string? Empresa { get; set; }

            public bool? Autorizado { get; set; }

            public int Total { get; set; }
        }

        public class ProveedorResponseGrid : ProveedorResponse
        {
            public string Acciones { get; set; }
        }
    }
}
