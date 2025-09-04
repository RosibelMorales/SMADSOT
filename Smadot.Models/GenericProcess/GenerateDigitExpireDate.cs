using Smadot.Models.DataBase;

namespace Smadot.Models.GenericProcess
{
    public static class GenerateDigitExpireDate
    {
        public static Dictionary<string, int> PrimerSemestre = new()
        {
            {"5,6",2},
            {"7,8",3},
            {"3,4",4},
            {"1,2",5},
            {"9,0",6}
        };
        public static Dictionary<string, int> SegundoSemestre = new()
        {
            {"5,6",8},
            {"7,8",9},
            {"3,4",10},
            {"1,2",11},
            {"9,0",12}
        };

        public static DateTime GetExpireDate(int digito, int semestre, int anio, List<CalendarioVerificacion> calendarioVerificacion)
        {

            var calendario = semestre == 2 ? calendarioVerificacion.Where(x => x.Semestre == 1).ToList() : calendarioVerificacion.Where(x => x.Semestre == 2).ToList();
            if (semestre == 2)
                anio += 1;
            var mes = calendario.FirstOrDefault(x => x.Digitos.Contains(digito.ToString())).Mes;
            return GetLastDayOfMonth(anio, mes);
        }
        static DateTime GetLastDayOfMonth(int year, int month)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            return new DateTime(year, month, daysInMonth);
        }
    }
}
