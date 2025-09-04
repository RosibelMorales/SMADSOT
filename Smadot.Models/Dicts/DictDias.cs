namespace Smadot.Models.Dicts
{
    public class DiasDict
    {
        public static Dictionary<string, DayOfWeek> spanishDays = new()
                    {
                        { "Lunes", DayOfWeek.Monday },
                        { "Martes", DayOfWeek.Tuesday },
                        { "Miércoles", DayOfWeek.Wednesday },
                        { "Jueves", DayOfWeek.Thursday },
                        { "Viernes", DayOfWeek.Friday },
                        { "Sábado", DayOfWeek.Saturday },
                        { "Domingo", DayOfWeek.Sunday }
                    };
    }
}