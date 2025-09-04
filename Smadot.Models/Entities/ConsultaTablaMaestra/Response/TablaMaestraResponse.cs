using System.Text.Json.Serialization;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.ConsultaTablaMaestra.Request;

namespace Smadot.Models.Entities.ConsultaTablaMaestra.Response
{
    public class TablaMaestraResponse : vTablaMaestra
    {
        [JsonIgnore]
        public List<string> CamposIgnorados
        {
            get
            {
                var _ = new List<string>
                {
                    nameof(Marca),
                    nameof(IdRegistroSubMarca),
                    nameof(SubMarca),
                    nameof(PROTOCOLO),
                    nameof(Id),
                    nameof(IdCatMarcaVehiculo),
                    nameof(IdCatSubmarcaVehiculo),
                    nameof(OBD),
                    nameof(DOBLECERO),
                    nameof(CERO_DSL),
                    nameof(CERO_GASOL),
                    nameof(CERO_GASLP),
                    nameof(CERO_GASNC),
                    nameof(REF_00),
                    nameof(CamposIgnorados),
                    nameof(CamposCheck),
                };
                return _;
            }
        }
        [JsonIgnore]
        public List<string> CamposCheck
        {
            get
            {
                var _ = new List<string>
                {
                    nameof(OBD),
                    nameof(DOBLECERO),
                    nameof(CERO_DSL),
                    nameof(CERO_GASOL),
                    nameof(CERO_GASLP),
                    nameof(CERO_GASNC),
                    nameof(REF_00),
                };
                return _;
            }
        }
    }
    public class RegistroTablaMaestraResponse
    {
        public TablaMaestraResponse TablaMaestra { get; set; }
        public List<Catalogo> Protocolos { get; set; }
        public List<Catalogo> Marcas { get; set; }
        public RegistroTablaMaestraResponse()
        {
            TablaMaestra = new();
            Protocolos = new();
            Marcas = new();
        }

    }
    public class Catalogo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
