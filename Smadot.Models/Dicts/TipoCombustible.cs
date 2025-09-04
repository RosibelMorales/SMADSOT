using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Dicts
{
    public class TipoCombustible
    {
        public const int Hibridos = 5;
        public const int Electricos = 6;
        public static Dictionary<int, string> DictTipoCombustible = new Dictionary<int, string>
                        {
                            {Hibridos,"Hibridos"},
                            {Electricos,"Electricos"}
                        };
        
    }
}
