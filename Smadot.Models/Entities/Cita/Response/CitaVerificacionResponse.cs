using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Cita.Response
{
    public class CitaVerificacionResponse
    {
        public long Id { get; set; }

        public string? Nombre { get; set; }

        public string? RazonSocial { get; set; }

        public string? Correo { get; set; }

        public DateTime Fecha { get; set; }

        public long IdCVV { get; set; }

        public string Placa { get; set; } = null!;

        public int IdCatMarcaVehiculo { get; set; }

        public int IdSubMarcaVehiculo { get; set; }

        public short Anio { get; set; }

        public bool Acepto { get; set; }

        public string Serie { get; set; } = null!;

        public string ColorVehiculo { get; set; } = null!;

        public bool? Cancelada { get; set; }

        public string UrlComprobante { get; set; } = null!;

        public string Folio { get; set; } = null!;

        public int IdTipoCombustible { get; set; }

        public string NombreGeneraCita { get; set; } = null!;

        public bool Poblano { get; set; }

        public string? Estado { get; set; }

        public List<string>? Observaciones { get; set; }

        public bool? Errores { get; set; }
    }
}
