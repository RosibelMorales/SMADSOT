using Smadot.Models.Entities.Personal.Request;

namespace Smadot.Models.Entities.Personal.Response
{
    public class PersonalResponse : PersonalRequest
    {
        public string NombrePuesto { get; set; }
        public string UrlIne { get; set; }
        public string UrlFoto { get; set; }
        public string UrlSeguroSocial { get; set; }
        public string UrlFirma { get; set; }
        public string UrlContrato { get; set; }
        public long IdPuestoVerificentro { get; set; }
        public int IdCatEstatusPuesto { get; set; }
    }
    public class BajaPersonalResponse 
    {
        public string Nombre { get; set; }
        public string NombrePuesto { get; set; }
    }
}
