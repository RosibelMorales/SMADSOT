namespace Smadot.Models.Dicts
{
    public class CatEstatusReporteCredencialDic
    {
        public const int SolicitarAutorizacion = 1;
        public const int ApruebaDVRF = 2;
        public const int RechazadoDVRF = 3;

        public List<string> Nombres = new()
        {
            "Solicitar Autorización","Aprueba DVRF","Rechazado DVRF"
        };
    }
}
