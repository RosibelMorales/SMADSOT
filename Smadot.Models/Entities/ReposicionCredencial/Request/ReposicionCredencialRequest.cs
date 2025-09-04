namespace Smadot.Models.Entities.ReposicionCredencial.Request
{
    public class ReposicionCredencialRequest
    {
        public long IdUserPuestoVerificentro { get; set; }

        public int IdCatMotivoReporteCredencial { get; set; }

        public string Denuncia { get; set; } = null!;

        public string? UrlCredencial { get; set; }
        public List<CredencialFile>? Files { get; set; }
    }

    public class CredencialFile
    {
        public string? Base64 { get; set; }

        public string? Nombre { get; set; }

        public string? Tipo { get; set; }
    }
}
