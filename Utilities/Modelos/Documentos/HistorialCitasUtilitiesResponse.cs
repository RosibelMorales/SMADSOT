
using System.Globalization;

namespace Smadot.Utilities.Modelos.Documentos
{
    public class HistorialCitasUtilitiesResponse
    {
        public List<HistorialCitasRowResponse>? Rows { get; set; }
        public string? Fecha1 { get; set; }
        public string? Fecha2 { get; set; }
    }
    public class HistorialCitasRowResponse
    {
        public long? Id { get; set; }

        public long IdCita { get; set; }

        public DateTime? Fecha { get; set; }

        public string? Placa { get; set; }

        public string? Marca { get; set; }

        public string? Serie { get; set; }

        public string? Modelo { get; set; }

        public long? IdLinea { get; set; }

        public string? Linea { get; set; }

        public long? IdVerificentro { get; set; }

        public string? NombrePersona { get; set; }

        public string CitaSerie { get; set; } = null!;

        public int? Orden { get; set; }

        public bool? IngresoManual { get; set; }

        public long? IdResultadosVerificacion { get; set; }

        public int? EstatusPrueba { get; set; }

        public int? C_RECHAZO { get; set; }

        public int? RESULTADO { get; set; }

        public bool? PruebaObd { get; set; }

        public bool? PruebaEmisiones { get; set; }

        public bool? PruebaOpacidad { get; set; }

        public int? IdTipoCertificado { get; set; }

        public string Folio { get; set; } = null!;

        public long? IdFolioFormaValoradaVerificentro { get; set; }

        public long? FolioAsignado { get; set; }

        public string? ClaveLinea { get; set; }
        public string? RazonSocial { get; set; }

        public bool? Cancelado { get; set; }

        public bool? Reposicion { get; set; }

        public string NombreCentro { get; set; } = null!;
        public int? IdRecepcionDocumentos { get; set; }

        public string FechaStr
        {
            get
            {
                return Fecha == null ? DateTime.Now.ToString("g", new CultureInfo("es-MX")) : Fecha.Value.ToString("g", new CultureInfo("es-MX"));
            }
        }

        public string NombrePropietario
        {
            get
            {
                return NombrePersona ?? RazonSocial ?? "-";
            }
        }
        public string Acciones { get; set; }
        public string IngresoManualStr
        {
            get
            {
                return EstatusPrueba == null || EstatusPrueba < EstatusVerificacionUtilities.TerminaPruebaVisual ? "-" : (IngresoManual ?? false) ? "Ingreso Manual" : "Ingreso OCR";
            }
        }
        public string ResultadoStr
        {
            get
            {
                try
                {
                    return ResultadosUtilities.DictResultados[RESULTADO ?? 0];

                }
                catch (Exception)
                {

                    return "";
                }
            }
        }
        public string Progreso
        {
            get
            {
                try
                {
                    return EstatusPrueba == null ? (IdRecepcionDocumentos != null ? "Documentos Recibidos" : "Cita Sin Atender") : EstatusVerificacionUtilities.Dict[EstatusPrueba.Value];

                }
                catch (Exception)
                {

                    return "";
                }
            }
        }

    }
    public class EstatusVerificacionUtilities
    {

        public const int TerminaPruebaVisual = 2;
        public const int EnPruebaEstaticaDinamica = 3;
        public const int TerminaPruebaEstaticaDinamica = 4;
        public const int EnPruebaOBD = 5;
        public const int TerminaPruebaOBD = 6;
        public const int EnPruebaOpacidad = 7;
        public const int TerminaPruebaOpacidad = 8;
        public const int VerificacionAbortada = 9;
        public const int VerificacionFinalizada = 10;
        public const int FolioImpreso = 11;
        public static Dictionary<int, string> Dict = new Dictionary<int, string>
                        {
                            {0, "Línea Asignada" },
                            {TerminaPruebaVisual, "Prueba Visual Finalizada" },
                            {EnPruebaEstaticaDinamica, "Prueba Emisiones Iniciada" },
                            {TerminaPruebaEstaticaDinamica, "Prueba Emisiones Finalizada" },
                            {EnPruebaOBD, "Prueba OBD Iniciada" },
                            {TerminaPruebaOBD, "Prueba OBD Finalizada" },
                            {EnPruebaOpacidad, "Prueba Opacidad Iniciada" },
                            {TerminaPruebaOpacidad, "Prueba Opacidad Finalizada" },
                            {VerificacionAbortada, "Verificación Abortada" },
                            {FolioImpreso, "Folio Impreso" },
        };

    }
    public struct ResultadosUtilities
    {
        public static Dictionary<int, string> DictResultados = new Dictionary<int, string>
                        {
                            {Escapado, "Sin Resultado" },
                            {Cero, "Tipo 0" },
                            {Uno, "Tipo 1" },
                            {Dos, "Tipo 2" },
                            {DobleCero, "Tipo 00" },
                            {Rechazo, "Rechazo" },
                        };


        public const int Escapado = 0;
        public const int Cero = 1;
        public const int Uno = 2;
        public const int Dos = 3;
        public const int DobleCero = 4;
        public const int Rechazo = 5;
    }
}
