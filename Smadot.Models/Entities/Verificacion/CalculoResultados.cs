using Smadot.Models.DataBase;

namespace Smadot.Models.Entities.Verificacion
{

    public class ResponseValidacion
    {
        public long recordId { get; set; }
        public bool IsSucces { get; set; }
        public string? Description { get; set; }
    }
    public class CertificadoResultado
    {
        public bool Error { get; set; }	
        public int ResultadoTipoCertificado { get; set; }
        public int ResultadoPrueba { get; set; }
        public int CausaRechazo { get; set; }
        public int CausaRechazoOBD { get; set; }
        public string Mensaje { get; set; }
        public LimiteVerificacion? LimiteVerificacion { get; set; }
    }
}