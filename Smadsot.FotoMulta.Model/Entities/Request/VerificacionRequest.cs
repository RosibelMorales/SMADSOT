using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Smadsot.FotoMulta.Model.Entities.Request
{
    public class VerificacionRequest
    {
        [Required]
        [JsonPropertyName("placa")]
        public string Placa { get; set; }
    }
}