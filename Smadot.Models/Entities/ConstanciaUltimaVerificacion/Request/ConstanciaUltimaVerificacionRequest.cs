using Smadot.Models.Entities.Refrendo.Request;

namespace Smadot.Models.Entities.ConstanciaUltimaVerificacion.Request
{
    public class ConstanciaUltimaVerificacionRequest
    {
        public long Id { get; set; }

        public long? IdFv { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string NumeroReferencia { get; set; } = null!;

        public string? UrlDoc1 { get; set; }

        public string? UrlDoc2 { get; set; }

        public string? UrlDoc3 { get; set; }

        public List<RefrendoFiles> Files { get; set; }
        public DateTime FechaEmisionRef { get; set; }
        public DateTime FechaPago { get; set; }
    }
}
