
namespace Smadot.Utilities.Modelos.Documentos
{
    public class ConsultaTablaMaestraPDFUtilitiesResponse
    {
        public List<ConsultaTablaMaestraUtilitiesResponse> Rows { get; set; }
        public string DirecciónVerificentro { get; set; }
        public string TelefonoVerificentro { get; set; }
    }
    public class ConsultaTablaMaestraUtilitiesResponse
    {
        public long? IdTablaMaestra { get; set; }
        public long? IdCicloVerificacion { get; set; }
        public string? Marca { get; set; }
        public string? SubMarca { get; set; }
        public string? CicloVerificacion { get; set; }
        public int? PROTOCOLO { get; set; }
        public DateTime? FechcaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? Nombre { get; set; }
        public bool? Activo { get; set; }
        public decimal? ImporteFv { get; set; }
        public int? CILINDROS { get; set; }
        public int? PBV_EQUIV { get; set; }

        public int? PBV_ASM { get; set; }
        public int? CILINDRADA { get; set; }
        public int? DobleCero { get; set; }
        public int? ANO_DESDE { get; set; }
        public int? ANO_HASTA { get; set; }
        public int? CERO_GASOL { get; set; }

        public int? CERO_GASLP { get; set; }

        public int? CERO_GASNC { get; set; }

        public int? CERO_DSL { get; set; }
        public string CombustibleStr
        {
            get
            {
                try
                {
                    string resultado = "";
                    if (CERO_GASOL == 1)
                        resultado += "Gasolina, ";

                    if (CERO_GASLP == 1)
                        resultado += "Gas LP, ";

                    if (CERO_GASNC == 1)
                        resultado += "Gas Natural, ";

                    if (CERO_DSL == 1)
                        resultado += "Diesel, ";
                    // Eliminar la última coma y espacio si existen
                    if (!string.IsNullOrEmpty(resultado))
                        resultado = resultado.TrimEnd(',', ' ');
                    return resultado;
                }
                catch (global::System.Exception)
                {
                    return "";
                }
            }
        }
        public string ProtocoloStr
        {
            get
            {
                try
                {
                    return ProtocolosUtilites.DictProtocolo[PROTOCOLO ?? 0];
                }
                catch (global::System.Exception)
                {
                    return "";
                }
            }
        }
        public string AplicaDobleCero
        {
            get
            {
                try
                {
                    return DobleCero == 1 ? "SÍ" : "NO";
                }
                catch (global::System.Exception)
                {
                    return "";
                }
            }
        }

        public struct ProtocolosUtilites
        {
            public const int ESTATICO = 2;
            public const int DIESELSINDATOS = 5;
            public const int ACELERACIONDIESEL = 6;
            public const int DINAMICO = 7;
            public static Dictionary<int, string> DictProtocolo = new Dictionary<int, string>
                       {
                            {ESTATICO, "Estático (TSI)" },
                            {ACELERACIONDIESEL, "Aceleración libre (Diesel)" },
                            {DINAMICO, "dinámico (ASM)" },
                            {DIESELSINDATOS, "Diesel sin encontrar datos" },

        };
        }
    }
}
