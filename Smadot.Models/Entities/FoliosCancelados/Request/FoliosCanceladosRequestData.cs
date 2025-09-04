using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.FoliosCancelados.Request
{
    public class FoliosCanceladosRequestData
    {
        public class FolioCanceladosRequest
        {
            public long? IdFolio { get; set; }
            public long? IdVerificacion { get; set; }
            public string? FechaCancelacion { get; set; }
            public int? MotivoCancelacion { get; set; }
            public string? OtroMotivo { get; set; }
            public string? PersonaAprobo { get; set; }
            public long? IdUser { get; set; }
            public bool FolioExists { get; set; }
            public long? FolioNuevo { get; set; }
            public int? IdCatTipoCertificado { get; set; }
        }
    }
}
