namespace Smadot.Models.Dicts
{
    public static class CatColoresDic
    {
        public const int Gris = 0;
        public const int Rojo = 1;
        public const int Naranja = 2;
        public const int Verde = 3;

        public static List<string> Nombres = new()
        {
            "Gris",
            "Rojo",
            "Naranja",
            "Verde"
        };
    }
    public static class EstatusCalibracion
    {
        public const int DocumentacionSinRevisar = 1;
        public const int Rechazado = 2;
        public const int ValidadoDVRF = 3;
        public const int SolicitaModificar = 4;
        public const int RechazaModificar = 5;
        public const int PermiteModificar = 6;
        public const int PendienteCalibracion = 7;
        public const int CalibracionFallida = 8;
        public static Dictionary<int, string> Colores = new()
        {
            {1,"Gris"},
            {2,"Rojo"},
            {8,"Rojo"},
            {7,"Rojo"},
            {4 ,"Naranja"},
            {5 ,"Naranja"},
            {6 ,"Naranja"},
            {3,"Verde"}
        };


    }
}
