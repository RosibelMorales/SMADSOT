namespace Smadot.Models.Entities.Verificacion.Response
{
    public class PlacasResponse
    {
        public byte[]? Delantera { get; set; }
        public byte[]? Trasera { get; set; }
        public bool Error { get; set; }
    }
}