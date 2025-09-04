using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.RecepcionDocumentos.Response
{
    public class RecepcionDocumentosResponseData
    {
        public class RecepcionDocumentosGridResponse : vCitaVerificacion
        {
            public DateTime? FechaFacturacion { get; set; }
            public List<vCatMarcaVehiculoNombre?> ListaMarcas { get; set; }
            public List<vDieselVehiculo?> ListaMarcasDisel { get; set; }
            public string NumeroReferencia { get; set; } = string.Empty;
            public string FechaStr
            {
                get
                {

                    return Fecha.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("es-ES"));
                }
            }
            public string NombrePersona
            {
                get
                {

                    return NombrePropietario ?? RazonSocial ?? "-";
                }
            }

            public string VerificentroAbrv
            {
                get
                {

                    return Verificentro != null ? Verificentro.Contains("CENTRO DE VERIFICACION VEHICULAR")
                                        ? Verificentro.Replace("CENTRO DE VERIFICACION VEHICULAR", "CVV")
                                        : Verificentro.Contains("CENTRO DE VERIFICACIÓN VEHICULAR")
                                            ? Verificentro.Replace("CENTRO DE VERIFICACIÓN VEHICULAR", "CVV") : Verificentro
                                            : "";
                }
            }
            public DocumentosCitaResponse? DataDocumentos { get; set; }
            public string Acciones { get; set; }
            public string VchLinea { get; set; }
            public string Progreso
            {
                get
                {
                    try
                    {
                        return EstatusPrueba == null ? (IdRecepcionDocumento != null ? "Documentos Recibidos" : "Cita Sin Atender") : EstatusVerificacion.Dict[EstatusPrueba.Value];

                    }
                    catch (Exception)
                    {

                        return "";
                    }
                }
            }
            public long IdMotivoVerificacion { get; set; }//TODO Remover porque deberian venir de la vista
            public string FolioCertificadoAnterior { get; set; }//TODO Remover porque deberian venir de la vista
        }

        public class DocumentosCitaResponse : vDocumentosCitum
        {

        }

        public class CatalogoTablaMaestraResponse : vTablaMaestra
        {

        }

        public class DieselAutocompleteResponse
        {
            public int? Id { get; set; }
            public string Text { get; set; }
        }

        public class SubDieselResponse
        {
            public long? Id { get; set; }
            public string Nombre { get; set; }
            public int Total { get; set; }
        }

    }
}
