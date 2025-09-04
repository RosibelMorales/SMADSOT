using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Dicts
{
    public class TipoServicio
    {
        public const int TRANSPPASAJEPARTICULAR = 1;
        public const int TRANSPPASAJEPUBINDIVIDUAL = 2;
        public const int TRANSPPASAJEPUBCOLECTIVO = 3;
        public const int TRANSPPASAJEMERCANTILYPRIV = 7;
        public const int TRANSPCARGAPUBMERCPRIVPA = 8;
        public const int DISCAPACITADOS = 20;
        public const int EMERGENCIA = 21;
        public const int SERVICIOSURBANOS = 22;
        public const int VEHICULOANTIGUO = 23;
        public const int OTROS = 99;
        public static Dictionary<int, string> DictTipoServicio = new Dictionary<int, string>
                       {
                            {TRANSPPASAJEPARTICULAR,"TRANSP  PASAJE PARTICULAR"},
                            {TRANSPPASAJEPUBINDIVIDUAL,"TRANSP PASAJE PUB INDIVIDUAL"},
                            {TRANSPPASAJEPUBCOLECTIVO,"TRANSP PASAJE PUB COLECTIVO"},
                            {TRANSPPASAJEMERCANTILYPRIV,"TRANSP PASAJE MERCANTIL Y PRIV"},
                            {TRANSPCARGAPUBMERCPRIVPA,"TRANSP CARGA PUB, MERC,PRIV,PA"},
                            {DISCAPACITADOS,"DISCAPACITADOS"},
                            {EMERGENCIA,"EMERGENCIA"},
                            {SERVICIOSURBANOS,"SERVICIOS URBANOS"},
                            {VEHICULOANTIGUO,"VEHICULO ANTIGUO"},
                            {OTROS,"OTROS"},
                        };
    }
}
