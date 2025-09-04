using Smadot.Models.Dicts;

namespace Smadot.Models.Entities.Personal.Extensions
{
    public static class PersonalExtension
    {
        //Método para obtener el mensaje de error al lanzarse una excepción desde lógica de negocio.
        public static string ObtenerMensajeExcepcion(this string mensajeExcepcion)
        {
			if (mensajeExcepcion.Contains("ERRORCONTROLADOSMADSOT"))
				return mensajeExcepcion.Replace("ERRORCONTROLADOSMADSOT", "");
			string[] separadores = new string[] { "ValidationException:", "\r\n   at" };
            string[] mensajesSeparados = mensajeExcepcion.Split(separadores, StringSplitOptions.None);

            return (mensajesSeparados.Count() > 2) ? mensajesSeparados[1] : string.Empty;
        }

        public static string ObtenerMensajeExcepcionSystem(this string mensajeExcepcion)
        {
            if (mensajeExcepcion.Contains("ERRORCONTROLADOSMADSOT"))
                return mensajeExcepcion.Replace("ERRORCONTROLADOSMADSOT", "");
            string[] separadores = new string[] { "System.Exception:", "\r\n   at" };
            string[] mensajesSeparados = mensajeExcepcion.Split(separadores, StringSplitOptions.None);

            return (mensajesSeparados.Count() > 2) ? mensajesSeparados[1] : string.Empty;
        }
    }
}
