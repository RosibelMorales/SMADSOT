using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.DirectorioCentrosVerificacion.Response
{
    public class NuevoVerificentroResponse
    {
        public long? Id { get; set; }

        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }

        public string? Clave { get; set; }

        public string? Direccion { get; set; }

        public string? Rfc { get; set; }

        public string? Telefono { get; set; }

        public string? Correo { get; set; }

        public string? GerenteTecnico { get; set; }

        public string? RepresentanteLegal { get; set; }

        public decimal? Longitud { get; set; }

        public decimal? Latitud { get; set; }

        public string DirectorGestionCalidadAire { get; set; } = null!;

        public string ApiEndPoint { get; set; } = null!;

        public string ApiKey { get; set; } = null!;

        public string Municipio { get; set; } = null!;
    }
}
