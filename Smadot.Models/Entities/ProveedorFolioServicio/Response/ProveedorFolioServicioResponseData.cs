using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.ProveedorFolioServicio.Response
{
    public class ProveedorFolioServicioResponseData
    {
        public class ProveedorFolioServicioResponse
        {
            public long? Id { get; set; }
            public long? FolioOS { get; set; }
            public string? ProveedorEmpresa { get; set; }
            public string? FechaRegistro { get; set; }
            public string? Proveedor { get; set; }
            public string? Motivo { get; set; }
            public string? Equipo { get; set; }
            public bool EstatusFolio { get; set; }
            public long? FolioPF { get; set; }
            public bool? EsLaboratorio { get; set; }
            public int Total { get; set; }
            public int TotalFilter { get; set; }
        }

        public class ProveedorFolioServicioResponseGrid : ProveedorFolioServicioResponse
        {
            public string Acciones { get; set; }
        }

        public class ProveedorFolioServicioAutocompleteResponse
        {
            public long Id { get; set; }
            public string Text { get; set; }
        }
    }
}
