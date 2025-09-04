using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.FoliosCancelados.Response
{
    public class FoliosCanceladosResponseData
    {

        public class FoliosCanceladosResponse
        {
            public long? Id { get; set; }
            public string? Folio { get; set; }
            public string? FechaCancelacion { get; set; }
            public string? Motivo { get; set; }
            public string? UsuarioAprobo { get; set; }
            public string? TipoTramite { get; set; }
            public string? DatosVehiculo { get; set; }
            public string? PersonaRealizoTramite { get; set; }

            public string? Fecha { get; set; }

            public int Total { get; set; }

            public int? IdCatMotivoCancelacion { get; set; }
            public long? IdUserCancelo { get; set; }
            public int? IdCatTipoTramite { get; set; }
            public int? IdCatEstatusFV { get; set; }
            public string? EstatusFV { get; set; }
            public string? OtroMotivo { get; set; }

            public bool FolioExists { get; set; }
            public long? FolioNuevo { get; set; }
            public int? IdCatTipoCertificado { get; set; }
        }

        public class FoliosCanceladosResponseGrid : FoliosCanceladosResponse
        {
            public string Acciones { get; set; }
        }

        public class FoliosCanceladosAutoCompleteResponse
        {
            public long Id { get; set; }
            public string Folio { get; set; }
            public bool Cancelado { get; set; }
        }

    }
}
