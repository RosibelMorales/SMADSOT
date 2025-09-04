using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Smadot.Models.Entities.StockMinimo.Response
{
    public class tStockMinimoResponse
    {
        public long Id { get; set; }

        public long IdAlmacen { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long? IdUserRegistro { get; set; }
    }
}
