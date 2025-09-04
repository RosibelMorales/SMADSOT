namespace Smadot.Models.Dicts
{
    public class CatTipoCalibracionDic
    {
        public const int Anual = 1;
        public const int Mensual = 2;
        public const int Diario = 3;
        public const int Semanal = 4;
        public const int Quincenal = 5;
        public const int Trimestral = 6;
        public const int Semestral = 7;

        public static Dictionary<int, string> Nombres = new()
        {
            {1, "Anual" },
            {2,"Mensual"},
            {3,"Diario"},
            {4,"Semanal"},
            {5,"Quincenal"},
            {6,"Trimestral"},
            {7,"Semestral"}
        };
    }
}
