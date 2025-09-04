using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Verificacion.Response
{
    public class VerificacionResponse
    {
        public long Id { get; set; }

        public DateTime Fecha { get; set; }

        public string Placa { get; set; } = null!;

        public string Serie { get; set; } = null!;

        public string FolioCertificado { get; set; } = null!;

        public DateTime Vigencia { get; set; }

        public string Marca { get; set; } = null!;

        public string Modelo { get; set; } = null!;

        public int Anio { get; set; }

        public string Combustible { get; set; } = null!;

        public string TarjetaCirculacion { get; set; } = null!;

        public long IdLinea { get; set; }

        public int? Orden { get; set; }

        public DateTime? FechaVerificacion { get; set; }

        public long? FolioFormaValorada { get; set; }

        public long IdCicloVerificacion { get; set; }

        public int? IdTipoCertificado { get; set; }

        public long? IdCatSubDiesel { get; set; }

        public long? IdCitaVerificacion { get; set; }

        public string? URLPlacaDelantera { get; set; }

        public string? URLPlacaTrasera { get; set; }

        public string? URLPlacaCamara { get; set; }

        public bool IngresoManual { get; set; }

        public long? IdUserTecnico { get; set; }

        public string? URLFotoTecnico { get; set; }

        public int IdTipoCombustible { get; set; }

        public string NumeroReferencia { get; set; } = null!;

        public long? IdEquipoVerificacion { get; set; }

        public bool? REF00 { get; set; }

        public int? IdMotivoVerificacion { get; set; }

        public string? URLPlacaCamaraTrasera { get; set; }

        public int? NoIntento { get; set; }

        public int Semestre { get; set; }

        public int AnioVerificacion { get; set; }

        public int? IdLimiteVerificacion { get; set; }

        public List<string>? Observaciones { get; set; }

        public bool? Errores { get; set; }
    }
}
