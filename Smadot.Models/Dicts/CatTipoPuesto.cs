namespace Smadot.Models.Dicts
{
    public class CatTipoPuesto
    {
        public const int TecnicoVerificadorCapacitación = 1;
        public const int Administrativo = 2;

        public List<string> Nombres = new()
        {
            "Técnico Verificador en Capacitación","Administrativo"
        };
    }
}
