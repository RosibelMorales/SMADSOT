using Smadot.Models.DataBase;

namespace Smadot.Models.Entities.ReposicionCredencial.Response
{
    public class ReposicionCredencialGridResponse : vReposicionCredencialResponse
    {
        public string FechaStr
        {
            get
            {
                return FechaRegistro != null ? FechaRegistro.Value.ToString("dd/MM/yyyy") : "-/-/-";
            }
        }
        public string Acciones { get; set; }
    }    
}
