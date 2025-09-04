using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Smadot.Models.Entities.FoliosFormaValorada.Request
{
    public class ImpresionRequest
    {
        [JsonProperty("tipoCertificado")]
        public int TipoCertificado { get; set; }
        [JsonProperty("pdfBytes")]
        public byte[] PdfBytes { get; set; }
    }
    public class GenerateImpresionRequest
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("idVerificacion")]
        public long? Idverificacion { get; set; }
        [JsonProperty("imprimir")]
        public bool Imprimir { get; set; }
        
        
    }
}
