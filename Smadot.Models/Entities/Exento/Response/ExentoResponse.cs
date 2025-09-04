using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Smadot.Models.Entities.Exento.Response
{
    public class ExentoResponse
    {
        public long Id { get; set; }
        public string? Marca { get; set; }
        public string? Submarca { get; set; }
        public int Modelo { get; set; }
        public string? Placa { get; set; }
        public string? Serie { get; set; } 
        public string? ResultadoPrueba { get; set; } 

        public int IdCatTipoCertificado { get; set; }

        public DateTime Vigencia { get; set; }

        public DateTime FechaCartaFactura { get; set; }

        public string? Propietario { get; set; } 

        public string? Combustible { get; set; } 

        public string? NumTarjetaCirculacion { get; set; } 

        public string? UltimoFolio { get; set; } 

        public DateTime? VigenciaHoloAnterior { get; set; }

        public string? NumeroReferencia { get; set; } 

        public List<TipoCertificadoResponse>? Certificados { get; set; }
        public int Total { get; set; }
        public string? FilesString { get; set; }
        public List<ExentoFiles> Files { get; set; }

        public string? UrlDoc1 { get; set; }

        public string? UrlDoc2 { get; set; }

        public string? UrlDoc3 { get; set; }
        public string? UrlDoc4 { get; set; }
        public DateTime FechaEmisionRef { get; set; }
        public DateTime FechaPago { get; set; }
        public string EntidadProcedencia { get; set; }
        public string? ServidorPublico { get; set; }
    }

    public class ExentoResponseGrid : ExentoResponse
    {
        public string Acciones { get; set; }
    }
    public class ExentoFiles
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }
}
