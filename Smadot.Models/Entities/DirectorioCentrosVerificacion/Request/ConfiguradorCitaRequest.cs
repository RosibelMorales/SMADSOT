using Smadot.Models.DataBase;

namespace Smadot.Models.Entities.DirectorioCentrosVerificacion.Request
{
    public class ConfiguradorCitaRequest
    {
        public long IdCVV { get; set; }
        public List<ConfiguradorCitaDiasRequest> Dias { get; set; }
    }

    public class ConfiguradorCitaDiasRequest : vConfiguradorCitum
    {
        //public string HoraFinString { get; set; }

    }public class ConfiguradorCitaEditRequest
    {
        //public string HoraFinString { get; set; }
        public bool NoValidar { get; set; }
        public ConfiguradorCitaDiasRequest Data { get; set; }
    }
}
