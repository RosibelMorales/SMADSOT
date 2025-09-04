using Newtonsoft.Json;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.EstadisticasUsoFormaValorada.Request
{
    public class EstadisticaUsoFormaValoradaRequest
    {
        [JsonProperty("fechaInicial")]
        public DateTime FechaInicial { get; set; }

        [JsonProperty("fechaFinal")]
        public DateTime FechaFinal { get; set; }

        [JsonProperty("idVerificentro")]
        public long? IdVerificentro { get; set; }
    }

    public class EstadisticaUsoFormaValoradaGridRequest : RequestList
    {
        [JsonProperty("fechaInicial")]
        public DateTime FechaInicial { get; set; }

        [JsonProperty("fechaFinal")]
        public DateTime FechaFinal { get; set; }
        [JsonProperty("idVerificentro")]
        public long? IdVerificentro { get; set; }

        [JsonProperty("tipoHolograma")]
        public long? TipoHolograma { get; set; }

        [JsonProperty("ModeloMin")]
        public int? ModeloMin { get; set; }
        [JsonProperty("modeloMax")]
        public int? ModeloMax { get; set; }

        [JsonProperty("causaRechazo")]
        public long? CausaRechazo { get; set; }
        [JsonProperty("idMotivoVerificacion")]
        public int? IdMotivoVerificacion { get; set; }
        [JsonProperty("idCatTipoServicio")]
        public int? IdCatTipoServicio { get; set; }

        [JsonProperty("esGrid")]
        public bool EsGrid { get; set; }

        //[JsonProperty("tipoCertificado")]
        //public long? TipoCertificado { get; set; }
    }
}
