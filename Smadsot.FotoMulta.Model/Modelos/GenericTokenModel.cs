using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Smadsot.FotoMulta.Model.Modelos
{
    /// <summary>
    /// Clase del modelo genererico para la creacion de un token
    /// </summary>
    public class GenericTokenModel
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public List<Claim> Claims { get; set; }
        public string KeySecret { get; set; }
        public int TimeExpire { get; set; }
    }
}
