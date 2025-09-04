using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos.Documentos
{
    public class RefrendoCertificadoResponse
    {
        public long Id { get; set; }

        public long IdExento { get; set; }

        public string NumeroReferencia { get; set; } = null!;

        public string? UrlDoc1 { get; set; }

        public string? UrlDoc2 { get; set; }

        public string? UrlDoc3 { get; set; }

        public DateTime FechaCartaFactura { get; set; }

        public DateTime VigenciaHoloAnterior { get; set; }

        public string? Placa { get; set; }

        public string? Propietario { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long IdUserRegistro { get; set; }

        public string? Nombre { get; set; }

        public string Marca { get; set; } = null!;

        public string Submarca { get; set; } = null!;

        public int Modelo { get; set; }

        public string PlacaExento { get; set; } = null!;

        public string Serie { get; set; } = null!;

        public string ResultadoPrueba { get; set; } = null!;

        public int IdCatTipoCertificado { get; set; }

        public string CatTipoCcertificado { get; set; } = null!;

        public DateTime Vigencia { get; set; }

        public DateTime FechaCartaFacturaExento { get; set; }

        public string PropietarioExento { get; set; } = null!;

        public string Combustible { get; set; } = null!;

        public string NumTarjetaCirculacion { get; set; } = null!;

        public string UltimoFolio { get; set; } = null!;

        public DateTime VigenciaHoloAnteriorExento { get; set; }

        public string NumeroReferenciaExento { get; set; } = null!;

        public string? UrlDoc1Exento { get; set; }

        public string? UrlDoc2Exento { get; set; }

        public string? UrlDoc3Exento { get; set; }
    }
}
