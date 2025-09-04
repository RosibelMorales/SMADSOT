using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Smadot.Models.Dicts;

public class MotivoVerificacionDict
{
    public const int VerificacionNormal = 2;
    public const int AltaBajaPlacas = 3;
    public const int NuevoSinVerificacionAnterior = 4;
    public const int HologramaDoblecero = 6;
    public const int HologramaDobleceroRefrendo = 15;
    public const int Extemporaneo = 9;
    public const int ExencionPeriodo = 11;
    public const int ReverificacionCambioHolograma = 12;
    public const int Rechazo = 14;
    public const int VERIFICACIONVOLUNTARIA = 7;
    public const int EXENCIONFUERADEPERIODOPRIMERINTENTO = 16;
    public const int RECHAZOVOLUNTARIO = 17;
    public const int RECHAZOHOLOGRAMADOBLECERO = 18;
}