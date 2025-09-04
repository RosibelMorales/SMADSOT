using System.ComponentModel;

namespace Smadot.Models.Dicts
{
    public static class CatHorarioUserPuestoVerificentro
    {
        public const int Lunes = 0;
        public const int Martes = 1;
        public const int Miercoles = 2;
        public const int Jueves = 3;
        public const int Viernes = 4;
        public const int Sabado = 5;
        public const int Domingo = 6;

        public static List<string> Nombres = new()
        {
            "Lunes","Martes","Miércoles","Jueves","Viernes","Sábado","Domingo"
        };
    }
}
