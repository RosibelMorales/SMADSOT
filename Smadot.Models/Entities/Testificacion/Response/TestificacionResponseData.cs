
using System.Text.Json.Serialization;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;

namespace Smadot.Models.Entities.Testificacion.Response
{
    public class TestificacionResponseGrid : vVerificacionReposicion
    {
        // [JsonIgnore]
        // public int Total { get; set; }
        
        public string Acciones { get; set; }
        [JsonIgnore]
        public string FechaRegistroStr
        {
            get
            {
                return FechaRegistro.Value.ToString("dd/MM/yyyy");
            }
        }
    }
    public class TestificacionResponse : vTestificacion
    {
        [JsonIgnore]
        public string VigenciaStr
        {
            get
            {
                return VigenciaOrigen.ToString("dd/MM/yyyy");
            }
        }
        [JsonIgnore]
        public string FechaRegistroStr
        {
            get
            {
                return FechaRegistro.ToString("dd/MM/yyyy");
            }
        }
        [JsonIgnore]
        public string CertificadoOrigen
        {
            get
            {
                return DictTipoCertificadoTestificacion.DictNombreCertificado[IdTipoCertificadoOrigen];
            }
        }
    }
    public class PruebaAutocompletePlacaApi
    {
        public long Id { get; set; }
        public string Placa { get; set; }
        public string Serie { get; set; }
    }
}
