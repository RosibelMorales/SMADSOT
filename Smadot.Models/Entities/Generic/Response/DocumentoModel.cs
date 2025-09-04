using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Smadot.Models.Entities.Generic.Response
{
    public class DocumentoModel
    {
        public string UrlDocumento { get; set; }
        public string Nombre { get; set; }
        public string Linea { get; set; }

    }
}
