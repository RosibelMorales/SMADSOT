using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Administrativa.Response
{
    public class AdministrativaResponseData
    {
        public class AdministrativaResponse
        {
            public long? IdFolio { get; set; }
            public long? IdAdministrativa { get; set; }

            public int? IdCatMotivoTramite { get; set; }

            public string? MotivoTramite { get; set; }

            public string? Placa { get; set; }

            public string? Serie { get; set; }

            public string? NumeroReferencia { get; set; }

            public string? UrlDoc1 { get; set; }

            public string? UrlDoc2 { get; set; }

            public string? UrlDoc3 { get; set; }

            public string? UrlDoc4 { get; set; }

            public string? FolioAsignado { get; set; }

            public long? IdUserRegistro { get; set; }

            public string? UsuarioRegistro { get; set; }

            public string? FechaRegistro { get; set; }

            public string Vigencia { get; set; }

            public string NombrePropietario { get; set; }

            public string Marca { get; set; }

            public string SubMarca { get; set; }

            public int Modelo { get; set; }

            public string TarjetaCirculacion { get; set; }

            public int IdTipoCombustible { get; set; }

            public string Combustible { get; set; }

            public int? IdCatTipoCertificado { get; set; }

            public string EntidadProcedencia { get; set; }

            public string FechaEmisionRef { get; set; }

            public string FechaPago { get; set; }

            public string? ClaveTramite { get; set; } 
            public int Total { get; set; }
        }

        public class AdministrativaResponseGrid : AdministrativaResponse
        {
            public string Acciones { get; set; }
        }

        public class PruebaAutocompletePlacaApi {
            public long Id { get; set; }
            public string Placa { get; set; }
            public string Serie { get; set; }
        }
    }
}
