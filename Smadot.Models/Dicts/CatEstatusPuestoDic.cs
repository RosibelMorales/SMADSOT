using Smadot.Models.DataBase;

namespace Smadot.Models.Dicts
{
    public static class CatEstatusPuestoDic
    {
        public const int PendienteValidacion = 1;

        public const int AprobadoDVRF = 2;

        public const int RechazadoDVRF = 3;

        public const int SolicitaModificar = 4;

        public const int RechazaModificar = 5;

        public const int PermiteModificar = 6;

        public static List<string> Nombres = new()
        {
            "Pendiente Validación",
            "Aprobado DVRF",
            "Rechazado DVRF",
            "Solicitar Modificar",
            "Rechaza Modificar",
            "Permite Modificar"
        };
    }
}
