namespace Smadsot.FotoMulta.Model.Entities.Request
{
    public class CambioContraseniaRequest
    {
        public string ContraseniaActual { get; set; }
        public string ContraseniaNueva { get; set; }
        public string ContraseniaConfirmar { get; set; }
        public string Email { get; set; }
    }
}
