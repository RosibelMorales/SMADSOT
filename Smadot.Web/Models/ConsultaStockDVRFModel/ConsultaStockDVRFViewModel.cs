using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Smadot.Web.Models.ConsultaStockDVRFModel
{
    public class ConsultaStockDVRFViewModel
    {
        public class ConsultaStockResponse
        {
            public string NombreTipoCertificado { get; set; }

            public int? CantidadStock { get; set; }
            public long? IdAlmacen { get; set; }

            public int? NumeroCaja { get; set; }

            public int? FolioInicial { get; set; }

            public int? FolioFinal { get; set; }

            public string? ClaveCertificado { get; set; }

            public int? CantidadMinima { get; set; }

            public int? CantidadMedia { get; set; }

        }
        public class Registro : ConsultaStockResponse
        {
            public List<SelectListItem> Almacenes { get; set; }
        }
    }
}
