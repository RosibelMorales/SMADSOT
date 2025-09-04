namespace Smadot.Models.Entities.Catalogos.Response
{
    public class VerificentroResponse
    {
        public long Id { get; set; }

        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }

        public string? Direccion { get; set; }

        public string? Telefono { get; set; }

        public string? DirectorGestionCalidadAire { get; set; }
    }
}
