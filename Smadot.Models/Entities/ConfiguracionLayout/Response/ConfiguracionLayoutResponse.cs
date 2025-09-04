using Smadot.Models.DataBase;

namespace Smadot.Models.Entities.ConfiguracionLayout.Response
{
    public class ConfiguracionLayoutResponse : vFoliosFormaValoradaVerificentroActuale
    {
        public decimal Precio { get; set; }
        public bool Selected { get; set; }
        public int TipoTramiteLayout { get; set; }
    }
}
