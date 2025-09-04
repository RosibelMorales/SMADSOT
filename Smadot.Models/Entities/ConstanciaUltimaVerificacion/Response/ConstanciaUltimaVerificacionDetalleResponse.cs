using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.ConstanciaUltimaVerificacion.Response
{
    public class ConstanciaUltimaVerificacionDetalleResponse
    {
        public long Id { get; set; }

        public long? IdVerificacion { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long IdUserRegistro { get; set; }

        public string? Nombre { get; set; }

        public string Placa { get; set; } = null!;

        public string Serie { get; set; } = null!;

        public string PlacaSerie { get; set; } = null!;

        public string NumeroReferencia { get; set; } = null!;

        public string? UrlDoc1 { get; set; }

        public string? UrlDoc2 { get; set; }

        public string? UrlDoc3 { get; set; }

        public string? CodigoQr { get; set; }

        public int ConsecutivoTramite { get; set; }

        public DateTime FechaEmisionRef { get; set; }

        public DateTime FechaPago { get; set; }

        public string ClaveTramite { get; set; } = null!;

        public string EntidadProcedencia { get; set; } = null!;

        public string ServidorPublico { get; set; } = null!;

        public int IdFormaValorada { get; set; }

        public DateTime Fecha { get; set; }

        public string PlacaVerificacion { get; set; } = null!;

        public string SerieVerificacion { get; set; } = null!;

        public string FolioCertificado { get; set; } = null!;

        public DateTime Vigencia { get; set; }

        public string Marca { get; set; } = null!;

        public string Modelo { get; set; } = null!;

        public int Anio { get; set; }

        public string? Semestre { get; set; }

        public int ResultadosPrueba { get; set; }

        public string TipoCertificado { get; set; } = null!;

        public string Combustible { get; set; } = null!;

        public string? TarjetaCirculacion { get; set; }

        public string? Direccion { get; set; }

        public string? Telefono { get; set; }

        public string DirectorGestionCalidadAire { get; set; } = null!;

        public string? NombreRazonSocial { get; set; }

        public string? Folio { get; set; }
    }
}
