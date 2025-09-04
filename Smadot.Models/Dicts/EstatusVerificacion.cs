using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Dicts
{
    public class EstatusVerificacion
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
                            {VerificacionFinalizada, "Verificacion Finalizada" },
                            {FolioImpreso, "Folio Impreso" },
        };
        public static List<int> EstatusExcluidosActualizacion = new()
        {
            VerificacionAbortada,
            VerificacionFinalizada,
            FolioImpreso
        };

    }
}
