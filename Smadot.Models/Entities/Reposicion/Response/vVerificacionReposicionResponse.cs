

using Smadot.Models.DataBase;

namespace Smadot.Models.Entities.Reposicion.Response
{
    public class vVerificacionReposicionResponse : vVerificacionReposicion
    {
        public string Semestre
        {
            get
            {
                var fechaActual = FechaRegistro;
                var semestre1FechaInicio = new DateTime(fechaActual.Value.Year, 1, 1);
                var semestre1FechaFinal = new DateTime(fechaActual.Value.Year, 6, 30, 23, 59, 59);
                var semestre = $"Segundo Semestre {fechaActual.Value.Year}";
                // Validamos que la sesión tenga el semestre correcti
                if (fechaActual >= semestre1FechaInicio && fechaActual <= semestre1FechaFinal)
                {
                    semestre = $"Primer Semestre {fechaActual.Value.Year}";

                }
                return semestre;

            }
        }
    }
}