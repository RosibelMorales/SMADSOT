using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Catalogos.Response
{
    public class SubMarcaVehiculoResponse
    {
        public int Id { get; set; }

        public int IdCatMarcaVehiculo { get; set; }

        public int Clave { get; set; }

        public string? Nombre { get; set; }

        public string? Alias { get; set; }

        public int Total { get; set; }
    }
}
