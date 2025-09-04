using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Dicts
{
    public struct DictTipoEvento
    {
        public const int PuertaAbierta = 1;
        public const int PuertaCerrada = 2;
        public const int BloquearLinea = 3;
        public const int DesbloquearLinea = 4;
        public const int CalibracionRequerida = 5;
        public const int CalibracionFinalizada = 6;

        public const int IniciarVerificacion = 7;
        public const int ActualizarPrueba = 8;
        public const int FinalizarVerificacion = 9;
    }
}
