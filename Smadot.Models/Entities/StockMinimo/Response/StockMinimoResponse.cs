using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Smadot.Models.Entities.StockMinimo.Response
{
    public class StockMinimoResponse
    {
        [JsonProperty("idStockMinimo")]
        public long? IdStockMinimo { get; set; }

        [JsonProperty("idAlmacen")]
        public long? IdAlmacen { get; set; }
        [JsonProperty("almacen")]
        public string? Almacen { get; set; }

        [JsonProperty("nombreAlmacen")]
        public string? NombreAlmacen { get; set; }

        [JsonProperty("cantidadMinima")]
        public int? CantidadMinima { get; set; }

        [JsonProperty("cantidadMedia")]
        public int? CantidadMedia { get; set; }

        [JsonProperty("idCatTipoCertificado")]
        public int? IdCatTipoCertificado { get; set; }

        [JsonProperty("nombre")]
        public string? Nombre { get; set; } = null!;

        [JsonProperty("claveCertificado")]
        public string? ClaveCertificado { get; set; }

        [JsonProperty("numeroSolucitud")]
        public long? NumeroSolucitud { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("almacenes")]
        public List<AlmacenResponse>? Almacenes { get; set; }
    }

}

