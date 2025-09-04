using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts.ProveedorDicts;
namespace Smadot.Models.Entities.ConsultaTablaMaestra.Response
{
    public class ConsultaTablaMaestraResponseData
    {



        public class ConsultaTablaMaestraResponseGrid : vTablaMaestra
        {
            public string ProtocoloStr
            {
                get
                {
                    try
                    {
                        return Dicts.ProveedorDicts.Protocolos.DictProtocolo[PROTOCOLO];
                    }
                    catch (global::System.Exception)
                    {
                        return "";
                    }
                }
            }
            public string AplicaDobleCero
            {
                get
                {
                    try
                    {
                        return DOBLECERO == 1 ? "SÍ" : "NO";
                    }
                    catch (global::System.Exception)
                    {
                        return "";
                    }
                }
            }
            public string Acciones { get; set; }
        }

        public class ConsultaTablaMaestraCicloVerificacionResponse
        {
            public long? Id { get; set; }

            public DateTime? FechaInicio { get; set; }

            public DateTime? FechaFin { get; set; }

            public string? Nombre { get; set; } = null!;

            public bool? Activo { get; set; }

            public decimal? ImporteFv { get; set; }

            public int Total { get; set; }
        }

        public class TablaMaestraAutocompleteResponse
        {
            public string Id { get; set; }
            public string Text { get; set; }
        }

    }
}
