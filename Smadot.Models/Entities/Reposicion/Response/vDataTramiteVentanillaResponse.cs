using Smadot.Models.DataBase;

namespace Smadot.Models.Entities.Reposicion.Response
{
    public class vDataTramiteVentanillaResponse : vDataTramitesVentanilla
    {
        public string Semestre
        {
            get
            {
                var fechaActual = Fecha;
                var semestre1FechaInicio = new DateTime(fechaActual.Year, 1, 1);
                var semestre1FechaFinal = new DateTime(fechaActual.Year, 6, 30, 23, 59, 59);
                var semestre = $"Segundo Semestre {fechaActual.Year}";
                // Validamos que la sesión tenga el semestre correcti
                if (fechaActual >= semestre1FechaInicio && fechaActual <= semestre1FechaFinal)
                {
                    semestre = $"Primer Semestre {fechaActual.Year}";

                }
                return semestre;

            }
        }
    }
}
