using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.FoliosFormaValorada.Response
{
    public class FolioPendienteImprimirResponse : vPendientesImprimir
    {
        public string Nombre { get; set; }

        public int IdCatTipoCertificado { get; set; }

        public string Actions { get; set; }
        public string FechaStr
        {
            get
            {
                return Fecha.ToString("g");
            }
        }
        public string FolioStr
        {
            get
            {
                try
                {

                    return FolioFoliosFormaValoradaVerificentro.Value.ToString("000000000");
                }
                catch (Exception)
                {

                    return "-";
                }
            }
        }
        
    }
}
