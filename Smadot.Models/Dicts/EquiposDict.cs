namespace Smadot.Models.Dicts
{
    public class EquiposDict
    {
        public const int ESTACIONMETEOROLOGICA = 1;
        public const int PESAS = 2;
        public const int ANALIZADOR = 3;
        public const int DINAMOMETRO = 4;
        public const int GABINETE = 5;
        public const int OPACIMETRO = 6;
        public const int MICROBANCA = 7;
        public const int TACOMETROPINZAS = 8;
        public const int TACOMETRONOCONTACTO = 9;
        public const int TACOMETROENCENDEDOR = 10;
        public const int TACOMETROOPTICO = 11;
        public const int TACOMETROTRANSDUCTOR = 12;
        public static Dictionary<int, string> Equipos = new(){
            {ESTACIONMETEOROLOGICA, "ESTACIÓN METEOROLÓGICA"},
            {PESAS, "PESAS"},
            {ANALIZADOR, "ANALIZADOR DE GASES"},
            {DINAMOMETRO, "DINAMÓMETRO"},
            {GABINETE, "GABINETE"},
            {OPACIMETRO, "OPACÍMETRO"},
            {MICROBANCA, "MICROBANCA"},
            {TACOMETROPINZAS, "TACÓMETRO DE PINZAS"},
            {TACOMETRONOCONTACTO, "TACÓMETRO DE NO CONTACTO"},
            {TACOMETROENCENDEDOR, "TACÓMETRO DE ENCENDEDOR"},
            {TACOMETROOPTICO, "TACÓMETRO ÓPTICO"},
            {TACOMETROTRANSDUCTOR, "TACÓMETRO TRANSDUCTOR MEDIANTE OBD"}
        };
    }
}