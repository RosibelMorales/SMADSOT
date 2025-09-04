namespace Smadot.Models.Entities.ConsultaPruebaVerificacion.Response
{
    public class ConsultaPruebaVerificacionResponse
    {
        public DateTime Fecha { get; set; }
        public string Placa { get; set; }
        public string Serie { get; set; }
        public string FolioCertificado { get; set; }
        public DateTime Vigencia { get; set; }
        public long? ResultadosPrueba { get; set; }
        public string TipoCertificado { get; set; }
        public string Semestre { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Anio { get; set; }
        public string Combustible { get; set; }
        public string TarjetaCirculacion { get; set; }
        public int? Total { get; set; }
    }
}
