using System.ComponentModel.DataAnnotations;

namespace Smadot.Models.Entities.Autenticacion.Request
{
    public class RegistroRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Contrasenia { get; set; }
        public string Nombre { get; set; }
        public string? APaterno { get; set; }
        public string? AMaterno { get; set; }
    }
}
