using Smadot.Models.Entities.ReporteMensual.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Roles.Response
{
    public class RolesResponse
    {
        public long? Id { get; set; }

        public string? Nombre { get; set; }

        public string? Alias { get; set; }

        public bool? AccesoTotalVerificentros { get; set; }

        public int? Total { get; set; }

    }

    public class RolesResponseGrid : RolesResponse
    {
        public string Acciones { get; set; }
    }
}
