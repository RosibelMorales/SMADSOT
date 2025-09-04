using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.SeguimientoCVV.Request
{
    public class VerificacionRequest
    {
        public long Id { get; set; }
        public long IdUserTecnico { get; set; }
        public long IdVerificentro { get; set; }
        public string Vin { get; set; }
        public string VinUpper
        {
            get
            {
                return Vin.ToUpper();
            }
        }
        public string? Placas { get; set; }
        public long? IdVerificacion { get; set; }
        public string UrlFotoEvidencia { get; set; }
        public byte[] ImgPlacaDelantera { get; set; }
        public byte[] ImgPlacaTrasera { get; set; }
    }
}
