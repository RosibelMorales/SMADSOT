using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Smadot.Models.Entities.Linea.Response
{
    public class vLineaEquipoResponse
    {
        public long IdLinea { get; set; }

        public string Nombre { get; set; } = null!;

        public int IdCatEstatusEquipo { get; set; }

        public long Id { get; set; }

        public int Total { get; set; }

        public List<SelectListItem>? LineaEquipos { get; set; }
    }
}
