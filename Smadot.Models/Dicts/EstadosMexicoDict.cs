using Newtonsoft.Json;
namespace Smadot.Models.Dicts
{
    public struct EstadosMexicoDict
    {
        public static List<EstadoMexico> DictEstados = new List<EstadoMexico>{
            new EstadoMexico{Clave="AGUASCALIENTES",Nombre="AGUASCALIENTES"},
            new EstadoMexico{Clave="BAJA CALIFORNIA NTE",Nombre="BAJA CALIFORNIA NTE"},
            new EstadoMexico{Clave="BAJA CALIFORNIA SUR",Nombre="BAJA CALIFORNIA SUR"},
            new EstadoMexico{Clave="CAMPECHE",Nombre="CAMPECHE"},
            new EstadoMexico{Clave="CHIHUAHUA",Nombre="CHIHUAHUA"},
            new EstadoMexico{Clave="CHIAPAS",Nombre="CHIAPAS"},
            new EstadoMexico{Clave="COLIMA",Nombre="COLIMA"},
            new EstadoMexico{Clave="COAHUILA",Nombre="COAHUILA"},
            new EstadoMexico{Clave="CIUDAD DE MÉXICO",Nombre="CIUDAD DE MÉXICO"},
            new EstadoMexico{Clave="DURANGO",Nombre="DURANGO"},
            new EstadoMexico{Clave="ESTADO DE MEXICO",Nombre="ESTADO DE MEXICO"},
            new EstadoMexico{Clave="GUERRERO",Nombre="GUERRERO"},
            new EstadoMexico{Clave="GUANAJUATO",Nombre="GUANAJUATO"},
            new EstadoMexico{Clave="HIDALGO",Nombre="HIDALGO"},
            new EstadoMexico{Clave="JALISCO",Nombre="JALISCO"},
            new EstadoMexico{Clave="MICHOACAN",Nombre="MICHOACAN"},
            new EstadoMexico{Clave="MORELOS",Nombre="MORELOS"},
            new EstadoMexico{Clave="NAYARIT",Nombre="NAYARIT"},
            new EstadoMexico{Clave="NUEVO LEON",Nombre="NUEVO LEON"},
            new EstadoMexico{Clave="OAXACA",Nombre="OAXACA"},
            new EstadoMexico{Clave="PUEBLA",Nombre="PUEBLA"},
            new EstadoMexico{Clave="QUINTANA ROO",Nombre="QUINTANA ROO"},
            new EstadoMexico{Clave="QUERETARO",Nombre="QUERETARO"},
            new EstadoMexico{Clave="SINALOA",Nombre="SINALOA"},
            new EstadoMexico{Clave="SAN LUIS POTOSI",Nombre="SAN LUIS POTOSI"},
            new EstadoMexico{Clave="SONORA",Nombre="SONORA"},
            new EstadoMexico{Clave="TAMAULIPAS",Nombre="TAMAULIPAS"},
            new EstadoMexico{Clave="TABASCO",Nombre="TABASCO"},
            new EstadoMexico{Clave="TLAXCALA",Nombre="TLAXCALA"},
            new EstadoMexico{Clave="VERACRUZ",Nombre="VERACRUZ"},
            new EstadoMexico{Clave="YUCATAN",Nombre="YUCATAN"},
            new EstadoMexico{Clave="ZACATECAS",Nombre="ZACATECAS"},
        };
    }
    public class EstadoMexico
    {
        [JsonProperty("clave")]
        public string? Clave { get; set; }
        [JsonProperty("nombre")]
        public string? Nombre { get; set; }

    }
    // public static class JsonFileReader
    // {
    //     public static T Read<T>(string filePath)
    //     {
    //         string text = File.ReadAllText(filePath);
    //         var json = JsonConvert.DeserializeObject<T>(text);
    //         return json;
    //     }
    // }
}
