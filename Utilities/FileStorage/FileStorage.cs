using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Smadot.Utilities.FileStorage
{
    /// <summary>
    /// Gestion de imagenes
    /// </summary>
    public class StorageManager
    {
        private readonly string BasePath = Directory.GetCurrentDirectory() + "/SmadotStorage/"; //Path
        /// <summary>
        /// Guardar archivo mediante cadena base64
        /// </summary>
        /// <param name="strbase64"></param>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public string GuardarArchivo(string strbase64, string fileName, string path)
        {
            string extension = Path.GetExtension(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName);
            if (!System.IO.Directory.Exists(BasePath)) { Directory.CreateDirectory(BasePath); }
            string[] subpathArray = path.Split('/');
            string subPath = "";
            string fullPath = "";
            foreach (var item in subpathArray)
            {
                subPath += $"{item}/";
                fullPath = Path.Combine(BasePath, subPath);
                if (!System.IO.Directory.Exists(fullPath)) { Directory.CreateDirectory(fullPath); }
            }
            fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
            string filePath = Path.Combine(fullPath, fileName);
            byte[] imageBytes = Convert.FromBase64String(strbase64);
            subPath = Path.Combine(subPath, fileName).Replace("//", "/");
            File.WriteAllBytes(filePath, imageBytes);
            return subPath;
        }
        /// <summary>
        /// Guardar archivo mediante un arreglo de bytes
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public string GuardarArchivoBytes(byte[] fileBytes, string fileName, string path)
        {

            string extension = Path.GetExtension(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName);
            if (!System.IO.Directory.Exists(BasePath)) { Directory.CreateDirectory(BasePath); }
            string[] subpathArray = path.Split('/');
            string subPath = "";
            string fullPath = "";
            foreach (var item in subpathArray)
            {
                subPath += $"{item}/";
                fullPath = Path.Combine(BasePath, subPath);
                if (!System.IO.Directory.Exists(fullPath)) { Directory.CreateDirectory(fullPath); }
            }
            fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
            subPath = Path.Combine(subPath, fileName);
            string filePath = Path.Combine(fullPath, fileName);
            File.WriteAllBytes(filePath, fileBytes);
            return subPath;
        }
        /// <summary>
        /// Obtener archivo en base64
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string ObtenerArchivo(string path)
        {
            try
            {
                string filePath = Path.Combine(BasePath, path);
                byte[] fileArray = System.IO.File.ReadAllBytes(filePath);
                string base64ImageRepresentation = Convert.ToBase64String(fileArray);
                return base64ImageRepresentation;
            }
            catch (Exception)
            {

                return "";
            }

        }
        /// <summary>
        /// Eliminar archivo
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int EliminarArchivo(string path)
        {
            try
            {
                string filePath = Path.Combine(BasePath, path);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return 1;

                }
                else
                {
                    return 0;

                }
            }
            catch (Exception)
            {

                return 0;
            }

        }
        public string ObtenerRuta(string path)
        {
            try
            {
                string filePath = Path.Combine(BasePath, path);
                if (System.IO.File.Exists(filePath))
                {
                    return filePath;
                }
                else
                    return "";
            }
            catch (Exception)
            {
                return "";
            }

        }
        public static string GetParsePath(string path)
        {
            try
            {
                Uri uri = new Uri(path);
                string filePath = uri.LocalPath;
                return WebUtility.UrlEncode(filePath);
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string GetParsePathWithOutContainer(string path)
        {
            try
            {
                Uri uri = new Uri(path);
                string filePath = uri.LocalPath;
                string pathe = WebUtility.UrlEncode(filePath).Replace("%2Fdocumentos", "");
                return pathe;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string GetPathExtension(string url)
        {
            try
            {
                // Utiliza el método Path para obtener la extensión del archivo
                string extension = System.IO.Path.GetExtension(new Uri(url).LocalPath);

                if (!string.IsNullOrEmpty(extension))
                {
                    // Quita el punto inicial de la extensión si es necesario
                    if (extension.StartsWith("."))
                    {
                        extension = extension.Substring(1);
                    }


                }
                else
                {
                }
                return extension;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
