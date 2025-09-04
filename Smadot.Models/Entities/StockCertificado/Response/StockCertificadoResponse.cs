using Smadot.Models.Entities.Catalogos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.StockCertificado.Response
{
    public class StockCertificadoResponse
    {
        public long Id { get; set; }

        public int IdCatTipoCertificado { get; set; }

        public int CantidadMinima { get; set; }

        public int CantidadMedia { get; set; }

        public long IdStockMinimo { get; set; }

        public decimal? DobleCeroCantidadMinima { get; set; }

        public decimal? DobleCeroCantidadMedia { get; set; }

        public decimal? CeroCantidadMinima { get; set; }

        public decimal? CeroCantidadMedia { get; set; }

        public decimal? UnoCantidadMinima { get; set; }

        public decimal? UnoCantidadMedia { get; set; }

        public decimal? DosCantidadMinima { get; set; }

        public decimal? DosCantidadMedia { get; set; }

        public decimal? ConstanciaNoAprobadaCantidadMinima { get; set; }

        public decimal? ConstanciaNoAprobadaCantidadMedia { get; set; }

        public decimal? ExentosCantidadMinima { get; set; }

        public decimal? ExentosCantidadMedia { get; set; }

        public int Total { get; set; }

        public List<AlmacenResponse> Almacenes { get; set; }
    }
}
