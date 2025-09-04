using Smadot.Models.Entities.IngresoFormaValorada.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.DirectorioCentrosVerificacion.Response
{
    public class DirectorioCentrosVerificacionResponse
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public bool Activo { get; set; }

        public string? Clave { get; set; }

        public string? Direccion { get; set; }

        public string? Rfc { get; set; }

        public string? Telefono { get; set; }

        public string? Correo { get; set;  }

        public string? GerenteTecnico { get; set; }

        public string? RepresentanteLegal { get; set; }

        public float? Longitud { get; set; }

        public float? Latitud { get; set; }

        public string? Link { get; set; }


        public int? IdAutorizadoVerificentro { get; set; }

        public string? NombreAutorizadoVerificentro { get; set; }

        public string? TelefonoAutorizadoVerificentro { get; set; }

        public string? CorreoAutorizadoVerificentro { get; set; }

        public int? Total { get; set; }
    }
    public class DirectorioCentrosVerificacionResponseGrid : DirectorioCentrosVerificacionResponse
    {
        public string Acciones { get; set; }
    }
}
