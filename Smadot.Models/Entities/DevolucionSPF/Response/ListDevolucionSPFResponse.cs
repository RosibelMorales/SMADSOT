using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.DevolucionSPF.Response
{
    public class ListDevolucionSPFResponse : vDevolucionSPF
    {
        public string FechaRegistroStr
        {
            get
            {

                return FechaRegistro.ToString("d",CultureInfo.GetCultureInfo("es-ES"));
            }
        }
        public string FechaEntregaStr
        {
            get
            {

                return FechaEntrega?.ToString("d", CultureInfo.GetCultureInfo("es-ES"));
            }
        }
        public string Actions { get; set; }
    }
}
