using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.AsignacionStock.Response
{
    public class ListAsignacionStockResponse : vAsignacionStock
    {
        public string FechaRegistroStr
        {
            get {

                return FechaRegistro.ToString("d");
            }
        }
        public string FechaEntregaStr
        {
            get
            {

                return FechaEntrega.ToString("d");
            }
        }
        public string Actions { get; set; }
    }
}
