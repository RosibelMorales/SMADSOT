using System.ComponentModel.DataAnnotations;

namespace Smadot.Models.Entities.Autenticacion.Request
{
    public class AutenticacionRequest
    {
        [Required]
        public string Usuario { get; set; }

        [Required]
        public string Contrasenia { get; set; }

		[Required]
		public int IdVerificentro { get; set; }
	}
}
