using Microsoft.IdentityModel.Tokens;
using Smadot.Models.DataBase;

namespace Smadot.Models.Entities.Personal.Response
{
    public class PersonalGridResponse : vPersonalAutorizacion
    {
        public string Actions { get; set; }
        public string FechaIncorporacionPuestoString 
            => FechaIncorporacionPuesto.ToString("dd/MM/yyyy");
        
        public string FechaSeparacionPuestoString 
            => FechaSeparacionPuesto is not null ? FechaSeparacionPuesto?.ToString("dd/MM/yyyy") : "";

        public string Iniciales { get; set; }

        public string? UrlFotoBase64 { get; set; }
    }
}
