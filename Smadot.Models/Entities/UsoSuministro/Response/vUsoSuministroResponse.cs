using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.UsoSuministro.Response
{
    public class vUsoSuministroResponse
    {
        public long Id { get; set; }

        public long IdVerificentro { get; set; }

        public string NombreVerificentro { get; set; }

        public string NombreUsoSuministro { get; set; }

        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; }

        public string Nota { get; set; } 

        public string UrlFactura { get; set; } 

        public string Proveedor { get; set; } 

        public DateTime FechaRegistro { get; set; }

        public long IdUserRegistro { get; set; }

        public string? NombreUser { get; set; }

        public int Total { get; set; }
    }

    public class vUsoSuministroResponseGrid : vUsoSuministroResponse
    {
        public string Acciones { get; set; }
    }
}
