namespace Smadot.Models.Entities.CargaMasiva.Response
{
    public enum  MotivoVerificacionEnum
    {
        VerificacionNormal = 2,
        AltaYBajaDePlacas = 3,
        NuevoSinVerificAnterior = 4,
        HologramaDobleCero = 6,
        VerificacionVoluntaria = 7,
        Extemporaneo = 9,
        ExencionFueraDePeriodo = 11,
        ReverifPorCambioDeHolograma = 12,
        Rechazo = 14,
        HologramaDobleCeroRefrendo = 15,
        ExencionFueraDePeriodoPrimerIntento = 16
    }
}
