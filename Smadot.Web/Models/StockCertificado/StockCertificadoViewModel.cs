using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Smadot.Web.Models.StockCertificado
{
    public class StockCertificadoViewModel
    {
        public class StockCertificadoResponse
        {
            public long Id { get; set; }

            public int IdCatTipoCertificado { get; set; }

            public int CantidadMinima { get; set; }

            public int CantidadMedia { get; set; }

            public long IdStockMinimo { get; set; }
        }
    }
}
