using Newtonsoft.Json;
using Smadot.Models.Entities.Equipo.Response;
using Smadot.Models.Entities.IngresoFormaValorada.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.VentaCVV.Request
{
    public class VentaFVRequest
    {
        [JsonProperty("idVentaFV")]
        public long IdVentaFV { get; set; }

        [JsonProperty("idIngresoFV")]
        public long IdIngresoFV { get; set; }

        [JsonProperty("fechaVenta")]
        public DateTime FechaVenta { get; set; }

        [JsonProperty("numeroCompra")]
        public string NumeroCompra { get; set; }

        [JsonProperty("numeroReferencia")]
        public string NumeroReferencia { get; set; }
        [JsonProperty]
        public List<CertificadoVentaData> certificadoVentas { get; set; }

        //[JsonProperty("urlDoc1")]
        //public IngresoFormaValoradaFiles? UrlDoc1 { get; set; }

        //[JsonProperty("urlDoc2")]
        //public IngresoFormaValoradaFiles? UrlDoc2 { get; set; }

        //[JsonProperty("urlDoc3")]
        //public IngresoFormaValoradaFiles? UrlDoc3 { get; set; }

        [JsonProperty("filesString")]
        public string? FilesString { get; set; }

        [JsonProperty("files")]
        public List<VentaFiles> Files { get; set; }
    }

    public class VentaFiles
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }

    public class CertificadoVentaData
    {
        public long IdIngresoCertificado { get; set; }

        public decimal PrecioUnitario { get; set; }
    }

}
