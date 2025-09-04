using Newtonsoft.Json;
using Smadot.Utilities.Modelos;

namespace Smadot.Models.Entities.RecepcionDocumentos.Request
{
    public class RecepcionDocumentosRequestData
    {

        public class RecepcionDocumentosRequest
        {
            [JsonProperty("idCitaVerificacion")]
            public long IdCitaVerificacion { get; set; }

            [JsonProperty("idCatTipoServicio")]
            public int IdCatTipoServicio { get; set; }

            [JsonProperty("idTipoCombustible")]
            public int IdTipoCombustible { get; set; }

            [JsonProperty("colorVehiculo")]
            public string? ColorVehiculo { get; set; }

            [JsonProperty("folioTarjetaCirculacion")]
            public string FolioTarjetaCirculacion { get; set; }

            [JsonProperty("cambioPlacas")]
            public bool CambioPlacas { get; set; }

            [JsonProperty("fileIdentificacion")]
            public RecepcionDocumentoFiles? FileIdentificacion { get; set; }

            [JsonProperty("fileFactura")]
            public RecepcionDocumentoFiles? FileFactura { get; set; }

            [JsonProperty("fileValidacionCertificado")]
            public RecepcionDocumentoFiles? FileValidacionCertificado { get; set; }

            [JsonProperty("fileBajaPlacas")]
            public RecepcionDocumentoFiles? FileBajaPlacas { get; set; }

            [JsonProperty("fileAltaPlacas")]
            public RecepcionDocumentoFiles? FileAltaPlacas { get; set; }

            [JsonProperty("fileMulta")]
            public RecepcionDocumentoFiles? FileMulta { get; set; }

            [JsonProperty("numeroReferencia")]
            public string NumeroReferencia { get; set; }

            [JsonProperty("fechaRecepcion")]
            public DateTime FechaRecepcion { get; set; }

            [JsonProperty("anio")]
            public short? Anio { get; set; }

            [JsonProperty("modelo")]
            public int? Modelo { get; set; }

            //[JsonProperty("cilindros")]
            //public int? Cilindros { get; set; }

            //[JsonProperty("cilindrada")]
            //public int? Cilindrada { get; set; }

            //[JsonProperty("diesel")]
            //public int? Diesel { get; set; }

            //[JsonProperty("subDiesel")]
            //public long? SubDiesel { get; set; }

            [JsonProperty("idTablaMaestra")]
            public long IdTablaMaestra { get; set; }

            [JsonProperty("nombrePersona")]
            public string? NombrePersona { get; set; }

            [JsonProperty("estado")]
            public string? Estado { get; set; }

            [JsonProperty("nombreGeneraCita")]
            public string? NombreGeneraCita { get; set; }

            [JsonProperty("personaMoral")]
            public bool PersonaMoral { get; set; }

            [JsonProperty("placa")]
            public string? Placa { get; set; }

            [JsonProperty("serie")]
            public string? Serie { get; set; }

            [JsonProperty("fechaFacturacion")]
            public DateTime? FechaFacturacion { get; set; }

            [JsonProperty("idMotivoVerificacion")]
            public int IdMotivoVerificacion { get; set; }

            [JsonProperty("folioCertificadoAnterior")]
            public string? FolioCertificadoAnterior { get; set; }
        }

        public class RecepcionDocumentoFiles
        {
            [JsonProperty("nombre")]
            public string Nombre { get; set; }

            [JsonProperty("tipo")]
            public string Tipo { get; set; }

            [JsonProperty("base64")]
            public string Base64 { get; set; }
        }
        public class ReagendarCitaRequest
        {
            public long IdCita { get; set; }

            public string Folio { get; set; }

            public DateTime Fecha { get; set; }

            public long IdCVV { get; set; }

            public string FechaString { get; set; }

            public string HoraString { get; set; }

            public string? Logo { get; set; }
        }

        public class CatalogoTablaMaestraRequest
        {
            [JsonProperty("idCita")]
            public long IdCita { get; set; }
            [JsonProperty("idSubmarca")]
            public long IdSubmarca { get; set; }
            [JsonProperty("idMarca")]
            public long IdMarca { get; set; }

            [JsonProperty("cilindros")]
            public int? Cilindros { get; set; }

            [JsonProperty("registroTMaestra")]
            public bool RegistroTMaestra { get; set; }
        }

        public class GenericSelect2AutocompleRequest
        {
            [JsonProperty("term")]
            public string Term { get; set; }

            [JsonProperty("start")]
            public int Start { get; set; }

            [JsonProperty("end")]
            public int End { get; set; }

        }

        public class SubDieselRequest : RequestList
        {
            [JsonProperty("idCatMarcaVehiculo")]
            public int? IdCatMarcaVehiculo { get; set; }
        }
        public class RecepcionDocumentosCambiarLineaRequest
        {
            public long IdVerificacion { get; set; }

            public int IdLinea { get; set; }

        }
    }
}

