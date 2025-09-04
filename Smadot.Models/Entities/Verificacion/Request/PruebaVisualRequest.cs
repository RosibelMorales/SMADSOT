using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Verificacion.Request
{
    public class PruebaVisualRequest<T>
    {
        public long IdVerificacion { get; set; }
        public long IdVerificentro { get; set; }
        public int NumeroEscapes { get; set; }
        public byte[]? FotoEvidencia { get; set; }
        public ResultadosPrueba<T> Resultados { get; set; }
    }
    public class ResultadosPrueba<T>
    {
        public int TipoCombustible { get; set; }
        public T Resultados { get; set; }
    }




    public class ResultadosGasolina
    {
        public bool SistemaEscape { get; set; }
        public bool PortafiltroAire { get; set; }
        public bool TaponDispositivoAceite { get; set; }
        public bool TaponCombustible { get; set; }
        public bool Bayoneta { get; set; }
        public bool NeumaticosBuenEstato { get; set; }
        public bool ComponenteControlEmisiones { get; set; }
        public bool FugaFluidos { get; set; }
    }



    public class ResultadosDiesel
    {
        public bool SistemaEscape { get; set; }
        public bool GobernadorBuenEstado { get; set; }
    }
}
