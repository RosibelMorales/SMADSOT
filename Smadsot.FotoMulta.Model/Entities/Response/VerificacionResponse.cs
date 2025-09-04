using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Smadsot.FotoMulta.Model.Entities.Response
{
    public class VerificacionResponse
    {
        [JsonPropertyName("verificacionVigente")]
        public bool VerificacionVigente { get; set; }
        [JsonPropertyName("placa")]
        public string Placa { get; set; }
    }
}