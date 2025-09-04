using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Smadot.Models.Entities.Calibracion.Request
{
    public class CalibracionProveedorRequest
    {
        [JsonProperty("numeroSerie")]
        public string NumeroSerie { get; set; }//:"ML3AB26JXKH004672",

        [JsonProperty("fechaCalibracion")]
        public string FechaCalibracion { get; set; }//: "27-04-2023 10:05:00",

        [JsonProperty("fechaProximaCalibracion")]
        public string FechaProximaCalibracion { get; set; }//: "27-04-2024 10:05:00",

        [JsonProperty("nombreTecnico")]
        public string NombreTecnico { get; set; }//: "José Luis Fernández",

        [JsonProperty("calibracionExitosa")]
        public bool CalibracionExitosa { get; set; }//: true,

        [JsonProperty("motivoFalla")]
        public string? MotivoFalla { get; set; }//: ""
        [JsonIgnore]
        public DateTime FechaCalibracionParse
        {
            get
            {
                return DateTime.ParseExact(FechaCalibracion, "dd/MM/yyyy hh:mm:ss tt", new CultureInfo("es-MX"));
            }
        }//: "27-04-2023 10:05:00",
        [JsonIgnore]
        public DateTime FechaProximaCalibracioParse
        {
            get
            {
                return DateTime.ParseExact(FechaProximaCalibracion, "dd/MM/yyyy hh:mm:ss tt", new CultureInfo("es-MX"));
            }
        }//: "27-04-2024 10:05:00",

    }
}