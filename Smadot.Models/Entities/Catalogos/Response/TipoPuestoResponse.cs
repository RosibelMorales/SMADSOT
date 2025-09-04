namespace Smadot.Models.Entities.Catalogos.Response
{
    public class TipoPuestoResponse
    {
        public int Id { get; set; }

        public string? Nombre { get; set; } 

        public bool Activo { get; set; }
        public int Total { get; set; }
    }
}
