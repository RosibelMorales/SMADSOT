using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Smadot.Web.Models.StockMinimoModel
{
    public class StockMinimoViewModel
    {
        public class StockMinimoResponse
        {
            public long IdStockMinimo { get; set; }

            public long IdAlmacen { get; set; }

            public string? NombreAlmacen { get; set; }

            public int CantidadMinima { get; set; }

            public int CantidadMedia { get; set; }

            public int IdCatTipoCertificado { get; set; }

            public string Nombre { get; set; } = null!;

            public string? ClaveCertificado { get; set; }

            public int Total { get; set; }

        }

        public class Registro : StockMinimoResponse
        {
            public List<SelectListItem> Almacenes { get; set; }
        }
    }
}
