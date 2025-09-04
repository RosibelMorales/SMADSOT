using Smadot.Models.DataBase;
using Smadot.Models.Entities.Generic.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;


namespace Smadot.Models.Entities.Linea.Response
{
    public class LineaDetalle : vCALinea
    {
        public string EstatusString { get { return Estatus ? "Apertura Línea" : "Cierre Línea"; } }

        public string Acciones { get; set; }
        public string Documento { get; set; }
        [JsonIgnore]
        public DocumentoModel? DocumentoGrid
        {
            get
            {
                var titulo = Estatus ? "Apertura" : "Cierre";
                return string.IsNullOrEmpty(UrlDocumento) ? null : new DocumentoModel
                {
                    Nombre = $"Documento {titulo}",
                    UrlDocumento = UrlDocumento ?? "",
                    Linea = $"{IdLineaMotivo}_{titulo}"

                };
            }
        }
    }
}
