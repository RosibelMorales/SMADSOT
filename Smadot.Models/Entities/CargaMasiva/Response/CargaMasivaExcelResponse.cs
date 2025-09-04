using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.CargaMasiva.Response
{
    public class DataCarga
    {
        public List<CargaMasivaExcelResponse> Lista { get; set; } = new();
        public string? FileName { get; set; }
        public string? FileBase64 { get; set; }
    }
    public class CargaMasivaExcelResponse
    {
        public string? Nombre { get; set; }

        public string? RazonSocial { get; set; }

        public string? Correo { get; set; }

        public DateTime Fecha { get; set; }

        public long IdCVV { get; set; }

        public string Placa { get; set; } = null!;

        public int IdCatMarcaVehiculo { get; set; }

        public int IdSubMarcaVehiculo { get; set; }

        public short Anio { get; set; }

        public string Serie { get; set; } = null!;

        public string ColorVehiculo { get; set; } = null!;

        public int IdTipoCombustible { get; set; }
        public string ClaveLinea { get; set; }

        public bool Poblano { get; set; }

        public string? Estado { get; set; }

        public decimal ImporteActual { get; set; }

        public DateTime FechaRegistro { get; set; }

        public int ConsecutivoTramite { get; set; }

        public DateTime FechaEmisionRef { get; set; }

        public DateTime FechaPago { get; set; }

        public long FolioCertificado { get; set; }

        public string? FolioAnterior { get; set; }

        public DateTime Vigencia { get; set; }

        public string Marca { get; set; } = null!;

        public string Modelo { get; set; } = null!;

        public string Combustible { get; set; } = null!;

        public string TarjetaCirculacion { get; set; } = null!;

        public string NumeroReferencia { get; set; } = null!;

        public int Semestre { get; set; }

        public int AnioVerificacion { get; set; }

        public DateTime FechaVerificacion { get; set; }

        public int IdTipoCertificado { get; set; }

        public int? IdMotivoVerificacion { get; set; }

        public List<string>? Observaciones { get; set; }

        public bool? Errores { get; set; }
    }
}
