using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.ProveedorFolioServicio.Request
{
    public class ProveedorFolioServicioRequestData
    {
        public class ProveedorFolioServicioRequest
        {
            public long FolioInicial { get; set; }
            public long FolioFinal { get; set; }
            public long IdProveedor { get; set; }
        }

        public class ProveedorFolioServicioAutocompletRequest
        {
            [JsonProperty("term")]
            public string Term { get; set; }

            [JsonProperty("start")]
            public int Start { get; set; }

            [JsonProperty("end")]
            public int End { get; set; }

            [JsonProperty("esLaboratorio")]
            public bool EsLaboratorio { get; set; }
        }
    }
}
