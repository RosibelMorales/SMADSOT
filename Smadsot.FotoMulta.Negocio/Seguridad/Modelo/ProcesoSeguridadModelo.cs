using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Smadsot.FotoMulta.Negocio.Seguridad.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcesoSeguridadModelo
    {
        #region Propiedades
        public byte[] ArgValor { get; set; }
        public byte[] ArgLlave { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor de iniciar  el proceso del cifrado
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="llave"></param>
        public ProcesoSeguridadModelo(string valor, string llave)
        {
            ArgValor = Encoding.UTF8.GetBytes(valor);
            ArgLlave = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(llave));
        }
        #endregion


    }
}
