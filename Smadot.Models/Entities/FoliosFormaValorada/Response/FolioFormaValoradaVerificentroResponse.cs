using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.FoliosFormaValorada.Response
{
    public class FolioFormaValoradaVerificentroResponse
    {
        public long Id { get; set; }

        public long Folio { get; set; }

        public long IdVerificentro { get; set; }

        public decimal ImporteActual { get; set; }

        public long? IdVerificacion { get; set; }

        public bool Cancelado { get; set; }

        public int? IdCatTipoCertificado { get; set; }

        public DateTime? FechaCancelacion { get; set; }

        public int? IdCatMotivoCancelacion { get; set; }

        public string? OtroMotivo { get; set; }

        public long? IdUserCancelo { get; set; }

        public int? ConsecutivoTramiteCancelado { get; set; }

        public string? ClaveTramiteCancelado { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long IdUserRegistro { get; set; }

        public int? IdCatTipoTramite { get; set; }

        public bool Impreso { get; set; }

        public int ConsecutivoTramite { get; set; }

        public DateTime FechaEmisionRef { get; set; }

        public DateTime FechaPago { get; set; }

        public string ClaveTramite { get; set; } = null!;

        public string EntidadProcedencia { get; set; } = null!;

        public string ServidorPublico { get; set; } = null!;

        public bool Reposicion { get; set; }

        public long? IdExento { get; set; }

        public long? IdAdministrativa { get; set; }

        public long? IdTestificacion { get; set; }

        public List<string>? Observaciones { get; set; }

        public bool? Errores { get; set; }
    }
}
