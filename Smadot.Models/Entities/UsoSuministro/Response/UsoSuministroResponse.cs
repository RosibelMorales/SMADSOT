using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.UsoSuministro.Response
{
    public class UsoSuministroResponse
    {
        public long Id { get; set; }

        public long IdVerificentro { get; set; }

        public string Nombre { get; set; } 

        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; }

        public string Nota { get; set; } 

        public string UrlFactura { get; set; } 

        public string Proveedor { get; set; } 

        public DateTime FechaRegistro { get; set; }

        public long IdUserRegistro { get; set; }

        public List<UsoSuministroFile>? Files { get; set; }

        public string? FilesString { get; set; }
    }

    public class UsoSuministroFile
    {
        public string? Base64 { get; set; }

        public string? Nombre { get; set; }

        public string? Tipo { get; set; }
    }
}
