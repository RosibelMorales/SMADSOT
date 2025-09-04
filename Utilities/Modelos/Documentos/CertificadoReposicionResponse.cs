using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos.Documentos
{
    public class CertificadoReposicionResponse
    {
        public long IdVerificacion { get; set; }

        public long? IdReposicion { get; set; }

        public DateTime Fecha { get; set; }

        public string Placa { get; set; } = null!;

        public string Serie { get; set; } = null!;

        public string FolioCertificado { get; set; } = null!;

        public DateTime Vigencia { get; set; }

        public long? ResultadosPrueba { get; set; } = null!;

        public string TipoCertificado { get; set; } = null!;

        public string? Semestre { get; set; }

        public string Marca { get; set; } = null!;

        public string Modelo { get; set; }

        public string Combustible { get; set; } = null!;

        public string TarjetaCirculacion { get; set; } = null!;

        public string TipoCertificadoFV { get; set; } = null!;

        public int Anio { get; set; }

        public string? NumeroReferencia { get; set; }

        public string? UrlDoc1 { get; set; }

        public string? UrlDoc2 { get; set; }

        public string? UrlDoc3 { get; set; }
    }
}
