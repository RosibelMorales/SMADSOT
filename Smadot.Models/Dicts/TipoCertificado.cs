namespace Smadot.Models.Dicts
{
    public class TipoCertificado
    {
        public const int DobleCero = 1;
        public const int Cero = 2;
        public const int Uno = 3;
        public const int Dos = 4;
        public const int ConstanciasNoAprobado = 5;
        public const int Exentos = 6;
        public const int Testificacion = 7;

        public List<object> NombreCertificado = new List<object>()
        {
            "DOBLECERO","CERO","UNO","DOS","CONSTANCIASNOAPROBADO","EXENTOS", "TESTIFICACION"
        };
        public static Dictionary<int, string> DictNombreCertificado = new()
        {
            { DobleCero,"Tipo 00"},
            { Cero ,"Tipo 0"},
            { Uno ,"Tipo 1"},
            { Dos ,"Tipo 2"},
            { ConstanciasNoAprobado ,"Tipo CNA"},
            { Exentos ,"Tipo Exento"},
            { Testificacion ,"Testificación"},
        };
        public static Dictionary<int, string> DictNombreClave = new()
        {
            { Exentos ,"1701001"},
            { Uno ,"1701002"},
            { Dos ,"1701003"},
            { Cero ,"1701004"},
            { DobleCero,"1701005"},
            { ConstanciasNoAprobado ,"1701006"},
            { Testificacion ,"1701014"},
        };
        public static Dictionary<int, string> DictNombreCertificadoCVV = new()
        {
            { DobleCero,"Tipo 00"},
            { Cero ,"Tipo 0"},
            { Uno ,"Tipo 1"},
            { Dos ,"Tipo 2"},
            { ConstanciasNoAprobado ,"Tipo CNA"},
        };
        public static Dictionary<int, string> DictNombreCertificadoSimbolo = new()
        {
            { DobleCero,"00"},
            { Cero ,"0"},
            { Uno ,"1"},
            { Dos ,"2"},
            { ConstanciasNoAprobado ,"CNA"},
            { Exentos ,"E"},
            { Testificacion ,"T"},
        };
        public static Dictionary<int, string> ColorCertificado = new()
        {
            { DobleCero,"text-success bg-light-success"},
            { Cero ,"text-warning bg-light-warning"},
            { Uno ,"text-orange bg-light-orange"},
            { Dos ,"text-orange2 bg-light-orange2"},
            { ConstanciasNoAprobado ,"text-secondary bg-light-secondary"},
            { Exentos ,"text-danger bg-light-danger"},
            { Testificacion ,"text-danger bg-light-danger"},
        };
    }
    public class TipoTramite
    {
        public const int CV = 1;
        public const int CE = 2;
        public const int CA = 3;
        public const int CR = 4;
        public const int CoV = 5;
        public const int CANCELADO = 6;
        public const int CT = 7;

        public static Dictionary<int, string> Dict = new()
        {
            { CV,"CV"},
            { CE ,"CE"},
            { CA ,"CA"},
            { CR ,"CR"},
            { CoV ,"CoV"},
            { CANCELADO ,"CANCELADO"},
            { CT ,"CT"},
        };


    }
}
