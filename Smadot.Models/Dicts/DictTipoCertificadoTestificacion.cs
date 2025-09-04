namespace Smadot.Models.Dicts
{
    public class DictTipoCertificadoTestificacion
    {
        public const int Testificacion = 1;
        

        public static Dictionary<int, string> DictNombreCertificado = new()
        {
            {Testificacion, "Testificación"}
        };


    }
}