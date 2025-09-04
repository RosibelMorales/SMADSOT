using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadsot.FotoMulta.Model.Entities.Response
{
    public class AutenticacionResponse
    {
        public long IdUser { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string CicloVerificacion { get; set; }
        public string? NombreVerificentro { get; set; }
        public string ClaveVerificentro { get; set; }
        public int? IdVerificentro { get; set; }
        public int? Semestre { get; set; }
        public int? IdPuesto { get; set; }
        public long? IdCicloVerificacion { get; set; }
        public int? IdNivelConsulta { get; set; }
        public List<long> Roles { get; set; }
        public List<string> RoleNames { get; set; }
        public List<long> Permisos { get; set; }

        public int? CodigoError { get; set; }
    }
}
