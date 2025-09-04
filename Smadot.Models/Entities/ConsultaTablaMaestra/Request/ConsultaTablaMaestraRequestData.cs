using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.ConsultaTablaMaestra.Request
{
	public class ConsultaTablaMaestraRequestData
	{
		public class ConsultaTablaMaestraRequestList : RequestList
		{
            [JsonProperty("idCicloVerificacion")]
            public long? IdCicloVerificacion { get; set; }

            [JsonProperty("marca")]
            public string? Marca { get; set; }

            [JsonProperty("subMarca")]
            public string? SubMarca { get; set; }

            [JsonProperty("idMarca")]
            public long? IdMarca { get; set; }

            [JsonProperty("idSubMarca")]
            public long? IdSubMarca { get; set; }

            [JsonProperty("idTipoCombustible")]
            public int? IdTipoCombustible { get; set; }

            //[JsonProperty("cicloVerificacion")]
            //public string? CicloVerificacion { get; set; }

            [JsonProperty("protocolo")]
            public string? Protocolo { get; set; }
            [JsonProperty("modelo")]
            public string? Modelo { get; set; }
            [JsonProperty("validarCombustible")]
            public bool? ValidarCombustible { get; set; }
        }
	}

    public class TablaMaestraDbfRequest
    {
        public List<TablaMaestra> TablaMaestra { get; set; }
        public List<CatTipoDiesel> CatTipoDiesel { get; set; }
        public List<CatSubdieselVehiculo> CatSubdieselVehiculo { get; set; }
        public List<CatMarcaVehiculo> CatMarcaVehiculo { get; set; }
        public List<TablaMaestraDbfCatSubMarcaVehiculoRequest> CatSubMarcaVehiculo { get; set; }
    }

    public class TablaMaestraDbfCatSubMarcaVehiculoRequest
    {
        public int IdCatMarcaVehiculo { get; set; }
        public int Clave { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Alias { get; set; }
        public int Id { get; set; }
    }
}
