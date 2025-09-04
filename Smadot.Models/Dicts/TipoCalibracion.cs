namespace Smadot.Models.Dicts
{
    public class TipoCalibracion
    {
        public const int Anual = 1;
        public const int Mensual = 2;
        public const int Diario = 3;
        public const int Semanal = 4;
        public const int Quincenal = 5;
        public const int Trimestral = 6;
        public const int Semestral = 4;


        public List<object> NombreTipoCalibracion = new List<object>()
        {
            "Anual", "Mensual", "Diario" ,"Semanal" , "Quincenal" , "Trimestral" , "Semestral"
        };

    }
}
