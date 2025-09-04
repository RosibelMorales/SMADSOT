namespace Smadot.Models.Entities.Autenticacion.Request
{
    public class CambioContraseniaRequest
    {
        public string ContraseniaActual { get; set; }
        public string ContraseniaNueva { get; set; }
        public string ContraseniaConfirmar { get; set; }
        public long? IdUser { get; set; }
    }
}
