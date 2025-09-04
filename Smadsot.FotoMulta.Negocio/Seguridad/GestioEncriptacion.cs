using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Smadsot.FotoMulta.Negocio.Seguridad.Modelo;

namespace Smadsot.FotoMulta.Negocio.Seguridad
{
    /// <summary>
    /// Clase encargada de encriptar y desencriptar la información
    /// </summary>
    public static class GestioEncriptacion
    {
        #region Métodos Publicos
        /// <summary>
        /// Método para encriptarun valor
        /// </summary>
        /// <param name="seguridad"> Modelo que recibe || Llave || Valor a cifrar</param>
        /// <returns></returns>
        public static string Cifrar(SeguridadModelo seguridad)
        {
            string valor = null;
            try
            {
                if (seguridad == null || seguridad.LlaveCifrado == null || seguridad.Valor == null)
                {
                    return null;
                }

                var argCifrado = ProcesoCifrado(new ProcesoSeguridadModelo(seguridad.Valor, seguridad.LlaveCifrado));

                valor = Convert.ToBase64String(argCifrado);
            }
            catch (Exception) { }
            return valor;
        }

        /// <summary>
        /// Método para decifrar valor
        /// </summary>
        /// <param name="seguridad"> Modelo que recibe || Llave || Valor a decifrar</param>
        /// <returns></returns>
        public static string Decifrar(SeguridadModelo seguridad)
        {
            string valor = null;
            try
            {
                if (seguridad == null || seguridad.LlaveCifrado == null || seguridad.Valor == null)
                {
                    return null;
                }

                var argDecifrado = ProcesoDecifrado(new ProcesoSeguridadModelo(seguridad.Valor, seguridad.LlaveCifrado));

                valor = Encoding.UTF8.GetString(argDecifrado);
            }
            catch (Exception) { }
            return valor;
        }

        #endregion

        #region Métodos privados
        /// <summary>
        /// Método encargado de cifrado de bytes para una forma más segura
        /// </summary>
        /// <param name="proceso">Valor a cifrar || Llave encargado del cifrado </param>
        /// <returns></returns>
        private static byte[] ProcesoCifrado(ProcesoSeguridadModelo proceso)
        {
            byte[] argCifrado = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(proceso.ArgLlave, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(proceso.ArgValor, 0, proceso.ArgValor.Length);
                        cs.Close();
                    }

                    argCifrado = ms.ToArray();
                }
            }

            return argCifrado;
        }

        /// <summary>
        /// Método encargado para desencritar los bytes
        /// </summary>
        /// <param name="proceso">Valor a cifrar || Llave encargado del cifrado </param>
        /// <returns></returns>
        private static byte[] ProcesoDecifrado(ProcesoSeguridadModelo proceso)
        {
            byte[] argDecifrado = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(proceso.ArgLlave, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(proceso.ArgValor, 0, proceso.ArgValor.Length);
                        cs.Close();
                    }

                    argDecifrado = ms.ToArray();
                }
            }

            return argDecifrado;
        }
        #endregion


    }
}
