using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos.Documentos
{

    public class PersonalVistaPreviaResponse
    {
        public long IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string CorreoUsuario { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; }
        public string GeneroText { get; set; }
        public string Rfc { get; set; }
        public string Curp { get; set; }
        public int IdPuesto { get; set; }
        public string Puesto { get; set; }
        public DateTime FechaCapacitacionInicio { get; set; }
        public DateTime FechaCapacitacionFinal { get; set; }
        public DateTime FechaIncorporacion { get; set; }
        public DateTime? FechaAcreditacionNorma { get; set; }
        public string NumeroTrabajador { get; set; }
        public DateTime? FechaSeparacion { get; set; }
        public List<HorarioPersonalVistaPreviaResponse> HorarioRequest { get; set; }
        public List<PuestosVistaPreviaResponse> PuestosResult { get; set; }
    }

    public class HorarioPersonalVistaPreviaResponse
    {
        public int Id { get; set; }
        public string? Dia { get; set; }
        public string? HoraInicio { get; set; }
        public TimeSpan HoraInicioTS => !string.IsNullOrEmpty(HoraInicio) ? TimeSpan.Parse(HoraInicio) : TimeSpan.Parse("00:00");
        public string? HoraFin { get; set; }
        public TimeSpan HoraFinTS => !string.IsNullOrEmpty(HoraFin) ? TimeSpan.Parse(HoraFin) : TimeSpan.Parse("00:00");

        public string? HoraInicioResponse => HoraInicioResponseTS.ToString() is "00:00:00" ? string.Empty : HoraInicioResponseTS.ToString();
        public TimeSpan HoraInicioResponseTS { get; set; }
        public string? HoraFinResponse => HoraFinResponseTS.ToString() is "00:00:00" ? string.Empty : HoraFinResponseTS.ToString();
        public TimeSpan HoraFinResponseTS { get; set; }
    }

    public class PuestosVistaPreviaResponse
    {
        public int IdPuesto { get; set; }
        public string Puesto { get; set; }
        public DateTime FechaCapacitacionInicio { get; set; }
        public DateTime FechaCapacitacionFinal { get; set; }
        public DateTime FechaIncorporacion { get; set; }
        public DateTime? FechaAcreditacionNorma { get; set; }
        public string NumeroTrabajador { get; set; }
        public DateTime? FechaSeparacion { get; set; }
    }
}
